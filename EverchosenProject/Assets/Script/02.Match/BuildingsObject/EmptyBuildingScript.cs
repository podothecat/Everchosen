using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public class EmptyBuildingScript : MonoBehaviour {
    private GameObject _player1BuildingPrefab;
    private GameObject _player1Building;
    private GameObject _player2BuildingPrefab;
    private GameObject _player2Building;

    public int NodeNumber;
    

    private GameControllScript _gamecontroll;
    
    // Use this for initialization
    void Start()
    {
        _gamecontroll = GameObject.Find("GameControllerObject").GetComponent<GameControllScript>();
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

            Debug.Log(other.gameObject.GetComponent<UnitControllScript>().Team);
            if (other.gameObject.GetComponent<UnitControllScript>().Team == 1)
            {
                if (_player1Building == null)
                {
                    _player1Building = Instantiate(_player1BuildingPrefab);
                    _player1Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
                    _player1Building.transform.position = this.gameObject.transform.position;
                    _player1Building.transform.localScale = Vector3.one;
                    _player1Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    _player1Building.GetComponent<BuildingControllScript>().PlayerTeam = 1;
                    _player1Building.GetComponent<BuildingControllScript>().NodeNumber = NodeNumber;

                    _gamecontroll.BuildingNode[NodeNumber] = null;
                    _gamecontroll.BuildingNode[NodeNumber] = _player1Building;
                 
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
                    _player2Building.GetComponent<BuildingControllScript>().PlayerTeam = 2;
                    _player2Building.GetComponent<BuildingControllScript>().NodeNumber = NodeNumber;
                   
                    _gamecontroll.BuildingNode[NodeNumber] = null;
                    _gamecontroll.BuildingNode[NodeNumber] = _player2Building;
                }
            }
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
    
}
