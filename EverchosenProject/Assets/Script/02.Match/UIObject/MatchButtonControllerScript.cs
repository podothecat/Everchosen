using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MatchButtonControllerScript : MonoBehaviour {
    
    public void MainButtonInvoke()
    {
        SceneManager.LoadScene("01.MainMenu");
    }
}
	
