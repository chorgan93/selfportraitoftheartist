using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CinematicHandlerS : MonoBehaviour {

	[Header("What to do here:")]
	public GameObject[] cinemaObjects;
	public float[] cinemaTimings;

	private float currentCountdown;
	private int currentStep;

	[Header("Where to go next:")]
	public string loadSceneString;
	public int loadSceneSpawnPos = -1;
	private bool startedLoading = false;

	private ControlManagerS _controller;
	public Text skipText;
	public Text loadText;
	public Image skipScreen;
	private Color skipCol;
	private float skipFadeRate = 3f;
	private float skipCount = 0f;
	private float skipRequireTime = 1.25f;

	private bool skipActivated = false;
	public static bool inCutscene = false;

	[Header("Next Scene Properties")]
	public bool loadPlayerDown = false;
	public int setProgress = -1;
	public bool noFade = false;

	AsyncOperation async;

	void Awake(){
		inCutscene = true;
		if (loadPlayerDown){
			PlayerController.doWakeUp = true;
		}
	}

	// Use this for initialization
	void Start () {

		_controller = GetComponent<ControlManagerS>();
		skipCol = skipText.color;
		skipCol.a = 0;
		loadText.color = skipText.color = skipCol;

		skipScreen.gameObject.SetActive(false);

		loadText.enabled = false;

		Time.timeScale = 1f;

		currentStep = 0;
		ActivateStep();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!startedLoading){

			if (_controller.StartButton() && !skipActivated){
				if (skipCol.a < 1f){
					skipCol = skipText.color;
					skipCol.a += skipFadeRate*Time.deltaTime;
					if (skipCol.a > 1f){
						skipCol.a = 1f;
					}
					skipText.color = skipCol;
				}
				skipCount += Time.deltaTime;
				if (skipCount >= skipRequireTime){
					skipActivated = true;
					skipScreen.gameObject.SetActive(true);
				}
			}else{
				skipCount = 0f;
				if (skipCol.a > 0f){
					skipCol = skipText.color;
					skipCol.a -= skipFadeRate*Time.deltaTime;
					if (skipCol.a < 0f){
						skipCol.a = 0f;
					}
					skipText.color = skipCol;
				}
			}

			currentCountdown -= Time.deltaTime;

			if (skipActivated){
				currentCountdown = 0f;
			}

			if (currentCountdown <= 0){
				if (currentStep > cinemaObjects.Length-1){
					StartNextLoad();
				}else{
					ActivateStep();
				}
			}
		}else{
			if (loadText.color.a < 1f){
				skipCol = loadText.color;
				skipCol.a += Time.deltaTime*skipFadeRate;
				if (skipCol.a > 1f){
					skipCol.a = 1f;
				}
				loadText.color = skipCol;
			}
			if (async.progress >= 0.9f){
				async.allowSceneActivation = true;
			}
		}
	
	}

	private void ActivateStep(){
		currentCountdown = cinemaTimings[currentStep];
		cinemaObjects[currentStep].SetActive(true);
		if (skipActivated && currentStep > 0){
			if (cinemaObjects[currentStep-1] != null){
				cinemaObjects[currentStep-1].SetActive(false);
			}
		}
		currentStep++;
	}

	private void StartNextLoad(){
		if (skipActivated){
			if (cinemaObjects[currentStep-1] != null){
				cinemaObjects[currentStep-1].SetActive(false);
			}
		}
		loadText.enabled = true;
		StartCoroutine(LoadNextScene());
		startedLoading = true;
	}

	private IEnumerator LoadNextScene(){
		FadeScreenUI.NoFade = noFade;
		if (loadSceneSpawnPos > -1){
			SpawnPosManager.whereToSpawn = loadSceneSpawnPos;
		}
		if (setProgress > -1){
			StoryProgressionS.SetStory(setProgress);
		}
		async = Application.LoadLevelAsync(loadSceneString);
		async.allowSceneActivation = false;
		yield return async;
	}
}
