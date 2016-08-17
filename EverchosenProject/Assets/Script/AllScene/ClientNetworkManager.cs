using System.Net.Sockets;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;
using LitJson;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

//2.0만사용가능 3.5이후로는 호환이 안됨 유니티


namespace Client
{

    static class ClientNetworkManager 
    {
        public static Socket _clientSocket = null;
        private static Socket _serverSocket = null;
        private static readonly byte[] _buffer = new byte[1024];

        // Save device unique id.
        public static string ClientDeviceId;

        public static bool Connected = false;
        public static MatchingPacket PacketData; // 유니티에서 사용할 데이터를 담을변수
        
        public static string ReceiveMsg = null; //유니티쪽에서 사용할 메시지를 담을 변수
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
                _serverSocket = tempSocket;

                if (_serverSocket.Connected == true)
                {
                    Debug.Log("연결됨");
                }

             
                Receive();
                Connected = true;

            }
            catch (Exception e)
            {
                Debug.Log(e);
               Debug.Log("연결에 실패했습니다.2");
            }

        }

       

        //매칭된시간 알려주고 3초뒤에시작한다 . 
        //지금이 19분인데 19분 30초에 매칭시작 33초에 게임시작해 패킷을 보낸다음에 
        //데이터 전송
        public static void Send(string msg, object data)
        {
            try
            {
                if (_clientSocket.Connected)
                {
                    var setData = data;
                    Packet sample = new Packet
                    {
                        MsgName = msg,
                        Data = JsonConvert.SerializeObject(setData)
                    };

                   // Debug.Log();
                    var json = JsonConvert.SerializeObject(sample);
                    var sendData = Encoding.UTF8.GetBytes(json);
                    _clientSocket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, SendCallBack, _clientSocket);
                    
                }
                else
                {
                    //소켓이 연결되어있지않음
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("전송에러");
            }
        }

        private static void SendCallBack(IAsyncResult ar)
        {
            string message = (string)ar.AsyncState; //완료메시지 같은거 보내기..
        }







#region receive

        public static void Receive()
        {
            Array.Clear(_buffer, 0, _buffer.Length);
            try
            {
                _serverSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallBack, _serverSocket);
            }
            catch (Exception e)
            {
                
               Debug.Log(e);
            }
           
        }

    
        //수신콜백함수
        private static void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                //var tempSocket = (Socket)ar.AsyncState;
               // int readSize = tempSocket.EndReceive(ar);//버퍼 사이즈 받아옴
               
                var receiveJson = new UTF8Encoding().GetString(_buffer);
                
                var receiveData = JsonConvert.DeserializeObject<Packet>(receiveJson);
                ReceiveMsg = receiveData.MsgName;
                Debug.Log("test : " + receiveData.Data);

                switch (ReceiveMsg)
                {
                    case "OnSucceedMatching":
                        PacketData = JsonConvert.DeserializeObject<MatchingPacket>(receiveData.Data);
                        break;
                    case "InGame":
                        break;
                }

                if (_serverSocket.Connected == true)
                {
                    Receive();
                }
                    
            }
            catch (SocketException e)
            {
                Debug.Log("Socket error : " + e);
                //데이저 수신 에러
            }
            catch (Exception e)
            {
                Debug.Log("exeption 에러 : "+e);
            }

            var matchingPakcet = new MatchingPacket("id","tribe",0,1);

            var packetwrapper = new Packet()
            {
                MsgName = "MatchingPacket",
                Data = JsonConvert.SerializeObject(matchingPakcet)
            };
        }
        
        public static void SocketClose()
        {
            if (!_clientSocket.Connected) return;

            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }
    }

#endregion
    
    public class Packet
    {
        public string MsgName { get; set; }
        public string Data { get; set; }
    }

    public class MatchingPacket
    {
        public string Id { get; set; }
        public string Tribe { get; set;}
        public int Spell { get; set; }
        public int TeamColor { get; set;}
      

        public MatchingPacket(string nickName, string tribe, int spell, int teamColor)
        {
            this.Id = nickName;
            this.Tribe = tribe;
            this.Spell = spell;
            this.TeamColor = teamColor;

        }
    }

    public class IngamePacket
    {
        
    }
}
