using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CampaignButtonControllScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
    public void CampaignBackButton()
    {
        SceneManager.LoadScene("01.MainMenu");
    }
}
