using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BuildingControllScript : MonoBehaviour
{
    private GameObject EmptyBuildingPrefab;
    private GameObject EmptyBuidling;




    public GameObject UnitPrefab;
    private GameObject _unit;
    public int _unitNumber = 1;
    private GameObject _unitNumberPanelPrefab;
    private GameObject _unitNumberPanel;











    // Use this for initialization
    void Start()
    {

        //  DragCirclePrefab = Resources.Load<GameObject>("DragCircleObject");
        UnitPrefab = Resources.Load<GameObject>("Unit1");
        EmptyBuildingPrefab = Resources.Load<GameObject>("EmptyBuilding");



        //UNITNUMBER UI 생성
        _unitNumberPanelPrefab = Resources.Load<GameObject>("UnitNumberPanel");
        _unitNumberPanel = Instantiate(_unitNumberPanelPrefab);
        _unitNumberPanel.transform.SetParent(GameObject.Find("UnitNumberUIObject").gameObject.transform);
        _unitNumberPanel.transform.position =
            Camera.main.WorldToScreenPoint(new Vector3(this.gameObject.transform.position.x,
                this.gameObject.transform.position.y, this.gameObject.transform.position.z + 1f));
        _unitNumberPanel.GetComponentInChildren<Text>().text = "" + _unitNumber;

        StartCoroutine(UnitNumberCounter()); //ui 유닛 넘버 카운트 스타트

    }

    // Update is called once per frame
    void Update()
    {




    }





    public void UnitSpawn(Vector3 _EndDesPosition) //유닛 생성
    {
     
        if (_unitNumber >= 5)
        {
            for (int i = 0; i < 5; i++) //유닛 생성
            {

                _unit = Instantiate(UnitPrefab);
                _unit.transform.SetParent(this.gameObject.transform);
                _unit.transform.position = this.gameObject.transform.position;
                _unit.transform.rotation = Quaternion.Euler(Vector3.zero);
                _unit.transform.localScale = Vector3.one;

                _unit.GetComponent<NavMeshAgent>().SetDestination(_EndDesPosition);
                _unit.GetComponent<NavMeshAgent>().updateRotation = false;
                // 유닛이 목적지가 아닌곳에서 trigger가 시작되지 않게하기위함
                _unit.GetComponent<UnitControllScript>().destination = _EndDesPosition;
                _unit.GetComponent<BoxCollider>().enabled = false;
              
                _unitNumber--;

            }
        }
        else if (_unitNumber > 0 && _unitNumber < 5)
        {
            int createUnitnumber = _unitNumber;
            for (int i = 0; i < createUnitnumber; i++) //유닛 생성
            {

                _unit = Instantiate(UnitPrefab);
                _unit.transform.SetParent(this.gameObject.transform);
                _unit.transform.position = this.gameObject.transform.position;
                _unit.transform.rotation = Quaternion.Euler(Vector3.zero);
                _unit.transform.localScale = Vector3.one;
                _unit.GetComponent<NavMeshAgent>().SetDestination(_EndDesPosition);
                _unit.GetComponent<NavMeshAgent>().updateRotation = false;
                // 유닛이 목적지가 아닌곳에서 trigger가 시작되지 않게하기위함
                _unit.GetComponent<UnitControllScript>().destination = _EndDesPosition;
                _unit.GetComponent<BoxCollider>().enabled = false;
                _unitNumber--;
            }
        }
        else
        {

        }
        unitNumbersetText();

    }




    public void unitNumbersetText() // 유닛number text생성
    {
        _unitNumberPanel.GetComponentInChildren<Text>().text = "" + _unitNumber;
    }


    public void DestroythisBuilding() //침공당해서 숫자가 적어졌을경우 모두파괴
    {
        EmptyBuidling = Instantiate(EmptyBuildingPrefab);
        EmptyBuidling.transform.SetParent(GameObject.Find("MapObject").gameObject.transform);
        EmptyBuidling.transform.position = this.gameObject.transform.position;
        EmptyBuidling.transform.localScale = this.gameObject.transform.localScale;

        Destroy(this.gameObject);
        Destroy(_unitNumberPanel);


    }

    IEnumerator UnitNumberCounter() //매칭 카운터
    {

        while (_unitNumberPanel)
        {
            yield return new WaitForSeconds(1.5f);
            {
                _unitNumber++;
                unitNumbersetText();
            }
        }
        yield break;
    }







}
