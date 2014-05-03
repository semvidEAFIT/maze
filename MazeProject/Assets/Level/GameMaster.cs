using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// In charge of managing all control events and other situations that
/// changes the state of the game.
              /// </summary>
public class GameMaster : MonoBehaviour {

	#region Variables
	private static GameMaster instance;
	public int numbrerOfPlayers = 5;//solo esta publico para probar temporalmente.
	public List<GameObject> monsters;
	public GameObject human;
	public float viewRadius;

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

	void Update(){
		CheckVicinity();
	}

	/// <summary>
	/// Checks the human's vicinity to monsters, 
	/// to freeze / unfreeze monsters if necessary (if the player is seeing them).
	/// </summary>
	void CheckVicinity ()
	{
		//TODO: Refactorizar freezing / unfreezing del monstruo para que sea compatible con el networking.
		//Se sacan el script y la posicion del humano.
		Vector2 humanPos = new Vector2(human.transform.position.x, human.transform.position.z);
		Human humanScript = human.GetComponent<Human>();
		//Se comparara la posicion del humano con la de cada monstruo 
		//para ver cual esta dentro del radio de vision.
		foreach(GameObject monster in monsters){
			//Se sacan el script y la pos. del monstruo actual.
			Monster monsterScript = monster.GetComponent<Monster>();
			Vector2 monsterPos = new Vector2(monster.transform.position.x, monster.transform.position.z);
			//Se mira si el monstruo esta dentro del radio de vision.
			if(Mathf.Sqrt(Vector2.SqrMagnitude(monsterPos - humanPos)) <= viewRadius){
				//CheckSeeingMonster retorna verdadero si tiene vision directa del monstruo.
				bool seeingMonster = humanScript.CheckSeeingMonster(monster);
				if(seeingMonster){
				   if(!monsterScript.Frozen){
						monsterScript.Freeze();
					}
				}
				else{
					if(monsterScript.Frozen){
						monsterScript.Unfreeze();
					}
				}
			}
			//Si no esta en el rango de vision, revisar si el monstruo esta congelado y descongelarlo.
			else{
				if(monsterScript.Frozen){
					monsterScript.Unfreeze();
				}
			}
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
