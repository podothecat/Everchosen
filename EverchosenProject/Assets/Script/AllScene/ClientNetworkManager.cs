using System.Net.Sockets;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Debug = UnityEngine.Debug;
using EverChosenPacketLib;
using UnityEngine.SceneManagement;

//2.0만사용가능 3.5이후로는 호환이 안됨 유니티

namespace Client
{
    static class ClientNetworkManager
    {
       
        public static Socket ClientSocket = null;
        private static Socket _serverSocket = null;
        private static readonly byte[] Buffer = new byte[4096];
        
        public static string ClientDeviceId;
        public static bool Connected = false;
        public static bool ReconnectedIngame = false;
        public static bool IsReadyToPlay = false;

        public static EnemyProfileInfo EnemyProfileData;
        public static MyProfileInfo ProfileData;
        public static EnemyMatchingInfo EnemyMatchingData;
        public static MapInfo MapInfo;

        //인게임
        public static UnitInfo MyMoveUnitInfo;
        public static UnitInfo EnemyMoveUnitInfo;
        public static ChangeBuildingInfo MyInfo;
        public static ChangeBuildingInfo EnemyInfo;
        public static ChangeBuildingInfo BuildingInfo;
        public static CreateUnitInfo IncrementeUnitInfo;
        public static FightResultInfo FightResultinfo;
        

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
        public static void Send(Packet packet) 
        {
            try
            {
                var json = JsonConvert.SerializeObject(packet, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                });
                
                var sendData = Encoding.UTF8.GetBytes(json);
                var sendDataLength = BitConverter.GetBytes(sendData.Length);

                var totalBuf = new byte[sendData.Length + 4];
                System.Buffer.BlockCopy(sendDataLength, 0, totalBuf, 0, 4);
                System.Buffer.BlockCopy(sendData, 0, totalBuf, 4, sendData.Length);

                Debug.Log("Sent " + packet.MsgName);
                ClientSocket.BeginSend(totalBuf, 0, totalBuf.Length, SocketFlags.None, SendCallBack, ClientSocket);
            }
            catch (Exception e)
            {
                Debug.Log("Send Fail 관련 e : "+e);
            }
        }

        private static void SendCallBack(IAsyncResult ar)
        {
            var sock = (Socket)ar.AsyncState;
            var size = sock.EndSend(ar);
            //Debug.Log("Sent " + size + " bytes to server.");
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


        private static List<byte> _buffer = new List<byte>();
        private static int _currentPacketLength = int.MinValue;

        //수신콜백함수
        private static void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                var tempSocket = (Socket)ar.AsyncState;
                int size = tempSocket.EndReceive(ar);//버퍼 사이즈 받아옴

               // Debug.Log("Receive " + size + " bytes from server.");

                _buffer.AddRange(Buffer.ToArray().Take(size));

                ProcessData();
            }
            catch (Exception e)
            {
                Debug.Log("ReceiveCallBack Function Exeption : " + e);
            }
        }

        private static void ProcessData()
        {
            //Debug.Log("Buffer size : " + _buffer.Count);
            
            if (_buffer.Count < 4)
                Receive();
            else
            {
                if (_currentPacketLength < 0)
                {
                    _currentPacketLength = BitConverter.ToInt32(_buffer.Take(4).ToArray(), 0);
                  //  Debug.Log("Payload length " + _currentPacketLength);
                }

                if (_buffer.Count < _currentPacketLength + 4)
                    Receive();
                else
                {
                    var receiveJson = Encoding.UTF8.GetString
                    (
                        _buffer.Skip(4).Take(_currentPacketLength).ToArray()
                    );

                    _buffer.RemoveRange(0, _currentPacketLength + 4);
                    _currentPacketLength = int.MinValue;

                    //Debug.Log(receiveJson.Length + "\n" + receiveJson);

                    var receiveData = JsonConvert.DeserializeObject<Packet>(receiveJson, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects
                    });
                    ReceiveMsg = receiveData.MsgName;
                    
                    switch (ReceiveMsg)
                    {
                        //로딩씬
                        case "MyProfileInfo":
                            ProfileData = JsonConvert.DeserializeObject<MyProfileInfo>(receiveData.Data);
                            break;

                        //메인메뉴
                        case "NickNameInfo":
                            var nickName = JsonConvert.DeserializeObject<NickNameInfo>(receiveData.Data);
                            ProfileData.NickName = nickName.NickName;
                            break;
                        case "MyMatchingInfo":

                            break;
                        case "EnemyMatchingInfo": //종족 스펠
                            EnemyMatchingData = JsonConvert.DeserializeObject<EnemyMatchingInfo>(receiveData.Data);
                            break;
                        case "EnemyProfileInfo": //닉네임, 승패
                            EnemyProfileData = JsonConvert.DeserializeObject<EnemyProfileInfo>(receiveData.Data);
                            Send(new MapReq { Req = "Map" });
                            break;
                        case "MapInfo": //맵데이터                             
                            MapInfo = JsonConvert.DeserializeObject<MapInfo>(receiveData.Data);
                            break;

                        case "ReconnectToIngameInfo":
                            var result = JsonConvert.DeserializeObject<ReconnectToIngameInfo>(receiveData.Data);
                            MapInfo = result.Map;
                            switch (result.MyMatchingData.Tribe)
                            {
                                case "Chaos":
                                    TribeSetManager.PData.Tribe = 0;
                                    break;
                                case "Dwarf":
                                    TribeSetManager.PData.Tribe = 1;
                                    break;
                                case "Green":
                                    TribeSetManager.PData.Tribe = 2;
                                    break;
                                case "Human":
                                    TribeSetManager.PData.Tribe = 3;
                                    break;
                            }
                            TribeSetManager.PData.TribeName = result.MyMatchingData.Tribe;
                            TribeSetManager.PData.Spell = result.MyMatchingData.Spell;
                            ProfileData = result.MyProfile;
                            EnemyMatchingData = result.EnemyMatchingData;
                            EnemyProfileData = result.EnemyProfile;
                            ReconnectedIngame = true;
                            Debug.Log("Reconnect");
                            break;
                    }

                    if (IsReadyToPlay)
                    {
                        switch (ReceiveMsg)
                        {
                            //ingame
                            case "UnitInfo":
                                var units = JsonConvert.DeserializeObject<UnitInfo>(receiveData.Data);

                                if (units.Units.Owner == 1)
                                    EnemyMoveUnitInfo = units;
                                else if (units.Units.Owner == 2)
                                    MyMoveUnitInfo = units;
                                break;
                            case "ChangeBuildingInfo":
                                var building = JsonConvert.DeserializeObject<ChangeBuildingInfo>(receiveData.Data);
                                BuildingInfo = building;
                                break;
                            case "CreateUnitInfo":
                                Debug.Log("CreateUnitInfo Message.");
                                var createInfo = JsonConvert.DeserializeObject<CreateUnitInfo>(receiveData.Data);
                                IncrementeUnitInfo = createInfo;
                                break;
                            case "FightResultInfo":
                                Debug.Log(receiveData.Data);
                                FightResultinfo = JsonConvert.DeserializeObject<FightResultInfo>(receiveData.Data);
                                break;
                            case "OutcomeInfo":
                                var result = JsonConvert.DeserializeObject<OutcomeInfo>(receiveData.Data);
                                Debug.Log("Outcome : " + result.Outcome);
                                break;
                        }
                    }

                    if (_buffer.Count > 0)
                        ProcessData();

                    //Debug.Log("Buffer size after process : " + _buffer.Count);
                    Receive();
                }
            }

            
        }
    }
}

#endregion

