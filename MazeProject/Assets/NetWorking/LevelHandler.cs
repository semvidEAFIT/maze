using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;


public class LevelHandler : MonoBehaviour {
	


	bool isReady = false;
	public bool levelLoaded = false;
	int playersReady = 1;
	
	void Start(){
		if(Network.isServer){
			StartCoroutine(ServerReady());
		}
	}

    [RPC]
    public void EndMatch(JSONObject info)
    {
        //TODO: Go to lobby and destroy this object
    }

    [RPC]
    public void HumanDied(JSONObject info)
    {
        //TODO: Call method in the listener
    }

    [RPC]
    public void HumanEscaped(JSONObject info)
    {
        //TODO: Call method in the listener
    }

	IEnumerator ServerReady(){
		while(!levelLoaded){
			yield return new WaitForSeconds(0.1f);
		}
		networkView.RPC("SendReady", RPCMode.OthersBuffered);
	}

	[RPC]
	public void SendReady(){
		StartCoroutine(WaitForLoad());
	}

	IEnumerator WaitForLoad(){
		while(!levelLoaded){yield return new WaitForSeconds(0.1f);}
		if(Network.isServer){
			Debug.Log("nope");
		}else{
			Debug.Log("yep");
		}
		networkView.RPC("CheckReady", RPCMode.Server);
	}

	[RPC]//server
	public void CheckReady()
	{
		playersReady++;

		if(playersReady == Networker.Instance.players.Count){// si todos listos

			isReady = true;
			int r = Random.Range(0,Networker.Instance.players.Count-1);//quien es el humano
			//SetRole(r==0);
			if(r==0){
				Networker.Instance.humanPrefab.name = Networker.Instance.UserName;
				GameObject human = (GameObject)Network.Instantiate(Networker.Instance.humanPrefab, Maze.Instance.GetMazePosition(Maze.Instance.startingX,Maze.Instance.startingX)
				                    , Quaternion.identity,0);
				GameMaster.Instance.human = human;
				GameMaster.Instance.npHuman = Network.player;
			}else{
				Networker.Instance.monsterPrefab.name = Networker.Instance.UserName;
				Network.Instantiate(Networker.Instance.monsterPrefab, Maze.Instance.GetMazePosition(Maze.Instance.startingX,Maze.Instance.startingX)
				                    , Quaternion.identity,0);
			}
			r--;
			for(int i = 1; i < Networker.Instance.NetworkPlayers.Count; i++){
				if(r == i){// si es el humano
					networkView.RPC("SetRole", Networker.Instance.NetworkPlayers[i], true);
				}else{// si es un monstruo
					networkView.RPC("SetRole", Networker.Instance.NetworkPlayers[i], false);
				}
			}
			LevelGUI.Instance.State = EState.Playing;
		}
	}
	/// <summary>
	/// Sets the role of the player will be(monster or human).
	/// </summary>
	/// <param name="isHuman">If set to <c>true</c> the player will be human.</param>
	[RPC]
	public void SetRole(bool isHuman){
		if(isHuman){
			Networker.Instance.humanPrefab.name = Networker.Instance.UserName;
			GameObject human =(GameObject)Network.Instantiate(Networker.Instance.humanPrefab, Maze.Instance.GetMazePosition(Maze.Instance.startingX,Maze.Instance.startingX)
			                    , Quaternion.identity,0);
			GameMaster.Instance.human = human;
			GameMaster.Instance.npHuman = Network.player;
		}else{
			Networker.Instance.monsterPrefab.name = Networker.Instance.UserName;
			GameObject monster = (GameObject)Network.Instantiate(Networker.Instance.monsterPrefab, Maze.Instance.GetMazePosition(Maze.Instance.startingX,Maze.Instance.startingX)
			                    , Quaternion.identity,0);
			GameMaster.Instance.AddMonster(monster);
		}
	}

	void OnLevelWasLoaded(int level) {
		if(Network.isServer){
			Network.Instantiate(Networker.Instance.gameMasterPrefab, Vector3.zero, Quaternion.identity, 0);
		}else{
			levelLoaded = true;
		}
	}
}
