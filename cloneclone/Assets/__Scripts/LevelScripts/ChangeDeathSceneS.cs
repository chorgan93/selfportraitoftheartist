using UnityEngine;
using System.Collections;

public class ChangeDeathSceneS : MonoBehaviour {

	public string newDeathScene = "";
	public Color newFadeColor = Color.black;
	public bool dontDoCountUp;

	// Use this for initialization
	void Start () {
	
		if (newDeathScene != ""){
			GameOverS.reviveScene = newDeathScene;
		}

		if (dontDoCountUp){
			PlayerStatsS.dontDoCountUp = true;
		}

		if (newFadeColor != Color.black){
			CameraEffectsS.E.ChangeFadeColor(newFadeColor);
		}
	}
}
