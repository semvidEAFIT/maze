using UnityEngine;
using System.Collections;

public class HitSkill : Skill {

	/// <summary>
	/// The minimum distance at which the monster's attack will be effective.
	/// </summary>
	public float attackDistance = 1f;

	///The minimum time that must pass to be able to strike again
	public float strikeSpeed = 0.5f;

	private float timeSinceLastStrike;

	/// <summary>
	/// Indicates if the monster killed the human to avoid calling the Die method more than once.
	/// </summary>
	private bool killedHuman = false;

	public AudioClip hitSound;

	public AudioClip[] swipeSounds;

	// Use this for initialization
	void Start () {
		timeSinceLastStrike = strikeSpeed;
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();

		if(timeSinceLastStrike <= strikeSpeed){
			timeSinceLastStrike += Time.deltaTime;
		}
	}

	public override bool CheckInput(){
		return Input.GetMouseButtonDown(0);
	}

	/// <summary>
	/// Executes the skill. Shoots a spherecast and checks if it hit the player, if they are within te attackDistance.
	/// </summary>
	public override void Execute(){
		if(timeSinceLastStrike >= strikeSpeed){
			timeSinceLastStrike = 0;

			int randomIndex = Random.Range(0, swipeSounds.Length);
//			Debug.Log(randomIndex);
//			audio.clip = swipeSounds[randomIndex];
//			audio.Play();	

			Ray ray = new Ray(transform.position, transform.forward);
			RaycastHit hit;
			if(Physics.SphereCast(ray, 1f, out hit, attackDistance)){
				if(hit.collider.CompareTag(ETag.Human.ToString()) && !killedHuman){
	//				audio.PlayOneShot(hitSound);
					string name = hit.transform.GetComponent<Human>().name;
					StartCoroutine(CheckForSwipeEnd());
					hit.collider.gameObject.GetComponent<Human>().Die();
					killedHuman = true;
				}
			}
		}
	}


	//revisa si el swipe termino de reproducir y reproduce el sonido de que mato el humano
	private IEnumerator CheckForSwipeEnd(){
		//revisar si el sonido de intro termino.
		/*for(;;){
			//si si, poner el clip del audiosource como el loop ppal.
			if(!audio.isPlaying){
				audio.clip = hitSound;
				audio.Play();
				break;
			}*/
			yield return new WaitForSeconds(0);
		//}
		
	}
}
