using UnityEngine;
using System.Collections;

public class testerStepsLoop : MonoBehaviour {

	public AudioClip[] stepSounds;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (!audio.isPlaying)
		{
			audio.clip = stepSounds[Random.Range(0, stepSounds.Length-1)];
			audio.Play();
		}
	}
}
