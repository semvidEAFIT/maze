using UnityEngine;
using System.Collections;

/// <summary>
/// In charge of managing all control events and other situations that
/// changes the state of the game.
              /// </summary>
public class GameMaster : MonoBehaviour {

	#region Variables
	private static GameMaster instance;
	public int numbrerOfPlayers = 5;//solo esta publico para probar temporalmente.
	#endregion

	public int NumbrerOfPlayers {
		get {
			return numbrerOfPlayers;
		}
		set {
			numbrerOfPlayers = value;
		}
	}


	public static GameMaster Instance {
		get{
			return instance;
		}
	}

	void Start(){
		LevelGUI.Instance.State = EState.Playing;
	}

	// Use this for initialization
	void Awake () {
		if(instance != null){
			Debug.Log("You can only have one GameMaster per match.");
			Destroy (this);
		}else{
			instance = this;
		}
	}

	/// <summary>
	/// Human player reached one exit.
	/// </summary>
	/// <param name="human">Game object.</param>
	public void PlayerReachedExit (GameObject human, GameObject exit)
	{
		//TODO: End of game logic.
		LevelGUI.Instance.State = EState.Ended;
	}
}
