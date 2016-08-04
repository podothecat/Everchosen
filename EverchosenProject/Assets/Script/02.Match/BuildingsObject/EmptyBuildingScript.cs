using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmptyBuildingScript : MonoBehaviour {
    private GameObject _player1BuildingPrefab;
    private GameObject _player1Building;

   
    private GameObject _player2BuildingPrefab;
    private GameObject _player2Building;
    
    // Use this for initialization
    void Start()
    {
        _player1BuildingPrefab = Resources.Load<GameObject>("Player1building");
        _player2BuildingPrefab = Resources.Load<GameObject>("Player2building");

       

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
                if (_player1Building == null)
                {
                    _player1Building = Instantiate(_player1BuildingPrefab);
                    _player1Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
                    _player1Building.transform.position = this.gameObject.transform.position;
                    _player1Building.transform.localScale = Vector3.one;
                    _player1Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
             

                }

            }
            else if (other.gameObject.GetComponent<UnitControllScript>().Team==2)
            {
                if (_player2Building == null)
                {
                    _player2Building = Instantiate(_player2BuildingPrefab);
                    _player2Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
                    _player2Building.transform.position = this.gameObject.transform.position;
                    _player2Building.transform.localScale = Vector3.one;
                    _player2Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    
                }
            }
            Destroy(other.gameObject);
            Destroy(this.gameObject);


        }
    }

   
}
