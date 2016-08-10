using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
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
     
	    StartCoroutine(loadingRoutine());
        
	}
	
	// Update is called once per frame
	void Update () {
      
        



	    if (AccountValid == true)
	    {
            //SceneManager.LoadScene("01.MainMenu");
        }

        //뒤로가기 버튼 종료 처리 함수
	 

	}


    IEnumerator loadingRoutine()//테스트로 만든 로딩하는데 걸리는  시간 설정
    {
        yield return new WaitForSeconds(Random.Range(2,3));
        LoadingComplete = true;
        AccountValid = true;
       // Debug.Log("로딩완료");

    }

    


}
