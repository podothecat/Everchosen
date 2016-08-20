using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EverChosenServer
{
    internal class Client
    {
        public Socket Sock { get; set; }
        public ProfilePacket LoginData;
        public MatchingPacket MatchingData;
        public IngamePacket InGameData;

        public bool IsIngame;

        private readonly byte[] _buffer = new byte[1024];

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
        /// Send packet to client.
        /// </summary>
        public void BeginSend(string msg, dynamic data)
        {
            var p = new Packet(msg, data);
            var sendBuf = new UTF8Encoding().GetBytes(
                JsonConvert.SerializeObject(p, Formatting.Indented));
           
            Sock.BeginSend(sendBuf, 0, sendBuf.Length, SocketFlags.None, OnSendCallback, Sock);
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
        /// Receive asynchronous data
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

            ProcessRequest(x);
            
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
                    GameManager.LoginRequest(this);
                    break;
                case "OnMatchingRequest":
                    Console.WriteLine("Request : Matching");
                    MatchingData = JsonConvert.DeserializeObject<MatchingPacket>(req.Data);
                    GameManager.MatchingRequest(this);
                    break;
                case "OnMatchingCancelRequest":
                    Console.WriteLine("Request : Matching Cancel");
                    GameManager.MatchingCancelRequest(this);
                    break;
                case "OnInGameRequest":
                    Console.WriteLine("Request : Ingame");
                    InGameData = JsonConvert.DeserializeObject<IngamePacket>(req.Data);
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
    }
}
