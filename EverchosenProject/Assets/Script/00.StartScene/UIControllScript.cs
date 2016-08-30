using UnityEngine;
using Client;
using UnityEngine.SceneManagement;

public class UIControllScript : MonoBehaviour
{
    public bool LoadingComplete=false;//로딩확인
    public bool AccountValid=false;//계정확인
    public bool Send;

    void Awake()
    {
        ClientNetworkManager.ClientDeviceId = SystemInfo.deviceUniqueIdentifier;
        ClientNetworkManager.ConnectToServer("211.245.70.35", 23000);
    }

	void Start () {
        Application.runInBackground = true;//백그라운드에서도 게임이 실행되도록 
    }

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
