using UnityEngine;
using System.Collections;
using Client;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
        ClientNetworkManager.ConnectToServer("211.245.70.35", 23000);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
