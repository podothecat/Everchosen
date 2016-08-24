using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Client;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BuildingControllScript : MonoBehaviour
{

    private GameObject _gameController;
    //생성하는 유닛관련 변수들
    public GameObject UnitPrefab;
    
    public float UnitNumber = 1;
    public int SendUnitCount;
    private GameObject _unit;
    private GameObject _unitNumberPanelPrefab;
    private GameObject _unitNumberPanel;

    public bool PlayerCastle;
    
    public GameObject BuildingSettingObject; //오브젝트로 만든것   ui와 오브젝트 2개중 하나사용하면될듯
    private List<Tribe> _buildingDataList; //게임오브젝트 변수에서 받아올 빌딩데이터
    //db에서 받아온 값들을 셋팅 해줄 변수들
    public int BuildingId;//데이터베이스에서 데이터를 가져올때 필요한 빌딩 아이디
    private float _delayCreateCount;//유닛생성시 스폰 딜레이 카운트 설정
    private float _unitCreateCounter;//유닛 숫자가 1 올라가는데 걸리는 시간

    public float BuildingValue;//빌딩 value는 그 빌딩의 유닛이 생산하는 유닛의 체력값 
    public float UnitPower;
    public Sprite UnitSprite;
    public int PlayerTeam;//생성할 유닛의 색상설정을 알기위한 변수 

    public int NodeNumber;

    private readonly BuildingChangeData _sendLevelData = new BuildingChangeData();


    void Awake()
    {
        UnitPrefab = Resources.Load<GameObject>("Unit");
    }


    // Use this for initialization
    void Start()
    {
        //각 종족에따라 가져올 db설정
        _gameController = GameObject.Find("GameControllerObject");
        if (this.gameObject.tag == _gameController.GetComponent<TeamSettingScript>().Playerbuilding)
        {
            _buildingDataList = _gameController.GetComponent<TeamSettingScript>().PlayertribeDataToAdd;
        }
        else if (this.gameObject.tag == _gameController.GetComponent<TeamSettingScript>().Enemybuilding)
        {
            _buildingDataList = _gameController.GetComponent<TeamSettingScript>().EnemytribeDataToAdd;
        }


        _unitNumberPanelPrefab = Resources.Load<GameObject>("UnitNumberPanel");

        //UNITNUMBER UI 생성

        _unitNumberPanel = Instantiate(_unitNumberPanelPrefab);
        _unitNumberPanel.transform.SetParent(GameObject.Find("UnitNumberUIObject").gameObject.transform);
        _unitNumberPanel.transform.position =
            Camera.main.WorldToScreenPoint(new Vector3(this.gameObject.transform.position.x,
                this.gameObject.transform.position.y, this.gameObject.transform.position.z + 1f));
        _unitNumberPanel.GetComponentInChildren<Text>().text = "" + UnitNumber;


        if (PlayerCastle) //본진이면 아이디 0 , 아니면 1
        {
            BuildingId = 0;
        }
        else
        {
            BuildingId = 1;
        }
        
        BuildingDataSet(BuildingId); //빌딩 데이터 설정
        
        // 빌드레벨 설정 오브젝트들
        BuildingSettingObject = this.transform.FindChild("BuildingSetting").gameObject;
        BuildingSettingObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        UnitCreateCounterFunction();
    }
    
    public void BuildingDataSet(int buildingId)
    {
        //건물관련
        this.gameObject.GetComponent<SpriteRenderer>().sprite = _buildingDataList[buildingId].BuildingSprite;//선택한 빌딩에 따라 건물스프라이트 가져옴
     
        _delayCreateCount = _buildingDataList[buildingId].CreateCount;//생성하는 유닛에 따라 유닛수증가 딜레이 설정
        BuildingValue = _buildingDataList[buildingId].Value;
        
        //유닛관련
        if (PlayerTeam == 1)
        {
            UnitSprite = _buildingDataList[buildingId].BUnitSprite;
        }
        else if (PlayerTeam == 2)
        {
            UnitSprite = _buildingDataList[buildingId].RUnitSprite;
        }
        
        //생성할 유닛 sprite
        UnitPower = _buildingDataList[buildingId].UnitPower;
      
        UnitNumber = 0; // 건물레벨업시 유닛숫자 초기화
        _unitCreateCounter = 0;
        
        UnitNumbersetText();
    }



    public void UnitCreateCounterFunction()//유닛 생성 카운트 함수
    {
        _unitCreateCounter += Time.deltaTime;

        if (_unitCreateCounter > _delayCreateCount)
        {
            UnitNumber++;
            UnitNumbersetText();
            _unitCreateCounter = 0;
        }
    }



    public void UnitSpawn(Vector3 des) //유닛 생성
    {
        if (UnitNumber >= 5)
        {
            for (int i = 0; i < 5; i++) //유닛 생성
            {
                UnitIns(des,i);
            }
        }
        else if (UnitNumber > 0 && UnitNumber < 5)
        {
            int createUnitnumber = (int)UnitNumber;
            for (int i = 0; i < createUnitnumber; i++) //유닛 생성
            {
                UnitIns(des, i);
            }
        }
        else
        {
            Debug.Log("유닛이 업성");
        }
        UnitNumbersetText();

    }


    void UnitIns(Vector3 endDesPosition, int i)//목적지와 유닛수
    {
        _unit = Instantiate(UnitPrefab);
        _unit.transform.SetParent(this.gameObject.transform);
        _unit.transform.position = this.gameObject.transform.position;
        _unit.transform.rotation = Quaternion.Euler(Vector3.zero);
        _unit.transform.localScale = Vector3.one;
        //생성할 유닛 스폰 아이디 설정
        _unit.GetComponent<NavMeshAgent>().SetDestination(endDesPosition); //목적지설정
        _unit.GetComponent<NavMeshAgent>().updateRotation = false;
        // 유닛이 목적지가 아닌곳에서 trigger가 시작되지 않게하기위함
        _unit.GetComponent<UnitControllScript>().Destination = endDesPosition;//목적지에 도착하면 trigger가 켜지게하기위함
        _unit.GetComponent<BoxCollider>().enabled = false;
        //유닛 공격력 및 sprite설정
        _unit.GetComponent<UnitControllScript>().UnitPower = UnitPower;
        _unit.GetComponent<UnitControllScript>().UnitSprite = UnitSprite;
        _unit.GetComponent<UnitControllScript>().Team = PlayerTeam;
        Debug.Log(PlayerTeam);
        Debug.Log(_unit.GetComponent<UnitControllScript>().Team);

        SendUnitCount = i;
        UnitNumber--;
        //sprite 좌우반전
        if (_unit.transform.position.x > endDesPosition.x)
        {
            _unit.GetComponentInChildren<SpriteRenderer>().flipX = true;
        }

    }


    public void UnitNumbersetText() // 유닛number text생성
    {
        _unitNumberPanel.GetComponentInChildren<Text>().text = "" + UnitNumber;
    }


    public void DestroythisBuilding() //침공당해서 숫자가 적어졌을경우 모두파괴
    {

     GameObject buildingPrefab;
     GameObject building;
        switch (this.gameObject.tag)
        {
            case "Player1building":
                buildingPrefab = Resources.Load<GameObject>("Player2building");
                building = Instantiate(buildingPrefab);
                building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
                building.transform.position = this.gameObject.transform.position;
                building.transform.localScale = this.gameObject.transform.localScale;
                building.transform.localRotation = Quaternion.Euler(Vector3.zero);
              
               
                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber] = building;
                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber]
                    .GetComponent<BuildingControllScript>().NodeNumber = NodeNumber;
                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber]
                    .GetComponent<BuildingControllScript>().PlayerTeam = 2;
                break;
            case "Player2building":
                buildingPrefab = Resources.Load<GameObject>("Player1building");
                building = Instantiate(buildingPrefab);
                building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
                building.transform.position = this.gameObject.transform.position;
                building.transform.localScale = this.gameObject.transform.localScale;
                building.transform.localRotation = Quaternion.Euler(Vector3.zero);

             
                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber] = building;
                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber]
                  .GetComponent<BuildingControllScript>().NodeNumber = NodeNumber;
                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber]
                    .GetComponent<BuildingControllScript>().PlayerTeam = 1;
                break;
        }
     
        Destroy(this.gameObject);
        Destroy(_unitNumberPanel);
    }
    

    public void BuildingSet(int level)//파라미터 레벨에따라서 변경
    {
        int offsetbuildingId = BuildingId;
        BuildingId = level;
        if (BuildingId != _buildingDataList[offsetbuildingId].BuildingId)
        {
            BuildingDataSet(BuildingId);//data set
            LevelSend(BuildingId);//server send
           
        }
        else
        {
            Debug.Log("이미 같은 종류의 건물입니다.");
        }
    }
    
    
    public void LevelSend(int lv)
    {
        _sendLevelData.Node = NodeNumber;
        _sendLevelData.Kinds = lv;

        Debug.Log(_sendLevelData);
        ClientNetworkManager.Send("Change", _sendLevelData);
    }





    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "unit")
        {
            if (other.gameObject.GetComponent<UnitControllScript>().Team == PlayerTeam)
            {
                UnitNumber++;
                UnitNumbersetText();
                
            }
            else if (other.gameObject.GetComponent<UnitControllScript>().Team != PlayerTeam)
            {
                UnitNumber--;
                UnitNumbersetText();
                if (UnitNumber <= 0)
                {
                    DestroythisBuilding();
                   
                }
            }
            Destroy(other.gameObject);
        }
    }



}



/*
    IEnumerator UnitNumberCounter(float spawnCount) //유닛스폰시간
    {
       
        while (_unitNumberPanel)
        {
            yield return new WaitForSeconds(spawnCount);
            {
                _unitNumber++;
                unitNumbersetText();
            }
        }
        yield break;
    }
*/
