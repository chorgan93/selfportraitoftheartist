using UnityEngine;
using System.Collections;

public class FadeInOnTimerSpriteS: MonoBehaviour {



	public float fadeTime = 1f;
	private float currentTime;
	public float delayFadeTime;
	private float startDelayFadeTime;
	public float startFadeAlpha = 1f;
	public float maxFade = 1f;

	private bool stopFading = false;
	public bool destroyOnFadeIn = false;

	private SpriteRenderer myRenderer;
	private Color currentCol;


	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<SpriteRenderer>();
		currentCol = myRenderer.color;
		currentCol.a = startFadeAlpha;
		myRenderer.color = currentCol;

		startDelayFadeTime = delayFadeTime;

	
	}
	
	// Update is called once per frame
	void Update () {

	

		if (delayFadeTime > 0){
			delayFadeTime -= Time.deltaTime;
		}
		else{
			if (!stopFading){
				currentTime += Time.deltaTime;
			currentCol = myRenderer.color;
			currentCol.a = currentTime/(fadeTime-startDelayFadeTime)*maxFade;
			if (currentCol.a >= maxFade){
			
					currentCol.a = maxFade;
				stopFading = true;

						if (destroyOnFadeIn){
							Destroy(gameObject);
						}

			


				}
				
				myRenderer.color = currentCol;
			}
		}

	
	}
}
