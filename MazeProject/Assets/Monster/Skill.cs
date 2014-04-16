using UnityEngine;
using System.Collections;

public abstract class Skill : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if(CheckInput()){
			Execute();
		}
	}

	/// <summary>
	/// Checks the input necessary to call Execute.
	/// </summary>
	/// <returns><c>true</c>, if input was checked, <c>false</c> otherwise.</returns>
	public abstract bool CheckInput();

	/// <summary>
	/// Executes the skill.
	/// </summary>
	public abstract void Execute();
}