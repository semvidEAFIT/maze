using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;


public class LevelHandler : MonoBehaviour {
	


	bool isReady = false;
	public bool levelLoaded = false;
	int playersReady = 1;
	
	void Start(){
		if(Network.isClient){
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
		while(!isReady){
			networkView.RPC("SendReady", RPCMode.Others);
			yield return new WaitForSeconds(1f);
		}
	}

	[RPC]
	public void SendReady(){
		if(!isReady&&levelLoaded){
			networkView.RPC("UpdateReady", RPCMode.Server);
			isReady = true;
		}
	}

	[RPC]//server
	public void UpdateReady()
	{
		playersReady++;
		if(playersReady == Networker.Instance.players.Count){// si todos listos

			isReady = true;
			int r = Random.Range(0,Networker.Instance.players.Count-1);//quien es el humano
			//SetRole(r==0);
			if(r==0){
				Network.Instantiate(Networker.Instance.humanPrefab, Maze.Instance.GetMazePosition(Maze.Instance.startingX,Maze.Instance.startingX)
				                    , Quaternion.identity,0);
			}else{
				Network.Instantiate(Networker.Instance.monsterPrefab, Maze.Instance.GetMazePosition(Maze.Instance.startingX,Maze.Instance.startingX)
				                    , Quaternion.identity,0);
			}
			r--;
			Debug.Log(Networker.Instance.NetworkPlayers.Count);
			for(int i = 0; i < Networker.Instance.NetworkPlayers.Count; i++){
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
		Debug.Log("seee");
		if(isHuman){
			//Maze.Instance.InstantiateObject(Networker.Instance.humanPrefab, Maze.Instance.startingX, Maze.Instance.startingY);
			Network.Instantiate(Networker.Instance.humanPrefab, Maze.Instance.GetMazePosition(Maze.Instance.startingX,Maze.Instance.startingX)
			                    , Quaternion.identity,0);
		}else{
			//Maze.Instance.InstantiateObject(Networker.Instance.monsterPrefab, Maze.Instance.startingX, Maze.Instance.startingY+1);
			Network.Instantiate(Networker.Instance.monsterPrefab, Maze.Instance.GetMazePosition(Maze.Instance.startingX,Maze.Instance.startingX)
			                    , Quaternion.identity,0);
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
