using UnityEngine;
using System.Collections;

public class FadeInSpriteObjectS : MonoBehaviour {



	public float fadeRate = 1f;
	public float maxFade = 1f;
	public float delayFadeTime;
	private float startDelayFadeTime;
	public float startFadeAlpha = 1f;


	private bool stopFading = false;
	public bool destroyOnFadeIn = false;
	public bool affectedByDifficulty = false;
	private float[] difficultyMults = new float[4]{0.9f, 1f, 1.05f, 1.1f};

	public float fadeIncrement = -1f;
	private float fadeIncrementCountdown;

	private SpriteRenderer myRenderer;
	private Color currentCol;


	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<SpriteRenderer>();
		currentCol = myRenderer.color;
		currentCol.a = startFadeAlpha;
		myRenderer.color = currentCol;

		startDelayFadeTime = delayFadeTime/DifficultyMult();

		if (fadeIncrement > 0){
			fadeIncrementCountdown = fadeIncrement;
		}

	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

	

		if (delayFadeTime > 0){
			delayFadeTime -= Time.deltaTime*DifficultyMult();
		}
		else{
			if (!stopFading){
				if (fadeIncrementCountdown > 0){
					fadeIncrementCountdown -= Time.deltaTime*DifficultyMult();
				}else{
			currentCol = myRenderer.color;
					currentCol.a += Time.deltaTime*fadeRate*DifficultyMult();
			if (currentCol.a >= maxFade){
			
					currentCol.a = maxFade;
				stopFading = true;

						if (destroyOnFadeIn){
							Destroy(gameObject);
						}

			}
			myRenderer.color = currentCol;

					if (fadeIncrement > 0){
						fadeIncrementCountdown = fadeIncrement;
					}
				}
			}
		}

	
	}

	private float DifficultyMult(){
		if (affectedByDifficulty){
			return difficultyMults[DifficultyS.GetSinInt()];
		}else{
			return 1f;
		}
	}
}
