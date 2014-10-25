using UnityEngine;
using System.Collections;

public class MonsterLevelGUI : LevelGUI {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region implemented abstract members of LevelGUI

	public override void DrawPlaying ()
	{
		throw new System.NotImplementedException ();
	}

	public override void DrawEndOfGame ()
	{
		throw new System.NotImplementedException ();
	}

	public override void DrawHumanKilled ()
	{
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), blackScreen);
		
		Rect headerArea = new Rect(Screen.width / 6f, Screen.height / 5f, 2f*Screen.width / 3f, Screen.height / 5f);
		GUI.Label(headerArea, "Human has perished.", GUI.skin.GetStyle("Header"));
	}

	public override void SetEndOfGame ()
	{
	}

	#endregion
}
