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
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		if(Physics.SphereCast(ray, 1f, out hit, attackDistance)){
			if(hit.collider.CompareTag(ETag.Human.ToString()) && !killedHuman){
				audio.PlayOneShot(hitSound);
				hit.collider.gameObject.GetComponent<Human>().Die();
				killedHuman = true;
			}
		}
	}
}
