using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// Handles the Level GUI, uses states to know how to draw in a given moment,
/// to change the current state use the State property.
/// </summary>
public class HumanLevelGUI : LevelGUI
{
	#region EndOfLevel
	/// <summary>
	/// Time until the level completes.
	/// </summary>
	private float endTime;
	private string formattedTime;
	#endregion
	
	#region Singleton
	private LevelGUI instance;
	
	public LevelGUI Instance
	{
		get { return instance; }
	}
	
	public override void Awake () {
		base.Awake();
	}
	#endregion
	
	/// <summary>
	/// Modifies the LevelGUI according to the current game state
	/// </summary>
	public override void OnGUI() {
		base.OnGUI();
	}

	public override void DrawPlaying(){

	}

	#region EndOfGame
	public override void DrawEndOfGame() {
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), blackScreen);
		
		Rect headerArea = new Rect(Screen.width / 6f, Screen.height / 5f, 2f*Screen.width / 3f, Screen.height / 5f);
		GUI.Label(headerArea, "You won't go crazy\n... yet.", GUI.skin.GetStyle("Header"));
		
		Rect timeArea = new Rect(Screen.width / 4f, 3f*Screen.height/5f, Screen.width/2f, Screen.height/10f);
		GUI.Label(timeArea, "Time "+ formattedTime);
	}
	
	public override void SetEndOfGame() {
		endTime = Time.timeSinceLevelLoad;
		formattedTime = FormatNumber((int)endTime/60) + ":" + FormatNumber((int)endTime%60);
	}
	
	private string FormatNumber(int n) { 
		return (n <= 9)? "0"+n : ""+n;
	}
	#endregion
}
