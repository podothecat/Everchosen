using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BuildingControllScript : MonoBehaviour
{

    private BuildingDatabase buildingDB;
    //부서졌을대 다시 기본 엠티빌딩 생성 변수들
    private GameObject EmptyBuildingPrefab;
    private GameObject EmptyBuidling;



    //생성하는 유닛관련 변수들
    public GameObject UnitPrefab;
    private GameObject _unit;
    public int _unitNumber = 1;
    private GameObject _unitNumberPanelPrefab;
    private GameObject _unitNumberPanel;


   
    




    public GameObject buildingSettingObject; //오브젝트로 만든것   ui와 오브젝트 2개중 하나사용하면될듯
  

    private int buildingID;//데이터베이스에서 가져올 빌딩 아이디
    private Building buildingToAdd;// 데이터베이스에서 가져오는 모든데이터 저장할변수
    private int unitSpawnID; // unit생성시 unitid를 설정할 변수
    private float DelayCreateCount;//유닛생성시 스폰 딜레이 카운트 설정
    private bool couroutineReset;

    private float unitCreateCounter;





    // Use this for initialization
    void Start()
    {

        buildingDB = GameObject.Find("GameControllerObject").GetComponent<BuildingDatabase>();


        UnitPrefab = Resources.Load<GameObject>("Unit");
        EmptyBuildingPrefab = Resources.Load<GameObject>("EmptyBuilding");
      

        //UNITNUMBER UI 생성
        _unitNumberPanelPrefab = Resources.Load<GameObject>("UnitNumberPanel");
        _unitNumberPanel = Instantiate(_unitNumberPanelPrefab);
        _unitNumberPanel.transform.SetParent(GameObject.Find("UnitNumberUIObject").gameObject.transform);
        _unitNumberPanel.transform.position =
            Camera.main.WorldToScreenPoint(new Vector3(this.gameObject.transform.position.x,
                this.gameObject.transform.position.y, this.gameObject.transform.position.z + 1f));
        _unitNumberPanel.GetComponentInChildren<Text>().text = "" + _unitNumber;


        buildingID = 0; //처음 시작할땐 빌딩id 0으로설정
        BuildingDataSet(buildingID); //빌딩 데이터 설정
        

       //ui 유닛 넘버 카운트 스타트
        
        // 빌드레벨 설정 오브젝트들
        buildingSettingObject = this.transform.FindChild("BuildingSetting").gameObject;
        buildingSettingObject.SetActive(false);




       


    }

    // Update is called once per frame
    void Update()
    {


        UnitCreateCounterFunction();

    }


    void BuildingDataSet(int buildingID)
    {
        couroutineReset = false;
        buildingToAdd = buildingDB.FetchBuildingByID(buildingID);

        this.gameObject.GetComponent<SpriteRenderer>().sprite = buildingToAdd.Sprite;//선택한 빌딩에 따라 건물스프라이트 가져옴
        unitSpawnID = buildingToAdd.SpawnUnitID;// 생성할 유닛id 설정
        DelayCreateCount = buildingToAdd.CreateCount;//생성하는 유닛에 따라 유닛수증가 딜레이 설정
        _unitNumber = 0; // 건물레벨업시 유닛숫자 초기화
        unitCreateCounter = 0;
        unitNumbersetText();
    }



    public void UnitCreateCounterFunction()//유닛 생성 카운트 함수
    {
        unitCreateCounter += Time.deltaTime;

        if (unitCreateCounter > DelayCreateCount)
        {
            _unitNumber++;
            unitNumbersetText();
            unitCreateCounter = 0;
        }
    }



    public void UnitSpawn(Vector3 _EndDesPosition) //유닛 생성
    {
     
        if (_unitNumber >= 5)
        {
            for (int i = 0; i < 5; i++) //유닛 생성
            {

                _unit = Instantiate(UnitPrefab);
                _unit.GetComponent<UnitControllScript>().UnitId = unitSpawnID;

                _unit.transform.SetParent(this.gameObject.transform);
                _unit.transform.position = this.gameObject.transform.position;
                _unit.transform.rotation = Quaternion.Euler(Vector3.zero);
                _unit.transform.localScale = Vector3.one;
                //생성할 유닛 스폰 아이디 설정
                _unit.GetComponent<NavMeshAgent>().SetDestination(_EndDesPosition); //목적지설정
                _unit.GetComponent<NavMeshAgent>().updateRotation = false;
                // 유닛이 목적지가 아닌곳에서 trigger가 시작되지 않게하기위함
                _unit.GetComponent<UnitControllScript>().destination = _EndDesPosition;
                _unit.GetComponent<BoxCollider>().enabled = false;
              
                _unitNumber--;

            }
        }
        else if (_unitNumber > 0 && _unitNumber < 5)
        {
            int createUnitnumber = _unitNumber;
            for (int i = 0; i < createUnitnumber; i++) //유닛 생성
            {

                _unit = Instantiate(UnitPrefab);

                _unit.GetComponent<UnitControllScript>().UnitId = unitSpawnID;
                _unit.transform.SetParent(this.gameObject.transform);
                _unit.transform.position = this.gameObject.transform.position;
                _unit.transform.rotation = Quaternion.Euler(Vector3.zero);
                _unit.transform.localScale = Vector3.one;
               
                _unit.GetComponent<NavMeshAgent>().SetDestination(_EndDesPosition);
                _unit.GetComponent<NavMeshAgent>().updateRotation = false;
                // 유닛이 목적지가 아닌곳에서 trigger가 시작되지 않게하기위함
                _unit.GetComponent<UnitControllScript>().destination = _EndDesPosition;
                _unit.GetComponent<BoxCollider>().enabled = false;
                _unitNumber--;
            }
        }
        else
        {

        }
        unitNumbersetText();

    }




    public void unitNumbersetText() // 유닛number text생성
    {
        _unitNumberPanel.GetComponentInChildren<Text>().text = "" + _unitNumber;
    }


    public void DestroythisBuilding() //침공당해서 숫자가 적어졌을경우 모두파괴
    {
        EmptyBuidling = Instantiate(EmptyBuildingPrefab);
        EmptyBuidling.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
        EmptyBuidling.transform.position = this.gameObject.transform.position;
        EmptyBuidling.transform.localScale = this.gameObject.transform.localScale;
        EmptyBuidling.transform.localRotation = Quaternion.Euler(Vector3.zero);
        Destroy(this.gameObject);
        Destroy(_unitNumberPanel);


    }






    public void buildingSet1()
    {
        buildingID = 1;
        if (buildingID != buildingToAdd.ID)
        {
            
            BuildingDataSet(buildingID);
        }
      

    }

    public void buildingSet2()
    {
        buildingID = 2;
        if (buildingID != buildingToAdd.ID)
        {
           
            BuildingDataSet(buildingID);
        }
      
    }

    public void buildingSet3()
    {
        buildingID = 3;
        if (buildingID != buildingToAdd.ID)
        {
           
            BuildingDataSet(buildingID);
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