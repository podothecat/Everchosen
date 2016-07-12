using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainButtonController : MonoBehaviour {

    public GameObject QueuePanelPrefab;
    GameObject QueuePanel;
    GameObject QueueText;
    GameObject matchButton;

    GameObject CampaignPanel;
    
    public float queueTime;
    float currentQueueTime;
    public bool MatchSuccess;

    void Awake()
    {
        matchButton = GameObject.Find("MatchButton");
        CampaignPanel = GameObject.Find("CampaignPanel");


        queueTime = 5;
        currentQueueTime = 0;
    }

    void Start()
    {
        CampaignPanel.SetActive(false);
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
    
   

    //Main Menu Button
    public void MatchButtonInvoke()
    {
        QueuePanel = Instantiate(QueuePanelPrefab);//버튼 클릭시 queue panel prefab생성
        QueuePanel.transform.SetParent(GameObject.Find("QueueSetPanel").gameObject.transform);
        QueuePanel.transform.localPosition = Vector2.zero;
        QueuePanel.transform.SetAsLastSibling();


        QueueText = GameObject.Find("QueueText");//queueText
        QueueText.GetComponent<Text>().text = "Searching..";

        GameObject QueueCancelButton = GameObject.Find("Queue cancel Button");//queuecancel button
        QueueCancelButton.GetComponent<Button>().onClick.AddListener(() => CancelButtonInvoke());

        StartCoroutine(queueTimeCounter());



        matchButton.GetComponent<Button>().interactable = false; //매칭대기열 시작시 매칭버튼 interactable;
    }

    

    public void CampaignInvoke() //매칭을 켜두고 캠페인같은것을 들어갈시에 레벨을 넘겨버리면 안되려나 DonDestroy사용
    {
        CampaignPanel.SetActive(true);
        CampaignPanel.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public void ExitButtonInvoke()
    {
        Application.Quit();
    }

    public void CancelButtonInvoke()
    {
        Destroy(QueuePanel);
        matchButton.GetComponent<Button>().interactable = true;
        
    }

    
    public void BackButtonInvoke()
    {
        CampaignPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        CampaignPanel.SetActive(false);
      
    }
    
    
    
    IEnumerator queueTimeCounter() //매칭 카운터
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

    IEnumerator MatchStart(float count) // 매칭 카운터 완료후 2초후 시작
    {
        yield return new WaitForSeconds(count);
        Application.LoadLevel("Match");
    }


}
