using UnityEngine;
using System.Collections;

/// <summary>
/// In charge of managing all control events and other situations that
/// changes the state of the game.
              /// </summary>
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
        numberOfPlayers = Networker.Instance.players.Count;
        if(Network.isServer){
            Instantiate(mazeGenerator);
        }
        //TODO: Spawn players in different positions and different prefabs
        Maze.Instance.InstantiateObject(human, Maze.Instance.startingX, Maze.Instance.startingY);
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

	public void HumanWasKilled(){
		//TODO: End of game logic.
		LevelGUI.Instance.State = EState.HumanKilled;
	}
}
