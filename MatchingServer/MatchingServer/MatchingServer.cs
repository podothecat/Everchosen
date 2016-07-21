using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MatchingServer
{
    class MatchingServer
    {
        /// <summary>
        /// Connected clients list.
        /// </summary>
        private List<Socket> clientsSocketList = null;

        /// <summary>
        /// Server socket for open and close.
        /// </summary>
		private Socket serverSocket = null;

        /// <summary>
        /// Delegates for processing asynchronous job
        /// </summary>
		private AsyncCallback m_fnReceiveHandler;
		private AsyncCallback m_fnSendHandler;
		private AsyncCallback m_fnAcceptHandler;
		
        /// <summary>
        /// Constructor of matching server.
        /// </summary>
		public MatchingServer() 
        {
            Console.WriteLine("Matching Server Constructor was called.");
            clientsSocketList = new List<Socket>();
			m_fnReceiveHandler = handleDataReceive;
			m_fnSendHandler = handleDataSend;
			m_fnAcceptHandler = handleClientConnectionRequest;
		}
		
        /// <summary>
        /// Method for start server program.
        /// </summary>
        /// <param name="port"> Port number of server computer. </param>
		public void StartServer(UInt16 port) 
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            // Bind socket to endpoint of local(server).
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));

            // Set server can get request of client.
            serverSocket.Listen(5);

            // Start asynchronous job of connection attempt.
            serverSocket.BeginAccept(m_fnAcceptHandler, null);

            Console.WriteLine("Server was open successfully.");
		}
		
        /// <summary>
        /// Method for stop server.
        /// </summary>
		public void StopServer() 
        {
            serverSocket.Close();
		}
		
        /// <summary>
        /// Called when client requests connection to server.
        /// </summary>
        /// <param name="ar"> ... </param>
		private void handleClientConnectionRequest(IAsyncResult ar)
		{
		    Socket clientSocket;
		    try
		    {
                 clientSocket = serverSocket.EndAccept(ar);
		    }
		    catch (Exception e)
		    {
		        Console.WriteLine("Connection error to client. : " + e.Message);
		        return;
		    }

            clientsSocketList.Add(clientSocket);
            Console.WriteLine("Client was connected.");
            Console.WriteLine("The number of connected client : " + clientsSocketList.Count);

		    Byte[] buf;
            buf = new Byte[256];

		    try
		    {
                clientSocket.BeginReceive(buf, 0, buf.Length, SocketFlags.None, m_fnReceiveHandler, null);
		    }
		    catch (Exception e)
		    {
                Console.WriteLine("Receiving data error from client. : " + e.Message);
		        return;
		    }
		}

        /// <summary>
        /// Called when receive data from client.
        /// </summary>
        /// <param name="ar"></param>
		private void handleDataReceive(IAsyncResult ar) 
        {
		    
		}

        /// <summary>
        /// Called when send data to client.
        /// </summary>
        /// <param name="ar"></param>
		private void handleDataSend(IAsyncResult ar) 
        {
		
		}


    }
}
