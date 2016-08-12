using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EverChosenServer
{
    internal class Client
    {
        public Socket Sock { get; set; }
        public Test1 MatchingData;
        public Test2 InGameData;

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
        /// Process received packet.
        /// </summary>
        /// <param name="ar"> Asynchronous state result. </param>
        private void OnReceive(IAsyncResult ar)
        {
            Console.WriteLine("OnReceive..");
            var clientSock = (Socket)ar.AsyncState;

            var packetStr = Encoding.UTF8.GetString(_buffer);
            
            //JsonConverter[] converters = {new PacketConverter()};
            //var x = JsonConvert.DeserializeObject<Packet>(packetStr, 
            //    new JsonSerializerSettings() { Converters = converters });            

            var x = JsonConvert.DeserializeObject<Packet>(packetStr);
            Console.WriteLine(string.IsNullOrEmpty(x.MsgName) ? 
                "MsgName has no request." : x.MsgName);
            switch (x.MsgName)
            {
                case "OnMatchingRequest":
                    MatchingData = JsonConvert.DeserializeObject<Test1>(x.Data.ToString());
                    Console.WriteLine(
                        MatchingData.Id + " " + MatchingData.Tribe + " " + MatchingData.Spell);
                    //MatchingData = new Test1();
                    //MatchingData.ParseData(x.Data);
                    GameManager.OnMatchingRequest(this);
                    break;
                case "OnInGameRequest":
                    InGameData = x.Data;
                    Console.WriteLine("In Game Request");
                    break;
                case "OnExitRequest":
                    Close();
                    break;
                default:
                    Console.WriteLine("Received MsgName of client is wrong.");
                    break;
            }
        }

        /// <summary>
        /// Process packet to send.
        /// </summary>
        /// <param name="ar"> Async State </param>
        private void OnSend(IAsyncResult ar)
        {
            Console.WriteLine("OnSend..");
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
                        SocketFlags.None, OnSend, Sock);
                    break;
                default:
                    Console.WriteLine("Msg error");
                    break;
            }            

            //c.Sock.BeginSend(sendBuf, 0, sendBuf.Length, SocketFlags.None, OnSend, c.Sock);
            //Sock.BeginSend(sendBuf, 0, sendBuf.Length, SocketFlags.None, OnSend, Sock);
        }

        /// <summary>
        /// Asynchronous receive data callback
        /// </summary>
        public void BeginReceive()
        {
            // Message received from client is assigned to variable _buffer.
            Sock.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceive, Sock);
        }

        /// <summary>
        /// Close client socket
        /// </summary>
        public void Close()
        {
            Sock.Shutdown(SocketShutdown.Both);
            Sock.Close();
            GameManager.ReleaseClient(this);
        }
    }
}
