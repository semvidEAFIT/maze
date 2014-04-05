using UnityEngine;
using System.Collections;

/// <summary>
/// Plays a random sound when the human player crosses an invisible
/// trigger, the next sound will be played when a defined cool down
/// time has passed.
/// </summary>
[RequireComponent (typeof (AudioSource))]
public class SoundTrigger : MonoBehaviour {

	/// <summary>
	/// The pool of sounds to take one randomly.
	/// </summary>
	public AudioClip[] environmentSounds;
	/// <summary>
	/// The time that will pass between two random sounds played.
	/// </summary>
	public float coolDownTime;

	private AudioSource source;
	private bool coolDown = false;
	private float elapsedTime = 0f;

	void Awake(){
		source = GetComponent<AudioSource>();
	}

	void Update(){
		if(coolDown){
			if(elapsedTime < coolDownTime){
				elapsedTime += Time.deltaTime;
			}else{
				coolDown = false;
				elapsedTime = 0f;
			}
		}
	}

	void OnTriggerEnter(Collider other){
		if(!coolDown && other.CompareTag(ETag.Human.ToString())){
			PlayRandomSound();
		}
	}

	/// <summary>
	/// Plays a random sound from the public pool.
	/// </summary>
	private void PlayRandomSound(){
		AudioClip c = environmentSounds[Random.Range(0, environmentSounds.Length)];
		source.PlayOneShot(c);
	}
}
