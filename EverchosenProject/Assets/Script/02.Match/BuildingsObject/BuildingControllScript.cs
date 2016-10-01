using UnityEngine;
using System.Collections.Generic;
using Client;
using EverChosenPacketLib;
using UnityEngine.UI;


public class BuildingControllScript : MonoBehaviour
{
    private GameObject _gameController;
    //Unit Variable
    public GameObject UnitsPrefab;
    private GameObject _unitTroop;
    public GameObject UnitsSpritePrefab;
    private GameObject _unit;
    private GameObject _buildingUnitNumberPanelPrefab;
    private GameObject _buildingUnitNumberPanel;
    public float UnitNumber;
    private int _unitSpawnId;
    public int SendUnitCount;

    public bool PlayerCastle;
    public GameObject BuildingSettingObject; //오브젝트로 만든것   ui와 오브젝트 2개중 하나사용하면될듯
    private List<Tribe> _buildingDataList; //게임오브젝트 변수에서 받아올 빌딩데이터

    //db에서 받아온 값들을 셋팅 해줄 변수들
    public int BuildingId;//데이터베이스에서 데이터를 가져올때 필요한 빌딩 아이디
    private float _delayCreateCount;//유닛생성시 스폰 딜레이 카운트 설정
    private float _unitCreateCounter;//유닛 숫자가 1 올라가는데 걸리는 시간
    public float BuildingValue;//빌딩 value는 그 빌딩의 유닛이 생산하는 유닛의 체력값 
    public float UnitPower;
    public string UnitKind;
    public string PlayerTeam;//생성할 유닛의 색상설정을 알기위한 변수 
    public Sprite UnitSprite;

    public int NodeNumber;

    void Awake()
    {
        _gameController = GameObject.Find("GameControllerObject");
        UnitsPrefab = Resources.Load<GameObject>("Units");
        UnitsSpritePrefab = Resources.Load<GameObject>("UnitSprite");
        // 빌딩레벨 설정 오브젝트
        BuildingSettingObject = this.transform.FindChild("BuildingSetting").gameObject;
        BuildingSettingObject.SetActive(false);
    }
    
    void Start()
    {
        //tag에 따라서 빌딩설정
        if (this.gameObject.tag == _gameController.GetComponent<TeamSettingScript>().Playerbuilding)
        {
            _buildingDataList = _gameController.GetComponent<TeamSettingScript>().PlayertribeDataToAdd;
        }
        else if (this.gameObject.tag == _gameController.GetComponent<TeamSettingScript>().Enemybuilding)
        {
            _buildingDataList = _gameController.GetComponent<TeamSettingScript>().EnemytribeDataToAdd;
        }

        BuildingUnitNumberViewPanelIns();

        BuildingId = (PlayerCastle ? 0 : 1);//본진확인 playercastle이 true면 building id = 0
        BuildingDataSet(BuildingId); //빌딩 데이터 설정


    }
    
    void Update()
    {
            //BuildingUnitCreateCounterFunction();
    }
   
 
    #region Functions to related Unit
    //Unit Creation Function
    public void UnitSpawn(Vector3 des, int unitcount)
    {
        UnitTroopIns(des, unitcount);
        BuildingUnitNumbersetText();
    }

    //Unit Ins 
    void UnitTroopIns(Vector3 endDesPosition, int unitcount)//목적지와 유닛수
    {
        _unitTroop = Instantiate(UnitsPrefab);
        _unitTroop.transform.SetParent(this.gameObject.transform);
        _unitTroop.transform.position = this.gameObject.transform.position;
        _unitTroop.transform.rotation = Quaternion.Euler(Vector3.zero);
        _unitTroop.transform.localScale = Vector3.one;
        //생성할 유닛 스폰 아이디 설정
        _unitTroop.GetComponent<NavMeshAgent>().SetDestination(endDesPosition); //목적지설정
        _unitTroop.GetComponent<NavMeshAgent>().updateRotation = false;
        // 유닛이 목적지가 아닌곳에서 trigger가 시작되지 않게하기위함
        _unitTroop.GetComponent<UnitControllScript>().Destination = endDesPosition;//목적지에 도착하면 trigger가 켜지게하기위함
        _unitTroop.GetComponent<BoxCollider>().enabled = false;
        //유닛 공격력 및 sprite설정
        _unitTroop.GetComponent<UnitControllScript>().UnitPower = UnitPower;
        _unitTroop.GetComponent<UnitControllScript>().UnitNumber = unitcount;
        _unitTroop.GetComponent<UnitControllScript>().UnitKind = UnitKind;
        _unitTroop.GetComponent<UnitControllScript>().UnitId = _unitSpawnId;
        _unitTroop.GetComponent<UnitControllScript>().Team = PlayerTeam;
        
        UnitNumber -= unitcount;
        UnitSpriteIns(unitcount,endDesPosition);
        //sprite 좌우반전
        

    }

    void UnitSpriteIns(int unitCount, Vector3 endDes)
    {
        for (int i = 0; i < (unitCount/5)+1; i++)
        {
            _unit = Instantiate(UnitsSpritePrefab);
            _unit.transform.SetParent(_unitTroop.transform);
            _unit.transform.localPosition = new Vector3(-(float)i/5,1, -(float)i / 5);
            _unit.GetComponentInChildren<SpriteRenderer>().sprite = UnitSprite;
            if (_unitTroop.transform.position.x > endDes.x)
            {
               
                _unit.GetComponentInChildren<SpriteRenderer>().flipX = true;
            }
        }
    }

    //Building Unit Number Counter
    //public void BuildingUnitCreateCounterFunction()//유닛 생성 카운트 함수
    //{
    //    _unitCreateCounter += Time.deltaTime;
    //    if (_unitCreateCounter > _delayCreateCount)
    //    {
    //        UnitNumber++;
    //        BuildingUnitNumbersetText();
    //        _unitCreateCounter = 0;
    //    }
    //}

    public void BuildingUnitCreateCounterFunction(int increment)
    {
        UnitNumber = increment;
        BuildingUnitNumbersetText();
    }

    //Building Unit Number panel Text set
    public void BuildingUnitNumbersetText() // 유닛number text생성
    {
        _buildingUnitNumberPanel.GetComponentInChildren<Text>().text = "" + UnitNumber;
    }

    //Building UnitNumberPanel
    void BuildingUnitNumberViewPanelIns()
    {
        _buildingUnitNumberPanelPrefab = Resources.Load<GameObject>("BuildingNumberPanel");
        //UNITNUMBER UI 생성
        _buildingUnitNumberPanel = Instantiate(_buildingUnitNumberPanelPrefab);
        _buildingUnitNumberPanel.transform.SetParent(GameObject.Find("UnitNumberUIObject").gameObject.transform);
        _buildingUnitNumberPanel.transform.position =
            Camera.main.WorldToScreenPoint(new Vector3(this.gameObject.transform.position.x,
                this.gameObject.transform.position.y, this.gameObject.transform.position.z + 1f));
        _buildingUnitNumberPanel.GetComponentInChildren<Text>().text = "" + UnitNumber;

    }

       
    #endregion
    
    #region Functions to related Building

    public void BuildingSet(int level)//파라미터 레벨에따라서 변경
    {
        int offsetbuildingId = BuildingId;
        BuildingId = level;
        if (BuildingId != _buildingDataList[offsetbuildingId].BuildingId)
        {
            UnitNumber = 0;
            BuildingDataSet(BuildingId);//data set
        }
        else
        {
            Debug.Log("이미 같은 종류의 건물입니다.");
        }
    }

    //Building Data Setting Function
    public void BuildingDataSet(int buildingId)
    {
        //건물관련
        Debug.Log(_buildingDataList[buildingId].BuildingSprite.name);
        this.gameObject.GetComponent<SpriteRenderer>().sprite = _buildingDataList[buildingId].BuildingSprite;//선택한 빌딩에 따라 건물스프라이트 가져옴

        _delayCreateCount = _buildingDataList[buildingId].CreateCount;//건물에 따라 유닛수증가 딜레이 설정
        BuildingValue = _buildingDataList[buildingId].Value;
        //유닛관련
        if (PlayerTeam == "Blue")
        {
            UnitSprite = _buildingDataList[buildingId].BUnitSprite;
        }
        else if (PlayerTeam == "Red")
        {
            UnitSprite = _buildingDataList[buildingId].RUnitSprite;
        }
        
        UnitPower = _buildingDataList[buildingId].UnitPower;
        Debug.Log(_buildingDataList[buildingId].SpawnUnitId);
        _unitSpawnId = _buildingDataList[buildingId].SpawnUnitId;
        UnitKind = _buildingDataList[buildingId].UnitKind;
        _unitCreateCounter = 0;

        BuildingUnitNumbersetText();
    }

    //Destroy building
    public void DestroythisBuilding(int unitcount) //침공당해서 숫자가 적어졌을경우 모두파괴
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
                    .GetComponent<BuildingControllScript>().PlayerTeam = "Red";
                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber]
                    .GetComponent<BuildingControllScript>().UnitNumber = unitcount;
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
                    .GetComponent<BuildingControllScript>().PlayerTeam = "Blue";
                _gameController.GetComponent<GameControllScript>().BuildingNode[NodeNumber]
                  .GetComponent<BuildingControllScript>().UnitNumber = unitcount;
                break;
        }

        Destroy(this.gameObject);
        Destroy(_buildingUnitNumberPanel);
    }
    #endregion

    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "unit")
        {
            var unit = new Building
            {
                Owner = other.GetComponent<UnitControllScript>().Team == "Blue" ? 1 : 2,
                Kinds = other.GetComponent<UnitControllScript>().UnitId,
                XPos = other.transform.position.x,
                ZPos = other.transform.position.z,
                UnitCount = other.GetComponent<UnitControllScript>().UnitNumber
            };
            var fightinfo = new FightInfo
            {
                Units = unit,
                FightBuildingIdx = NodeNumber
            };
            Debug.Log("test-----------------" + other.GetComponent<UnitControllScript>().UnitId);
            Debug.Log("test++++++++++++++++++++++" + unit.Kinds);
            ClientNetworkManager.Send(fightinfo);
            Destroy(other.gameObject);
        }
    }
}

/*
  if (other.tag == "unit")
        {
            if (other.gameObject.GetComponent<UnitControllScript>().Team == PlayerTeam)
            {
              
                    UnitNumber+= other.GetComponent<UnitControllScript>().UnitNumber;
                    BuildingUnitNumbersetText();
            }
            else if (other.gameObject.GetComponent<UnitControllScript>().Team != PlayerTeam)
            {


                UnitNumber -= other.GetComponent<UnitControllScript>().UnitNumber;
                BuildingUnitNumbersetText();
                if (UnitNumber <= 0)
                  {
                       DestroythisBuilding();
                   }
            }
            Destroy(other.gameObject);
           
        }

    */