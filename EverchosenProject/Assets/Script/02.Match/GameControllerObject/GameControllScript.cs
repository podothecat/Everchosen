using UnityEngine;
using System.Collections;
using Client;
using UnityEngine.UI;
using System.Collections.Generic;
using EverChosenPacketLib;

public class GameControllScript : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject MatchingDataViewPanelPrefab;
    private GameObject _matchingDataViewPanel;

    private GameObject _player1BuildingPrefab;
    private GameObject _player1Building;

    private GameObject _player2BuildingPrefab;
    private GameObject _player2Building;

    private GameObject _emptyBuildingPrefab;
    private GameObject _emptyBuilding;
    
    public GameObject ParentObject;

    private MatchingInfo _enemyViewPanelSetdata;
    private EnemyProfileInfo _enemyViewPanelProfileData;
    
    public List<GameObject> NodePosition;
    public List<GameObject> BuildingNode;
    
    private Sprite _mapSpriteData;
    private SpriteRenderer _backGroundObject;
    
    void Awake()
    {
        _player1BuildingPrefab = Resources.Load<GameObject>("Player1building");
        _player2BuildingPrefab = Resources.Load<GameObject>("Player2building");
        _emptyBuildingPrefab = Resources.Load<GameObject>("EmptyBuilding");
        _backGroundObject = GameObject.Find("MapObject").transform.FindChild("background").GetComponent<SpriteRenderer>();
    }
    void Start ()
    {
        DataSetting();//서버에서 받아온 데이터처리
    }
	
    //시스템시간 , 게임이시작됫을때 틱을 저장 서버에서 게임시작됬을떄 틱에서 얼마나 지낫는지 보내주고 클라에서도 
    //예를들어 시작시간이 0초 , 시스템틱이라는게 있음, (어떤기준시간을 가지고서 어느정도 지났는지 잼)
    //time.deltatime 은 프레임 갱신시간 

	void Update ()
	{
	    IngameFunction();
	}

//Server's receive Data reraltion
    void DataSetting()
    {
        if (ClientNetworkManager.EnemyMatchingData != null && ClientNetworkManager.EnemyProfileData != null && ClientNetworkManager.MapInfo != null)
        {
            _mapSpriteData = Resources.Load<Sprite>("Sprite/MapData/" + ClientNetworkManager.MapInfo.MapName);
            _backGroundObject.sprite = _mapSpriteData;
            
            _enemyViewPanelSetdata = ClientNetworkManager.EnemyMatchingData;
            _enemyViewPanelProfileData = ClientNetworkManager.EnemyProfileData;
            MatchingDataViewIns();//매칭데이터 패널 생성
            ClientNetworkManager.EnemyMatchingData = null;
            ClientNetworkManager.EnemyProfileData = null;
        }
        else
        {
            Debug.Log("데이터 관련이 존재하지 않습니다.");
            Debug.Log("EnemyMatchingData : " + ClientNetworkManager.EnemyMatchingData);
            Debug.Log("EnemyProfileData :" + ClientNetworkManager.EnemyProfileData);
            Debug.Log("MapInfo : " + ClientNetworkManager.MapInfo);
        }
    }

#region MatchingPanel's Function
    private void MatchingDataViewIns() //매칭 데이터 패널 생성
    {
        _matchingDataViewPanel = Instantiate(MatchingDataViewPanelPrefab);
        _matchingDataViewPanel.transform.SetParent(Canvas.transform);
        _matchingDataViewPanel.transform.SetAsLastSibling(); //가장 앞에서 보여주기위해
        _matchingDataViewPanel.transform.position = Camera.main.WorldToScreenPoint(Vector3.zero);
        _matchingDataViewPanel.transform.FindChild("MapName").GetComponent<Text>().text =
            ClientNetworkManager.MapInfo.MapName;
        StartCoroutine(GameStartCounter());
        MatchingDataSetting();//데이터셋팅
    }


    //데이터패널 내부 데이터 표시 셋팅 , 어떤팀으로 실행하던지 자신의 데이터는 왼쪽
    private void MatchingDataSetting() 
    {
        if (ClientNetworkManager.EnemyMatchingData.TeamColor == 2)
        {
            _matchingDataViewPanel.transform.FindChild("Player1Panel").transform.FindChild("Player1Team").GetComponent<Text>().text = "Blue Team";
            _matchingDataViewPanel.transform.FindChild("Player2Panel").transform.FindChild("Player2Team").GetComponent<Text>().text = "Red Team";
        }
        else if(ClientNetworkManager.EnemyMatchingData.TeamColor == 1)
        {
            _matchingDataViewPanel.transform.FindChild("Player1Panel").transform.FindChild("Player1Team").GetComponent<Text>().text = "Red Team";
            _matchingDataViewPanel.transform.FindChild("Player2Panel").transform.FindChild("Player2Team").GetComponent<Text>().text = "Blue Team";
        }
            _matchingDataViewPanel.transform.FindChild("Player1Panel").transform.FindChild("Player1ID").GetComponent<Text>().text = "아이디 : " + TribeSetManager.PData.NickName;
            _matchingDataViewPanel.transform.FindChild("Player1Panel").transform.FindChild("Player1Tribe").GetComponent<Text>().text = "종족 : " + TribeSetManager.PData.TribeName;
            _matchingDataViewPanel.transform.FindChild("Player1Panel").transform.FindChild("Player1Spell").GetComponent<Text>().text = "스펠 : " + TribeSetManager.PData.Spell;
        
            _matchingDataViewPanel.transform.FindChild("Player2Panel").transform.FindChild("Player2ID").GetComponent<Text>().text = "아이디 : " + _enemyViewPanelProfileData.NickName;
            _matchingDataViewPanel.transform.FindChild("Player2Panel").transform.FindChild("Player2Tribe").GetComponent<Text>().text = "종족 : " + _enemyViewPanelSetdata.Tribe;
            _matchingDataViewPanel.transform.FindChild("Player2Panel").transform.FindChild("Player2Spell").GetComponent<Text>().text = "스펠 : " + _enemyViewPanelSetdata.Spell;
        }
#endregion
    
#region Creation Node
    void Player1Creation(float x, float z, int node)
    {
        _player1Building = Instantiate(_player1BuildingPrefab);
        _player1Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
        _player1Building.transform.position = new Vector3(x,0,z);
        _player1Building.transform.localScale = Vector3.one;
        _player1Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _player1Building.GetComponent<BuildingControllScript>().PlayerCastle = true;//본진
        _player1Building.GetComponent<BuildingControllScript>().PlayerTeam = "Blue";
        _player1Building.GetComponent<BuildingControllScript>().NodeNumber = node;

        BuildingNode.Add(_player1Building);
     
    }

    void Player2Creation(float x, float z, int node)
    {
        _player2Building = Instantiate(_player2BuildingPrefab);
        _player2Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
        _player2Building.transform.position = new Vector3(x, 0, z);
        _player2Building.transform.localScale = Vector3.one;
        _player2Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _player2Building.GetComponent<BuildingControllScript>().PlayerCastle = true;//본진
        _player2Building.GetComponent<BuildingControllScript>().PlayerTeam = "Red";
        _player2Building.GetComponent<BuildingControllScript>().NodeNumber = node;

        BuildingNode.Add(_player2Building);

    }

    void EmptyNodeCreation(float x, float z, int node)
    {
        _emptyBuilding=Instantiate(_emptyBuildingPrefab);
        _emptyBuilding.transform.SetParent(ParentObject.transform);
        _emptyBuilding.transform.position = new Vector3(x, 0, z);
        _emptyBuilding.transform.localScale = Vector3.one;
        _emptyBuilding.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _emptyBuilding.GetComponent<EmptyBuildingScript>().NodeNumber = node;

        BuildingNode.Add(_emptyBuilding);
    }
    #endregion

#region Ingame Data 
    private void IngameFunction()
    {
        if (ClientNetworkManager.EnemyMoveUnitInfo != null)//적유닛이동
        {
            UnitMove(ClientNetworkManager.EnemyMoveUnitInfo.StartNode, ClientNetworkManager.EnemyMoveUnitInfo.EndNode);
            ClientNetworkManager.EnemyMoveUnitInfo = null;
        }

        if (ClientNetworkManager.MyMoveUnitInfo != null)//내유닛이동 
        {
            UnitMove(ClientNetworkManager.MyMoveUnitInfo.StartNode, ClientNetworkManager.MyMoveUnitInfo.EndNode);//카운트도 원래 같이보냇었음
            ClientNetworkManager.MyMoveUnitInfo = null;
        }

        //빌딩 변경
        if (ClientNetworkManager.EnemyInfo != null)
        {
            BuildingNode[ClientNetworkManager.EnemyInfo.Node].GetComponent<BuildingControllScript>().BuildingDataSet(ClientNetworkManager.EnemyInfo.Kinds);
            ClientNetworkManager.EnemyInfo = null;
        }
        if (ClientNetworkManager.MyInfo != null)
        {
            BuildingNode[ClientNetworkManager.MyInfo.Node].GetComponent<BuildingControllScript>().BuildingDataSet(ClientNetworkManager.MyInfo.Kinds);
            ClientNetworkManager.MyInfo = null;
        }

        //건물인원수

    }
    //유닛이동관련
    private void UnitMove(int stNode, int endNode)
    {
        BuildingNode[stNode].GetComponent<BuildingControllScript>().UnitSpawn(BuildingNode[endNode].transform.position);
    }
    #endregion

#region Corutine
    IEnumerator GameStartCounter() //게임데이터정보를 보여주면서 게임 준비시간카운터 텍스트 변경 함수
    {
        int currentStartTime = 2;
        Text startCounterText = _matchingDataViewPanel.transform.FindChild("GameStartCountText").GetComponent<Text>();
        startCounterText.text = "" + currentStartTime;
        while (currentStartTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            currentStartTime--;//
            startCounterText.text = "" + currentStartTime;
        }

        startCounterText.text = "Start!";
        StartCoroutine(GameStart());

        yield break;
    }

    //매칭잡힌후 로딩시간 // 매칭 카운터 완료후 2초후 시작
    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1f);
        if (ClientNetworkManager.MapInfo != null)
        {
            for (int i = 0; i < ClientNetworkManager.MapInfo.MapNodes.Count; i++)
            {
                if (ClientNetworkManager.MapInfo.MapNodes[i].Owner == 1)
                {
                    Player1Creation((float)ClientNetworkManager.MapInfo.MapNodes[i].XPos,
                        (float)ClientNetworkManager.MapInfo.MapNodes[i].ZPos, i);
                }
                else if (ClientNetworkManager.MapInfo.MapNodes[i].Owner == 2)
                {
                    Player2Creation((float)ClientNetworkManager.MapInfo.MapNodes[i].XPos,
                        (float)ClientNetworkManager.MapInfo.MapNodes[i].ZPos, i);
                }
                else
                {
                    EmptyNodeCreation((float)ClientNetworkManager.MapInfo.MapNodes[i].XPos,
                        (float)ClientNetworkManager.MapInfo.MapNodes[i].ZPos, i);
                }
            }
        }
        else
        {
            Debug.Log("맵데이터가 없습니다.");
        }
        //PlayerCreation();
        if (_matchingDataViewPanel.activeSelf)
        {
            _matchingDataViewPanel.SetActive(false);
        }

        yield break;
    }
    #endregion


}



