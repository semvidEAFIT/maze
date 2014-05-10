using UnityEngine;
using System.Collections;

public class ServerListGUI : MonoBehaviour {

    private string ip = "";
    private string name = "";

    void OnGUI()
    {
        Rect area = new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2);
        GUILayout.BeginArea(area);
        GUILayout.BeginHorizontal();
        GUILayout.Label("userName");
        name = GUILayout.TextField(name);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Create Server")){
            Networker.Instance.UserName = name;
            Networker.Instance.CreateServer();
        }

        GUILayout.BeginVertical();
        if (GUILayout.Button("Join")) {
            Networker.Instance.UserName = name;
            Networker.Instance.JoinMatch(ip);
        }

        ip = GUILayout.TextField(ip);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    void OnConnectedToServer()
    {
        Networker.Instance.LoadLevel("Lobby");
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Could not connect to server: " + error);
    }

    void OnServerInitialized()
    {
        Networker.Instance.LoadLevel("Lobby");
    }
}
