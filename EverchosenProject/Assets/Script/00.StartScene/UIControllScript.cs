using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using Client;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIControllScript : MonoBehaviour
{
    private GameObject _loadingPanel;
    public bool LoadingComplete=false;//로딩확인
    public bool AccountValid=false;//계정확인
    public bool Send;

    void Awake()
    {
        ClientNetworkManager.ClientDeviceId = SystemInfo.deviceUniqueIdentifier;
        ClientNetworkManager.ConnectToServer("211.245.70.35", 23000);
    }
    
	// Use this for initialization
	void Start () {
        Application.runInBackground = true;//백그라운드에서도 게임이 실행되도록 
        
        _loadingPanel = GameObject.Find("LoadingPanel");
        //211.245.70.35 , 127.0.0.1<-자기자신한테 보낼떄 , 219.254.17.66<--내공유기외부 ip
       
    }

	
	// Update is called once per frame
	void Update () {

	    if (ClientNetworkManager.Connected==true&&Send==false)//소켓이 연결되고 아이디를 받아올때 다음씬으로 넘어감 
	    {
            ClientNetworkManager.Send("OnLoginRequest", ClientNetworkManager.ClientDeviceId);
	        Send = true;
	    }

        if (ClientNetworkManager.ProfileData != null)
        {
            TribeSetManager.PData.NickName = ClientNetworkManager.ProfileData.NickName;
            SceneManager.LoadScene("01.MainMenu");
        }
    }
    

}
