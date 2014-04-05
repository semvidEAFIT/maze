using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Handles the Level GUI, uses states to know how to draw in a given moment,
/// to change the current state use the State property.
/// </summary>
public class LevelGUI : MonoBehaviour
{
    #region Textures
    public Texture2D blackScreen;
    #endregion

    #region Variables
    private GameState currentState = GameState.Playing;

    /// <summary>
    /// Contains the methods to be called when the given GameState value is
    /// set as the current value.
    /// </summary>
    private Action[] setup;

    public GameState State
    {
        get { return currentState; }
        set { 
            currentState = value; 
            if(setup[(int)currentState] != null) setup[(int)currentState]();
        }
    }

    public GUISkin skin;

    #region EndOfLevel
    /// <summary>
    /// Time until the level completes.
    /// </summary>
    private float endTime;
    private string formattedTime;
    #endregion
    #endregion

    #region Singleton
    private LevelGUI instance;

    public LevelGUI Instance
    {
        get { return instance; }
    }

	void Awake () {
        if (instance != null)
        {
            Debug.Log("Only one LevelGUI can exist at a time.");
            Destroy(this);
        }
        else 
        {
            instance = this;
            setup = new Action[] {null, SetEndOfGame};
        }
	}
    #endregion

    /// <summary>
    /// Modifies the LevelGUI according to the current game state
    /// </summary>
    public void OnGUI() {
        if (skin != null) GUI.skin = skin;
        switch (currentState)
        {
            case GameState.Playing:
                break;
            case GameState.Ended:
                DrawEndOfGame();
                break;
            default:
                break;
        }
    }

    #region EndOfGame
    private void DrawEndOfGame() {
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), blackScreen);

        Rect headerArea = new Rect(Screen.width / 6f, Screen.height / 5f, 2f*Screen.width / 3f, Screen.height / 5f);
        GUI.Label(headerArea, "You won't go crazy\n... yet.", GUI.skin.GetStyle("Header"));

        Rect timeArea = new Rect(Screen.width / 4f, 3f*Screen.height/5f, Screen.width/2f, Screen.height/10f);
        GUI.Label(timeArea, "Time "+ formattedTime);
    }

    private void SetEndOfGame() {
        endTime = Time.timeSinceLevelLoad;
        formattedTime = FormatNumber((int)endTime/60) + ":" + FormatNumber((int)endTime%60);
    }

    private string FormatNumber(int n) { 
        return (n <= 9)? "0"+n : ""+n;
    }
    #endregion
}
