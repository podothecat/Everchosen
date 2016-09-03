using System.Net.Sockets;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using EverChosenPacketLib;
//2.0만사용가능 3.5이후로는 호환이 안됨 유니티

namespace Client
{
    static class ClientNetworkManager
    {
       
        public static Socket ClientSocket = null;
        private static Socket _serverSocket = null;
        private static readonly byte[] Buffer = new byte[1024];
        
        public static string ClientDeviceId;
        public static bool Connected = false;

        public static ProfileInfo EnemyProfileData;
        public static ProfileInfo ProfileData;
        public static MatchingInfo EnemyMatchingData;
        public static MapInfo MapInfo;

        //인게임
        public static MoveUnitInfo MyMoveUnitInfo;
        public static MoveUnitInfo EnemyMoveUnitInfo;
        public static ChangeBuildingInfo MyInfo;
        public static ChangeBuildingInfo EnemyInfo;

        public static string ReceiveMsg = null; //유니티쪽에서 사용할 메시지를 담을 변수

        #region Connect / Close Function  

        public static void ConnectToServer(string hostName, int hostPort)
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                ClientSocket.BeginConnect(hostName, hostPort, ConnectCallback, ClientSocket);
            }
            catch (Exception e)
            {
                Debug.Log("ConnectToServer 연결 실패 관련 e :" + e);
            }
        }

        //beginReceive 콜백함수
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                var tempSocket = (Socket) ar.AsyncState;
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
                Debug.Log("ConnectCallback 연결 실패 관련 e :"+e);
            }

        }

        public static void SocketClose()
        {
            if (!ClientSocket.Connected) return;
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
        }

        #endregion

        #region Send Function

        //데이터 전송
        public static void Send(string msg, object data)
        {
            try
            {
                    var setData = data;
                    Packet sample = new Packet(msg, JsonConvert.SerializeObject(setData));
                    var json = JsonConvert.SerializeObject(sample);
                    var sendData = Encoding.UTF8.GetBytes(json);
                    ClientSocket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, SendCallBack, ClientSocket);
                
            }
            catch (Exception e)
            {
                Debug.Log("Send Fail 관련 e : "+e);
            }
        }

        private static void SendCallBack(IAsyncResult ar)
        {
            string message = (string) ar.AsyncState; 
        }

        #endregion

        #region Receive Function

        public static void Receive()
        {
            Array.Clear(Buffer, 0, Buffer.Length);
            try
            {
                _serverSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, ReceiveCallBack, _serverSocket);
            }
            catch (Exception e)
            {
                Debug.Log("Receive Exeption : " + e);
            }
        }

        //수신콜백함수
        private static void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                //var tempSocket = (Socket)ar.AsyncState;
                // int readSize = tempSocket.EndReceive(ar);//버퍼 사이즈 받아옴
                if (Buffer[0] == 0)
                {
                    Debug.Log("Buffer null");
                    return;
                }
                var receiveJson = new UTF8Encoding().GetString(Buffer);
                Debug.Log(receiveJson);
                var receiveData = JsonConvert.DeserializeObject<Packet>(receiveJson);
                ReceiveMsg = receiveData.MsgName;



                switch (ReceiveMsg)
                {
                    //로딩씬
                    case "OnSucceedLogin":
                        ProfileData = JsonConvert.DeserializeObject<ProfileInfo>(receiveData.Data);
                        break;

                    //메인메뉴
                    case "OnChangedProfile":
                        ProfileData.NickName = JsonConvert.DeserializeObject<string>(receiveData.Data);
                        break;
                    case "OnSucceedMatching1": //종족 스펠
                        EnemyMatchingData = JsonConvert.DeserializeObject<MatchingInfo>(receiveData.Data);
                        break;
                    case "OnSucceedMatching2": //닉네임, 승패
                        EnemyProfileData = JsonConvert.DeserializeObject<ProfileInfo>(receiveData.Data);
                        Send("MapReq", null);
                        break;
                    case "MapInfo": //맵데이터 
                        MapInfo = JsonConvert.DeserializeObject<MapInfo>(receiveData.Data);
                        break;

                    //ingame
                    case "MoveMine":
                        MyMoveUnitInfo = JsonConvert.DeserializeObject<MoveUnitInfo>(receiveData.Data);
                        break;
                    case "MoveOppo":
                        EnemyMoveUnitInfo = JsonConvert.DeserializeObject<MoveUnitInfo>(receiveData.Data);
                        break;
                    case "ChangeMine":
                        MyInfo = JsonConvert.DeserializeObject<ChangeBuildingInfo>(receiveData.Data);
                        break;
                    case "ChangeOppo":
                        EnemyInfo = JsonConvert.DeserializeObject<ChangeBuildingInfo>(receiveData.Data);
                        break;
                }
                if (_serverSocket.Connected == true)
                {
                    Receive();
                }

            }
         
            catch (Exception e)
            {
                Debug.Log("ReceiveCallBack Function Exeption : " + e);
            }
        }
    }
}

#endregion



