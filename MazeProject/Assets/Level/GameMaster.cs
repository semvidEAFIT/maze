using UnityEngine;
using System.Collections;

/// <summary>
/// In charge of managing all control events and other situations that
/// changes the state of the game.
/// </summary>
[RequireComponent (typeof (LevelGUI))]
public class GameMaster : MonoBehaviour {

	#region Variables
	private static GameMaster instance;
	private LevelGUI levelGUI;
	#endregion

	public static GameMaster Instance {
		get{
			return instance;
		}
	}

	void Start(){
		levelGUI.State = GameState.Playing;
	}

	// Use this for initialization
	void Awake () {
		if(instance != null){
			Debug.Log("You can only have one GameMaster per match.");
			Destroy (this);
		}else{
			instance = this;
			levelGUI = GetComponent<LevelGUI>();
		}
	}

	/// <summary>
	/// Human player reached one exit exit.
	/// </summary>
	/// <param name="human">Game object.</param>
	public void PlayerReachedExit (GameObject human, GameObject exit)
	{
		//TODO: End of game logic.
		levelGUI.State = GameState.Ended;
	}
}
