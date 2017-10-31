using UnityEngine;
using System.Collections;

public class CinematicBgmS : MonoBehaviour {

	public BGMLayerS myTarget;

	public bool fadeIn;
	public bool fadeOut;
	public bool instant;
	public bool destroyOnFade = true;

	// Use this for initialization
	void Start () {
	
		if (fadeIn){
			myTarget.FadeIn(instant);
		}
		if (fadeOut){
			myTarget.FadeOut(instant, destroyOnFade);
		}

	}
}
