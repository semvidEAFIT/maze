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

	public abstract bool CheckInput();

	public abstract void Execute();
}