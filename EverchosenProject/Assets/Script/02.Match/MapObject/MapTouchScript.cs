using UnityEngine;
using Client;
using EverChosenPacketLib;

public class MapTouchScript : MonoBehaviour
{
    private GameObject _gameControllerObject;
    public string Playerbuilding;
    public string Enemybuilding;
    private string[] _data = new string[2];

    private RaycastHit _hit;
    private GameObject _startSelectedbuilding;//선택된 빌딩
 
    public bool DraggingMode = false;

    public GameObject DragCirclePrefab; //생성될 dragobject;
    private GameObject _dragCircle;
    
    private Vector3 _touchPosition; // 터치,클릭 포지션
    public Vector3 EndDesPosition;
    private float _touchCounter;

    public int StartNode;
    public int EndNode;

    //빌딩셋팅이펙트 관리를 위한 오브젝트변수들
    GameObject _buildingLevel1;
    GameObject _buildingLevel2;
    GameObject _buildingLevel3;
    
    private readonly MoveUnitInfo _ingamePacket = new MoveUnitInfo();
    private readonly ChangeBuildingInfo _sendLevelData = new ChangeBuildingInfo();
    
    void Start ()
    {
        _gameControllerObject = GameObject.Find("GameControllerObject");
        _data= _gameControllerObject.GetComponent<TeamSettingScript>().PlayerTeamSetting();
        Debug.Log(_data[0]);
        Debug.Log(_data[1]);
        Playerbuilding = _data[0];
        Enemybuilding = _data[1];
        DragCirclePrefab = Resources.Load<GameObject>("DragCircleObject");
    }
	
	// Update is called once per frame
	void Update () {
	        PlayerTouch();
        //터치관련
    }
    
    public void PlayerTouch() // 플레이어 터치함수
    {

        if (Input.GetMouseButtonDown(0))//Mouse Down
        {
            Ray startray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(startray, out _hit)) //collider
            {
                if (_hit.collider.tag == Playerbuilding) //플레이어의 빌딩을 클릭했을시에 DragCircle 생성 
                {

                    _startSelectedbuilding = _hit.collider.transform.gameObject;
                    DragEffectSpawn();
                    if (!_startSelectedbuilding.GetComponent<BuildingControllScript>().BuildingSettingObject.activeSelf)//선택한 빌딩의 빌딩셋팅 패널이 꺼져있을경우 
                    {
                        _touchCounter = 0;
                        _startSelectedbuilding.GetComponent<BuildingControllScript>().BuildingSettingObject.SetActive(false);

                        StartNode = _startSelectedbuilding.GetComponent<BuildingControllScript>().NodeNumber;

                    }
                }
            }
        }
        if (Input.GetMouseButton(0))//Mouse Drag
        {
            if (DraggingMode)
            {
                _touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
             
                _dragCircle.transform.position = new Vector3(_touchPosition.x, _touchPosition.y - 10f, _touchPosition.z);
                Ray dragRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(dragRay, out _hit)) //collider
                {
                    
                    if (_hit.collider.transform.gameObject == _startSelectedbuilding) //해당빌딩을 지속적으로 터치했을시에 카운터스타트 , 카운터가 일정시간넘어가면 해당 buildingsettingpanel 보여줌  
                    {
                        if (_startSelectedbuilding.GetComponent<BuildingControllScript>().PlayerCastle == false)
                        {
                            if (!_startSelectedbuilding.GetComponent<BuildingControllScript>()
                                    .BuildingSettingObject.activeSelf)
                            {
                                if (_touchCounter < 1)
                                {
                                    _touchCounter += Time.deltaTime;
                                }
                                else
                                {
                                    _startSelectedbuilding.GetComponent<BuildingControllScript>()
                                        .BuildingSettingObject.SetActive(true);
                                }
                            }
                        }

                    }
                    ///건물레벨설정 오브젝트위로 버튼을 누를시 이펙트
                    BuildingSettingObjectDragEffect();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))//Mouse Up
        {
            if (DraggingMode == true)
            {
                DraggingMode = false;
                Ray Endray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(Endray, out _hit)) //collider
                {
                    if (_hit.collider.tag == Enemybuilding|| _hit.collider.tag == "EmptyBuilding"||(_hit.collider.tag == Playerbuilding && _hit.collider.gameObject != _startSelectedbuilding)) //빌딩을 클릭했을시에 DragCircle 생성 , //선택된 빌딩을 다시 클릭했을때 유닛이 스폰되지않도록
                    {
                        EndDesPosition = _hit.collider.gameObject.transform.position;
                        if (_startSelectedbuilding.GetComponent<BuildingControllScript>().UnitNumber > 0)//해당건물의 유닛 숫자가 0보다클때만 실행
                        {
                            if (_hit.collider.tag == "EmptyBuilding") //emptyBuilding과 playerBuilding은 스크립트 접근을따로해야하므로 분리
                            {
                                EndNode = _hit.collider.gameObject.GetComponent<EmptyBuildingScript>().NodeNumber;
                            }
                            else if(_hit.collider.tag == Enemybuilding || (_hit.collider.tag == Playerbuilding && _hit.collider.gameObject != _startSelectedbuilding))
                            { 
                                EndNode = _hit.collider.gameObject.GetComponent<BuildingControllScript>().NodeNumber;
                            }
                           // _startSelectedbuilding.GetComponent<BuildingControllScript>().UnitSpawn(EndDesPosition);//클라 유닛생성
                            
                            MoveDataset(StartNode,EndNode);
                        }
                    }

                    //건물데이터변경 서버send함수
                    BuildingSettingFunction(StartNode);
                    //해당 빌딩 레벨업 셋팅이 켜져있을땐 꺼줌
                    if (_startSelectedbuilding.GetComponent<BuildingControllScript>()
                        .BuildingSettingObject.activeSelf)
                    {
                        _startSelectedbuilding.GetComponent<BuildingControllScript>()
                                   .BuildingSettingObject.SetActive(false);
                    }

                    Destroy(_dragCircle);
                }
            }
        }
    }
    
    public void LevelDataSet(int node, int lv) 
    {
        _sendLevelData.Node = node;
        _sendLevelData.Kinds = lv;
        _sendLevelData.UnitCount = 0;
        ClientNetworkManager.Send(_sendLevelData);
    }
    
    void MoveDataset(int st, int end)
    {
        _ingamePacket.StartNode = st;
        _ingamePacket.EndNode = end;
        ClientNetworkManager.Send(_ingamePacket);
    }

    void BuildingSettingFunction(int node)//빌딩함수
    {
        //빌딩 변경부분
        if (_hit.collider.tag == "Level1Setting")
        {
            LevelDataSet(node, 2);
        }
        else if (_hit.collider.tag == "Level2Setting")
        {
            LevelDataSet(node, 3);
        }
        else if (_hit.collider.tag == "Level3Setting")
        {
            LevelDataSet(node, 4);
        }

    }

    void BuildingSettingObjectDragEffect()//빌딩 터치시 view 함수 (포인터가 접근하면 원이 커지도록)
    {
        if (_startSelectedbuilding.GetComponent<BuildingControllScript>().BuildingSettingObject.activeSelf)
        {
            if (_hit.collider.tag == "Level1Setting")
            {
                _buildingLevel1 = _hit.collider.transform.gameObject;
                _buildingLevel1.transform.localScale = Vector3.one*1.5f;
            }
            else
            {
                if (_buildingLevel1)
                {
                    _buildingLevel1.transform.localScale = Vector3.one;
                }
            }

            if (_hit.collider.tag == "Level2Setting")
            {
                _buildingLevel2 = _hit.collider.transform.gameObject;
                _buildingLevel2.transform.localScale = Vector3.one*1.5f;
            }
            else
            {
                if (_buildingLevel2)
                {
                    _buildingLevel2.transform.localScale = Vector3.one;
                }
            }

            if (_hit.collider.tag == "Level3Setting")
            {
                _buildingLevel3 = _hit.collider.transform.gameObject;
                _buildingLevel3.transform.localScale = Vector3.one*1.5f;
            }
            else
            {
                if (_buildingLevel3)
                {
                    _buildingLevel3.transform.localScale = Vector3.one;
                }
            }
        }
    }
    
    public void DragEffectSpawn() // 드래그이펙트 생성함수
    {
        if (_dragCircle == null)
        {
            _dragCircle = Instantiate(DragCirclePrefab); //dragcircle생성
            _dragCircle.transform.SetParent(this.gameObject.transform);
            _touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            _dragCircle.transform.localRotation = Quaternion.Euler(new Vector3(90,0,0));
            _dragCircle.transform.localScale = Vector3.one;
            _dragCircle.transform.position = new Vector3(_touchPosition.x, _touchPosition.y - 10f,
                _touchPosition.z);
      
            DraggingMode = true;
        }
    }
}
