using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Handles the Lobby GUI, uses states to know how to draw in a given moment,
/// to change the current state use the State property.
/// </summary>
public class LobbyGUI : MonoBehaviour
{
    #region Textures

    #endregion

    #region Variables
    public GUISkin skin;

	public string lobbyName; //public for testing TODO: delete once connection logic is ready
	public string LobbyName {
		get {
			return lobbyName;
		}
		set {
			lobbyName = value;
		}
	}

	public string lobbyIp; //public for testing TODO: delete once connection logic is ready
	public string LobbyIp {
		get {
			return lobbyIp;
		}
		set {
			lobbyIp = value;
		}
	}

    private LobbyHandler handler;
    #endregion

    #region Singleton
    private static LobbyGUI instance;

    public static LobbyGUI Instance
    {
        get { return LobbyGUI.instance; }
    }

	public virtual void Awake () {
        if (instance != null)
        {
            Debug.Log("Only one LobbyGUI can exist at a time.");
            Destroy(this);
        }
        else 
        {
            instance = this;
        }
	}
    #endregion

    void Start() {
        handler = GameObject.Find("Networker").GetComponent<LobbyHandler>();
    }

    /// <summary>
    /// Modifies the LobbyGUI according to the current game state
    /// </summary>
    public virtual void OnGUI() {
        if (skin != null) GUI.skin = skin;

		//lobby Name and ip (title)
		int titleHeigh = 2*Screen.height/9; //espacio en y que ocupa
		GUI.BeginGroup(new Rect(20,5,Screen.width, titleHeigh));
		DrawLobyInfo(titleHeigh);
		GUI.EndGroup();

		//Players in the lobby
		int playerInfoHeigh = Screen.height/3; //espacio en y que ocupa
		int playerInfoWidth = 2*Screen.width/3; //espacio en x que ocupa
		GUI.BeginGroup(new Rect(10,titleHeigh + 10, playerInfoWidth, playerInfoHeigh));
		DrawPlayers(playerInfoWidth, playerInfoHeigh);
		GUI.EndGroup();

		//Ready toggle
		int readyHeigh = (Screen.height/4) -20; //espacio en y que ocupa
		int readyWidth = (Screen.width/6) -20; //espacio en x que ocupa
		GUI.BeginGroup(new Rect((5*Screen.width/6)+10, 3*Screen.height/4, readyWidth, readyHeigh));
		DrawReady(readyWidth, readyHeigh);
		GUI.EndGroup();
	}

	private void DrawLobyInfo(int ySize){ 
		GUI.Label(new Rect(0, 0,Screen.width, ySize/2), lobbyName); //loby name
		GUI.Label(new Rect(10, ySize/2, Screen.width, ySize/2), lobbyIp); //lobby ip
	}

	private void DrawPlayers(int xSize, int ySize){
		int labelsHeigh = 25;
		for(int i = 0; i < Networker.Instance.players.Count; i++){
			GUI.BeginGroup(new Rect(0, i*(labelsHeigh+5), xSize, labelsHeigh));
			GUI.Box(new Rect(0, 0, xSize, labelsHeigh), "  " + Networker.Instance.players[i]);
            GUI.Toggle(new Rect(xSize - 40, 0, 40, labelsHeigh), handler.Ready[Networker.Instance.players[i]], "");
			GUI.EndGroup();
		}
	}

	private void DrawReady(int xSize, int ySize){
		if(GUI.Button(new Rect(0,0,xSize,ySize), "Ready")){
            handler.SetReady(!handler.Ready[Networker.Instance.UserName]);
		}
        GUI.Toggle(new Rect(3 * xSize / 4, (ySize / 2) - 10, 15, ySize), handler.Ready[Networker.Instance.UserName], "");
	}
}
