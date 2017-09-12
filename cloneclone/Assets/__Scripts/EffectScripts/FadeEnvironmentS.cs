using UnityEngine;
using System.Collections;

public class FadeEnvironmentS : MonoBehaviour {

	public float[] fadeAmt;

	private int fadeLevel = 0;
	private float fadeRate = 0.5f;
	private SpriteRenderer myRender;
	private Color myColor;

	public bool showOnStart = false;

	private bool fadingIn;
	private bool fadingOut;
	private float currentFadeTarget;

	// Use this for initialization
	void Start () {

		myRender = GetComponent<SpriteRenderer>();
		myColor = myRender.color;
		if (showOnStart){
			myColor.a = fadeAmt[fadeLevel];
		}else{
			myColor.a = 0;
		}
		myRender.color = myColor;

	
	}
	
	// Update is called once per frame
	void Update () {

		if (fadingIn){
			myColor = myRender.color;
			myColor.a += fadeRate*Time.deltaTime;
			if (myColor.a > currentFadeTarget){
				myColor.a = currentFadeTarget;
				fadingIn = false;
			}
			myRender.color = myColor;
		}

		if (fadingOut){
			myColor = myRender.color;
			myColor.a -= fadeRate*Time.deltaTime;
			if (myColor.a < currentFadeTarget){
				myColor.a = currentFadeTarget;
				fadingOut = false;
			}
			myRender.color = myColor;
		}
	
	}

	public void FadeLvUp(){
		fadeLevel++;
		if (fadeLevel > fadeAmt.Length-1){
			fadeLevel = fadeAmt.Length-1;
		}
		currentFadeTarget = fadeAmt[fadeLevel];
		fadingIn = true;
		fadingOut = false;
	}

	public void FadeLvDown(){
		fadeLevel--;
		if (fadeLevel < 0){
			fadeLevel = 0;
		}
		currentFadeTarget = fadeAmt[fadeLevel];
		fadingOut = true;
		fadingIn = false;
	}
}
