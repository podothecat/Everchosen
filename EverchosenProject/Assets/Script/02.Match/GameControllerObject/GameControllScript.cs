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
    public GameObject ParentObject;

    private MatchingPacket _enemyViewPanelSetdata;


    public List<GameObject> NodePosition;
    public List<GameObject> BuildingNode;

    

    void Start () {
      
        if (ClientNetworkManager.PacketData != null)
        {
            _enemyViewPanelSetdata = ClientNetworkManager.PacketData;
        }
        MatchingDataViewIns();//매칭데이터 패널 생성
        StartCoroutine(GameStartCounter()); //게임카운트;
    }
	
	// Update is called once per frame
	void Update () {

	    if (ClientNetworkManager.MoveData != null)
	    {
            EnemyUnitMove(ClientNetworkManager.MoveData.UnitCount, ClientNetworkManager.MoveData.StartNode, ClientNetworkManager.MoveData.EndNode);
	        ClientNetworkManager.MoveData = null;
	    }

	    if (ClientNetworkManager.ChangeData != null)
	    {
	        BuildingNode[ClientNetworkManager.ChangeData.Node].GetComponent<BuildingControllScript>().BuildingDataSet(ClientNetworkManager.ChangeData.Kinds);
	        ClientNetworkManager.ChangeData = null;
	    }
	}


    private void MatchingDataViewIns() //매칭 데이터 패널 생성
    {
        _matchingDataViewPanel = Instantiate(MatchingDataViewPanelPrefab);
        _matchingDataViewPanel.transform.SetParent(Canvas.transform);
        _matchingDataViewPanel.transform.SetAsLastSibling(); //가장 앞에서 보여주기위해
        _matchingDataViewPanel.transform.position = Camera.main.WorldToScreenPoint(Vector3.zero);
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
        
            _matchingDataViewPanel.transform.FindChild("Player1Panel").transform.FindChild("Player1ID").GetComponent<Text>().text = "아이디 : " + TribeSetManager.PData.UserID;
            _matchingDataViewPanel.transform.FindChild("Player1Panel").transform.FindChild("Player1Tribe").GetComponent<Text>().text = "종족 : " + TribeSetManager.PData.TribeName;
            _matchingDataViewPanel.transform.FindChild("Player1Panel").transform.FindChild("Player1Spell").GetComponent<Text>().text = "스펠 : " + TribeSetManager.PData.Spell;
        
            _matchingDataViewPanel.transform.FindChild("Player2Panel").transform.FindChild("Player2ID").GetComponent<Text>().text = "아이디 : " + _enemyViewPanelSetdata.Id;
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
       //플레이어 본진 생성
       PlayerCreation();
        if (_matchingDataViewPanel.activeSelf)
        {
            _matchingDataViewPanel.SetActive(false);
        }
        
        yield break;
    }


    void PlayerCreation()
    {
        _player1BuildingPrefab = Resources.Load<GameObject>("Player1building");
        _player2BuildingPrefab = Resources.Load<GameObject>("Player2building");
        _emptyBuildingPrefab = Resources.Load<GameObject>("EmptyBuilding");

        _player1Building = Instantiate(_player1BuildingPrefab);
        _player1Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
        _player1Building.transform.position = NodePosition[0].transform.position;
        _player1Building.transform.localScale = Vector3.one;
        _player1Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _player1Building.GetComponent<BuildingControllScript>().PlayerCastle = true;//본진
        _player1Building.GetComponent<BuildingControllScript>().PlayerTeam = 1;
        _player1Building.GetComponent<BuildingControllScript>().NodeNumber = 0;
        
        _player2Building = Instantiate(_player2BuildingPrefab);
        _player2Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
        _player2Building.transform.position = NodePosition[1].transform.position;
        _player2Building.transform.localScale = Vector3.one;
        _player2Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _player2Building.GetComponent<BuildingControllScript>().PlayerCastle = true;//본진
        _player2Building.GetComponent<BuildingControllScript>().PlayerTeam = 2;
        _player2Building.GetComponent<BuildingControllScript>().NodeNumber = 1;

        BuildingNode.Add(_player1Building);
        BuildingNode.Add(_player2Building);

        EmptyNodeCreation();
    }


    void EmptyNodeCreation()
    {
        for (int i = 2; i < NodePosition.Count; i++)
        {
            BuildingNode.Add(Instantiate(_emptyBuildingPrefab));
            BuildingNode[i].transform.SetParent(ParentObject.transform);
            BuildingNode[i].transform.position = NodePosition[i].transform.position;
            BuildingNode[i].transform.localScale = Vector3.one;
            BuildingNode[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
            BuildingNode[i].GetComponent<EmptyBuildingScript>().NodeNumber = i;
        }

    }


    public void EnemyUnitMove(int unitCount,int stNode, int endNode)
    {
        BuildingNode[stNode].GetComponent<BuildingControllScript>().UnitSpawn(BuildingNode[endNode].transform.position);
    }

   

}


