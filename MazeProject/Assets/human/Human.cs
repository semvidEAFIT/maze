using UnityEngine;
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

	private bool canRun;

	/// <summary>
	/// The time needed to recover between sprints.
	/// </summary>
	public float restTime = 10;

	private float timeRested;

	/// <summary>
	/// The sprint speed.
	/// </summary>
	public float sprintSpeed = 10;

	/// <summary>
	/// The regen rate.
	/// </summary>
	public float regenRate = 1.5f;

	//the one in charge of the character movement
	private CharacterMotor motor;

	void Start () {
		if(stepSound == null){
			Debug.LogError("StepSoundPlayer: step sound not asigned to object " + gameObject.name);
		}
		sprintTimeLeft = sprintTime;
		motor = GetComponent<CharacterMotor>();
		defaultSpeed = motor.movement.maxForwardSpeed;
		canRun = true;
		timeRested = 0;
	}

	void Update () {
        Run();
	}

    /// <summary>
    /// Checks if the player is able to run in a given moment, if so,
    /// changes the speed used by the CharacterMotor component.
    /// If the player runs a given amount of time, he must rest before
    /// running again.
    /// </summary>
    private void Run() {
        //TODO: Delete this line, this line is necessary because we don't have the animation yet.
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && !audio.isPlaying)
        {
            PlayStep();
        }

        //The user still has time to run.
        if (canRun)
        {
            if (Input.GetAxis("sprint") > 0)
            {
                motor.movement.maxForwardSpeed = sprintSpeed;

                //Run time limit logic.
                if (sprintTimeLeft <= 0)
                {
                    canRun = false;
                    timeRested = 0;
                }
                else
                {
                    sprintTimeLeft -= Time.deltaTime;
                }
            }
            else
            {
                //Resting time logic.
                motor.movement.maxForwardSpeed = defaultSpeed;
                if (sprintTimeLeft < sprintTime)
                {
                    sprintTimeLeft += Time.deltaTime * regenRate;
                }
            }
        }
        else
        {
            //Resting time logic.
            motor.movement.maxForwardSpeed = defaultSpeed;
            timeRested += Time.deltaTime * regenRate;
            if (timeRested >= restTime)
            {
                canRun = true;
                timeRested = 0;
                sprintTimeLeft = sprintTime;
            }
        }
    }

    /// <summary>
    /// Plays the step sound, intended to be used with the Animation.
    /// </summary>
	public void PlayStep(){
		audio.PlayOneShot(stepSound);
	}
}
