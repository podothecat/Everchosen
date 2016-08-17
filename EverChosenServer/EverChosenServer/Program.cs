using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EverChosenServer
{
    internal class Program
    {
        private readonly Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static void Main(string[] args)
        {
            var p = new Program();

            p.Start();
        }

        /// <summary>
        /// Open server socket and wait connection.
        /// </summary>
        private void Start()
        {
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 23000));
            _serverSocket.Listen(10);

            _serverSocket.BeginAccept(OnAcceptCallback, _serverSocket);

            Console.WriteLine("Server On\n");
            while (true)
            {
                Task.Delay(1000);
            }
        }

        /// <summary>
        /// Accept connection request of client.
        /// </summary>
        /// <param name="ar"></param>
        private void OnAcceptCallback(IAsyncResult ar)
        {
            var socket = (Socket)ar.AsyncState;
            var clientSocket = socket.EndAccept(ar);
            
            var newClient = new Client(clientSocket);
            GameManager.AddClient(newClient);
            newClient.BeginReceive();
            
            _serverSocket.BeginAccept(OnAcceptCallback, _serverSocket);
        }
    }
}
