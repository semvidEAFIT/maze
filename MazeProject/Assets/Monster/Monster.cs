using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	private Movement moveScript;
	public string monsterName;
	private bool frozen;
	public bool Frozen {
		get{
			return frozen;
		}
	}
	void Awake(){
		if(networkView.isMine){
			transform.GetComponentInChildren<Camera>().enabled = true;
			monsterName = Networker.Instance.UserName;
		}else{
			transform.GetComponentInChildren<Camera>().enabled = false;
			transform.GetComponent<AudioSource>().enabled = false;
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
		if(!frozen){
			moveScript.FreezeMovement();
		}
		//play the freeze sound


		frozen = true;
	}

	public void Unfreeze ()
	{
		//TODO: Refactorizar unfreezing del monstruo para que sea compatible con el networking.

		//reanudar movimiento
		if(frozen){
			moveScript.UnfreezeMovement();

			frozen = false;
		}
	}

	
	void OnGUI(){
		if(networkView.isMine){
			GUI.Label(new Rect(0,0,Screen.width*0.1f,Screen.height*0.05f),"Monster");
		}
	}
}
