using UnityEngine;
using System.Collections.Generic;
using Boomlagoon.JSON;

public class LobbyHandler : MonoBehaviour {

    private Dictionary<string, bool> ready;

    public Dictionary<string, bool> Ready
    {
        get { return ready; }
    }

    void Awake() {
        ready = new Dictionary<string, bool>(Networker.MAXPLAYERS);
    }

    void Start() {
        ready.Add(Networker.Instance.UserName, false);
        networkView.RPC("JoinedMatch", RPCMode.OthersBuffered, Networker.Instance.UserName);
    }

    [RPC]
    public void JoinedMatch(string userName) {
        ready.Add(userName, false);
        Networker.Instance.players.Add(userName);
        Debug.Log(userName + " has joined the match.");
    }

    public void SetReady(bool isReady)
    {
        ready[Networker.Instance.UserName] = isReady;
        JSONObject json = new JSONObject();
        json.Add("userName", Networker.Instance.UserName);
        json.Add("isReady", isReady);
        networkView.RPC("UpdateReady", RPCMode.OthersBuffered, json);
    }
    
    [RPC]
    public void UpdateReady(JSONObject isReady)
    {
        Ready[isReady.GetString("userName")] = isReady.GetBoolean("isReady");
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
