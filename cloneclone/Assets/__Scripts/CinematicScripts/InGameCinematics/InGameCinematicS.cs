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
	public float fadeRate = 0.4f;
	public bool wakeNextScene = true;
	public bool noBuddy = false;
	public bool resetPOIOnEnd = false;

	public static bool turnOffBuddies = false;

	public static bool inGameCinematic = false;
	

	// Use this for initialization
	void Awake () {
	
		_pRef = GameObject.Find("Player").GetComponent<PlayerController>();
		inGameCinematic = true;
		if(noBuddy){
			turnOffBuddies = true;
		}

	}

	void Start () {

		CheckCurrentStep();

		/*if (noBuddy){
			_pRef.SetBuddy(false);
		}
		*/

		_pRef.SetTalking(true);

	}
	
	// Update is called once per frame
	void Update () {

		if (timedStep){
			currentCountdown-=Time.deltaTime;
			if (currentCountdown <= 0){
				AdvanceCinematic();
			}
		}
	
	}

	public void AdvanceCinematic(){

		timedStep = false;
		currentCountdown = 0f;

		currentStep++;

		if (CheckCurrentStep()){
			if (endCinemaScene != ""){
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

		bool cinematicDone = true;
		if (cinemaDialogues != null){
			foreach (InGameCinemaTextS t in cinemaDialogues){
				if (t.myCinemaStep == currentStep){
					cinematicDone = false;
					t.gameObject.SetActive(true);
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
					if (c.moveTime > currentCountdown){
						currentCountdown = c.moveTime;
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
