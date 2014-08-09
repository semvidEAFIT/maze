using UnityEngine;
using System.Collections;

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

	/// <summary>
	/// The sound played when the player dies.
	/// </summary>
	public AudioClip playerDead;
	private bool playerIsDead;

//	private bool dead = false;
	#endregion

	void Start(){
		timeToPlayNear = 0;
		timeToPlaySeeingMonster = 0;
		seeingMonster = false;
		playerIsDead = false;
		Camera.SetupCurrent(camera);
	}

	void Update(){
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
		//reproducir sonido de mounstuo cercano.
		if(timeToPlayNear <= 0){
			AudioClip clip = monsterNear[Random.Range(0,monsterNear.Length)]; //escoje el clip
			timeToPlayNear = clip.length+delayMonsterNear; //toma el tiempo que se demora el clip
			audio.PlayOneShot(clip); //lo reproduce
		}

		
	}
}
