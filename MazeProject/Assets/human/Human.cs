using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

	/// <summary>
	/// The available amount of sanity.
	/// </summary>
	public float sanity = 100f;
//	private bool dead = false;
//
//	void Update(){
//		if(sanity == 0 && !dead){
//			dead = true;
//			Die();
//		}
//	}

	public void Die(){
		sanity = 0f;
		GameMaster.Instance.HumanWasKilled();
		PlayDeath();
	}

	private void PlayDeath(){
		gameObject.transform.localScale *= 0.1f;
		transform.Rotate(new Vector3(90f, 0f, 0f));
		GetComponent<CharacterController>().radius *= 10f;
	}
}
