using UnityEngine;
using System.Collections.Generic;
using Boomlagoon.JSON;

public class LobbyHandler : MonoBehaviour {

    private volatile Dictionary<string, bool> ready;

    public Dictionary<string, bool> Ready
    {
        get { return ready; }
    }

    void Awake() {
        ready = new Dictionary<string, bool>(Networker.MAXPLAYERS);
    }

    void Start() {
        ready.Add(Networker.Instance.UserName, false);
        if (Network.isClient) {
            networkView.RPC("JoinedMatch", RPCMode.Server, Networker.Instance.UserName, Network.player);
        }
    }

    [RPC] // Server
    public void JoinedMatch(string userName, NetworkPlayer networkPlayer)
    {
        Networker.Instance.players.Add(userName);

        JSONArray players = new JSONArray();
        foreach (string playerName in ready.Keys)
        {
            JSONObject player = new JSONObject();
            player.Add("userName", playerName);
            player.Add("isReady", ready[playerName]);
            players.Add(player);
        }

		networkView.RPC("InitializePlayers", networkPlayer, players.ToString());
		networkView.RPC("AddNetworkView", RPCMode.Others, Network.player);
		networkView.RPC("AddNetworkView", RPCMode.Others, networkPlayer);
		Networker.Instance.NetworkPlayers.Add(networkPlayer);

        ready.Add(userName, false);
        
        JSONObject newPlayer = new JSONObject();
        newPlayer.Add("userName", userName);
        newPlayer.Add("isReady", false);
        networkView.RPC("UpdateReady", RPCMode.Others, newPlayer.ToString());
    }

    [RPC]
    public void InitializePlayers(string jsonPlayers) {
        JSONArray players = JSONArray.Parse(jsonPlayers);
        for (int i = 0; i < players.Length; i++)
        {
            JSONObject player = players[i].Obj;
            ready.Add(player.GetString("userName"), player.GetBoolean("isReady"));
            Networker.Instance.players.Add(player.GetString("userName"));
        }
    }

	[RPC]
	public void AddNetworkView(NetworkPlayer networkPlayer){
		Networker.Instance.NetworkPlayers.Add(networkPlayer);
	}
    public void SetReady(bool isReady)
    {
        ready[Networker.Instance.UserName] = isReady;
        JSONObject json = new JSONObject();
        json.Add("userName", Networker.Instance.UserName);
        json.Add("isReady", isReady);
        networkView.RPC("UpdateReady", RPCMode.Others, json.ToString());
    }
    
    [RPC]
    public void UpdateReady(string isReadyJSON)
    {
        JSONObject isReady = JSONObject.Parse(isReadyJSON);
        if (ready.ContainsKey(isReady.GetString("userName")))
        {
            ready[isReady.GetString("userName")] = isReady.GetBoolean("isReady");
        }
        else 
        {
            Networker.Instance.players.Add(isReady.GetString("userName"));
            ready.Add(isReady.GetString("userName"), isReady.GetBoolean("isReady"));
        }
    }

    public void StartMatch() {
        if (Network.isServer) { 
            // TODO: Call begin match in all players.
			networkView.RPC("BeginMatch", RPCMode.Others);
			Networker.Instance.LoadLevel("Level");
        }
    }
	/// <summary>
	/// Begins the match.
	/// </summary>
	/// <param name="isHuman">If set to <c>true</c> the player will be the human.</param>
    [RPC]
    public void BeginMatch()
    {
        //TODO: Load level and set player type and position
		Debug.Log("lol");
		Networker.Instance.LoadLevel("Level");
    }

}
