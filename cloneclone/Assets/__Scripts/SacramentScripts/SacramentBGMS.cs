using UnityEngine;
using System.Collections;

public class SacramentBGMS : MonoBehaviour {

	public BGMLayerS myTarget;

	public bool fadeIn;
	public bool fadeOut;
	public bool instant;
	public bool destroyOnFade = false;

	// Use this for initialization
	public void Activate () {
	
		if (myTarget){
		if (fadeIn){
			myTarget.FadeIn(instant);
		}
		if (fadeOut){
			myTarget.FadeOut(instant, destroyOnFade);
		}
		}

	}
}
