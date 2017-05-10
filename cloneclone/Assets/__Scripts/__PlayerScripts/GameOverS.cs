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

	private bool triggerRespawn = false;

	// Use this for initialization
	void Start () {

		if (reviveScene == ""){
			reviveScene = Application.loadedLevelName;
		}
		playerReference = GetComponent<PlayerStatsS>();
	
	}
	
	// Update is called once per frame
	void Update () {

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
				PlayerInventoryS.I.dManager.ClearAll();
				PlayerInventoryS.I.dManager.ClearCompletedCombat();
				StoryProgressionS.ResetToSavedProgress();
				SpawnPosManager.whereToSpawn = revivePosition;
				SpawnPosManager.spawningFromDeath = true;
				CameraEffectsS.E.SetNextScene(reviveScene);
				CameraEffectsS.E.FadeIn();
			}

		}
	
	}

	public void FakeDeath(bool returnToMain = false){
		triggerRespawn = true;
		if( returnToMain){
			BGMHolderS.BG.EndAllLayers(false,true);
			reviveScene = "MenuScene";
			MainMenuNavigationS.inMain = true;
		}
	}

}
