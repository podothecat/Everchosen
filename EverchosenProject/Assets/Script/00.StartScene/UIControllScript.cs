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


    

    
	// Use this for initialization
	void Start () {
	    _loadingPanel = GameObject.Find("LoadingPanel");

        //211.245.70.35 , 127.0.0.1
        ClientNetworkManager.ConnectToServer("211.245.70.35", 23000);

    }
	
	// Update is called once per frame
	void Update () {
      
        



	    if (ClientNetworkManager.Connected==true)
	    {
            SceneManager.LoadScene("01.MainMenu");
        }
	   

        //뒤로가기 버튼 종료 처리 함수
	 

	}


    

    


}
