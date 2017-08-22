using UnityEngine;
using System.Collections;

public class InGameCinematicS : MonoBehaviour {

	private PlayerController _pRef;
	public PlayerController pRef { get { return _pRef; } }

	public InGameCinemaTextS[] cinemaDialogues;
	public InGameCinemaCameraS[] cinemaCameraMoves;
	public InGameCinemaActivateS[] cinemaOnOff;
	public InGameCinemaMoveObjS[] cinemaMoveObj;
	public InGameCinemaWaitS[] cinemaWait;

	private int currentStep = 0;
	private float currentCountdown = 0f;
	private bool timedStep = false;

	public string endCinemaScene = "";
	public int endCinemaSpawn = 0;
	public bool resetInventoryStats = false;
	public float fadeRate = 0.4f;
	public bool wakeNextScene = true;
	public bool noBuddy = false;
	public bool resetPOIOnEnd = false;

	[HideInInspector]
	public bool dialogueDone = true;

	public static bool turnOffBuddies = false;

	public static bool inGameCinematic = false;
	

	// Use this for initialization
	void Awake () {
	
		inGameCinematic = true;
		if(noBuddy){
			turnOffBuddies = true;
		}
		_pRef = GameObject.Find("Player").GetComponent<PlayerController>();

	}

	void Start () {

		CheckCurrentStep();


		_pRef.SetTalking(true);

	}
	
	// Update is called once per frame
	void Update () {

		if (timedStep){
			currentCountdown-=Time.deltaTime;
			if (currentCountdown <= 0 && dialogueDone){
				AdvanceCinematic();
			}
		}
	
	}

	public void TurnOnTime(){
		timedStep = true;
	}

	public void AdvanceCinematic(){

		timedStep = false;
		currentCountdown = 0f;

		currentStep++;

		if (CheckCurrentStep()){
			if (endCinemaScene != ""){
				if (resetInventoryStats){
					pRef.GetComponent<GameOverS>().PrepareForRespawn();
				}
				SpawnPosManager.whereToSpawn = endCinemaSpawn;
				CameraEffectsS.E.SetNextScene(endCinemaScene);
				CameraEffectsS.E.FadeIn(fadeRate);
				PlayerController.doWakeUp = wakeNextScene;
				turnOffBuddies = false;
			}else{
				EndCinematic();
			}
		}

	}

	private bool CheckCurrentStep(){

		currentCountdown = 0f;

		dialogueDone = true;
		bool cinematicDone = true;
		if (cinemaDialogues != null){
			for (int i = 0; i < cinemaDialogues.Length; i++){
				if (cinemaDialogues[i].myCinemaStep == currentStep){
					cinematicDone = false;
					cinemaDialogues[i].gameObject.SetActive(true);
					dialogueDone = false;
				}
			}
		}
		if (cinemaCameraMoves != null){
		foreach (InGameCinemaCameraS c in cinemaCameraMoves){
			if (c.myCinemaStep == currentStep){
				cinematicDone = false;
				c.gameObject.SetActive(true);
				if (c.moveTime > 0){
					timedStep = true;
						if (c.moveTime > currentCountdown){
						currentCountdown = c.moveTime;
					}
				}
			}
		}
		}
		if (cinemaMoveObj != null){
		foreach (InGameCinemaMoveObjS c in cinemaMoveObj){
			if (c.myCinemaStep == currentStep){
				cinematicDone = false;
				c.gameObject.SetActive(true);
				if (c.moveTime > 0){
					timedStep = true;
						if (c.turnOnEnd.Length > 0){
						if (c.moveTime+c.turnOnTime > currentCountdown){
							currentCountdown = c.moveTime+c.turnOnTime;
					}
						}else{
							if (c.moveTime > currentCountdown){
								currentCountdown = c.moveTime;
							}
						}
				}
			}
		}
		}
		if (cinemaOnOff != null){
		foreach (InGameCinemaActivateS a in cinemaOnOff){
			if (a.myCinemaStep == currentStep){
				cinematicDone = false;
				a.gameObject.SetActive(true);
				if (a.activateTime > 0){
					timedStep = true;
					if (a.activateTime > currentCountdown){
						currentCountdown = a.activateTime;
					}
				}
			}
		}
		}
		if (cinemaWait != null){
		foreach (InGameCinemaWaitS w in cinemaWait){
			if (w.myCinemaStep == currentStep){
				cinematicDone = false;
				if (w.waitTime > 0){
					timedStep = true;
					if (w.waitTime > currentCountdown){
						currentCountdown = w.waitTime;
					}
				}
			}
		}
		}

		return cinematicDone;

	}

	private void EndCinematic(){

		inGameCinematic = false;
		turnOffBuddies = false;
		pRef.SetTalking(false);
		if (noBuddy){
			pRef.SetBuddy(true);
		}
		if (resetPOIOnEnd){
			CameraFollowS.F.ResetPOI();
		}
		Destroy(gameObject);

	}
}
