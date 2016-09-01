using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using Client;
using UnityEngine.UI;

public class UnitControllScript : MonoBehaviour
{
    private GameObject _gameControllerObject;
    private GameObject _unitNumberSetPanelPrefab;
    public GameObject UnitNumberSetPanel;

  
    public string Team;
    public int UnitNumber;
    public int UnitId;
    public string UnitKind;
    public float UnitPower;
    public float UnitDamageAmount;
    public float UnitSupportAmount;

    public Vector3 Destination;
    // Use this for initialization
    void Start ()
    {
        _gameControllerObject = GameObject.Find("GameControllerObject");
        UnitNumberTextPanelIns(UnitNumber);
        // UnitDataSet(); //생성될 유닛 데이터 설정 unitid는 buildingControllerScript에서 unit을 spawn할시 값 초기화
    }
	
	// Update is called once per frame
	void Update ()
	{
	    UnitNumberPanelPositionUpdate();//패널포지션이 유닛포지션을 따라가도록 
        
        if (Vector3.Distance(this.transform.position, Destination) < 2)//목적지로 설정한 곳에 도착했을때만 trigger온
	    {
	        this.GetComponent<BoxCollider>().enabled = true;
	    }
	}


    void UnitNumberTextPanelIns(int unitCount)
    {
        _unitNumberSetPanelPrefab = Resources.Load<GameObject>("UnitNumberPanel");
        UnitNumberSetPanel = Instantiate(_unitNumberSetPanelPrefab);
        UnitNumberSetPanel.transform.SetParent(GameObject.Find("UnitNumberUIObject").gameObject.transform);
        UnitNumberSetPanel.transform.position =
           Camera.main.WorldToScreenPoint(new Vector3(this.gameObject.transform.position.x,
               this.gameObject.transform.position.y, this.gameObject.transform.position.z + 1f));
        UnitNumberSetPanel.GetComponentInChildren<Text>().text = ""+ unitCount;

    }

    void UnitNumberPanelPositionUpdate()
    {
        UnitNumberSetPanel.transform.position =
          Camera.main.WorldToScreenPoint(new Vector3(this.gameObject.transform.position.x,
              this.gameObject.transform.position.y, this.gameObject.transform.position.z + 1f));
    }


    void OnDestroy()
    {
        Destroy(UnitNumberSetPanel);

    }


    /*  void UnitDataSet()
      {
          this.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = UnitSprite;
      }*/


    /*
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
    */
}
