using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

	/// <summary>
	/// The available amount of sanity.
	/// </summary>
	public float sanity = 100f;

	void Update(){
	}

	public void Die(){
		sanity = 0f;
		GameMaster.Instance.HumanWasKilled();
	}
}
