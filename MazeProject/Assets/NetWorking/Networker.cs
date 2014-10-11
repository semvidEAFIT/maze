﻿using UnityEngine;
using System.Collections.Generic;
using Boomlagoon.JSON;
using System;

[RequireComponent (typeof(NetworkView))]
public class Networker : MonoBehaviour {

	public GameObject gameMasterPrefab;
	public GameObject humanPrefab;
	public GameObject monsterPrefab;

    private const int PORT = 7456;
    public const int MAXPLAYERS = 5;

    private EPlayerType playerType;
    private int spawnIndex;

    private static Networker instance;

    public List<string> players;
	
    private string userName;

	public string UserName {
		get {
			return userName;
		}
		set {
			userName = value;
		}
	}
	
   
	//TODO: quitar networkplayers y players, reemplazarlos por nameToNetworkPlayers
    private List<NetworkPlayer> networkPlayers;

    public List<NetworkPlayer> NetworkPlayers
    {
        get { return networkPlayers; }
    }

	private Dictionary<string,NetworkPlayer> nameToNetworkPlayer;

	public Dictionary<string, NetworkPlayer> NameToNetworkPlayer {
		get {
			return nameToNetworkPlayer;
		}
	}



    public static Networker Instance
    {
        get { return Networker.instance; }
    }

    public int SpawnIndex
    {
        get { return spawnIndex; }
    }

    public EPlayerType PlayerType
    {
        get { return playerType; }
        set { playerType = value; }
    }

    void Start() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Only one networker per client is allowed.");
            Destroy(gameObject);
        }
		nameToNetworkPlayer = new Dictionary<string,NetworkPlayer>(4);
        networkPlayers = new List<NetworkPlayer>(4);
    }

    public void JoinMatch(string ip) {
        NetworkConnectionError error = Network.Connect(ip, PORT);
        if (error != NetworkConnectionError.NoError) throw new Exception(error.ToString());
    }

    public void CreateServer() {
        NetworkConnectionError error = Network.InitializeServer(MAXPLAYERS-1, PORT, false);
        if (error != NetworkConnectionError.NoError) throw new Exception(error.ToString());
    }

    public void LoadLevel(string level) {
        switch (level) { 
            case "Level":
                DestroyImmediate(GetComponent<LobbyHandler>());
                gameObject.AddComponent<LevelHandler>();
                break;
            case "Lobby":
                DestroyImmediate(GetComponent<LevelHandler>());
                gameObject.AddComponent<LobbyHandler>();
                break;
            default:
                break;
        }
        Application.LoadLevel(level);
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
       // networkPlayers.Add(player);
    }

	public void SendToName(){

	}
	public void SendToAll(string method){
		foreach(NetworkPlayer np in nameToNetworkPlayer.Values){
			networkView.RPC(method,np,null);
		}
	}
	[RPC]
	public void EndGame(bool first){
		if(first){
			networkView.RPC("EndGame",RPCMode.Others,false);
		}else{
			GameMaster.Instance.HumanWasKilled();
		}
		foreach(GameObject g in GameMaster.Instance.Monsters.Keys){
			g.transform.GetComponent<Monster>().CallEnd();
		}
		GameMaster.Instance.human.transform.GetComponent<Human>().CallEnd();
	}
}
