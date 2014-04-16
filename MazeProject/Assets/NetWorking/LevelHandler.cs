using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;

public class LevelHandler : MonoBehaviour {

    [RPC]
    public void EndMatch(JSONObject info)
    {
        //TODO: Go to lobby and destroy this object
    }

    [RPC]
    public void HumanDied(JSONObject info)
    {
        //TODO: Call method in the listener
    }

    [RPC]
    public void HumanEscaped(JSONObject info)
    {
        //TODO: Call method in the listener
    }
}
