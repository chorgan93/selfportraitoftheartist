using UnityEngine;
using System.Collections;

public class DarkBitS : MonoBehaviour {

	public FadeSpriteObjectS fadeRef;

	public void ActivateFadeOut(){
		fadeRef.enabled = true;
	}
}
