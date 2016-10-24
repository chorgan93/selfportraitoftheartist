using UnityEngine;
using System.Collections;

public class ChangeDeathSceneS : MonoBehaviour {

	public string newDeathScene = "";
	public Color newFadeColor = Color.black;

	// Use this for initialization
	void Start () {
	
		if (newDeathScene != ""){
			GameOverS.reviveScene = newDeathScene;
		}

		if (newFadeColor != Color.black){
			CameraEffectsS.E.ChangeFadeColor(newFadeColor);
		}
	}
}
