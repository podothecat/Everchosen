using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EverChosenServer
{
    class Program
    {
        private readonly Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            var p = new Program();

            p.Start();
        }

        private void Start()
        {
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 23000));
            _serverSocket.Listen(10);

            _serverSocket.BeginAccept(OnAccept, _serverSocket);

            while (true)
            {
                Task.Delay(1000);
            }
        }

        private void OnAccept(IAsyncResult ar)
        {
            var socket = (Socket)ar.AsyncState;
            var clientSocket = socket.EndAccept(ar);

            var newClient = new Client(clientSocket);
            newClient.BeginReceive();
            GameManager._clients.Add(newClient);

            _serverSocket.BeginAccept(OnAccept, _serverSocket);
        }

        interface Packet
        {

        }
    }
}
