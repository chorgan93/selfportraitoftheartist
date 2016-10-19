using UnityEngine;
using System.Collections;

public class GameOverS : MonoBehaviour {

	public float delayFadeTimeMax = 3f;
	private float delayFadeTime;

	private bool gameOver = false;
	private bool startedFade = false;
	private PlayerStatsS playerReference;

	private string reviveScene = "HellScene";

	// Use this for initialization
	void Start () {

		playerReference = GetComponent<PlayerStatsS>();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (playerReference.PlayerIsDead() && !gameOver){
			gameOver = true;

			delayFadeTime = delayFadeTimeMax;
		}

		if (gameOver){

			delayFadeTime -= Time.deltaTime;

			if (delayFadeTime <= 0 && !startedFade){
				startedFade = true;
				CameraEffectsS.E.SetNextScene(reviveScene);
				CameraEffectsS.E.FadeIn();
			}

		}
	
	}
}
