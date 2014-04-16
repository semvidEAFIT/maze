using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(AudioSource))]
public class Human : MonoBehaviour {

	/// <summary>
	/// The step sound to be played. Must be loopeable.
	/// </summary>
	public AudioClip[] stepSounds;

	/// <summary>
	/// The time the human can run in seconds.
	/// </summary>
	public float sprintTime = 7;

	/// <summary>
	/// The time needed to recover between sprints.
	/// </summary>
	public float restTime = 10;

	/// <summary>
	/// The sprint speed.
	/// </summary>
	public float sprintSpeed = 10;
	
	/// <summary>
	/// The regen rate.
	/// </summary>
	public float regenRate = 1.5f;

	/// <summary>
	/// The available amount of sanity.
	/// </summary>
	public float sanity = 100f;

	private float sprintTimeLeft;

	private float defaultSpeed;

	private bool canRun;

	private float timeRested;


	//the one in charge of the character movement
	private CharacterMotor motor;

	void Start () {
		if(stepSounds == null || stepSounds.Length==0){
			Debug.LogError("StepSoundPlayer: step sound not asigned to object " + gameObject.name);
		}
		sprintTimeLeft = sprintTime;
		motor = GetComponent<CharacterMotor>();
		defaultSpeed = motor.movement.maxForwardSpeed;
		canRun = true;
		timeRested = 0;
	}

	void Update () {
		if((Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical")!=0) && !audio.isPlaying){
			Play();
		}


		if(canRun){
			if(Input.GetAxis("sprint") > 0){
				motor.movement.maxForwardSpeed = sprintSpeed;

				if(sprintTimeLeft <= 0){
					canRun = false;
					timeRested = 0;
				} else {
					sprintTimeLeft -= Time.deltaTime;
				}
			}
			else{
				motor.movement.maxForwardSpeed = defaultSpeed;
				if(sprintTimeLeft < sprintTime){
					sprintTimeLeft += Time.deltaTime * regenRate;
				}
			}
		}
		else{
			motor.movement.maxForwardSpeed = defaultSpeed;
			timeRested += Time.deltaTime * regenRate;
			if(timeRested >= restTime){
				canRun = true;
				timeRested = 0;
				sprintTimeLeft = sprintTime;
			}
		}


		
		
		//		if(sprintTimeLeft <= 0) {
//			canRun = false;
//			motor.movement.maxForwardSpeed = defaultSpeed;
//			timeRested = 0;
//		}
//
//		if(timeRested >= restTime){
//			canRun = true;
//		}
//
//		if(Input.GetAxis("sprint") != 0 && sprintTimeLeft > 0 && canRun){
//			motor.movement.maxForwardSpeed =  sprintSpeed;
//			sprintTimeLeft-=Time.deltaTime;
//
//			Debug.Log("runnnnnnnnn");
//		}
//		if(sprintTimeLeft < sprintTime && !canRun){
//			sprintTimeLeft += Time.deltaTime;
//		}
//
//		if(timeRested <= restTime){
//			timeRested += Time.deltaTime;
//		}
//

		//Debug.Log(sprintTimeLeft);
	}

	public void Die(){
		sanity = 0f;
	}

	public void PlayStep(){
		audio.PlayOneShot(stepSounds[(int) (Random.value * stepSounds.Length-1)]);
	}

	public void Play(){
		audio.clip = stepSounds[(int) (Random.value * stepSounds.Length-1)];
		audio.Play();
	}
}
