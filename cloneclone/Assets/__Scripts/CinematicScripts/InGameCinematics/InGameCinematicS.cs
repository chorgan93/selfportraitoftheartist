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

	public static bool inGameCinematic = false;
	

	// Use this for initialization
	void Awake () {
	
		_pRef = GameObject.Find("Player").GetComponent<PlayerController>();
		inGameCinematic = true;

	}

	void Start () {

		CheckCurrentStep();

		if (noBuddy){
			_pRef.SetBuddy(false);
		}
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
			}else{
				EndCinematic();
			}
		}

	}

	private bool CheckCurrentStep(){

		currentCountdown = 0f;

		bool cinematicDone = true;
		foreach (InGameCinemaTextS t in cinemaDialogues){
			if (t.myCinemaStep == currentStep){
				cinematicDone = false;
				t.gameObject.SetActive(true);
			}
		}
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

		return cinematicDone;

	}

	private void EndCinematic(){

		inGameCinematic = false;
		pRef.SetTalking(false);
		if (noBuddy){
			pRef.SetBuddy(true);
		}
		Destroy(gameObject);

	}
}
