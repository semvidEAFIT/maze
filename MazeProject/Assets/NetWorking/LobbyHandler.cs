using UnityEngine;
using System.Collections.Generic;
using Boomlagoon.JSON;

public class LobbyHandler : MonoBehaviour {

    private Dictionary<string, bool> ready;

    void Awake() {
        ready = new Dictionary<string, bool>(Networker.MAXPLAYERS);
    }

    void Start() {
        networkView.RPC("JoinedMatch", RPCMode.AllBuffered, "Player");
    }

    [RPC]
    public void JoinedMatch(string userName) {
        ready.Add(userName, false);
        Networker.Instance.players.Add(userName);
        Debug.Log(userName + " has joined the match.");
    }

    public void SetReady(string userName, bool isReady)
    {
        JSONObject json = new JSONObject();
        json.Add("userName", userName);
        json.Add("isReady", isReady);
        networkView.RPC("UpdateReady", RPCMode.AllBuffered, json);
    }
    
    [RPC]
    public void UpdateReady(JSONObject isReady)
    {
        ready[isReady.GetString("userName")] = isReady.GetBoolean("isReady");
    }

    public void StartMatch() {
        if (Network.isServer) { 
            // TODO: Call begin match in all players.
        }
    }

    [RPC]
    public void BeginMatch(JSONObject info)
    {
        //TODO: Load level and set player type and position
    }

}
