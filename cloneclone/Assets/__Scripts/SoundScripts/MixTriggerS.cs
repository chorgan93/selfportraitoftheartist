using UnityEngine;
using System.Collections;

public class MixTriggerS : MonoBehaviour {

	public BGMLayerS targetLayer;

	public bool fadeIn = false;
	public bool fadeOut = false;
	public bool instant = false;

	public bool activateOnce = false;
	private bool activated = false;


	void OnTriggerEnter (Collider other) {
	
		if (other.gameObject.tag == "Player"){
			if ((activateOnce && !activated) || !activateOnce){
				if (fadeIn){
					targetLayer.FadeIn(instant);
				}
				if (fadeOut){
					targetLayer.FadeOut(instant);
				}
				activated = true;
			}
		}

	}
}
