using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class UnitControllScript : MonoBehaviour
{
    private GameObject GameControllerObject;
  
    public int Team;
    

  
    
   
    public float unitPower;
    public Sprite UnitSprite;
    public float unitDamageAmount;
    public float unitSupportAmount;


    public Vector3 destination;
    // Use this for initialization
    void Start ()
    {
       
        GameControllerObject = GameObject.Find("GameControllerObject");
  
        Team = GameControllerObject.GetComponent<TeamSettingScript>().playerTeam;//유닛 팀설정

        
        unitDataSet(); //생성될 유닛 데이터 설정 unitid는 buildingControllerScript에서 unit을 spawn할시 값 초기화
        
        
    }
	
	// Update is called once per frame
	void Update () {
	    if (Vector3.Distance(this.transform.position, destination) < 2)//목적지로 설정한 곳에 도착했을때만 trigger온
	    {
	        this.GetComponent<BoxCollider>().enabled = true;
	    }
	}



    void unitDataSet()
    {
   
        this.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = UnitSprite;
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameControllerObject.GetComponent<TeamSettingScript>().Enemybuilding)
        {

            unitCalculateAttack(other);
            other.GetComponent<BuildingControllScript>()._unitNumber -= unitDamageAmount;//(int)unitToAdd.Power; ; //공격력에따라 데미지 변경
            other.GetComponent<BuildingControllScript>().unitNumbersetText();
            if(other.GetComponent<BuildingControllScript>()._unitNumber<=0)
            {
                other.GetComponent<BuildingControllScript>().DestroythisBuilding();
            }
            
            Destroy(this.gameObject);
        }
        else if (other.tag == GameControllerObject.GetComponent<TeamSettingScript>().playerbuilding) //&& other.gameObject!=this.gameObject.transform.parent.gameObject)
        {
            unitCalculateSupport(other);
            other.GetComponent<BuildingControllScript>()._unitNumber += unitSupportAmount;//(int)unitToAdd.Value;// 값에 따라 +변경
            other.GetComponent<BuildingControllScript>().unitNumbersetText();
            Destroy(this.gameObject);
        }
    }


    void unitCalculateAttack(Collider other) //적건물을 공격할때 식
    {
        
            unitDamageAmount =
                (float) Math.Round( unitPower/ other.GetComponent<BuildingControllScript>().buildingValue, 2); //유닛의 공격력/건물이 가지고 있는유닛의 value값
       
    }

    // 2, 1 일경우 2,1.5일경우 3,1 .. 3,1.5
    // 0.5 데미지   0.75데미지, 0.33데미지.. 0.5데미지

    void unitCalculateSupport(Collider other) // 우리진영건물을 지원할때 식
    {
            unitSupportAmount =
                (float)Math.Round( unitPower/ other.GetComponent<BuildingControllScript>().buildingValue, 2); 
    }
}
