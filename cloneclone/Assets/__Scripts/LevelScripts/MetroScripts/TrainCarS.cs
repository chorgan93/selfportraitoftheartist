using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrainCarS : MonoBehaviour {

	public static int currentStop = 0;
	public static int currentDirection = 1;

	[Header("Travel Properties")]
	public string[] trainStops;
	public string[] trainStopNames;
	private string currentStopString;
	private string currentStopName;

	public float[] travelTimes;
	private float timeToNextStop;

	public int[] stopSpawnPts;

	[Header("Timing Properties")]
	public float timePerStop = 8f;
	private float currentTimeAtStop;
	private string doorsClosingString = "Please step back. Doors are closing...";
	private bool doorsClosingMessageGiven = false;
	private float timeBeforeDoorsMessage = 4f;

	private bool trainIsMoving = false;
	private bool trainIsStopped = false;

	private bool nextStopMessageGiven = false;
	private string nextStopString = "Next stop: ";
	public float timeToNextStopMessage = 3f;
	private float nextStopMessageCountdown;

	private bool nowArrivingMessageGiven = false;
	private string nowArrivingString = "Now arriving: ";
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


	private bool messageIsUp = false;


	public Collider[] exitColliders;
	private List<ChangeSceneTriggerS> sceneChangeColliders = new List<ChangeSceneTriggerS>();


	// Use this for initialization
	void Start () {

		trainIsStopped = true;
		currentTimeAtStop = timePerStop/2f;

		AssignColliders();
		SetColliders(true);

		SetSmallShake();
		SetLargeShake();
		CameraShakeS.C.lockXShake = true;
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (trainIsStopped){
			currentTimeAtStop -= Time.deltaTime;
			if (currentTimeAtStop <= timeBeforeDoorsMessage && !doorsClosingMessageGiven){
				doorsClosingMessageGiven = true;
				DialogueManagerS.D.SetDisplayText(doorsClosingString,false,false);
			}
			if (currentTimeAtStop <= 0){
				SetColliders(false);
				trainIsStopped = false;
				trainIsMoving = true;
				currentStop+=currentDirection;
				if (currentStop > trainStops.Length-1){
					currentStop = trainStops.Length-2;
					currentDirection = -1;
				}
				if (currentStop < 0){
					currentStop = 1;
					currentDirection = 1;
				}
				currentStopString = trainStops[currentStop];
				currentStopName = trainStopNames[currentStop];
				timeToNextStop = travelTimes[currentStop];
				DialogueManagerS.D.EndText();
				nextStopMessageCountdown = timeToNextStopMessage;
				nextStopMessageGiven = false;
				doorsClosingMessageGiven = false;

				CameraShakeS.C.LargeShake();
			}
		}

		if (trainIsMoving){
			ManageShake();
			timeToNextStop -= Time.deltaTime;
			nextStopMessageCountdown -= Time.deltaTime;
			if (nextStopMessageCountdown <= 0 && !nextStopMessageGiven){

				nextStopMessageGiven = true;
				DialogueManagerS.D.SetDisplayText(nextStopString + currentStopName, false, false);
				messageTimeOut = messageTime;
				messageIsUp = true;
			}
			if (!nowArrivingMessageGiven && timeToNextStop <= timeBeforeNowArrivingMessage){
				nowArrivingMessageGiven = true;
				DialogueManagerS.D.SetDisplayText(nowArrivingString + currentStopName, false, false);
				messageTimeOut = messageTime;
				messageIsUp = true;
			}

			if (timeToNextStop <= 0){
				trainIsMoving = false;
				nextStopMessageGiven = false;
				nowArrivingMessageGiven = false;
				currentTimeAtStop = timePerStop;
				trainIsStopped = true;
				CameraShakeS.C.LargeShake();
				SetColliders(true);
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

	void SetColliders(bool setOn){
		if (setOn){
			SetSceneChanges();
		}
		for (int i = 0; i < exitColliders.Length; i++){
			exitColliders[i].gameObject.SetActive(setOn);
		}
	}

	void AssignColliders(){
		sceneChangeColliders.Clear();
		for (int i = 0; i < exitColliders.Length; i++){
			sceneChangeColliders.Add(exitColliders[i].GetComponent<ChangeSceneTriggerS>());
		}
	}

	void SetSceneChanges(){
		for (int i = 0; i < sceneChangeColliders.Count; i++){
			sceneChangeColliders[i].nextSceneString = currentStopString;
			sceneChangeColliders[i].whereToSpawn = stopSpawnPts[currentStop];
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
}
