using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

	/// <summary>
	/// The available amount of sanity.
	/// </summary>
	public float sanity = 100f;
	private bool dead = false;

	void Update(){
		if(sanity == 0 && !dead){
			Die();
			dead = true;
		}
	}

	public void Die(){
		sanity = 0f;
		GameMaster.Instance.HumanWasKilled();
	}
}
