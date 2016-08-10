using UnityEngine;
using System.Collections;
using System.Reflection.Emit;
using JetBrains.Annotations;

public class MapTouchScript : MonoBehaviour
{
    private GameObject GameControllerObject;
    public string playerbuilding;
    public string Enemybuilding;
    private string[] data = new string[2];

    private RaycastHit hit;
    private GameObject Selectedbuilding;//선택된 빌딩

    public bool draggingMode = false;

    public GameObject DragCirclePrefab; //생성될 dragobject;
    private GameObject DragCircle;

   
    private Vector3 _touchPosition; // 터치,클릭 포지션
    public Vector3 _EndDesPosition;
    private float TouchCounter;


    //빌딩셋팅이펙트 관리를 위한 오브젝트변수들
    GameObject BuildingLevel1;
    GameObject BuildingLevel2;
    GameObject BuildingLevel3;
    
    // Use this for initialization
    void Start ()
    {
        
        GameControllerObject = GameObject.Find("GameControllerObject");
        data= GameControllerObject.GetComponent<TeamSettingScript>().playerTeamSetting();
        playerbuilding = data[0];
        Enemybuilding = data[1];

        DragCirclePrefab = Resources.Load<GameObject>("DragCircleObject");

       
       
        
    }
	
	// Update is called once per frame
	void Update () {
	   
	        PlayerTouch();
        //터치관련
       
    }


   

    public void PlayerTouch() // 플레이어 터치함수
    {
        if (Input.GetMouseButtonDown(0))//다운
        {

            

            Ray Startray = Camera.main.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(Startray, out hit)) //collider
            {
                if (hit.collider.tag == playerbuilding) //플레이어의 빌딩을 클릭했을시에 DragCircle 생성 
                {

                    Selectedbuilding = hit.collider.transform.gameObject;
                    DragEffectSpawn();
                    if (!Selectedbuilding.GetComponent<BuildingControllScript>().buildingSettingObject.activeSelf)//선택한 빌딩의 빌딩셋팅 패널이 꺼져있을경우 
                    {
                        TouchCounter = 0;
                        Selectedbuilding.GetComponent<BuildingControllScript>().buildingSettingObject.SetActive(false);
                    }


                    /*if (!Selectedbuilding.GetComponent<BuildingControllScript>().buildingSettingPanel.activeSelf)//선택한 빌딩의 빌딩셋팅 패널이 꺼져있을경우 
                    {
                        TouchCounter = 0;
                        Selectedbuilding.GetComponent<BuildingControllScript>().buildingSettingPanel.SetActive(false);
                    }

                

                if(hit.collider.transform.gameObject.transform.position == Selectedbuilding.transform.position)
                {
                   if (Selectedbuilding.GetComponent<BuildingControllScript>().buildingSettingPanel.activeSelf)
                    {
                        Selectedbuilding.GetComponent<BuildingControllScript>()
                            .buildingSettingPanel.SetActive(false);
                    }
                }*/



                }




            }
        }

        if (Input.GetMouseButton(0))//드래그
        {
            if (draggingMode)
            {
                _touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
             
                DragCircle.transform.position = new Vector3(_touchPosition.x, _touchPosition.y - 10f, _touchPosition.z);
                Ray dragRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(dragRay, out hit)) //collider
                {
                    
                    if (hit.collider.transform.gameObject == Selectedbuilding) //해당빌딩을 지속적으로 터치했을시에 카운터스타트 , 카운터가 일정시간넘어가면 해당 buildingsettingpanel 보여줌  
                    {
                        if (Selectedbuilding.GetComponent<BuildingControllScript>().PlayerCastle == false)
                        {
                            if (!Selectedbuilding.GetComponent<BuildingControllScript>()
                                    .buildingSettingObject.activeSelf)
                            {
                                if (TouchCounter < 1)
                                {
                                    TouchCounter += Time.deltaTime;
                                }
                                else
                                {
                                    Selectedbuilding.GetComponent<BuildingControllScript>()
                                        .buildingSettingObject.SetActive(true);
                                }
                            }
                        }

                    }


                    
                        
                    ///건물레벨설정 오브젝트위로 버튼을 누를시 이펙트
                    BuildingSettingObjectDragEffect();

                

                }
            }

         
        }

        if (Input.GetMouseButtonUp(0))
        {

            if (draggingMode == true)
            {
                draggingMode = false;


                Ray Endray = Camera.main.ScreenPointToRay(Input.mousePosition);





                if (Physics.Raycast(Endray, out hit)) //collider
                {
                    if (hit.collider.tag == Enemybuilding) //빌딩을 클릭했을시에 DragCircle 생성 
                    {
                        _EndDesPosition = hit.collider.gameObject.transform.position;
                        Selectedbuilding.GetComponent<BuildingControllScript>().UnitSpawn(_EndDesPosition);
                    }
                    else if (hit.collider.tag == "EmptyBuilding")
                    {
                        _EndDesPosition = hit.collider.gameObject.transform.position;
                        Selectedbuilding.GetComponent<BuildingControllScript>().UnitSpawn(_EndDesPosition);
                    }
                    else if (hit.collider.tag == playerbuilding && hit.collider.gameObject != Selectedbuilding)//선택된 빌딩을 다시 클릭했을때 유닛이 스폰되지않도록
                    {
                        _EndDesPosition = hit.collider.gameObject.transform.position;
                        Selectedbuilding.GetComponent<BuildingControllScript>().UnitSpawn(_EndDesPosition);
                    }


                    buildingSettingFunction();//건물레벨 설정 오브젝트위에서 버튼을 땟을때 
                    


                    if (Selectedbuilding.GetComponent<BuildingControllScript>()
                        .buildingSettingObject.activeSelf)
                    {
                        Selectedbuilding.GetComponent<BuildingControllScript>()
                                   .buildingSettingObject.SetActive(false);
                    }





                    Destroy(DragCircle);
                }




            }
        }
    }


    void PhoneTouch()
    {
        
    }

    void buildingSettingFunction()
    {

        //건물 레벨 변경 
        if (hit.collider.tag == "Level1Setting")
        {
            Selectedbuilding.GetComponent<BuildingControllScript>().buildingSet1();
        }
        else if (hit.collider.tag == "Level2Setting")
        {
            Selectedbuilding.GetComponent<BuildingControllScript>().buildingSet2();
        }
        else if (hit.collider.tag == "Level3Setting")
        {
            Selectedbuilding.GetComponent<BuildingControllScript>().buildingSet3();
        }
    }



    void BuildingSettingObjectDragEffect()
    {
        if (Selectedbuilding.GetComponent<BuildingControllScript>().buildingSettingObject.activeSelf)
        {
            if (hit.collider.tag == "Level1Setting")
            {
                BuildingLevel1 = hit.collider.transform.gameObject;
                BuildingLevel1.transform.localScale = Vector3.one*1.5f;
            }
            else
            {
                if (BuildingLevel1)
                {
                    BuildingLevel1.transform.localScale = Vector3.one;
                }
            }

            if (hit.collider.tag == "Level2Setting")
            {
                BuildingLevel2 = hit.collider.transform.gameObject;
                BuildingLevel2.transform.localScale = Vector3.one*1.5f;
            }
            else
            {
                if (BuildingLevel2)
                {
                    BuildingLevel2.transform.localScale = Vector3.one;
                }
            }

            if (hit.collider.tag == "Level3Setting")
            {
                BuildingLevel3 = hit.collider.transform.gameObject;
                BuildingLevel3.transform.localScale = Vector3.one*1.5f;
            }
            else
            {
                if (BuildingLevel3)
                {
                    BuildingLevel3.transform.localScale = Vector3.one;
                }
            }
        }
        /*

        if (hit.collider.tag == "Level2Setting")
        {
            hit.collider.transform.localScale = Vector3.one * 1.2f;
        }
        else
        {
            hit.collider.transform.localScale = Vector3.one;
        }

        if (hit.collider.tag == "Level3Setting")
        {
            hit.collider.transform.localScale = Vector3.one * 1.2f;
        }
        else
        {
            hit.collider.transform.localScale = Vector3.one;
        }*/
    }

    public void DragEffectSpawn() // 드래그이펙트 생성함수
    {
        if (DragCircle == null)
        {
            DragCircle = Instantiate(DragCirclePrefab); //dragcircle생성
            DragCircle.transform.SetParent(this.gameObject.transform);
            _touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            DragCircle.transform.localRotation = Quaternion.Euler(new Vector3(90,0,0));
            DragCircle.transform.localScale = Vector3.one;
            DragCircle.transform.position = new Vector3(_touchPosition.x, _touchPosition.y - 10f,
                _touchPosition.z);
      
            draggingMode = true;
        }
    }
}
