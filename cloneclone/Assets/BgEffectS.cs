using UnityEngine;
using System.Collections;

public class BgEffectS : MonoBehaviour {

	public bool fadingIn;
	public bool fadingOut;

	private float fadeMin = 0.2f;
	private float fadeMax = 0.8f;

	public float fadeTimeMax = 3f;
	private float fadeTime;

	public float inbetweenTimeMax = 2f;
	private float inbetweenTime;

	private Renderer myRenderer;
	private Color fadeCol;

	// Use this for initialization
	void Start () {

		fadeTime = fadeTimeMax;

		fadingIn = false;
		fadingOut = true;

		myRenderer = GetComponent<Renderer>();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (inbetweenTime > 0){

			inbetweenTime -= Time.deltaTime;

		}
		else{ 
			if (fadingIn){

			fadeTime -= Time.deltaTime;

			if (fadeTime > 0){

				fadeCol = myRenderer.material.color;
				fadeCol.a = fadeMin + ((fadeMax-fadeMin)*(1-(fadeTime/fadeTimeMax)));
				myRenderer.material.color = fadeCol;

			}
			else{

				fadeCol = myRenderer.material.color;
				fadeCol.a = fadeMax;
				myRenderer.material.color = fadeCol;

				fadingIn = false;
				fadingOut = true;
				inbetweenTime = inbetweenTimeMax;

				fadeTime = fadeTimeMax;

			}

		}
		else{

			fadeTime -= Time.deltaTime;
			
			if (fadeTime > 0){
				
				fadeCol = myRenderer.material.color;
				fadeCol.a = fadeMin + ((fadeMax-fadeMin)*(fadeTime/fadeTimeMax));
				myRenderer.material.color = fadeCol;
				
			}
			else{

				fadeCol = myRenderer.material.color;
				fadeCol.a = fadeMin;
				myRenderer.material.color = fadeCol;
				
				fadingIn = true;
				fadingOut = false;
				inbetweenTime = inbetweenTimeMax;
				
				fadeTime = fadeTimeMax;
				
			}

		}
		}
	
	}
}
