using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrainCarS : MonoBehaviour {

	public static int currentStop = -666;
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

	[Header("Fake Train Properties")]
	public GameObject onOnFake;
	public GameObject offOnFake;
	public GameObject[] fakeColliders;
	public string checkpointStartScene;
	public int checkpointSpawnInt;
	public string fakeTrainEndScene;
	public int crashSpawnInt;
	public string[] fakeTrainStrings;
	public float[] fakeTrainStringTimes;
	private float currentFakeTrainTime;
	private int currentFakeMessage = 0;
	public int increaseIntensityStep;
	private bool doingFakeTrain = false;
	public float loadNextSceneTime = 2f;
	public GameObject onOnCrashObj;
	private bool beganLoad = false;
	private bool increaseIntensity = false;
	public int startZoomStep = 5;

	private PlayerController pRef;


	private bool messageIsUp = false;


	public Collider[] exitColliders;
	private List<ChangeSceneTriggerS> sceneChangeColliders = new List<ChangeSceneTriggerS>();


	// Use this for initialization
	void Start () {

		if (currentStop < -1){
			SetFakeTrain();
		}else{
			onOnFake.SetActive(false);
		trainIsStopped = true;
		currentTimeAtStop = timePerStop/2f;

		AssignColliders();
		SetColliders(true);

		SetSmallShake();
		SetLargeShake();
		CameraShakeS.C.lockXShake = true;
		}
	
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

				if (!doingFakeTrain){
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
				}else{
					timeToNextStop = 9999f;
					TurnOffFakeColliders();
				}
				DialogueManagerS.D.EndText();
				nextStopMessageCountdown = timeToNextStopMessage;
				nextStopMessageGiven = false;
				doorsClosingMessageGiven = false;

				CameraShakeS.C.LargeShake();
			}
		}

		if (trainIsMoving){
			ManageShake();
			if (!doingFakeTrain){
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
			}else{
				RunFakeTrain();
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

	void TurnOffFakeColliders(){

		for (int i = 0; i < fakeColliders.Length; i++){
			fakeColliders[i].SetActive(false);
		}
	}

	void AssignColliders(){
		sceneChangeColliders.Clear();
		for (int i = 0; i < exitColliders.Length; i++){
			sceneChangeColliders.Add(exitColliders[i].GetComponent<ChangeSceneTriggerS>());
		}
	}

	void SetSceneChanges(){

		if (currentStop < -1){
			currentStopString = checkpointStartScene;

			for (int i = 0; i < sceneChangeColliders.Count; i++){
				sceneChangeColliders[i].nextSceneString = checkpointStartScene;
				sceneChangeColliders[i].whereToSpawn = checkpointSpawnInt;
			}
		}else{
		currentStopString = trainStops[currentStop];

			for (int i = 0; i < sceneChangeColliders.Count; i++){
				sceneChangeColliders[i].nextSceneString = currentStopString;
				sceneChangeColliders[i].whereToSpawn = stopSpawnPts[currentStop];
			}
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
			if (increaseIntensity){
				CameraShakeS.C.SmallShake();
			}else{
			CameraShakeS.C.MicroShake();
			}
			SetSmallShake();
		}
		if (largeShakeCountdown <= 0){
			if (increaseIntensity){
				CameraShakeS.C.LargeShake();
			}else{
				CameraShakeS.C.SmallShake();
			}
			SetLargeShake();
		}
		smallShakeCountdown -= Time.deltaTime;
		largeShakeCountdown -= Time.deltaTime;
		if (increaseIntensity){

			smallShakeCountdown -= Time.deltaTime;
			largeShakeCountdown -= Time.deltaTime;
		}
	}

	void SetFakeTrain(){

		pRef = GameObject.Find("Player").GetComponent<PlayerController>();
		onOnFake.SetActive(true);
		offOnFake.SetActive(false);
		onOnCrashObj.SetActive(false);
		InGameMenuManagerS.allowMenuUse = false;
		trainIsStopped = true;
		currentTimeAtStop = timePerStop/2f;

		AssignColliders();
		SetColliders(false);

		SetSmallShake();
		SetLargeShake();
		CameraShakeS.C.lockYShake = true;

		doingFakeTrain = true;
	}

	void RunFakeTrain(){

		if (!onOnCrashObj.activeSelf){
		nextStopMessageCountdown -= Time.deltaTime;
		if (nextStopMessageCountdown <= 0){
			if (currentFakeMessage >= fakeTrainStrings.Length){
				DialogueManagerS.D.EndText();
				onOnCrashObj.SetActive(true);

			}else{
			nextStopMessageCountdown = fakeTrainStringTimes[currentFakeMessage];
					if (fakeTrainStrings[currentFakeMessage] == ""){
						DialogueManagerS.D.EndText();
					}else{
						if (startZoomStep >= currentFakeMessage){
							DialogueManagerS.D.SetDisplayText(fakeTrainStrings[currentFakeMessage], false, false);
						}else{
						DialogueManagerS.D.SetDisplayText(fakeTrainStrings[currentFakeMessage]);
						}
					}
					currentFakeMessage++;
					if (currentFakeMessage == increaseIntensityStep){
						increaseIntensity = true;
					}
		}
			}

		}else{
			if (!beganLoad){
			loadNextSceneTime -= Time.deltaTime;
				if (loadNextSceneTime <= 0){
					beganLoad = true;
					pRef.SetTalking(true);
					PlayerController.doWakeUp = true;
					SpawnPosManager.whereToSpawn = crashSpawnInt;
					CameraEffectsS.E.SetNextScene(fakeTrainEndScene);
					CameraEffectsS.E.FadeIn();
				}
		}
		}
	}
}
