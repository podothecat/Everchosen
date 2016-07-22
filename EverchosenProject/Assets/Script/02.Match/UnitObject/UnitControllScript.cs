using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class UnitControllScript : MonoBehaviour
{
    private GameObject GameControllerObject;
    public int Team;
    
    public Vector3 destination;
    // Use this for initialization
    void Start ()
    {
        GameControllerObject = GameObject.Find("GameControllerObject");
        Team = GameControllerObject.GetComponent<TeamSettingScript>().playerTeam;//유닛 팀설정
        
        
        
    }
	
	// Update is called once per frame
	void Update () {
	    if (Vector3.Distance(this.transform.position, destination) < 2)//목적지로 설정한 곳에 도착했을때만 trigger온
	    {
	        this.GetComponent<BoxCollider>().enabled = true;
	    }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameControllerObject.GetComponent<TeamSettingScript>().Enemybuilding)
        {
           
            other.GetComponent<BuildingControllScript>()._unitNumber--;
            other.GetComponent<BuildingControllScript>().unitNumbersetText();
            if(other.GetComponent<BuildingControllScript>()._unitNumber<=0)
            {
                other.GetComponent<BuildingControllScript>().DestroythisBuilding();
            }
            
            Destroy(this.gameObject);
        }
        else if (other.tag == GameControllerObject.GetComponent<TeamSettingScript>().playerbuilding) //&& other.gameObject!=this.gameObject.transform.parent.gameObject)
        {
            other.GetComponent<BuildingControllScript>()._unitNumber++;
            other.GetComponent<BuildingControllScript>().unitNumbersetText();
            Destroy(this.gameObject);
        }
    }
}
