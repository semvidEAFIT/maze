using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FPSInputController))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour {

	/// <summary>
	/// The step sound to be played. Must be loopeable.
	/// </summary>
	public AudioClip[] stepSounds;
	public AudioClip[] runningStepSounds;

	/// <summary>
	/// The time the human can run in seconds.
	/// </summary>
	public float sprintTime = 7;
	
	/// <summary>
	/// The sprint speed.
	/// </summary>
	public float sprintSpeed = 10;
	
	/// <summary>
	/// The regen rate.
	/// </summary>
	public float regenRate = 1.5f;

	/// <summary>
	/// The time needed to recover between sprints.
	/// </summary>
	public float restTime = 10;

	public float maximumFieldOfView;

	private float initialFieldOfView, currentFieldOfView;

	private float sprintTimeLeft;

	private float defaultSpeed;

	private bool canRun;

	private float timeRested;

	private bool frozen;

	//the one in charge of the character movement
	private CharacterMotor motor;

    //TODO: Delete this variables that 
	private float stepTimer;
	public float timeBetweenSteps = 1;

	void Start () {
		initialFieldOfView = currentFieldOfView = Camera.main.fieldOfView;
		if(stepSounds == null || stepSounds.Length==0){
			Debug.LogError("StepSoundPlayer: step sound not asigned to object " + gameObject.name);
		}
		sprintTimeLeft = sprintTime;
		motor = GetComponent<CharacterMotor>();
		defaultSpeed = motor.movement.maxForwardSpeed;
		canRun = true;
		timeRested = 0;
		stepTimer = timeBetweenSteps;
		frozen = false;
	}

	void Update () {
		if(!frozen){
        	Run();
		}
        //TODO: el volumen de los pasos debe ser mayor o menor dependiendo de la velocidad del jugador.
        //Debug.Log(motor.GetVelocity().magnitude);
	}

    /// <summary>
    /// Checks if the player is able to run in a given moment, if so,
    /// changes the speed used by the CharacterMotor component.
    /// If the player runs a given amount of time, he must rest before
    /// running again.
    /// </summary>
    private void Run() {
		bool running = Input.GetAxis("Sprint") > 0;
		bool walking = false;
        //TODO: Delete this line, this line is necessary because we don't have the animation yet.
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
			walking = true;
			if(stepTimer >= timeBetweenSteps){
				PlayStep(running);
				stepTimer = 0;
			} 
        }
		stepTimer+=Time.deltaTime;
        //The user still has time to run.
        if (canRun)
        {
            if (walking && running)
            {
                motor.movement.maxForwardSpeed = sprintSpeed;

				//double the stepTimer
				stepTimer+=Time.deltaTime;

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
				currentFieldOfView = Mathf.Lerp(currentFieldOfView, maximumFieldOfView, Time.deltaTime);
            }
            else
            {
                //Resting time logic.
                motor.movement.maxForwardSpeed = defaultSpeed;
                if (sprintTimeLeft < sprintTime)
                {
                    sprintTimeLeft += Time.deltaTime * regenRate;
				}
				currentFieldOfView = Mathf.Lerp(currentFieldOfView, initialFieldOfView, Time.deltaTime);
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
			currentFieldOfView = Mathf.Lerp(currentFieldOfView, initialFieldOfView, Time.deltaTime);
        }
		Camera.main.fieldOfView = currentFieldOfView;
    }

    /// <summary>
    /// Plays the step sound, intended to be used with the Animation.
    /// </summary>
	public void PlayStep(bool running){
		if(!running){
			audio.PlayOneShot(stepSounds[(int) (Random.value * stepSounds.Length-1)]);
		}
		else{
			audio.PlayOneShot(runningStepSounds[(int) (Random.value * stepSounds.Length-1)]);
		}
	}

    private void Play() {
        if (!audio.isPlaying)
        {
            audio.clip = stepSounds[Random.Range(0, stepSounds.Length)];
            audio.Play();
        }
    }


	public void FreezeMovement(){
		//desactiva el movimiento del mounstruo
		motor.enabled = false;
		frozen = true;
	}

	public void UnfreezeMovement(){
		//vuelve a activar el movimiento
		motor.enabled = true;
		frozen = false;
	}
}
