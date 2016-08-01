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

    private GameObject buildingSettingPanel;
    // Use this for initialization
    void Start ()
    {
        
        GameControllerObject = GameObject.Find("GameControllerObject");
        data= GameControllerObject.GetComponent<TeamSettingScript>().playerTeamSetting();
        playerbuilding = data[0];
        Enemybuilding = data[1];

        DragCirclePrefab = Resources.Load<GameObject>("DragCircleObject");

     //   buildingSettingPanel = GameObject.Find("BuildingSettingPanel");
     //   buildingSettingPanel.SetActive(false);
        
    }
	
	// Update is called once per frame
	void Update () {
	   
	        PlayerTouch();
        //터치관련
       
    }


   

    public void PlayerTouch() // 플레이어 터치함수
    {
        if (Input.GetMouseButtonDown(0))
        {

            

            Ray Startray = Camera.main.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(Startray, out hit)) //collider
            {
                if (hit.collider.tag == playerbuilding) //빌딩을 클릭했을시에 DragCircle 생성 
                {

                    Selectedbuilding = hit.collider.transform.gameObject;
                    DragEffectSpawn();
                 /*   if (buildingSettingPanel.activeSelf)
                    {
                        TouchCounter = 0;
                        buildingSettingPanel.SetActive(false);
                    }*/

                }
             
                
              
              
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (draggingMode)
            {
                _touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
             
                DragCircle.transform.position = new Vector3(_touchPosition.x, _touchPosition.y - 10f, _touchPosition.z);
            /*    Ray dragRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(dragRay, out hit)) //collider
                {
                    if (hit.collider.transform.gameObject == Selectedbuilding) //빌딩을 클릭했을시에 DragCircle 생성 
                    {
                        if (!buildingSettingPanel.activeSelf)
                        {
                            if (TouchCounter < 2)
                            {
                                TouchCounter += Time.deltaTime;
                            }
                            else
                            {
                                buildingSettingPanel.SetActive(true);
                            }
                        }
                    }
                }*/
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
                    else if (hit.collider.tag == "EnableBuilding")
                    {
                        _EndDesPosition = hit.collider.gameObject.transform.position;
                        Selectedbuilding.GetComponent<BuildingControllScript>().UnitSpawn(_EndDesPosition);
                    }
                    else if (hit.collider.tag == playerbuilding && hit.collider.gameObject != Selectedbuilding)//선택된 빌딩을 다시 클릭했을때 유닛이 스폰되지않도록
                    {
                        _EndDesPosition = hit.collider.gameObject.transform.position;
                        Selectedbuilding.GetComponent<BuildingControllScript>().UnitSpawn(_EndDesPosition);
                    }
                    Destroy(DragCircle);
                }
            }
        }
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
