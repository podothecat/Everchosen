using UnityEngine;
using System.Net.Sockets;
using System;
using System.Text;



namespace Client
{

    static class ClientNetworkManager
    {
        private static Socket _clientSocket = null;
        private static byte[] buffer = new byte[1024];

        public static void ConnectToServer(string hostName, int hostPort)
        {
           
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //연결성공
                _clientSocket.BeginConnect(hostName, hostPort, ConnectCallback, _clientSocket);
            
            }
            catch (Exception e)
            {
                Debug.Log(e);
                //연결실패
               Debug.Log("연결에 실패했습니다.");

            }
        }
        

        //beginReceive 콜백함수
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                var tempSocket = (Socket)ar.AsyncState;
                tempSocket.EndConnect(ar);
                var serverSocket = tempSocket;


                if (serverSocket.Connected == true)
                {
                  Debug.Log("연결됨");
                }

                
                serverSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallBack, serverSocket);
                


            }
            catch (Exception e)
            {
                Debug.Log(e);
               Debug.Log("연결에 실패했습니다.2");

            }

        }





        //데이터 전송
        public static void Send(string msg)
        {
            try
            {
                if (_clientSocket.Connected)
                {
                    byte[] buffer = new UTF8Encoding().GetBytes(msg);
                    _clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, msg);

                }
                else
                {
                    //소켓이 연결되어있지않음
                }
            }
            catch (Exception)
            {
                Console.WriteLine("전송에러");
            }

        }

        private static void SendCallBack(IAsyncResult ar)
        {
            string message = (string)ar.AsyncState; //완료메시지 같은거 보내기..
        }

        
        //수신콜백함수
        private static void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                Socket tempSocket = (Socket)ar.AsyncState;
                int readSize = tempSocket.EndReceive(ar);


                if (readSize != 0)
                {

                    string ReceiveMsg = new UTF8Encoding().GetString(buffer, 0, readSize);
                    Console.WriteLine(ReceiveMsg);
                }
            }
            catch (SocketException e)
            {
                //데이저 수신 에러
            }
        }

    }

}