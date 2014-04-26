using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	private bool frozen;
	public bool Frozen {
		get{
			return frozen;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Freeze ()
	{
		//TODO: Refactorizar freezing del monstruo para que sea compatible con el networking.
		frozen = true;
	}

	public void Unfreeze ()
	{
		//TODO: Refactorizar unfreezing del monstruo para que sea compatible con el networking.
		frozen = false;
	}
}
