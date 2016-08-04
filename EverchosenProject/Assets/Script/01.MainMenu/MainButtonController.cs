using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainButtonController : MonoBehaviour
{


    private GameObject Canvas;
    //queue관련 변수들
    public GameObject QueuePanelPrefab;
    private GameObject QueuePanel;
    private GameObject QueueText;
    public float queueTime;
    float currentQueueTime;
    public bool MatchSuccess;

    private Button queueButton;

    
    private GameObject GameStartButton;

    private GameObject SettingPanel;
    private GameObject OptionPanel;
    private GameObject CreditPanel;


    private Text TribeViewText;

    public int tribeNumberData;
    private string tribeStringData;

  

    void Awake()
    {
        Canvas = GameObject.Find("Canvas");
        GameStartButton = GameObject.Find("GameStartButton");
       
        

        SettingPanel = Canvas.transform.FindChild("SettingPanel").gameObject;
        CreditPanel = Canvas.transform.FindChild("CreditsPanel").gameObject;
        OptionPanel = GameObject.Find("BackObject").transform.FindChild("OptionPanel").gameObject;

        TribeViewText = Canvas.transform.FindChild("SettingPanel").transform.FindChild("SettingImage").transform.FindChild("SettingViewPanel").transform.FindChild("TribeText").gameObject.GetComponent<Text>();
        queueButton = SettingPanel.transform.FindChild("SettingImage").transform.FindChild("QueueButton").GetComponent<Button>();

       
        
        queueTime = 5;
        currentQueueTime = 0;
    }

    void Start()
    {
        
        CreditPanel.SetActive(false);
        queueButton.interactable = false;//종족선택이나 스펠선택이 되지않으면 대기열 참가 불가하게 하기위함

    }

    void Update()
    {
        if (QueuePanel)
        {
            if (MatchSuccess == true)
            {
                QueueText.GetComponent<Text>().text = "Game Start";
                StartCoroutine(MatchStart(2f));
            }
        }

        
    }
    
   
    //메인에서 보이는 4개의 버튼 함수들 
    //Game Start Button 함수
    public void GameStartButtonInvoke()
    {
       SettingPanel.SetActive(true);
    }

    public void TutorialButtonInvoke() //매칭을 켜두고 캠페인같은것을 들어갈시에 레벨을 넘겨버리면 안되려나 DonDestroy사용
    {
        SceneManager.LoadScene("03.Tutorial");
    }

    public void OptionInvoke()
    {
        OptionPanel.SetActive(true);
        
    }
    
    public void CreditsButtonInvoke()
    {
        CreditPanel.SetActive(true);
        
    }
    //





    //생성되는 queue panel버튼의 cancel버튼에 들어갈 함수
    public void CancelButtonInvoke()
    {
        Destroy(QueuePanel);
        GameStartButton.GetComponent<Button>().interactable = true;
        
    }




    //settingpanel에서 들어갈 함수들
    public void QueueButton()
    {

        TribeSetManager.SetUserData();//유저가 선택한 종족 저장
        TribeSetManager.SetEnemyData();
       
        SettingPanel.SetActive(false);//참여와 함께 셋팅패널 사라짐
        QueuePanel = Instantiate(QueuePanelPrefab);//버튼 클릭시 queue panel prefab생성
        QueuePanel.transform.SetParent(GameObject.Find("QueueSetPanel").gameObject.transform);
        QueuePanel.transform.localPosition = Vector2.zero;
        QueuePanel.transform.SetAsLastSibling();


        QueueText = GameObject.Find("QueueText");//queueText
        QueueText.GetComponent<Text>().text = "Searching..";

        GameObject QueueCancelButton = GameObject.Find("Queue cancel Button");//queuecancel button
        QueueCancelButton.GetComponent<Button>().onClick.AddListener(() => CancelButtonInvoke());

        StartCoroutine(queueTimeCounter());

        GameStartButton.GetComponent<Button>().interactable = false; //매칭대기열 시작시 매칭버튼 interactable;
    }

    public void SettingBackButtonInvoke()
    {
        
        TribeViewText.text = " ";
        queueButton.interactable = false;//다시 queue 버튼 선택 불가

        SettingPanel.SetActive(false);
        

    }
   




    //creditpanel의 backbutton
    public void CreditsBackButtonInvoke()
    {
        CreditPanel.SetActive(false);
    }









    //매칭 카운터
    IEnumerator queueTimeCounter() 
    {
        currentQueueTime = 0;
        while (currentQueueTime < queueTime)
        {
                yield return new WaitForSeconds(1.0f);

            if (QueuePanel)
            {
                {
                    currentQueueTime += 1f;
                    QueueText.GetComponent<Text>().text = "00:0" + currentQueueTime + " / 00:0" + queueTime;
                }
            }
            else
            {
                yield break;
            }
        }
        MatchSuccess = true;
       

        yield break;
    }

    //매칭잡힌후 로딩시간 // 매칭 카운터 완료후 2초후 시작
    IEnumerator MatchStart(float count) 
    {
        yield return new WaitForSeconds(count);
        SceneManager.LoadScene("02.Match");
    }





    //종족선택 버튼 4개
    public void Trbie1ButtonInvoke()
    {
        tribeStringData = "1종족";
        tribeNumberData = 0;
        TribeViewText.text = tribeStringData;
        queueButton.interactable = true;//선택이 되면 queue버튼 클릭가능


    }

    public void Trbie2ButtonInvoke()
    {
        tribeStringData = "2종족";
        tribeNumberData = 1;
        TribeViewText.text = tribeStringData;
        queueButton.interactable = true;

    }
    public void Trbie3ButtonInvoke()
    {
        tribeStringData = "3종족";
        tribeNumberData = 2;
        TribeViewText.text = tribeStringData;
        queueButton.interactable = true;
    }
    public void Trbie4ButtonInvoke()
    {
        tribeStringData = "4종족";
        tribeNumberData = 3;
        TribeViewText.text = tribeStringData;
        queueButton.interactable = true;
    }


    
}
