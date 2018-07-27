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
	public bool healPlayer = false;
	public int setProgress = -1;
	public bool noFade = false;
    public int setEnding = -1;

	[Header("Web Properties")]
	public bool disableSkip = false;

    [Header("Darkness Properties")]
    public DarknessPercentUIS darkness;
    private bool checkForDarkness = false;
    private bool darknessActivated = false;
    public bool setTo100 = false;
    public bool setToZero = false;
	
	AsyncOperation async;
	
	void Awake(){
		
		Application.targetFrameRate = 60;

        if (setEnding >= 0){
            CreditsManagerS.currentEnding = setEnding;
        }
		
		inCutscene = true;
		if (loadPlayerDown){
			PlayerController.doWakeUp = true;
		}
		PlayerStatsS.PlayerCantDie = false;
		
		if (healPlayer){
			PlayerStatsS.healOnStart = true;
		}

        if (darkness != null){
            checkForDarkness = true;
            DarknessPercentUIS.setTo100 = setTo100;
            DarknessPercentUIS.resetToZero = setToZero;

            if (setToZero){
                PlayerController.killedFamiliar = true;
            }
        }
	}
	
	// Use this for initialization
	void Start () {

		if (!disableSkip){
		_controller = GetComponent<ControlManagerS>();
		}
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
		
        if (!startedLoading && !darknessActivated){

			if (!disableSkip){
				if (_controller.GetCustomInput(10) && !skipActivated){
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


        }else if (darknessActivated){
            
                if (darkness.allowAdvance)
                {
                    loadText.enabled = true;
                    StartCoroutine(LoadNextScene());
                    startedLoading = true;
                darknessActivated = false;
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
        if (!checkForDarkness)
        {
            loadText.enabled = true;
            StartCoroutine(LoadNextScene());
            startedLoading = true;
        }else{
            darkness.ActivateDeathCountUp();
            darknessActivated = true;
        }
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
