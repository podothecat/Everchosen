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

    private GameObject _profileViewPanel;
    private Image _profileViewImage;
    private InputField _profileNameInputField;
    private Button _profileSetButton;
    
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

        _profileViewPanel = _canvas.transform.FindChild("MainPanel").transform.FindChild("ProfileViewPanel").gameObject;
        _profileViewImage = _profileViewPanel.transform.FindChild("ProfileImage").GetComponent<Image>();
        _profileNameInputField = _profileViewPanel.transform.FindChild("ProfileNameInputField").GetComponent<InputField>();
        _profileInputFieldText = _profileNameInputField.transform.FindChild("Text").GetComponent<Text>();
        _profileInputFieldPlaceHolder = _profileNameInputField.transform.FindChild("Placeholder").GetComponent<Text>();
        _profileSetButton = _profileViewPanel.transform.FindChild("NameChangeButton").gameObject.GetComponent<Button>();
     
        _settingPanel = _canvas.transform.FindChild("SettingPanel").gameObject;
        _creditPanel = _canvas.transform.FindChild("CreditsPanel").gameObject;
        _optionPanel = GameObject.Find("BackObject").transform.FindChild("OptionPanel").gameObject;
        
        _tribeViewText = _canvas.transform.FindChild("SettingPanel").transform.FindChild("SettingImage").transform.FindChild("SettingViewPanel").transform.FindChild("TribeText").gameObject.GetComponent<Text>();
        _spellViewText = _canvas.transform.FindChild("SettingPanel").transform.FindChild("SettingImage").transform.FindChild("SettingViewPanel").transform.FindChild("SpellText").gameObject.GetComponent<Text>();
        _queueButton = _settingPanel.transform.FindChild("SettingImage").transform.FindChild("QueueButton").GetComponent<Button>();
        
    }

    void Start()
    {
        if (ClientNetworkManager.ProfileData != null)
        {
            TribeSetManager.PData.NickName = ClientNetworkManager.ProfileData.NickName;
            _profileInputFieldText.text = TribeSetManager.PData.NickName;
            _profileInputFieldPlaceHolder.text = TribeSetManager.PData.NickName;
        }
        else
        {
            Debug.Log("자신의 프로필이 넘어오지 않았습니다.");
        }
        _creditPanel.SetActive(false);
        _queueButton.interactable = false;//종족선택이나 스펠선택이 되지않으면 대기열 참가 불가하게 하기위함

    }

    void Update()
    {
        //스펠과 종족이 모두선택될시에 queue버튼 활성화
        QueueSelectCheck();
        //매칭 관련
        if (_queuePanel)
        {
            if (ClientNetworkManager.EnemyMatchingData != null&&ClientNetworkManager.EnemyProfileData != null&&ClientNetworkManager.MapData!=null)
            {
                StartCoroutine(MatchStart(2));
            }
        }
        //프로필 변경관련
        if (ClientNetworkManager.ReceiveMsg == "OnChangedProfile"&& ClientNetworkManager.ProfileData.NickName != TribeSetManager.PData.NickName)
        {
            ProfileSet(ClientNetworkManager.ProfileData.NickName);
        }   
    }
    
#region Functions related to MainMenuButtons
    //Game Start Button 함수
    public void GameStartButtonInvoke()
    {
       _settingPanel.SetActive(true);
    }
    //Tutorial
    public void TutorialButtonInvoke() 
    {
        SceneManager.LoadScene("03.Tutorial");
    }
    //Option
    public void OptionInvoke()
    {
        _optionPanel.SetActive(true);
    }
    //Credits
    public void CreditsButtonInvoke()
    {
        _creditPanel.SetActive(true);
        
    }
    //Credit's Backbutton
    public void CreditsBackButtonInvoke()
    {
        _creditPanel.SetActive(false);
    }
    #endregion

#region Functions related to Queue
    // Send to Server
    public void ServerQueue()
    {
        MatchingInfo setData = new MatchingInfo(TribeSetManager.PData.TribeName, TribeSetManager.PData.Spell, 0);//마지막 파라미터는 teamflag 그냥 0 으로 보냄 
        ClientNetworkManager.Send("OnMatchingRequest", setData);
    }
    // QueuePanel Ins
    public void QueueButton()
    {
        _settingPanel.SetActive(false);//Queue 참여 후 셋팅패널 false
        _queuePanel = Instantiate(QueuePanelPrefab);//버튼 클릭시 queue panel prefab생성
        _queuePanel.transform.SetParent(GameObject.Find("QueueSetPanel").gameObject.transform);
        _queuePanel.transform.localPosition = Vector2.zero;
        _queuePanel.transform.SetAsLastSibling();
        _queueText = GameObject.Find("QueueText");
        _queueText.GetComponent<Text>().text = "Searching..";

        var queueCancelButton = GameObject.Find("Queue cancel Button");//queuecancel button
        queueCancelButton.GetComponent<Button>().onClick.AddListener(() => CancelButtonInvoke());

        StartCoroutine(QueueTimeCounter());

        _gameStartButton.GetComponent<Button>().interactable = false; //매칭대기열 시작시 매칭버튼 interactable;
        ServerQueue();//데이터와 함께 queue
    }

    //QueuePanel's CancelButton
    public void CancelButtonInvoke()
    {
        ClientNetworkManager.Send("OnMatchingCancelRequest", " ");
        Destroy(_queuePanel);
        _gameStartButton.GetComponent<Button>().interactable = true;

    }
    #endregion

#region Functions related to Game Tribe and Spell Seelct 
    //SettingBack Invoke
    public void SettingBackButtonInvoke()
    {
        _tribeViewText.text = " ";
        _queueButton.interactable = false;//다시 queue 버튼 선택 불가
        TribeSetManager.PData.Tribe = -1;
        TribeSetManager.PData.Spell = -1;
        _settingPanel.SetActive(false);
    }

    //Tribe 4 Button
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
    }
    public void Trbie3ButtonInvoke()
    {
        TribeSetManager.PData.Tribe = 2;
        TribeSetManager.PData.TribeName = "Green";

        _tribeViewText.text = TribeSetManager.PData.TribeName;
    }
    public void Trbie4ButtonInvoke()
    {
        TribeSetManager.PData.Tribe = 3;
        TribeSetManager.PData.TribeName = "Human";
       
        _tribeViewText.text = TribeSetManager.PData.TribeName;
    }
    
    //Spell Button
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

    //종족과 스펠 선택시 Queue버튼 intercatble True
    void QueueSelectCheck() 
    {
        if (_settingPanel.activeSelf)
        {
            if (TribeSetManager.PData.Tribe != -1 && TribeSetManager.PData.Spell != -1)
            {
                _queueButton.interactable = true;
            }
        }
    }
    #endregion

#region Functions related to Profile
    //Profile
    public void ProfileNameSetInvoke()
    {
        if (_profileNameInputField.interactable == false)
        {
            _profileNameInputField.interactable = true;
            _profileSetButton.GetComponentInChildren<Text>().text = "설정";

        }
        else
        {
            ClientNetworkManager.Send("OnNickChangeRequest", _profileInputFieldText.text);
            _profileNameInputField.interactable = false;
            _profileSetButton.GetComponentInChildren<Text>().text = "변경";
        }
    }


    public void ProfileSet(string data)
    {
        TribeSetManager.PData.NickName = data;
        _profileInputFieldText.text = data;
        _profileInputFieldPlaceHolder.text = data;
    }
    #endregion

#region Counter
    //매칭 카운터
    IEnumerator QueueTimeCounter()
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

    //매칭 성공 후 count 시간 후 씬 전환
    IEnumerator MatchStart(float count)
    {
        _queueText.GetComponent<Text>().text = "매칭을 찾았습니다.";
        yield return new WaitForSeconds(count);
        SceneManager.LoadScene("02.Match");
    }
#endregion
}
