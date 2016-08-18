using UnityEngine;
using System.Collections;
using Client;
using UnityEngine.UI;

public class BackUIScript : MonoBehaviour
{
    private GameObject _backObject;
    private GameObject _backPanel;
    private GameObject _exitPanel;
    private GameObject _optionPanel;

   

	// Use this for initialization
	void Start () {
	    _backObject = GameObject.Find("BackObject");
	    _backPanel = _backObject.transform.FindChild("BackPanel").gameObject;
	    _exitPanel = _backObject.transform.FindChild("ExitPanel").gameObject;
	    _optionPanel = _backObject.transform.FindChild("OptionPanel").gameObject;
       
        DontDestroyOnLoad(this.gameObject);
        _exitPanel.transform.SetAsLastSibling();

        
        _backPanel.SetActive(false);
        _optionPanel.SetActive(false);
        _exitPanel.SetActive(false);
        
        
	}
	
	// Update is called once per frame
	void Update ()
	{
	    BackFunction();//백버튼키 패널 실행 

	}

    
    //exit panel관련 함수들
    void BackFunction()//안드로이드 뒤로가기 버튼 누를시 exit패널 온
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_exitPanel.activeSelf)
            {
                _exitPanel.SetActive(false);
            }
            else if (_optionPanel.activeSelf)
            {
                _optionPanel.SetActive(false);
            }
            else
            {
                if (_backPanel.activeSelf)
                {
                    _backPanel.SetActive(false);
                }
                else
                {
                    _backPanel.SetActive(true);
                }
            }
        }
    }
    

    public void OptionButtonInvoke()
    {
        _optionPanel.SetActive(true);
    }

    public void ReturnButtonInvoke()
    {
        _backPanel.SetActive(false);
    }

    public void ExitButtonInvoke()
    {
        if(!_exitPanel.activeSelf)
        _exitPanel.SetActive(true);
    }
    
    //exit panel 오픈시 yes 버튼 함수
    public void ExitYesButtonInvoke() 
    {
        ClientNetworkManager.Send("OnExitRequest", null);
        Debug.Log(ClientNetworkManager.ClientSocket.Connected);
        Application.Quit();
    }
    //exit panel 오픈시 no 버튼 함수
    public void ExitNoButtonInvoke()
    {
            _exitPanel.SetActive(false);
    }

    
    //optionpanel의 backbutton
    public void OptionBackButtonInvoke()
    {
        _optionPanel.SetActive(false);
    }

    /// <summary>
    /// Called when BackUIController object was destroyed.
    /// </summary>
    void OnDestroy()
    {
        Debug.Log("BackUIController was destroyed.");
        ClientNetworkManager.SocketClose();
    }
    /*
    void OnApplicationQuit()
    {
        ClientNetworkManager.SocketClose();
    }*/
}
