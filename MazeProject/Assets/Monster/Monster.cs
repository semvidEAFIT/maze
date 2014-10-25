using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	private Movement moveScript;
	public string monsterName;
	private bool frozen;
	public MonsterLevelGUI mlg;
	public bool Frozen {
		get{
			return frozen;
		}
	}
	void Awake(){
		if(networkView.isMine){
			transform.GetComponentInChildren<Camera>().enabled = true;
			monsterName = Networker.Instance.UserName;
			gameObject.AddComponent<MonsterLevelGUI>();
			MonsterLevelGUI ml = (MonsterLevelGUI) GetComponent<MonsterLevelGUI>();
			ml = mlg;
			//transform.GetComponent<MonsterLevelGUI>().enabled = true;
		}else{
			transform.GetComponentInChildren<Camera>().enabled = false;
//			Destroy(gameObject.GetComponent<HumanLevelGUI>());
//			transform.GetComponent<AudioListener>().enabled = false;
		}
	}
	// Use this for initialization
	void Start () {
		moveScript = this.gameObject.GetComponent<Movement>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	[RPC]
	public void Freeze ()
	{
		//TODO: Refactorizar freezing del monstruo para que sea compatible con el networking.
		//Detener el movimiento del mounstruo
		if(!frozen){
			moveScript.FreezeMovement();
		}
		//play the freeze sound

		networkView.RPC("SetFrozen", RPCMode.Others , true);
		frozen = true;
	}
	[RPC]
	public void Unfreeze ()
	{
		//TODO: Refactorizar unfreezing del monstruo para que sea compatible con el networking
		//reanudar movimiento
		if(frozen){
			moveScript.UnfreezeMovement();
			frozen = false;
			networkView.RPC("SetFrozen", RPCMode.Others , false);
		}
	}

	[RPC]
	public void SetFrozen(bool frz){
		frozen = frz;
	}


	void OnGUI(){
		if(networkView.isMine){
			GUI.Label(new Rect(0,0,Screen.width*0.1f,Screen.height*0.05f),"Monster");
		}
	}

	bool changeName = true;
	void OnSerializeNetworkView(BitStream stream,NetworkMessageInfo info){
		if(changeName){
			if(stream.isWriting){
				char t = 'a';
				foreach(char c in monsterName){
					t = c;
					stream.Serialize(ref t);
				}
				t = '\n';
				stream.Serialize(ref t);
			}else{
				char c ='b';
				stream.Serialize(ref c);
				while(c!='\n'){
					monsterName += c;
					stream.Serialize(ref c);
				}
			}
			changeName=false;
			GameMaster.Instance.Monsters.Add (this.gameObject,monsterName);
		}
	}

	
	public void SendRPC(string method, NetworkPlayer player){
		networkView.RPC(method, player, null);
	}
	public void CallEnd(){
		networkView.SendMessage("End",RPCMode.All);
	}
	[RPC]
	public void End(){
		transform.GetComponent<CharacterController>().enabled = false;
	}
}
