using UnityEngine;
using System.Collections;

public class HitSkill : Skill {

	/// <summary>
	/// The minimum distance at which the monster's attack will be effective.
	/// </summary>
	public float attackDistance = 1f;

	/// <summary>
	/// Indicates if the monster killed the human to avoid calling the Die method more than once.
	/// </summary>
	private bool killedHuman = false;

	public AudioClip hitSound;

	public AudioClip swipeSound;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
	}

	public override bool CheckInput(){
		return Input.GetMouseButtonDown(0);
	}

	/// <summary>
	/// Executes the skill. Shoots a spherecast and checks if it hit the player, if they are within te attackDistance.
	/// </summary>
	public override void Execute(){
		audio.clip = swipeSound;
		audio.Play();	

		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		if(Physics.SphereCast(ray, 1f, out hit, attackDistance)){
			if(hit.collider.CompareTag(ETag.Human.ToString()) && !killedHuman){
//				audio.PlayOneShot(hitSound);
				StartCoroutine(CheckForSwipeEnd());
				hit.collider.gameObject.GetComponent<Human>().Die();
				killedHuman = true;
			}
		}
	}

	//revisa si el swipe termino de reproducir y reproduce el sonido de que mato el humano
	private IEnumerator CheckForSwipeEnd(){
		//revisar si el sonido de intro termino.
		for(;;){
			//si si, poner el clip del audiosource como el loop ppal.
			if(!audio.isPlaying){
				audio.clip = hitSound;
				audio.Play();
				break;
			}
			yield return new WaitForSeconds(0);
		}
		
	}
}
