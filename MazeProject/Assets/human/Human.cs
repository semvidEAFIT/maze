using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Human : MonoBehaviour {
	#region variables
	/// <summary>
	/// The available amount of sanity.
	/// </summary>
	public float sanity = 100f;

	/// <summary>
	/// The sanity loss quantity per second.
	/// </summary>
	public float sanityLossQtyPerSec = 0.5f;

	/// <summary>
	/// The sounds for when the monster is near.
	/// </summary>
	public AudioClip[] monsterNear;

	/// <summary>
	/// The delay to play monster near sounds after
	/// the human stopped seeing it.
	/// </summary>
	public float delayMonsterNear = 0.5f;


	private float timeToPlayNear;

	/// <summary>
	/// The seeing monster.
	/// </summary>
	private bool seeingMonster;

	private float timeToPlaySeeingMonster;

	public float monsterSeenDelay = 20f;

	public AudioClip[] seeingMonsterSound;

	private bool playedMonsterSeenSound;

	public List<GameObject> monsters;
	
	/// <summary>
	/// The sound played when the player dies.
	/// </summary>
	public AudioClip playerDead;
	private bool playerIsDead;

//	private bool dead = false;
	#endregion
	public string humanName;
	void Awake(){
		if(networkView.isMine){
			transform.GetComponentInChildren<Camera>().enabled = true;
			humanName = Networker.Instance.UserName;
		}else{
			transform.GetComponentInChildren<Camera>().enabled = false;
//			transform.GetComponent<AudioListener>().enabled = false;
		}
	}

	void Start(){
		timeToPlayNear = 0;
		timeToPlaySeeingMonster = 0;
		seeingMonster = false;
		playerIsDead = false;
	}

	void Update(){
		if(networkView.isMine){
			GameMaster.Instance.CheckVicinity();
			if(sanity > 0){
				sanity -= sanityLossQtyPerSec * Time.deltaTime;
			} else {
				if(!playerIsDead){
					Die();
				}
			}

			if(timeToPlayNear>0){
				timeToPlayNear -= Time.deltaTime;
			}

			if(timeToPlaySeeingMonster > 0){
				if(!seeingMonster){
					timeToPlaySeeingMonster -= Time.deltaTime;
				}
			} else if(seeingMonster) {
				//play seeing monster
				audio.PlayOneShot(seeingMonsterSound[Random.Range(0,seeingMonsterSound.Length)]);
				timeToPlaySeeingMonster=monsterSeenDelay;
			}
		}
	}

	void CheckVicinity ()
	{
		//TODO: Refactorizar freezing / unfreezing del monstruo para que sea compatible con el networking.
		//Se sacan el script y la posicion del humano.
		Vector2 humanPos = new Vector2(transform.position.x, transform.position.z);
		//Se comparara la posicion del humano con la de cada monstruo 
		//para ver cual esta dentro del radio de vision.
		foreach(GameObject monster in monsters){
			//Se sacan el script y la pos. del monstruo actual.
			Monster monsterScript = monster.GetComponent<Monster>();
			Vector2 monsterPos = new Vector2(monster.transform.position.x, monster.transform.position.z);
			//Se mira si el monstruo esta dentro del radio de vision.
			if(Mathf.Sqrt(Vector2.SqrMagnitude(monsterPos - humanPos)) <= GameMaster.Instance.viewRadius){
				//TODO:conectar con networker
				MonsterNear();
				//humanScript.MonsterNear();
				
				//CheckSeeingMonster retorna verdadero si tiene vision directa del monstruo.
				bool seeingMonster = CheckSeeingMonster(monster);
				string a = monsterScript.name;
				if(seeingMonster){

//					if(!monsterScript.Frozen){
					networkView.RPC("Freeze",Networker.Instance.NameToNetworkPlayer[a],null);
					//	monsterScript.Freeze();
//					}
				}
				else{
					networkView.RPC("Unfreeze",Networker.Instance.NameToNetworkPlayer[a],null);
//					if(monsterScript.Frozen){
//						monsterScript.Unfreeze();
//					}
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

	public void Die(){
		sanity = 0f;
		GameMaster.Instance.HumanWasKilled();
		gameObject.transform.localScale *= 0.1f;
		transform.Rotate(new Vector3(90f, 0f, 0f));
		GetComponent<CharacterController>().radius *= 10f;

		//play death sound.
		audio.PlayOneShot(playerDead);
		playerIsDead = true;
	}

	public bool CheckSeeingMonster (GameObject monster)
	{
		/**
		 * si tiro raycast a la pos. del monstruo y lo estoy viendo (no hay nada de por medio)
		 * entonces me quito cordura y devuelvo true.
		 * si no, devuelvo false.
		 */
		Debug.Log("Checking");
		Renderer monsterRenderer = (Renderer)monster.transform.GetComponentInChildren(typeof(Renderer));
		if(monsterRenderer.isVisible){
			Vector3 dir = monster.transform.position - this.transform.position;
			RaycastHit hit;
			if(Physics.Raycast(new Ray(this.transform.position, dir), out hit)){
				if(hit.collider.CompareTag(ETag.Monster.ToString())){
					if(!seeingMonster){
						seeingMonster = true;
					}


					sanity -= sanityLossQtyPerSec * Time.deltaTime;
					if(sanity <= 0){
						Die();
					}
					return true;
				}
			}
		}

		//not seeing monster
		if(seeingMonster){
			seeingMonster = false;
		}

		return false;
	}

	/// <summary>
	/// Function called when the monster is near.
	/// </summary>
	public void MonsterNear(){
		Debug.Log("Near");
		          //reproducir sonido de mounstuo cercano.
		if(timeToPlayNear <= 0){
			AudioClip clip = monsterNear[Random.Range(0,monsterNear.Length)]; //escoje el clip
			timeToPlayNear = clip.length+delayMonsterNear; //toma el tiempo que se demora el clip
			audio.PlayOneShot(clip); //lo reproduce
		}
	}

	void OnGUI(){
		if(networkView.isMine){
			GUI.Label(new Rect(0,0,Screen.width*0.1f,Screen.height*0.05f),"Humano");
		}
	}

	public List<GameObject> Monsters {
		get {
			return monsters;
		}
		set {
			monsters = value;
		}
	}
	public void AddMonster(GameObject monster){
		monsters.Add (monster);
	}

	bool changeName = true;
	void OnSerializeNetworkView(BitStream stream,NetworkMessageInfo info){
		if(changeName){
			if(stream.isWriting){
				char t='\n';
				foreach(char c in humanName){
					t = c;
					stream.Serialize(ref t);
				}
				t = '\n';
				stream.Serialize(ref t);
			}else{
				char c ='b';
				stream.Serialize(ref c);
				while(c!='\n'){
					humanName += c;
					stream.Serialize(ref c);
				}
			}
			changeName=false;
		}
	}
}
