using UnityEngine;
using System.Collections;

/// <summary>
/// Plays step sound when gameObjects changes position.
/// </summary>
public class StepSoundPlayer : MonoBehaviour {

	/// <summary>
	/// The step sound to be played. Must be loopeable.
	/// </summary>
	public AudioClip stepSound;


	void Start () {
		if(stepSound == null){
			Debug.LogError("StepSoundPlayer: step sound not asigned to object " + gameObject.name);
		}
		audio.clip = stepSound;
	}
	
	// Update is called once per frame
	void Update () {
		if((Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical")!=0) && !audio.isPlaying){
			audio.clip = stepSound;
			audio.Play();
		} 
	}
	
}
