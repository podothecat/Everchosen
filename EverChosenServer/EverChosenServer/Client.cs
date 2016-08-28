using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.Core.WireProtocol.Messages;
using Newtonsoft.Json;

namespace EverChosenServer
{
    internal class Client
    {
        public Socket Sock { get; set; }
        public ProfileInfo LoginData;
        public MatchingInfo MatchingData;

        public bool IsIngame;

        private readonly byte[] _buffer = new byte[1024];
        private string _uniqueId { get; set; }

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
        public void BeginSend(string msg, dynamic data)
        {
            var p = new Packet(msg, data);
            var sendBuf = new UTF8Encoding().GetBytes(
                JsonConvert.SerializeObject(p, Formatting.Indented));

            Sock.BeginSend(sendBuf, 0, sendBuf.Length, SocketFlags.None, OnSendCallback, Sock);
            Console.WriteLine("Send : " + p.MsgName + ", " + p.Data);
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
            
            var x = JsonConvert.DeserializeObject<Packet>(packetStr);
            
            // Refine received packet.
            x.Data = x.Data.Replace("\\\"", "\"");
            x.Data = x.Data.Substring(1, x.Data.Length - 2);

            // To distinguish whether client is ingame or not.
            if (!IsIngame)
                ProcessRequest(x);
            else
            {
                Console.WriteLine("Request : Ingame [" + x.MsgName + "], [" + x.Data + "]");
                
                InGameRequest(this, x);
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
                case "OnLoginRequest":
                    Console.WriteLine("Request : Login");
                    _uniqueId = JsonConvert.DeserializeObject<string>(req.Data);
                    LoginData = DatabaseManager.GetClientInfo(_uniqueId);
                    Console.WriteLine(_uniqueId);
                    BeginSend("OnSucceedLogin", LoginData);
                    break;

                case "OnNickChangeRequest":
                    Console.WriteLine("Request : Setting");
                    var nickName = JsonConvert.DeserializeObject<string>(req.Data);
                    LoginData.NickName = DatabaseManager.SetClientInfo(nickName, _uniqueId);
                    BeginSend("OnChangedProfile", LoginData.NickName);
                    break;

                case "OnMatchingRequest":
                    Console.WriteLine("Request : Matching");
                    MatchingData = JsonConvert.DeserializeObject<MatchingInfo>(req.Data);
                    GameManager.MatchingRequest(this);
                    break;

                case "OnMatchingCancelRequest":
                    Console.WriteLine("Request : Matching Cancel");
                    GameManager.MatchingCancelRequest(this);
                    break;

                case "OnExitRequest":
                    Console.WriteLine("Request : Exit");
                    Close();
                    break;

                default:
                    Console.WriteLine("Received MsgName of client is wrong.");
                    break;
            }
        }

        /// <summary>
        /// Ingame Event Handler.
        /// When request is arrived, then call attached method.
        /// </summary>
        public event EventHandler<Packet> InGameRequest;
    }
}
