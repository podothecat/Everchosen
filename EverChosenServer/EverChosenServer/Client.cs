﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.Core.WireProtocol.Messages;
using Newtonsoft.Json;
using EverChosenPacketLib;

namespace EverChosenServer
{
    internal class Client
    {
        public Socket Sock { get; set; }
        public MyProfileInfo ProfileData;
        public MyMatchingInfo MatchingData;
        public bool IsIngame;
        public bool IsReadyToBattle;
        public bool IsReadyToFight;
        public bool IsLogin;
        public string UniqueId { get; set; }

        private readonly byte[] _tempBuffer = new byte[4096];
        private List<byte> _buffer = new List<byte>();
        private int _currentPacketLength = int.MinValue;

        /// <summary>
        /// Ingame Event Handler.
        /// When request is arrived, then call attached method.
        /// </summary>
        public event EventHandler<Packet> InGameRequest;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="socket"></param>
        public Client(Socket socket)
        {
            Sock = socket;
            IsLogin = false;
            IsIngame = false;
            IsReadyToBattle = false;
            IsReadyToFight = false;
        }

        /// <summary>
        /// Send data to client.
        /// </summary>
        public void BeginSend(Packet packet)
        {
            var packetStr = JsonConvert.SerializeObject(packet, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
               
            var sendBuf = new UTF8Encoding().GetBytes(packetStr);
            var sendBufSize = BitConverter.GetBytes(packetStr.Length);

            var totalBuf = new byte[sendBuf.Length + 4];
            Buffer.BlockCopy(sendBufSize, 0, totalBuf, 0, 4);
            Buffer.BlockCopy(sendBuf, 0, totalBuf, 4, sendBuf.Length);

            Sock.BeginSend(totalBuf, 0, totalBuf.Length, SocketFlags.None, OnSendCallback, Sock);
            Console.WriteLine("Send : [" + packet.MsgName + "] to [" + Sock.RemoteEndPoint + "]");
        }

        /// <summary>
        /// Process packet to send.
        /// </summary>
        /// <param name="ar"> Async State </param>
        private void OnSendCallback(IAsyncResult ar)
        {
            var sock = (Socket)ar.AsyncState;
            var size = sock.EndSend(ar);
            //Console.WriteLine("Sent {0} bytes to client", size);
        }

        /// <summary>
        /// Receive asynchronous data from client.
        /// </summary>
        public void BeginReceive()
        {
            if (!Sock.Connected) return;

            // Initialize buffer when each time it receives packet.
            Array.Clear(_tempBuffer, 0, _tempBuffer.Length);

            try
            {
                Sock.BeginReceive(_tempBuffer, 0, _tempBuffer.Length, SocketFlags.None, OnReceiveCallback, Sock);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Close client socket
        /// </summary>
        public void Close()
        {
            Sock.Shutdown(SocketShutdown.Both);
            Sock.Close();
        }

        /// <summary>
        /// Process received packet.
        /// </summary>
        /// <param name="ar"> Asynchronous state result. </param>
        private void OnReceiveCallback(IAsyncResult ar)
        {
            var clientSock = (Socket)ar.AsyncState;

            if (!clientSock.Connected)
                return;

            try
            {
                var size = clientSock.EndReceive(ar);
                Console.WriteLine(size);
                // Unexpected request (ex : force quit)
                if (size == 0)
                {
                    Console.WriteLine("Unexpected Request. Remove client.");

                    if (IsLogin)
                        GameManager.ReleaseClient(this);
                    else
                        Close();

                    return;
                }

                //Console.WriteLine("Receive {0} bytes from client", size);

                _buffer.AddRange(_tempBuffer.ToArray().Take(size));

                ProcessData();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ErrorCode);
                throw;
            }
        }

        /// <summary>
        /// Process received packet.
        /// </summary>
        private void ProcessData()
        {
            if (_buffer.Count < 4)
                BeginReceive();
            else
            {
                if (_currentPacketLength < 0)
                {
                    _currentPacketLength = BitConverter.ToInt32(_buffer.Take(4).ToArray(), 0);
                    //Console.WriteLine("Parse {0} payload length.", _currentPacketLength);
                }

                if (_buffer.Count < _currentPacketLength + 4)
                    BeginReceive();
                else
                {
                    var packetStr = Encoding.UTF8.GetString(_buffer.Skip(4).Take(_currentPacketLength).ToArray());

                    _buffer.RemoveRange(0, _currentPacketLength + 4);
                    _currentPacketLength = int.MinValue;

                    var receivedPacket = JsonConvert.DeserializeObject<Packet>(packetStr, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects
                    });

                    // To distinguish whether client is ingame or not.
                    if (!IsIngame)
                    {
                        Console.WriteLine("Request : Lobby [" + receivedPacket.MsgName + "]");
                        ProcessRequest(receivedPacket);
                    }
                    else
                    {
                        Console.WriteLine("Request : Ingame [" + receivedPacket.MsgName + "]");

                        InGameRequest(this, receivedPacket);
                    }

                    if(_buffer.Count > 0)
                        ProcessData();

                    BeginReceive();
                }
            }
        }

        /// <summary>
        /// Process requests of client.
        /// </summary>
        /// <param name="req"></param>
        private void ProcessRequest(Packet req)
        {
            switch (req.MsgName)
            {
                case "LoginInfo":
                    var uniqueId = JsonConvert.DeserializeObject<LoginInfo>(req.Data);
                    var result = GameManager.FindClient(this, uniqueId.DeviceId);

                    // Same client is existing in server already.
                    if (result)
                    {
                        Console.WriteLine("Reconnected.");
                        // Send packet to client. (Inform reconnection.)
                    }
                    else
                    {
                        UniqueId = uniqueId.DeviceId;
                        ProfileData = DatabaseManager.GetClientInfo(UniqueId);
                        GameManager.AddClient(this);
                        BeginSend(ProfileData);
                        Console.WriteLine("New client.");
                        IsLogin = true;
                    }
                    break;

                case "NickNameInfo":
                    var nickName = JsonConvert.DeserializeObject<NickNameInfo>(req.Data);
                    ProfileData.NickName = DatabaseManager.SetClientInfo(nickName.NickName, UniqueId);
                    BeginSend(new NickNameInfo
                    {
                        NickName = ProfileData.NickName
                    });
                    break;

                case "MyMatchingInfo":
                    MatchingData = JsonConvert.DeserializeObject<MyMatchingInfo>(req.Data);
                    GameManager.MatchingRequest(this);
                    break;

                case "QueueCancelReq":
                    GameManager.MatchingCancelRequest(this);
                    break;

                case "ExitProgramReq":
                    Close();
                    break;

                default:
                    Console.WriteLine("Received MsgName of client is wrong.");
                    break;
            }
        }
    }
}
