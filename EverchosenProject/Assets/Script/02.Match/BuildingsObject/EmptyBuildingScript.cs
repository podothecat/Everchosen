using Client;
using EverChosenPacketLib;
using UnityEngine;

public class EmptyBuildingScript : MonoBehaviour {
    private GameObject _player1BuildingPrefab;
    private GameObject _player1Building;
    private GameObject _player2BuildingPrefab;
    private GameObject _player2Building;

    public int NodeNumber;
    public int Team = 0;
    
    private GameControllScript _gamecontroll;
    
    // Use this for initialization
    void Start()
    {
        _gamecontroll = GameObject.Find("GameControllerObject").GetComponent<GameControllScript>();
        _player1BuildingPrefab = Resources.Load<GameObject>("Player1building");
        _player2BuildingPrefab = Resources.Load<GameObject>("Player2building");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "unit")
        {
            var unit = new Building
            {
                Owner =  other.GetComponent<UnitControllScript>().Team == "Blue" ? 1 : 2,
                Kinds = other.GetComponent<UnitControllScript>().UnitId,
                XPos = other.transform.position.x,
                ZPos = other.transform.position.z,
                UnitCount = other.GetComponent<UnitControllScript>().UnitNumber
            };
            var fightinfo = new FightInfo
            {
                Units = unit,
                FightBuildingIdx = NodeNumber
            };
            Debug.Log("test-----------------"+ other.GetComponent<UnitControllScript>().UnitId);
            Debug.Log("test++++++++++++++++++++++" + unit.Kinds);
            ClientNetworkManager.Send(fightinfo);
            Destroy(other.gameObject);
        }
    }

    public void BuildingSpawn(string Team, int node, int UnitNumber)
    {
        if (Team == "Blue")
        {
            if (_player1Building == null)
            {
                _player1Building = Instantiate(_player1BuildingPrefab);
                _player1Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
                _player1Building.transform.position = this.gameObject.transform.position;
                _player1Building.transform.localScale = Vector3.one;
                _player1Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
                _player1Building.GetComponent<BuildingControllScript>().PlayerTeam = Team;
                _player1Building.GetComponent<BuildingControllScript>().NodeNumber = node;
                _player1Building.GetComponent<BuildingControllScript>().UnitNumber = UnitNumber;
                _gamecontroll.BuildingNode[NodeNumber] = null;
                _gamecontroll.BuildingNode[NodeNumber] = _player1Building;
            }
        }
        else if(Team == "Red")
        {
            if (_player2Building == null)
            {
                _player2Building = Instantiate(_player2BuildingPrefab);
                _player2Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
                _player2Building.transform.position = this.gameObject.transform.position;
                _player2Building.transform.localScale = Vector3.one;
                _player2Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
                _player2Building.GetComponent<BuildingControllScript>().PlayerTeam = Team;
                _player2Building.GetComponent<BuildingControllScript>().NodeNumber = node;
                _player2Building.GetComponent<BuildingControllScript>().UnitNumber = UnitNumber;
                _gamecontroll.BuildingNode[NodeNumber] = null;
                _gamecontroll.BuildingNode[NodeNumber] = _player2Building;
            }
        }
        Destroy(this.gameObject);
    }
    
}
/*
 *   if (other.tag == "unit")
        {

            if (other.gameObject.GetComponent<UnitControllScript>().Team == "Blue")
            {
              
            }
            else if (other.gameObject.GetComponent<UnitControllScript>().Team=="Red")
            {
                if (_player2Building == null)
                {
                    _player2Building = Instantiate(_player2BuildingPrefab);
                    _player2Building.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
                    _player2Building.transform.position = this.gameObject.transform.position;
                    _player2Building.transform.localScale = Vector3.one;
                    _player2Building.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    _player2Building.GetComponent<BuildingControllScript>().PlayerTeam = "Red";
                    _player2Building.GetComponent<BuildingControllScript>().NodeNumber = NodeNumber;
                    _player2Building.GetComponent<BuildingControllScript>().UnitNumber =
                        other.GetComponent<UnitControllScript>().UnitNumber;
                    _gamecontroll.BuildingNode[NodeNumber] = null;
                    _gamecontroll.BuildingNode[NodeNumber] = _player2Building;
                }
            }
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

    */
