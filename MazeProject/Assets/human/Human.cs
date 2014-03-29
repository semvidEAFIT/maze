using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

	/// <summary>
	/// The step sound to be played. Must be loopeable.
	/// </summary>
	public AudioClip stepSound;


	void Start () {
		if(stepSound == null){
			Debug.LogError("StepSoundPlayer: step sound not asigned to object " + gameObject.name);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if((Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical")!=0) && !audio.isPlaying){
			PlaySound(stepSound);
		} 
	}

	/// <summary>
	/// Plays the sound given as parameter.
	/// </summary>
	/// <param name="sound">Sound to play.</param>
	public void PlaySound(AudioClip sound){
		audio.clip = sound;
		audio.Play();
	}
}
