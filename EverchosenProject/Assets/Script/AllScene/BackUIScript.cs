using UnityEngine;
using System.Collections;
using Client;
using UnityEngine.UI;

public class BackUIScript : MonoBehaviour
{
    private GameObject BackObject;
    private GameObject BackPanel;
    private GameObject ExitPanel;
    private GameObject OptionPanel;

	// Use this for initialization
	void Start () {
	    BackObject = GameObject.Find("BackObject");
	    BackPanel = BackObject.transform.FindChild("BackPanel").gameObject;
	    ExitPanel = BackObject.transform.FindChild("ExitPanel").gameObject;
	    OptionPanel = BackObject.transform.FindChild("OptionPanel").gameObject;
       
        DontDestroyOnLoad(this.gameObject);
        ExitPanel.transform.SetAsLastSibling();

        
        BackPanel.SetActive(false);
        OptionPanel.SetActive(false);
        ExitPanel.SetActive(false);
        
        
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
            if (ExitPanel.activeSelf)
            {
                ExitPanel.SetActive(false);
            }
            else if (OptionPanel.activeSelf)
            {
                OptionPanel.SetActive(false);
            }
            else
            {
                if (BackPanel.activeSelf)
                {
                    BackPanel.SetActive(false);
                }
                else
                {
                    BackPanel.SetActive(true);
                }

            }
        }
    }

    //backpanel 3개 버튼 함수
    public void OptionButtonInvoke()
    {
        OptionPanel.SetActive(true);
       
    }

    public void ReturnButtonInvoke()
    {
        BackPanel.SetActive(false);
    }

    public void ExitButtonInvoke()
    {
        if(!ExitPanel.activeSelf)
        ExitPanel.SetActive(true);
    }





    
    //exit panel 오픈시 yes 버튼 함수
    public void ExitYesButtonInvoke() 
    {


        ClientNetworkManager.Send("OnExitRequest",null);

       
        
        ClientNetworkManager.SocketClose();
        Debug.Log(ClientNetworkManager._clientSocket.Connected);
        Application.Quit();
    }
    //exit panel 오픈시 no 버튼 함수
    public void ExitNoButtonInvoke()
    {
            ExitPanel.SetActive(false);
    }


   
    //optionpanel의 backbutton
    public void OptionBackButtonInvoke()
    {
        OptionPanel.SetActive(false);

    }

}
