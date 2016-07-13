using UnityEngine;
using System.Collections;

public class CampaignButtonControllScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
    public void CampaignBackButton()
    {
        Application.LoadLevel("MainMenu");
    }
}
