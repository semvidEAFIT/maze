using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	private Movement moveScript;

	private bool frozen;
	public bool Frozen {
		get{
			return frozen;
		}
	}
	void Awake(){
		if(networkView.isMine){
			transform.GetComponentInChildren<Camera>().enabled = true;
		}else{
			transform.GetComponentInChildren<Camera>().enabled = false;
		}
	}
	// Use this for initialization
	void Start () {
		moveScript = this.gameObject.GetComponent<Movement>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Freeze ()
	{
		//TODO: Refactorizar freezing del monstruo para que sea compatible con el networking.
		//Detener el movimiento del mounstruo
		moveScript.FreezeMovement();

		//play the freeze sound


		frozen = true;
	}

	public void Unfreeze ()
	{
		//TODO: Refactorizar unfreezing del monstruo para que sea compatible con el networking.

		//reanudar movimiento
		moveScript.UnfreezeMovement();

		frozen = false;
	}
}
