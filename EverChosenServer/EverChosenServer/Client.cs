using System;
using System.Collections.Generic;
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
        public LoginPacket LoginData;
        public MatchingPacket MatchingData;
        public IngamePacket InGameData;

        private readonly byte[] _buffer = new byte[1024];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="socket"></param>
        public Client(Socket socket)
        {
            Sock = socket;
        }

        /// <summary>
        /// Send packet to client.
        /// </summary>
        public void SendPacket(string msg, Client oppoClient)
        {
            Console.WriteLine(msg);         
            
            switch (msg)
            {
                case "OnSucceedMatching":
                    var opponentMatchData = oppoClient.MatchingData;
                    var p = new Packet(msg, opponentMatchData);
                    var sendBuf = new UTF8Encoding().GetBytes(
                        JsonConvert.SerializeObject(p, Formatting.Indented));

                    Sock.BeginSend(sendBuf, 0, sendBuf.Length,
                        SocketFlags.None, OnSendCallback, Sock);
                    break;
                default:
                    Console.WriteLine("Msg error");
                    break;
            }            
        }

        /// <summary>
        /// Process packet to send.
        /// </summary>
        /// <param name="ar"> Async State </param>
        private void OnSendCallback(IAsyncResult ar)
        {
            Console.WriteLine("OnSend..");
        }

        /// <summary>
        /// Receive asynchronous data
        /// </summary>
        public void BeginReceive()
        {
            if (!Sock.Connected) return;

            Array.Clear(_buffer, 0, _buffer.Length);

            // Message received from client is assigned to variable _buffer.
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
            
            x.Data = x.Data.Replace("\\\"", "\"");
            x.Data = x.Data.Substring(1, x.Data.Length - 2);

            switch (x.MsgName)
            {
                case "OnLoginRequest":
                    Console.WriteLine("Client unique ID : " + x.Data);
                    //GameManager.OnLoginRequest(this);

                    // Write code to get Login Information from DB (now temporary)
                    var nick = "Ragdoll";
                    var wins = 10;
                    var loses = 5;
                    // ...

                    LoginData = new LoginPacket
                    {
                        NickName = nick,
                        Wins = wins,
                        Loses = loses
                    };
                    break;
                case "OnMatchingRequest":
                    Console.WriteLine("Matching Request");
                    Console.Write(x.Data);
                    MatchingData = JsonConvert.DeserializeObject<MatchingPacket>(x.Data.ToString());
                    Console.WriteLine(
                        MatchingData.Id + " " + MatchingData.Tribe + " " + MatchingData.Spell);
                    GameManager.OnMatchingRequest(this);
                    break;
                case "OnInGameRequest":
                    Console.WriteLine("In Game Request");
                    InGameData = JsonConvert.DeserializeObject<IngamePacket>(x.Data);
                    break;
                case "OnExitRequest":
                    Close();
                    break;
                default:
                    Console.WriteLine("Received MsgName of client is wrong.");
                    break;
            }
            
            BeginReceive();
        }

        /// <summary>
        /// Close client socket
        /// </summary>
        public void Close()
        {
            Console.WriteLine("Close");
            Sock.Shutdown(SocketShutdown.Both);
            Sock.Close();
            GameManager.ReleaseClient(this);
        }
    }
}
