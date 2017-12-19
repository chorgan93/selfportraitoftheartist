using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SacramentHandlerS : MonoBehaviour {

	public List<SacramentStepS> sacramentSteps;
	public Image waitForAdvanceImage;

	public string nextSceneString;
	public int nextSceneSpawnPos;

	private int currentStep = 0;

	[Header("Standalone Properties")]
	public bool quitGameOnEnd = false;
	public bool enableEscToQuit = false;
	public bool loadPlayerDown = false;
	public bool healPlayer = false;
	public int setProgress = -1;
	public bool noFade = false;

	bool startedLoading = false;

	AsyncOperation async;

	// Use this for initialization
	void Awake(){
		CinematicHandlerS.inCutscene = true;
	}

	void Start () {
		currentStep = 0;
		InitializeSteps();
		sacramentSteps[currentStep].ActivateStep();
	}

	void Update(){
		if (startedLoading){
			if (async.progress >= 0.9f){
				async.allowSceneActivation = true;
			}
		}else{
			if (enableEscToQuit && Input.GetKeyDown(KeyCode.Escape)){
				Application.Quit();
			}
		}
	}

	public void GoToStep(SacramentStepS nextStep){

		sacramentSteps[currentStep].DeactivateStep();
		currentStep = sacramentSteps.IndexOf(nextStep);
		if (currentStep < sacramentSteps.Count){
			sacramentSteps[currentStep].ActivateStep();
		}else{
			if (quitGameOnEnd){
				Application.Quit();
			}else{
				StartCoroutine(LoadNextScene());
			}
		}
	}

	void InitializeSteps(){
		for (int i = 0; i < sacramentSteps.Count; i++){
			sacramentSteps[i].Initialize(this);
			sacramentSteps[i].gameObject.SetActive(false);
		}
	}

	public void AdvanceStep(){

		sacramentSteps[currentStep].DeactivateStep();
		if (sacramentSteps[currentStep].nextStep != null){
			GoToStep(sacramentSteps[currentStep].nextStep);
		}else{
		currentStep++;
		if (currentStep < sacramentSteps.Count){
			sacramentSteps[currentStep].ActivateStep();
		}else{
			if (quitGameOnEnd){
				Debug.Log("Exiting Sacrament...");
				Application.Quit();
			}else{
				StartCoroutine(LoadNextScene());
			}
		}
		}
	}

	public void ActivateWait(){
		if (!sacramentSteps[currentStep].waitOnOptions){
			waitForAdvanceImage.gameObject.SetActive(true);
		}
	}
	public void DeactivateWait(){
		waitForAdvanceImage.gameObject.SetActive(false);
	}

	private IEnumerator LoadNextScene(){
		FadeScreenUI.NoFade = noFade;
		if (nextSceneSpawnPos > -1){
			SpawnPosManager.whereToSpawn = nextSceneSpawnPos;
		}
		if (setProgress > -1){
			StoryProgressionS.SetStory(setProgress);
		}
		startedLoading = true;
		async = Application.LoadLevelAsync(nextSceneString);
		async.allowSceneActivation = false;
		yield return async;
	}
}
