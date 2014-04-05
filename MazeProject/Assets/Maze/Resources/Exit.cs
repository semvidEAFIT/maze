using UnityEngine;
using System.Collections;

/// <summary>
/// Maze exit that sends a message when the human player reaches the exit.
/// </summary>
public class Exit : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if(other.CompareTag(ETag.Human.ToString())){
			GameMaster.Instance.PlayerReachedExit(other.gameObject, gameObject);
		}
	}
}
