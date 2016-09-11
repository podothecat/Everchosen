using System;
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
        public MatchingInfo MatchingData;
        public bool IsIngame;
        public bool IsReadyToBattle;

        private readonly byte[] _buffer = new byte[1024];
        private string _uniqueId { get; set; }

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
            IsIngame = false;
        }
        
        /// <summary>
        /// Send data to client.
        /// </summary>
        public void BeginSend(Packet packet)
        {
            var sendBuf = new UTF8Encoding().GetBytes(
                JsonConvert.SerializeObject(packet, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                }));

            Sock.BeginSend(sendBuf, 0, sendBuf.Length, SocketFlags.None, OnSendCallback, Sock);
            Console.WriteLine("Send : " + packet.MsgName + ", " + packet.Data);
        }

        /// <summary>
        /// Receive asynchronous data from client.
        /// </summary>
        public void BeginReceive()
        {
            if (!Sock.Connected) return;

            // Initialize buffer when each time it receives packet.
            Array.Clear(_buffer, 0, _buffer.Length);

            try
            {
                Sock.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceiveCallback, Sock);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Process packet to send.
        /// </summary>
        /// <param name="ar"> Async State </param>
        private void OnSendCallback(IAsyncResult ar)
        {
            //Console.WriteLine("OnSend..");
        }

        /// <summary>
        /// Process received packet.
        /// </summary>
        /// <param name="ar"> Asynchronous state result. </param>
        private void OnReceiveCallback(IAsyncResult ar)
        {
            var clientSock = (Socket)ar.AsyncState;
            
            // Unexpected request (ex : force quit)
            if (_buffer[0] == 0)
            {
                Console.WriteLine("Unexpected Request. Remove client.");
                Close();
                return;
            }

            var packetStr = Encoding.UTF8.GetString(_buffer);
            
            var receivedPacket = JsonConvert.DeserializeObject<Packet>(packetStr, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });

            // Refine received packet.
            //receivedPacket.Data = receivedPacket.Data.Replace("\\\"", "\"");
            //x.Data = x.Data.Substring(0, x.Data.Length - 1);

            // To distinguish whether client is ingame or not.
            if (!IsIngame)
            {
                Console.WriteLine("Request : Lobby [" + receivedPacket.MsgName + "], ["
                                  + receivedPacket.Data + "]");
                ProcessRequest(receivedPacket);
            }
            else
            {
                Console.WriteLine("Request : Ingame [" + receivedPacket.MsgName + "], ["
                                  + receivedPacket.Data + "]");

                InGameRequest(this, receivedPacket);
            }

            BeginReceive();
        }

        /// <summary>
        /// Close client socket
        /// </summary>
        private void Close()
        {
            Console.WriteLine("Close");
            Sock.Shutdown(SocketShutdown.Both);
            Sock.Close();
            GameManager.ReleaseClient(this);
        }

        /// <summary>
        /// Processing requests of client.
        /// </summary>
        /// <param name="req"></param>
        private void ProcessRequest(Packet req)
        {
            switch (req.MsgName)
            {
                case "LoginInfo":
                    Console.WriteLine("Request : Login");
                    Console.WriteLine(req.Data);
                    var uniqueId = JsonConvert.DeserializeObject<LoginInfo>(req.Data);
                    _uniqueId = uniqueId.DeviceId;
                    ProfileData = DatabaseManager.GetClientInfo(_uniqueId);
                    Console.WriteLine(_uniqueId);
                    BeginSend(ProfileData);
                    break;

                case "NickNameInfo":
                    Console.WriteLine("Request : Setting");
                    var nickName = JsonConvert.DeserializeObject<NickNameInfo>(req.Data);
                    ProfileData.NickName = DatabaseManager.SetClientInfo(nickName.NickName, _uniqueId);
                    BeginSend(new NickNameInfo
                    {
                        NickName = ProfileData.NickName
                    });
                    break;

                case "MatchingInfo":
                    Console.WriteLine("Request : Matching");
                    MatchingData = JsonConvert.DeserializeObject<MatchingInfo>(req.Data);
                    GameManager.MatchingRequest(this);
                    break;

                case "QueueCancelReq":
                    Console.WriteLine("Request : Matching Cancel");
                    GameManager.MatchingCancelRequest(this);
                    break;

                case "ExitProgramReq":
                    Console.WriteLine("Request : Exit");
                    Close();
                    break;

                default:
                    Console.WriteLine("Received MsgName of client is wrong.");
                    break;
            }
        }
    }
}
