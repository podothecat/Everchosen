using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkController : MonoBehaviour
{
    public static bool connectionFlag;

    private Socket clientSocket;
    private string serverIP;

    private void Awake()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverIP = "127.0.0.1";
        connectionFlag = false;
        clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
        clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);

        try
        {
            IPAddress ipAddr = IPAddress.Parse(serverIP);
            Debug.Log(ipAddr);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 8889);
            Debug.Log(ipEndPoint);
            clientSocket.Connect(ipEndPoint);
            Debug.Log("Connection success to server.");
        }
        catch (SocketException SCE)
        {
            Debug.Log("Socket connect error! : " + SCE ); 
        }
    }

    // Use this for initialization
	private void Start ()
	{
	}
	
	// Update is called once per frame
	private void Update () 
    {   
	}
}
