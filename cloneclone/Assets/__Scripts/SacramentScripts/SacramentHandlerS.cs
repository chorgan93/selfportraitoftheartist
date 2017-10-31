using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SacramentHandlerS : MonoBehaviour {

	public List<SacramentStepS> sacramentSteps;

	public string nextSceneString;
	public int nextSceneSpawnPos;

	private int currentStep = 0;

	[Header("Standalone Properties")]
	public bool quitGameOnEnd = false;
	public bool loadPlayerDown = false;
	public bool healPlayer = false;
	public int setProgress = -1;
	public bool noFade = false;

	AsyncOperation async;

	// Use this for initialization
	void Start () {
		currentStep = 0;
		InitializeSteps();
		sacramentSteps[currentStep].ActivateStep();
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
		currentStep++;
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

	private IEnumerator LoadNextScene(){
		FadeScreenUI.NoFade = noFade;
		if (nextSceneSpawnPos > -1){
			SpawnPosManager.whereToSpawn = nextSceneSpawnPos;
		}
		if (setProgress > -1){
			StoryProgressionS.SetStory(setProgress);
		}
		async = Application.LoadLevelAsync(nextSceneString);
		async.allowSceneActivation = false;
		yield return async;
	}
}
