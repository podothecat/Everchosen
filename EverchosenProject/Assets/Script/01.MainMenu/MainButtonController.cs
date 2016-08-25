using UnityEngine;
using System.Collections;
using Client;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainButtonController : MonoBehaviour
{
    private GameObject _canvas;
    //queue관련 변수들
    public GameObject QueuePanelPrefab;
    private GameObject _queuePanel;
    private GameObject _queueText;
    float _currentQueueTime;
    public bool MatchSuccess;

    private Button _queueButton;
    private GameObject _gameStartButton;

    private GameObject _profileSettingPanel;

    private Image _profileViewImage;
    private static Text _profileViewNameText;

    private InputField _profileNameInputField;
    private Button _profileSetButton;

    private Image _profileImage;
    private Text _profileInputFieldText;
    private Text _profileInputFieldPlaceHolder;
  
    private GameObject _settingPanel;
    private GameObject _optionPanel;
    private GameObject _creditPanel;
    
    private Text _tribeViewText;
    private Text _spellViewText;
    
    
    void Awake()
    {
        _canvas = GameObject.Find("Canvas");
        _gameStartButton = GameObject.Find("GameStartButton");

        _profileSettingPanel = _canvas.transform.Find("ProfileSettingPanel").gameObject;

        _profileViewNameText =
            _canvas.transform.FindChild("MainPanel").transform.FindChild("ProfileViewPanel")
                .transform.FindChild("ProfileTextPanel")
                .transform.FindChild("ProfileText")
                .GetComponent<Text>();
       _profileViewImage = _canvas.transform.FindChild("MainPanel").transform.Find("ProfileViewPanel").transform.FindChild("ProfileImage").GetComponent<Image>();

        _profileNameInputField =
            _profileSettingPanel.transform.FindChild("ProfileSettingNamePanel")
                .transform.FindChild("ProfileNameInputField").GetComponent<InputField>();
        _profileInputFieldText = _profileNameInputField.transform.FindChild("Text").GetComponent<Text>();
        _profileInputFieldPlaceHolder = _profileNameInputField.transform.FindChild("Placeholder").GetComponent<Text>();

        _profileSetButton = _profileSettingPanel.transform.FindChild("ProfileSettingNamePanel")
            .transform.FindChild("NameChangeButton").gameObject.GetComponent<Button>();
        _profileImage = _profileSettingPanel.transform.FindChild("ProfileSettingImage").gameObject.GetComponent<Image>();
        
        _settingPanel = _canvas.transform.FindChild("SettingPanel").gameObject;
        _creditPanel = _canvas.transform.FindChild("CreditsPanel").gameObject;
        _optionPanel = GameObject.Find("BackObject").transform.FindChild("OptionPanel").gameObject;
        
        _tribeViewText = _canvas.transform.FindChild("SettingPanel").transform.FindChild("SettingImage").transform.FindChild("SettingViewPanel").transform.FindChild("TribeText").gameObject.GetComponent<Text>();
        _spellViewText = _canvas.transform.FindChild("SettingPanel").transform.FindChild("SettingImage").transform.FindChild("SettingViewPanel").transform.FindChild("SpellText").gameObject.GetComponent<Text>();
        _queueButton = _settingPanel.transform.FindChild("SettingImage").transform.FindChild("QueueButton").GetComponent<Button>();
        
    }

    void Start()
    {
        //_profileSettingPanel.SetActive(false);
        if (ClientNetworkManager.ProfileData != null)
        {
            TribeSetManager.PData.NickName = ClientNetworkManager.ProfileData.NickName;
           
            _profileViewNameText.text = TribeSetManager.PData.NickName;
            _profileInputFieldText.text = TribeSetManager.PData.NickName;
            _profileInputFieldPlaceHolder.text = TribeSetManager.PData.NickName;
        }
        else
        {
            Debug.Log("프로필이 안넘어왓성");
        }
        _creditPanel.SetActive(false);
        _queueButton.interactable = false;//종족선택이나 스펠선택이 되지않으면 대기열 참가 불가하게 하기위함

    }

    void Update()
    {
        if (_queuePanel)
        {
            if (ClientNetworkManager.ReceiveMsg == "OnSucceedMatching2")
            {
                StartCoroutine(MatchStart(2));
            }
        }

        if (ClientNetworkManager.ReceiveMsg == "OnChangedProfile"&& ClientNetworkManager.ProfileData.NickName != TribeSetManager.PData.NickName)
        {
            ProfileSet(ClientNetworkManager.ProfileData.NickName);
        }
        
    }
    
   
    //메인에서 보이는 4개의 버튼 함수들 
    //Game Start Button 함수
    public void GameStartButtonInvoke()
    {
       _settingPanel.SetActive(true);
    }

    public void TutorialButtonInvoke() //매칭을 켜두고 캠페인같은것을 들어갈시에 레벨을 넘겨버리면 안되려나 DonDestroy사용
    {
        SceneManager.LoadScene("03.Tutorial");
    }

    public void OptionInvoke()
    {
        _optionPanel.SetActive(true);
        
    }
    
    public void CreditsButtonInvoke()
    {
        _creditPanel.SetActive(true);
        
    }

    public void ProfileInvoke()
    {
        _profileSettingPanel.SetActive(true);
    }
    //
#region QueuePanel
    //settingpanel에서 들어갈 함수들
    public void QueueButton()
    {

        _settingPanel.SetActive(false);//참여와 함께 셋팅패널 사라짐
        _queuePanel = Instantiate(QueuePanelPrefab);//버튼 클릭시 queue panel prefab생성
        _queuePanel.transform.SetParent(GameObject.Find("QueueSetPanel").gameObject.transform);
        _queuePanel.transform.localPosition = Vector2.zero;
        _queuePanel.transform.SetAsLastSibling();


        _queueText = GameObject.Find("QueueText");//queueText
        _queueText.GetComponent<Text>().text = "Searching..";

        GameObject QueueCancelButton = GameObject.Find("Queue cancel Button");//queuecancel button
        QueueCancelButton.GetComponent<Button>().onClick.AddListener(() => CancelButtonInvoke());

        StartCoroutine(queueTimeCounter());

        _gameStartButton.GetComponent<Button>().interactable = false; //매칭대기열 시작시 매칭버튼 interactable;
        ServerQueue();//데이터와 함께 queue

    }

    //생성되는 queue panel버튼의 cancel버튼에 들어갈 함수
    public void CancelButtonInvoke()
    {
        ClientNetworkManager.Send("OnMatchingCancelRequest", " ");
        Destroy(_queuePanel);
        _gameStartButton.GetComponent<Button>().interactable = true;

    }
#endregion


    public void SettingBackButtonInvoke()
    {
        _tribeViewText.text = " ";
        _queueButton.interactable = false;//다시 queue 버튼 선택 불가

        _settingPanel.SetActive(false);
    }
   
    //creditpanel의 backbutton
    public void CreditsBackButtonInvoke()
    {
        _creditPanel.SetActive(false);
    }
    

    //큐버튼을 누를시에 서버로 보내는 함수
    public void ServerQueue()
    {
        MatchingPacket setData = new MatchingPacket(TribeSetManager.PData.TribeName, TribeSetManager.PData.Spell, 0);//마지막 파라미터는 teamflag 그냥 0 으로 보냄 

        ClientNetworkManager.Send("OnMatchingRequest", setData);
    }

    

    //매칭 카운터
    IEnumerator queueTimeCounter() 
    {
        _currentQueueTime = 0;
        while (true)
        {
                yield return new WaitForSeconds(1.0f);
            if (_queuePanel)
            {
                {
                    _currentQueueTime += 1f;
                    _queueText.GetComponent<Text>().text = "매칭중...  " + _currentQueueTime + " 초";
                }
            }
            else
            {
                yield break;
            }
        }
    }

    //매칭잡힌후 로딩시간 // 매칭 카운터 완료후 2초후 시작
    IEnumerator MatchStart(float count) 
    {
        _queueText.GetComponent<Text>().text = "매칭을 찾았습니다.";
        yield return new WaitForSeconds(count);
        SceneManager.LoadScene("02.Match");
    }
    
    //종족선택 버튼 4개
    public void Trbie1ButtonInvoke()
    {
     
        TribeSetManager.PData.Tribe = 0;
        TribeSetManager.PData.TribeName = "Chaos";
        _tribeViewText.text = TribeSetManager.PData.TribeName;
        _queueButton.interactable = true;//선택이 되면 queue버튼 클릭가능
    }

    public void Trbie2ButtonInvoke()
    {

        TribeSetManager.PData.Tribe = 1;
        TribeSetManager.PData.TribeName = "Dwarf";
       
        _tribeViewText.text = TribeSetManager.PData.TribeName;
        _queueButton.interactable = true;

    }
    public void Trbie3ButtonInvoke()
    {
        TribeSetManager.PData.Tribe = 2;
        TribeSetManager.PData.TribeName = "Green";

        _tribeViewText.text = TribeSetManager.PData.TribeName;
        _queueButton.interactable = true;
    }
    public void Trbie4ButtonInvoke()
    {
        TribeSetManager.PData.Tribe = 3;
        TribeSetManager.PData.TribeName = "Human";
       
        _tribeViewText.text = TribeSetManager.PData.TribeName;
        _queueButton.interactable = true;
    }
    
    public void Spell1ButtonInvoke()
    {
        TribeSetManager.PData.Spell = 1;
        _spellViewText.text = "One";

    }

    public void Spell2ButtonInvoke()
    {
        TribeSetManager.PData.Spell = 2;
        _spellViewText.text = "Two";
    }
    
    public void ProfileSettingBackInvoke()
    {
        _profileSettingPanel.SetActive(false);
    }

    public void ProfileNameSetInvoke()
    {
        if (_profileNameInputField.interactable == false)
        {
            _profileNameInputField.interactable = true;

        }
        else
        {
            ClientNetworkManager.Send("OnNickChangeRequest", _profileInputFieldText.text);
           
            _profileNameInputField.interactable = false;
        }
    }


    public static void ProfileSet(string data)
    {
        TribeSetManager.PData.NickName = data;
        _profileViewNameText.text = data;
    }
}
