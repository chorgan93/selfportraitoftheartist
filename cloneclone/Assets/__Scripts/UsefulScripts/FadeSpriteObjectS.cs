using UnityEngine;
using System.Collections;

public class FadeSpriteObjectS : MonoBehaviour {

	public float fadeRate = 1f;
	public float delayFadeTime;
	public float startFadeAlpha = 1f;
	public bool destroyOnFade = true;

	private SpriteRenderer myRenderer;
	private Color currentCol;

	private bool useFlashFrames = false;
	public int flashFrames = -1;

	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<SpriteRenderer>();
		currentCol = myRenderer.color;
		currentCol.a = startFadeAlpha;
		myRenderer.color = currentCol;

		if (flashFrames > 0){
			useFlashFrames = true;
			myRenderer.material.SetFloat("_FlashAmount", 1f);
			Debug.Log(myRenderer.material.GetFloat("_FlashAmount"));
		}

	
	}
	
	// Update is called once per frame
	void Update () {

		if (useFlashFrames){
			flashFrames--;
			if (flashFrames <= 0 && myRenderer.material.GetFloat("_FlashAmount") > 0){
				myRenderer.material.SetFloat("_FlashAmount", 0f);
				useFlashFrames = false;
			}
		}

		if (delayFadeTime > 0){
			delayFadeTime -= Time.deltaTime;
		}
		else{
			currentCol = myRenderer.color;
			currentCol.a -= Time.deltaTime*fadeRate;
			if (currentCol.a <= 0f){
			if (destroyOnFade){
					Destroy(gameObject);
				}else{
					currentCol.a = 0f;
				}
			}
			myRenderer.color = currentCol;
		}

	
	}
}
