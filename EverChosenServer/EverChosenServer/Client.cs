using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EverChosenServer
{
    class Client
    {
        public Socket Sock;
        public MatchingRequestParam MatchParam;

        private readonly byte[] _buffer = new byte[1024];

        public Client(Socket socket)
        {
            Sock = socket;
        }

        private void OnReceive(IAsyncResult ar)
        {
            var client = (Socket)ar.AsyncState;

            var packetStr = Encoding.UTF8.GetString(_buffer);

            //var x = JsonConvert.DeserializeObject<IPacket>(packetStr);

            //switch (x.Name)
            //{
            //    case "OnMachingRequest":
            //        GameManager.OnMatchingRequest(client, x.Data);
            //        break;
            //}
        }

        public void SendPacket()
        {

        }

        public void BeginReceive()
        {
            Sock.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceive, Sock);
        }
    }
}
