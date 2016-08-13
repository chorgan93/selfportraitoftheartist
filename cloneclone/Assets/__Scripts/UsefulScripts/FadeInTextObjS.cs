using UnityEngine;
using System.Collections;

public class FadeInTextObjS : MonoBehaviour {



	public float fadeRate = 1f;
	public float delayFadeTime;
	private float startDelayFadeTime;
	public float startFadeAlpha = 1f;

	public float maxFade = 1f;

	private bool stopFading = false;

	private TextMesh myRenderer;
	private Color currentCol;


	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<TextMesh>();
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
			currentCol = myRenderer.color;
			currentCol.a += Time.deltaTime*fadeRate;
			if (currentCol.a >= maxFade){
			
					currentCol.a = maxFade;
				stopFading = true;

			}
			myRenderer.color = currentCol;
			}
		}

	
	}
}
