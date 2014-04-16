using UnityEngine;
using System.Collections;

public class ServerListGUI : MonoBehaviour {

    private string ip = "";

    void OnGUI() {
        Rect area = new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2);
        GUILayout.BeginArea(area);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Create Server")){
            Debug.Log(Network.player.ipAddress);
            Networker.Instance.CreateServer();    
        }

        GUILayout.BeginVertical();
        if (GUILayout.Button("Join")) {
            Networker.Instance.JoinMatch(ip);
        }

        ip = GUILayout.TextField(ip);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
