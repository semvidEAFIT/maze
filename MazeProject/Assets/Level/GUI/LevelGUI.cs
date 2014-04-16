using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Handles the Level GUI, uses states to know how to draw in a given moment,
/// to change the current state use the State property.
/// </summary>
public abstract class LevelGUI : MonoBehaviour
{
    #region Textures
    public Texture2D blackScreen;
    #endregion

    #region Variables
    private EState currentState = EState.Playing;

    /// <summary>
    /// Contains the methods to be called when the given EState value is
    /// set as the current value.
    /// </summary>
    private Action[] setup;

	private Action[] stateActions;

    public EState State
    {
        get { return currentState; }
        set { 
            currentState = value; 
            if(setup[(int)currentState] != null) setup[(int)currentState]();
        }
    }

    public GUISkin skin;
    #endregion

    #region Singleton
    private static LevelGUI instance;

    public static LevelGUI Instance
    {
        get { return LevelGUI.instance; }
    }

	public virtual void Awake () {
        if (instance != null)
        {
            Debug.Log("Only one LevelGUI can exist at a time.");
            Destroy(this);
        }
        else 
        {
            instance = this;
            setup = new Action[] {null, SetEndOfGame, null};

			stateActions = new Action[Enum.GetNames(typeof(EState)).Length];
			stateActions[(int)EState.Playing] = DrawPlaying;
			stateActions[(int) EState.Ended] = DrawEndOfGame;
			stateActions[(int)EState.HumanKilled] = DrawHumanKilled;
        }
	}
    #endregion

    /// <summary>
    /// Modifies the LevelGUI according to the current game state
    /// </summary>
    public virtual void OnGUI() {
        if (skin != null) GUI.skin = skin;
		stateActions[(int)currentState]();
    }

	public abstract void DrawPlaying ();

    #region EndOfGame
	public abstract void DrawEndOfGame();

	public abstract void DrawHumanKilled();

	public abstract void SetEndOfGame();

	/// <summary>
	/// Formats the number.
	/// </summary>
	/// <returns>The number.</returns>
	/// <param name="n">N.</param>
    public static string FormatNumber(int n) { 
        return (n <= 9)? "0"+n : ""+n;
    }
    #endregion
}
