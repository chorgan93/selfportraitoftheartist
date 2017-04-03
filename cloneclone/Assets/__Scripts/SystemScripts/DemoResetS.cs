using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DemoResetS : MonoBehaviour {

	private float resetTimeMax = 2f;
	private float resetTime;

	private string resetScene = "MenuScene";
	private Image myImage;


	private bool startedLoading;
	AsyncOperation async;

	private PlayerController pRef;

	/*
	void Start(){
		if(GameObject.Find("Player") != null){
			pRef = GameObject.Find("Player").GetComponent<PlayerController>();
		}
		myImage = GetComponent<Image>();
		myImage.enabled = false; 
	}

	// Update is called once per frame
	void Update () {

		if (!startedLoading){
			if (Input.GetKey(KeyCode.Alpha0) && !FadeScreenUI.dontAllowReset){
				resetTime+=Time.deltaTime;
				if (resetTime >= resetTimeMax){
					StartLoading();
				}
			}else{
				resetTime = 0f;
			}
		}else{
			Time.timeScale = 1f;
			if (async.progress >= 0.9f){
				
				if (PlayerInventoryS.I != null){
					// this is reviving from game over, reset inventory
					PlayerInventoryS.I.RefreshRechargeables();
					PlayerInventoryS.I.dManager.ClearAll();
					PlayerInventoryS.I.dManager.ClearCompletedCombat();
					StoryProgressionS.ResetToSavedProgress();
				}

				PlayerStatsS.healOnStart = true;

				async.allowSceneActivation = true;
			}
		}

	}


	private void StartLoading(){
		StartCoroutine(LoadNextScene());
		startedLoading = true;
		if (BGMHolderS.BG != null){
			BGMHolderS.BG.EndAllLayers(true, true);
		}
		myImage.enabled = true;
		if (pRef){
			pRef.SetTalking(true);
		}
	}

	private IEnumerator LoadNextScene(){
		async = Application.LoadLevelAsync(resetScene);
		async.allowSceneActivation = false;
		yield return async;
	}**/
}
