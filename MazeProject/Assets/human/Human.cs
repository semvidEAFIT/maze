﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(AudioSource))]
public class Human : MonoBehaviour {

	/// <summary>
	/// The step sound to be played. Must be loopeable.
	/// </summary>
	public AudioClip stepSound;

	/// <summary>
	/// The time the human can run in seconds.
	/// </summary>
	public float sprintTime = 7;

	private float sprintTimeLeft;

	private float defaultSpeed;

	/// <summary>
	/// The sprint speed.
	/// </summary>
	public float sprintSpeed = 20;

	//the one in charge of the character movement
	private CharacterMotor motor;

	void Start () {
		if(stepSound == null){
			Debug.LogError("StepSoundPlayer: step sound not asigned to object " + gameObject.name);
		}
		sprintTime/=10;
		sprintTimeLeft = sprintTime;
		motor = GetComponent<CharacterMotor>();
		defaultSpeed = motor.movement.maxForwardSpeed;

	}

	void Update () {
		if((Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical")!=0) && !audio.isPlaying){
			PlaySound(stepSound);
		}


		if(Input.GetAxis("sprint")!=0 && sprintTimeLeft>0){
			motor.movement.maxForwardSpeed =  sprintSpeed;
			sprintTimeLeft-=Time.deltaTime;
		} else {
			motor.movement.maxForwardSpeed = defaultSpeed;
			sprintTimeLeft+=Time.deltaTime;
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
