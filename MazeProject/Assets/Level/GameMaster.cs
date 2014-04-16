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
    private int numberOfPlayers;
    public GameObject mazeGenerator;
    public GameObject human;
    public GameObject monster;
	#endregion

	public int NumberOfPlayers {
		get {
			return numberOfPlayers;
		}
	}

	public static GameMaster Instance {
		get{
			return instance;
		}
	}

	void Start(){
		levelGUI.State = EState.Playing;
        numberOfPlayers = Networker.Instance.players.Count;
        if(Network.isServer){
            Instantiate(mazeGenerator);
        }
        //TODO: Spawn players in different positions and different prefabs
        Maze.Instance.InstantiateObject(human, Maze.Instance.startingX, Maze.Instance.startingY);
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
	/// Human player reached one exit.
	/// </summary>
	/// <param name="human">Game object.</param>
	public void PlayerReachedExit (GameObject human, GameObject exit)
	{
		//TODO: End of game logic.
		levelGUI.State = EState.Ended;
	}
}
