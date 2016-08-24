using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class UnitControllScript : MonoBehaviour
{
    private GameObject _gameControllerObject;
  
    public int Team;
    
    public float UnitPower;
    public Sprite UnitSprite;
    public float UnitDamageAmount;
    public float UnitSupportAmount;

    public Vector3 Destination;
    // Use this for initialization
    void Start ()
    {
        _gameControllerObject = GameObject.Find("GameControllerObject");
        UnitDataSet(); //생성될 유닛 데이터 설정 unitid는 buildingControllerScript에서 unit을 spawn할시 값 초기화
    }
	
	// Update is called once per frame
	void Update () {
	    if (Vector3.Distance(this.transform.position, Destination) < 2)//목적지로 설정한 곳에 도착했을때만 trigger온
	    {
	        this.GetComponent<BoxCollider>().enabled = true;
	    }
	}
    void UnitDataSet()
    {
        this.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = UnitSprite;
    }


    void UnitCalculateAttack(Collider other) //적건물을 공격할때 식
    {
            UnitDamageAmount =
                (float) Math.Round( UnitPower/ other.GetComponent<BuildingControllScript>().BuildingValue, 2); //유닛의 공격력/건물이 가지고 있는유닛의 value값
    }

    // 2, 1 일경우 2,1.5일경우 3,1 .. 3,1.5
    // 0.5 데미지   0.75데미지, 0.33데미지.. 0.5데미지
    void UnitCalculateSupport(Collider other) // 우리진영건물을 지원할때 식
    {
            UnitSupportAmount =
                (float)Math.Round( UnitPower/ other.GetComponent<BuildingControllScript>().BuildingValue, 2); 
    }
}
