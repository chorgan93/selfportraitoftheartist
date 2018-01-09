using UnityEngine;
using System.Collections;

public class GameOverS : MonoBehaviour {

	public float delayFadeTimeMax = 3f;
	private float delayFadeTime;

	private bool gameOver = false;
	private bool startedFade = false;
	private PlayerStatsS playerReference;

	public static string reviveScene = "";
	public static int revivePosition = 0;
	public static string tempReviveScene = "";
	public static int tempRevivePosition = 0;

	private bool triggerRespawn = false;

	private bool resetSet = false;

	// Use this for initialization
	void Start () {

		if (reviveScene == ""){
			reviveScene = Application.loadedLevelName;
		}
		playerReference = GetComponent<PlayerStatsS>();
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!resetSet){

			playerReference.pRef.SetResetManager(this);
			resetSet = true;
		}

		if ((playerReference.PlayerIsDead() || triggerRespawn) && !gameOver){
			gameOver = true;
			if (!triggerRespawn){
				delayFadeTime = delayFadeTimeMax;
			}
			PlayerStatsS.healOnStart = true;
		}

		if (gameOver){

			delayFadeTime -= Time.deltaTime;

			if (delayFadeTime <= 0 && !startedFade){
				startedFade = true;
				PrepareForRespawn();
			}

		}
	
	}

	public void PrepareForRespawn(){
		PlayerInventoryS.I.dManager.ClearAll();
		PlayerInventoryS.I.dManager.ClearCompletedCombat();
		StoryProgressionS.ResetToSavedProgress();
		SpawnPosManager.whereToSpawn = revivePosition;
		SpawnPosManager.spawningFromDeath = true;
		if (tempReviveScene != ""){
			CameraEffectsS.E.SetNextScene(tempReviveScene);
			SpawnPosManager.tempWhereToSpawn = tempRevivePosition;
		}else{
			CameraEffectsS.E.SetNextScene(reviveScene);
		}
		CameraEffectsS.E.FadeIn();
	}

	public void FakeDeath(bool returnToMain = false){
		triggerRespawn = true;
		if( returnToMain){
			if (BGMHolderS.BG != null){
			BGMHolderS.BG.EndAllLayers(false,true);
			}
			// TODO change back after magfest!!
			//reviveScene = "MenuScene";
			reviveScene = "MenuSceneMAGFEST";
			MainMenuNavigationS.inMain = true;
		}
	}

}
