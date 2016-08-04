using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameControllScript : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject _matchingDataViewPanelPrefab;
    private GameObject _matchingDataViewPanel;



    private GameObject _player1BuildingPrefab;
    private GameObject _player1Building;

    private GameObject _player2BuildingPrefab;
    private GameObject _player2Building;




    void Start () {
      

        MatchingDataViewIns();
	    StartCoroutine(gameStartCounter());
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    private void MatchingDataViewIns() //매칭 데이터 패널 생성
    {
        _matchingDataViewPanel = Instantiate(_matchingDataViewPanelPrefab);
        _matchingDataViewPanel.transform.SetParent(Canvas.transform);
        _matchingDataViewPanel.transform.SetAsLastSibling(); //가장 앞에서 보여주기위해
        _matchingDataViewPanel.transform.position = Camera.main.WorldToScreenPoint(Vector3.zero);
        MatchingDataSetting();//데이터셋팅
    }

    private void MatchingDataSetting()
    {
        _matchingDataViewPanel.transform.FindChild("Player1Panel").transform.FindChild("Player1ID").GetComponent<Text>().text = TribeSetManager.p1Data.UserID;
        _matchingDataViewPanel.transform.FindChild("Player1Panel").transform.FindChild("Player1Tribe").GetComponent<Text>().text = ""+TribeSetManager.p1Data.Tribe+"종족";
        _matchingDataViewPanel.transform.FindChild("Player1Panel").transform.FindChild("Player1Spell").GetComponent<Text>().text = ""+TribeSetManager.p1Data.Spell+"Spell";

        _matchingDataViewPanel.transform.FindChild("Player2Panel").transform.FindChild("Player2ID").GetComponent<Text>().text = TribeSetManager.p2Data.UserID;
        _matchingDataViewPanel.transform.FindChild("Player2Panel").transform.FindChild("Player2Tribe").GetComponent<Text>().text = "" + TribeSetManager.p2Data.Tribe+"종족";
        _matchingDataViewPanel.transform.FindChild("Player2Panel").transform.FindChild("Player2Spell").GetComponent<Text>().text = "" + TribeSetManager.p2Data.Spell+"Spell";
        
    }



    IEnumerator gameStartCounter() //게임데이터정보를 보여주면서 게임 준비시간카운터 텍스트 변경 함수
    {
        
        int currentStartTime = 5;
        Text StartCounterText = _matchingDataViewPanel.transform.Find("GameStartCountText").GetComponent<Text>();
        StartCounterText.text = "" + currentStartTime;
        while (currentStartTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
                    currentStartTime--;//
                    StartCounterText.text = ""+currentStartTime;
        }


        StartCounterText.text = "Start!";
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

        _player1Building = Instantiate(_player1BuildingPrefab);
        _player1Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
        _player1Building.transform.position = new Vector3(-11,0,-5);
        _player1Building.transform.localScale = Vector3.one;
        _player1Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _player1Building.GetComponent<BuildingControllScript>().PlayerCastle = true;//본진


        _player2Building = Instantiate(_player2BuildingPrefab);
        _player2Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
        _player2Building.transform.position = new Vector3(10, 0, 5);
        _player2Building.transform.localScale = Vector3.one;
        _player2Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _player2Building.GetComponent<BuildingControllScript>().PlayerCastle = true;//본진
        
    }

}
