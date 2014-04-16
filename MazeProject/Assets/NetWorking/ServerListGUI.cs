using UnityEngine;
using System.Collections;

public class ServerListGUI : MonoBehaviour {

    private string ip = "";
    private string name = "";

    void OnGUI() {
        Rect area = new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2);
        GUILayout.BeginArea(area);
        GUILayout.BeginHorizontal();
        GUILayout.Label("userName");
        name = GUILayout.TextField(name);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Create Server")){
            Debug.Log(Network.player.ipAddress);
            Networker.Instance.CreateServer();
            Networker.Instance.UserName = name;
            Application.LoadLevel("Lobby");
        }

        GUILayout.BeginVertical();
        if (GUILayout.Button("Join")) {
            Networker.Instance.JoinMatch(ip);
            Networker.Instance.UserName = name;
            Application.LoadLevel("Lobby");
        }

        ip = GUILayout.TextField(ip);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
