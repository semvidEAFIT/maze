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

	private LevelGUI levelGUI;
    private int numberOfPlayers;
    public GameObject human;
	public NetworkPlayer npHuman;
    public GameObject monster;
	private Dictionary<GameObject,string> monsters;

	public float viewRadius;

	public AudioClip introAmbience;
	public AudioClip loopAmbience;

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
		GameObject.Find("Networker").GetComponent<LevelHandler>().levelLoaded = true;
		monsters = new Dictionary<GameObject,string >();
        //TODO: Spawn players in different positions and different prefabs

	}

	// Use this for initialization
	void Awake () {
		if(instance != null){
			Debug.Log("You can only have one GameMaster per match.");
			Destroy (this);
		}else{
			instance = this;
		}

		//reproducir el intro de la musica del ambiente
		playIntro();
	}

	void Update(){
		//CheckVicinity();
	}

	/// <summary>
	/// play the intro of the ambience.
	/// </summary>
	private void playIntro(){
		audio.clip = introAmbience;
		audio.Play();
		audio.loop = false;

		//reproduze el loop del ambiente cuando termina el intro.
		StartCoroutine(CheckForIntroEnd());
	}

	/// <summary>
	/// Checks the human's vicinity to monsters, 
	/// to freeze / unfreeze monsters if necessary (if the player is seeing them).
	/// </summary>
	public void CheckVicinity ()
	{
		//TODO: Refactorizar freezing / unfreezing del monstruo para que sea compatible con el networking.
		//Se sacan el script y la posicion del humano.
		Vector2 humanPos = new Vector2(human.transform.position.x, human.transform.position.z);
		Human humanScript = human.GetComponent<Human>();
		//Se comparara la posicion del humano con la de cada monstruo 
		//para ver cual esta dentro del radio de vision.
		foreach(GameObject monster in monsters.Keys){
			//Se sacan el script y la pos. del monstruo actual.
			Monster monsterScript = monster.GetComponent<Monster>();
			Vector2 monsterPos = new Vector2(monster.transform.position.x, monster.transform.position.z);
			//Se mira si el monstruo esta dentro del radio de vision.
			if(Mathf.Sqrt(Vector2.SqrMagnitude(monsterPos - humanPos)) <= viewRadius){
				//TODO:conectar con networker
				//networkView.RPC("MonsterNear",npHuman,null);
				//humanScript.MonsterNear();

				//CheckSeeingMonster retorna verdadero si tiene vision directa del monstruo.
				bool seeingMonster = humanScript.CheckSeeingMonster(monster);
				if(seeingMonster){
				   if(!monsterScript.Frozen){
						//monsterScript.Freeze();
						monster.GetComponent<Monster>().SendRPC("Freeze", Networker.Instance.NameToNetworkPlayer[monsters[monster]]);
					}
				}
				else{
					if(monsterScript.Frozen){
						//monsterScript.Unfreeze();
						monster.GetComponent<Monster>().SendRPC("Unfreeze", Networker.Instance.NameToNetworkPlayer[monsters[monster]]);
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

	//Revisa si el intro de ambiente termino y comienza a reproducir el loop de ambiente.s
	private IEnumerator CheckForIntroEnd(){
		//revisar si el sonido de intro termino.
		for(;;){
			//si si, poner el clip del audiosource como el loop ppal.
			if(!audio.isPlaying){
				audio.clip = loopAmbience;
				audio.loop = true;
				audio.Play();
				break;
			}
			yield return new WaitForSeconds(0);
		}

	}

	public Dictionary<GameObject,string> Monsters {
		get {
			return monsters;
		}
	}

	private bool humanReady = false;
	/*public void AddMonster(GameObject monster){
		monsters.Add(monster);
		if(!humanReady){
			if(human!=null){
				human.GetComponent<Human>().Monsters = monsters;
				humanReady = true;
			}
		}else{
			human.GetComponent<Human>().AddMonster(monster);
		}
	}*/
}
