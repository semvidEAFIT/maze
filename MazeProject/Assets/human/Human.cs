using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

	/// <summary>
	/// The available amount of sanity.
	/// </summary>
	public float sanity = 100f;
	public float sanityLossQtyPerSec = 0.5f;
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
					sanity -= sanityLossQtyPerSec * Time.deltaTime;
					Debug.Log(sanity);
					if(sanity <= 0f){
						PlayDeath();
					}
					return true;
				}
			}
		}

		return false;

	}

	private void PlayDeath(){
		gameObject.transform.localScale *= 0.1f;
		transform.Rotate(new Vector3(90f, 0f, 0f));
		GetComponent<CharacterController>().radius *= 10f;
	}
}
