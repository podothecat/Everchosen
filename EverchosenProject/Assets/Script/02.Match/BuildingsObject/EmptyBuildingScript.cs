using UnityEngine;
using System.Collections;

public class EmptyBuildingScript : MonoBehaviour {
    private GameObject Team1buildingPrefab;
    private GameObject Team1building;
    private GameObject Team2buildingPrefab;
    private GameObject Team2building;
    // Use this for initialization
    void Start()
    {
        Team1buildingPrefab = Resources.Load<GameObject>("Team1building");
        Team2buildingPrefab = Resources.Load<GameObject>("Team2building");

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "unit")
        {
            if (other.gameObject.GetComponent<UnitControllScript>().Team == 1)
            {
                if (Team1building == null)
                {
                    Team1building = Instantiate(Team1buildingPrefab);
                    Team1building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
                    Team1building.transform.position = this.gameObject.transform.position;
                    Team1building.transform.localScale = Vector3.one;
                }

            }
            else if (other.gameObject.GetComponent<UnitControllScript>().Team==2)
            {
                if (Team2building == null)
                {
                    Team2building = Instantiate(Team2buildingPrefab);
                    Team2building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
                    Team2building.transform.position = this.gameObject.transform.position;
                    Team2building.transform.localScale = Vector3.one;

                }
            }
            Destroy(this.gameObject);


        }
    }

   
}
