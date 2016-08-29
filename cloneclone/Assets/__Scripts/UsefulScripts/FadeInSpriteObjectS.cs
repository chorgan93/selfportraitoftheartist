using UnityEngine;
using System.Collections;

public class FadeInSpriteObjectS : MonoBehaviour {



	public float fadeRate = 1f;
	public float delayFadeTime;
	private float startDelayFadeTime;
	public float startFadeAlpha = 1f;


	private bool stopFading = false;
	public bool destroyOnFadeIn = false;

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

		startDelayFadeTime = delayFadeTime;

		if (fadeIncrement > 0){
			fadeIncrementCountdown = fadeIncrement;
		}

	
	}
	
	// Update is called once per frame
	void Update () {

	

		if (delayFadeTime > 0){
			delayFadeTime -= Time.deltaTime;
		}
		else{
			if (!stopFading){
				if (fadeIncrementCountdown > 0){
					fadeIncrementCountdown -= Time.deltaTime;
				}else{
			currentCol = myRenderer.color;
			currentCol.a += Time.deltaTime*fadeRate;
			if (currentCol.a >= 1f){
			
					currentCol.a = 1f;
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
}
