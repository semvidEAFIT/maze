using UnityEngine;
using System.Collections;

public class Networker : MonoBehaviour {
	
	
	public GameObject playerPrefab;
	public Transform spawnPoint;
    public int maxPlayers = 2;
    public int port = 80;
    private const string gameType = "maze_test";
	private bool no_pick = true;
	private bool refresh = false;
	private HostData[] hosts;
	
	// Update is called once per frame
	void Update () 
	{
		if(refresh && MasterServer.PollHostList().Length > 0)
		{
			Debug.Log(MasterServer.PollHostList().Length);
			hosts = MasterServer.PollHostList();
		}
	}

    void StartServer()
	{
		Network.InitializeServer(maxPlayers, port,!Network.HavePublicAddress());
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
		SpawnPlayer();
	}
	
	void OnConnectedToServer()
	{
		SpawnPlayer();
	}
	
	void SpawnPlayer()
	{
		Network.Instantiate(playerPrefab,spawnPoint.position,Quaternion.identity, 0);
	}
	
	void RefreshHostList()
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
				StartServer();
				
			}
			if(GUI.Button(new Rect(20,70,80,20), "Client")) 
			{
				Debug.Log ("Soy un cliente");
				RefreshHostList();
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