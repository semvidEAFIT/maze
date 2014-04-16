using UnityEngine;
using System.Collections;

public class HitSkill : Skill {

	public float attackDistance = 1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
	}

	public override bool CheckInput(){
		return Input.GetAxis("Attack") > 0;
	}

	public override void Execute(){
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		if(Physics.SphereCast(ray, 1f, out hit, attackDistance)){
			if(hit.collider.CompareTag(ETag.Human.ToString())){

			}
		}
	}
}
