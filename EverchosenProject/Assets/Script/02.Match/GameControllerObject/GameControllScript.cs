using UnityEngine;
using System.Collections;
using Client;
using UnityEngine.UI;
using System.Collections.Generic;

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

    private MatchingPacket _enemyViewPanelSetdata;
    private ProfileData _enemyViewPanelProfileData;


    public List<GameObject> NodePosition;
    public List<GameObject> BuildingNode;


    void Awake()
    {
        _player1BuildingPrefab = Resources.Load<GameObject>("Player1building");
        _player2BuildingPrefab = Resources.Load<GameObject>("Player2building");
        _emptyBuildingPrefab = Resources.Load<GameObject>("EmptyBuilding");
    }
    void Start () {
     

        //게임카운트;
    }
	
	// Update is called once per frame
	void Update () {

        if (ClientNetworkManager.PacketData != null&&ClientNetworkManager.EnemyProfileData!=null)
        {
            Debug.Log("확인");
            Debug.Log(ClientNetworkManager.PacketData);
            _enemyViewPanelSetdata = ClientNetworkManager.PacketData;
            _enemyViewPanelProfileData = ClientNetworkManager.EnemyProfileData;
            MatchingDataViewIns();//매칭데이터 패널 생성
            ClientNetworkManager.PacketData = null;
            ClientNetworkManager.EnemyProfileData = null;
        }

        if (ClientNetworkManager.EnemyMoveData != null)//적유닛이동
	    {
            UnitMove(ClientNetworkManager.EnemyMoveData.UnitCount, ClientNetworkManager.EnemyMoveData.StartNode, ClientNetworkManager.EnemyMoveData.EndNode);
	        ClientNetworkManager.EnemyMoveData = null;
	    }

	    if (ClientNetworkManager.MyMoveData != null)//내유닛이동 
	    {
	        UnitMove(ClientNetworkManager.MyMoveData.UnitCount,ClientNetworkManager.MyMoveData.StartNode,ClientNetworkManager.MyMoveData.EndNode);
	        ClientNetworkManager.MyMoveData = null;
	    }


        //빌딩 변경
	    if (ClientNetworkManager.EnemyChangeData != null)
	    {
            
	        BuildingNode[ClientNetworkManager.EnemyChangeData.Node].GetComponent<BuildingControllScript>().BuildingDataSet(ClientNetworkManager.EnemyChangeData.Kinds);
	        ClientNetworkManager.EnemyChangeData = null;
	    }
        if (ClientNetworkManager.MyChangeData != null)
        {
            BuildingNode[ClientNetworkManager.MyChangeData.Node].GetComponent<BuildingControllScript>().BuildingDataSet(ClientNetworkManager.MyChangeData.Kinds);
            ClientNetworkManager.MyChangeData = null;
        }


    }


    private void MatchingDataViewIns() //매칭 데이터 패널 생성
    {
        _matchingDataViewPanel = Instantiate(MatchingDataViewPanelPrefab);
        _matchingDataViewPanel.transform.SetParent(Canvas.transform);
        _matchingDataViewPanel.transform.SetAsLastSibling(); //가장 앞에서 보여주기위해
        _matchingDataViewPanel.transform.position = Camera.main.WorldToScreenPoint(Vector3.zero);
        StartCoroutine(GameStartCounter());
        MatchingDataSetting();//데이터셋팅
    }


    //데이터패널 내부 데이터 표시 셋팅 , 어떤팀으로 실행하던지 자신의 데이터는 왼쪽
    private void MatchingDataSetting() 
    {
        if (ClientNetworkManager.PacketData.TeamColor == 2)
        {
            _matchingDataViewPanel.transform.FindChild("Player1Panel").transform.FindChild("Player1Team").GetComponent<Text>().text = "Blue Team";
            _matchingDataViewPanel.transform.FindChild("Player2Panel").transform.FindChild("Player2Team").GetComponent<Text>().text = "Red Team";
        }
        else if(ClientNetworkManager.PacketData.TeamColor == 1)
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



    IEnumerator GameStartCounter() //게임데이터정보를 보여주면서 게임 준비시간카운터 텍스트 변경 함수
    {
        int currentStartTime = 2;
        Text startCounterText = _matchingDataViewPanel.transform.FindChild("GameStartCountText").GetComponent<Text>();
        startCounterText.text = "" + currentStartTime;
        while (currentStartTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
                    currentStartTime--;//
                    startCounterText.text = ""+currentStartTime;
        }
        
        startCounterText.text = "Start!";
        StartCoroutine(GameStart());

        yield break;
    }

    //매칭잡힌후 로딩시간 // 매칭 카운터 완료후 2초후 시작
    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1f);
        if (ClientNetworkManager.MapData != null)
        {
          
            for (int i = 0; i < ClientNetworkManager.MapData.MapNodes.Count; i++)
            {
                if (ClientNetworkManager.MapData.MapNodes[i].Owner == 1)
                {
                    Player1Creation((float) ClientNetworkManager.MapData.MapNodes[i].XPos,
                        (float) ClientNetworkManager.MapData.MapNodes[i].ZPos, i);
                }
                else if (ClientNetworkManager.MapData.MapNodes[i].Owner == 2)
                {
                    Player2Creation((float) ClientNetworkManager.MapData.MapNodes[i].XPos,
                        (float) ClientNetworkManager.MapData.MapNodes[i].ZPos, i);
                }
                else
                {
                    EmptyNodeCreation((float) ClientNetworkManager.MapData.MapNodes[i].XPos,
                        (float) ClientNetworkManager.MapData.MapNodes[i].ZPos, i);
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

    
    void Player1Creation(float x, float z, int node)
    {
        _player1Building = Instantiate(_player1BuildingPrefab);
        _player1Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
        _player1Building.transform.position = new Vector3(x,0,z);
        _player1Building.transform.localScale = Vector3.one;
        _player1Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _player1Building.GetComponent<BuildingControllScript>().PlayerCastle = true;//본진
        _player1Building.GetComponent<BuildingControllScript>().PlayerTeam = 1;
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
        _player2Building.GetComponent<BuildingControllScript>().PlayerTeam = 2;
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


    public void UnitMove(int unitCount,int stNode, int endNode)
    {
        BuildingNode[stNode].GetComponent<BuildingControllScript>().UnitSpawn(BuildingNode[endNode].transform.position);
    }

   

}


