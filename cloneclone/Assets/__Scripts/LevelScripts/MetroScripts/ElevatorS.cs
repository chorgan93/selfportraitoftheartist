using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorS : MonoBehaviour {

	public int currentStop = 0;

	[Header("Travel Properties")]
	public string elevatorDestination;
	public string destinationName;
	private string currentStopString;
	private string currentStopName;

	public float travelTime;
	private float timeToNextStop;

	public int stopSpawnPt;

	[Header("Timing Properties")]
	public float timePerStop = 8f;
	private float currentTimeAtStop;
	private string doorsClosingString = "train_text_15";
	private bool doorsClosingMessageGiven = false;
	private float timeBeforeDoorsMessage = 4f;

	private bool elevatorIsMoving = false;
	private bool elevatorIsStopped = false;

	private bool nextStopMessageGiven = false;
	private string nextStopString = "elevator_00";
	public float timeToNextStopMessage = 3f;
	private float nextStopMessageCountdown;

	private bool nowArrivingMessageGiven = false;
	private string nowArrivingString = "train_text_17";
	public float timeBeforeNowArrivingMessage = 4f;
	public float messageTime = 6f;
	private float messageTimeOut;

	[Header ("Shake Properties")]
	public float smallShakeTimeMin = 0.4f;
	public float smallShakeTimeMax = 0.9f;
	private float smallShakeCountdown;

	public float largeShakeTimeMin = 0.75f;
	public float largeShakeTimeMax = 2f;
	private float largeShakeCountdown;

    public ScrollingEnvironmentS[] scrollingObjs;

    [Header("Combat Properties")]
    public CombatManagerS elevatorCombat;
    private bool checkCombat = false;

	[Header("Sound Properties")]
	public GameObject doorOpenSound;
	public GameObject doorCloseSound;
	public GameObject announceSound;
	public float timeBeforeClosingSound = 1f;
	public float timeBeforeOpeningSound = 0.75f;
	private bool doorSoundMade = false;
	public AudioSource rumblingSource;
	private float rumblingMaxVolume;

	private float rumbleT;
	private float rumbleInTime = 0.9f;
	private float rumbleOutTime = 0.15f;
	private bool endedRumble = false;
	private float rumbleAdjustCount;
	private bool rumbleStart = false;
	private bool rumbleEnd = false;


	private PlayerController pRef;

	private bool arrived = false; // unlike trains, elevators are one time use only


	private bool messageIsUp = false;


	public Collider[] exitColliders;
	private List<ChangeSceneTriggerS> sceneChangeColliders = new List<ChangeSceneTriggerS>();
	public bool useAltCollider = false;

    public bool elevatorToNowhere = false;


	// Use this for initialization
	void Start () {

	
		elevatorIsStopped = true;
		currentTimeAtStop = timePerStop/2f;

        checkCombat = (elevatorCombat != null);

		AssignColliders();
		//SetColliders(true);

		SetSmallShake();
		SetLargeShake();
			CameraShakeS.C.lockYShake = true;


		rumblingMaxVolume = rumblingSource.volume;
		rumblingSource.volume = 0f;
		rumblingSource.Play();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (elevatorIsStopped){
			if (!arrived){
				currentTimeAtStop -= Time.deltaTime;
			}
			if (currentTimeAtStop <= timeBeforeDoorsMessage && !doorsClosingMessageGiven){
				doorsClosingMessageGiven = true;
				DialogueManagerS.D.SetDisplayText(doorsClosingString,false,false);
				if (announceSound){
					Instantiate(announceSound);
				}
			}
			if (currentTimeAtStop <= timeBeforeClosingSound && !doorSoundMade){

				if (doorCloseSound){
					Instantiate(doorCloseSound);
				}
				doorSoundMade = true;
			}
			if (currentTimeAtStop <= 0 && !arrived){
				SetColliders(false);
				elevatorIsStopped = false;
				elevatorIsMoving = true;
				doorSoundMade = false;
				StartRumble();


					
				currentStopString = elevatorDestination;
				currentStopName = destinationName;
				timeToNextStop = travelTime;
				
				DialogueManagerS.D.EndText();
				nextStopMessageCountdown = timeToNextStopMessage;
				nextStopMessageGiven = false;
				doorsClosingMessageGiven = false;

				CameraShakeS.C.LargeShake();
			}
		}

		if (elevatorIsMoving){
			HandleRumble();
			ManageShake();

            if ((!checkCombat || (checkCombat && !elevatorCombat.combatIsActive)) && !elevatorToNowhere)
            {
                timeToNextStop -= Time.deltaTime;
                nextStopMessageCountdown -= Time.deltaTime;
            }
			if (nextStopMessageCountdown <= 0 && !nextStopMessageGiven){

				nextStopMessageGiven = true;
					DialogueManagerS.D.SetDisplayText(nextStopString, false, false);
					if (announceSound){
						Instantiate(announceSound);
					}
				messageTimeOut = messageTime;
				messageIsUp = true;
			}
			if (!nowArrivingMessageGiven && timeToNextStop <= timeBeforeNowArrivingMessage){
				nowArrivingMessageGiven = true;
                DialogueManagerS.D.SetDisplayText(LocalizationManager.instance.GetLocalizedValue(nowArrivingString) + " "
                                                  + LocalizationManager.instance.GetLocalizedValue(currentStopName), false, false,
                                                  false, false, null,1f,true);
					if (announceSound){
						Instantiate(announceSound);
					}
				messageTimeOut = messageTime;
				messageIsUp = true;
			}

				if (!endedRumble && timeToNextStop <= rumbleOutTime){
					endedRumble = true;
					EndRumble();
				}

				if (!doorSoundMade && timeToNextStop <= timeBeforeOpeningSound){
					if (doorOpenSound){
						Instantiate(doorOpenSound);
					}
					doorSoundMade = true;
				}

			if (timeToNextStop <= 0){
				elevatorIsMoving = false;
					endedRumble = false;
				arrived = true;
				nextStopMessageGiven = false;
				nowArrivingMessageGiven = false;
				currentTimeAtStop = timePerStop;
				elevatorIsStopped = true;
				CameraShakeS.C.LargeShake();
					doorSoundMade = false;
				SetColliders(true, useAltCollider);
			}

		}

		if (messageIsUp){
			messageTimeOut -= Time.deltaTime;
			if (messageTimeOut <= 0){
				DialogueManagerS.D.EndText();
				messageIsUp = false;
			}
		}

	}

	void StartRumble(){
		rumbleStart = true;
		rumbleAdjustCount = rumbleInTime;

        SetScrolling(true);
	}

	void EndRumble(){
		rumbleAdjustCount = rumbleOutTime;
		rumbleEnd = true;

        SetScrolling(false);
	}

	void HandleRumble(){
		if (rumbleEnd){
			rumbleAdjustCount -= Time.deltaTime;
			if (rumbleAdjustCount <= 0){
				rumbleAdjustCount = 0;
				rumbleEnd = false;
			}
			rumbleT = rumbleAdjustCount/rumbleOutTime;
			rumbleT = Mathf.Sin(rumbleT * Mathf.PI * 0.5f);
			rumblingSource.volume = Mathf.Lerp(0f,rumblingMaxVolume,rumbleT)*SFXObjS.volumeSetting;
		}else if (rumbleStart){
			rumbleAdjustCount -= Time.deltaTime;
			if (rumbleAdjustCount <= 0){
				rumbleAdjustCount = 0;
				rumbleStart = false;
			}
			rumbleT = rumbleAdjustCount/rumbleInTime;
			rumbleT = Mathf.Sin(rumbleT * Mathf.PI * 0.5f);
			rumblingSource.volume = Mathf.Lerp(rumblingMaxVolume,0f,rumbleT)*SFXObjS.volumeSetting;
		}else{
			rumblingSource.volume = rumblingMaxVolume*SFXObjS.volumeSetting;
		}
	}

	void SetColliders(bool setOn, bool useAlt = false){
		if (setOn){
			SetSceneChanges();
		}
		for (int i = 0; i < exitColliders.Length; i++){
			if (!useAlt || (useAlt && i > 0)){
				exitColliders[i].gameObject.SetActive(setOn);
			}
		}
	}



	void AssignColliders(){
        if (!elevatorToNowhere)
        {
            sceneChangeColliders.Clear();
            for (int i = 0; i < exitColliders.Length; i++)
            {
                sceneChangeColliders.Add(exitColliders[i].GetComponent<ChangeSceneTriggerS>());
            }
        }
	}

	void SetSceneChanges(){


			for (int i = 0; i < sceneChangeColliders.Count; i++){
			sceneChangeColliders[i].nextSceneString = elevatorDestination;
			sceneChangeColliders[i].whereToSpawn = stopSpawnPt;
			}

	}

	void SetSmallShake(){
		smallShakeCountdown = Random.Range(smallShakeTimeMin, smallShakeTimeMax);
	}

	void SetLargeShake(){
		largeShakeCountdown = Random.Range(largeShakeTimeMin, largeShakeTimeMax);
	}

	void ManageShake(){
		if (smallShakeCountdown <= 0){
			CameraShakeS.C.MicroShake();

			SetSmallShake();
		}
		if (largeShakeCountdown <= 0){
				CameraShakeS.C.SmallShake();

			SetLargeShake();
		}
		smallShakeCountdown -= Time.deltaTime;
		largeShakeCountdown -= Time.deltaTime;

	}

    public void SetScrolling(bool newScroll = true){
        for (int i = 0; i < scrollingObjs.Length; i++){
            scrollingObjs[i].enabled = newScroll; 
        }
    }

}
