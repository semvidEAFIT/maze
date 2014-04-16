using UnityEngine;
using System.Collections.Generic;
using Boomlagoon.JSON;

public class LobbyHandler : MonoBehaviour {

    private Dictionary<string, bool> ready;

    void Awake() {
        ready = new Dictionary<string, bool>(Networker.MAXPLAYERS);
    }

    void Start() { 
        //TODO: Join Match
    }

    [RPC]
    public void JoinedMatch(string userName) {
        ready.Add(userName, false);
        Networker.Instance.players.Add(userName);
    }

    public void SetReady(string userName, bool isReady)
    {

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
