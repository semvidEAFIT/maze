using UnityEngine;
using System.Collections;

public class Networker : MonoBehaviour {
	
	
	public GameObject playerPrefab;
	public Transform spawn;
	
	
	private bool no_pick = true;
	private string gameType = "agomezl_test";
	private bool refresh = false;
	private HostData[] hosts;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(refresh && MasterServer.PollHostList().Length > 0)
		{
			Debug.Log(MasterServer.PollHostList().Length);
			hosts = MasterServer.PollHostList();
		}
		
	}
	
	
	//Server
	void startServer()
	{
		Network.InitializeServer(2,25001,!Network.HavePublicAddress());
		MasterServer.RegisterHost(gameType,"test01", "");
	}
	
	void OnMasterServerEvent(MasterServerEvent msg)
	{
		if(msg == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log ("Servidor registrado");
		}
		
	}
	
	void OnServerInitialized()
	{
		Debug.Log("Servidor iniciado");
		spawnPlayer();
	}
	
	void OnConnectedToServer()
	{
		spawnPlayer();
	}
	
	void spawnPlayer()
	{
		Network.Instantiate(playerPrefab,spawn.position,Quaternion.identity, 0);
	}
	
	
	void refreshHostList()
	{
		MasterServer.RequestHostList(gameType);
		refresh = true;
	}
	
	//GUI
	void OnGUI () 
	{
		if (!Network.isClient && !Network.isServer)
		{
			GUI.Box(new Rect(10,10,100,90), "Loader Menu");
			if(GUI.Button(new Rect(20,40,80,20), "Server")) 
			{
				Debug.Log("Soy un servidor");
				startServer();
				
			}
			if(GUI.Button(new Rect(20,70,80,20), "Client")) 
			{
				Debug.Log ("Soy un cliente");
				refreshHostList();
			}
			
			if (hosts != null)
			{
				if(hosts.Length > 0)
				{
					GUI.Box(new Rect(120, 10, 200, 30 + 30*hosts.Length), "Servers");
				}
				
				
				for(int i=0; i < hosts.Length ; i++)
				{
					if(GUI.Button(new Rect(130, 40 + 30*i, 180, 20), hosts[i].gameName))
					{
						Network.Connect(hosts[i]);
					}
				}
			}
		}
	}
	
	
}