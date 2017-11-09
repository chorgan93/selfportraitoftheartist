using UnityEngine;
using System.Collections;

public class ChangeDeathSceneS : MonoBehaviour {

	public string newDeathScene = "";
	public string newTempDeathScene ="";
	public int newTempDeathPos = 0;
	public Color newFadeColor = Color.black;
	public bool dontDoCountUp;

	// Use this for initialization
	void Start () {
	
		if (newDeathScene != ""){
			GameOverS.reviveScene = newDeathScene;
		}
		if (newTempDeathScene != ""){
			GameOverS.tempReviveScene = newTempDeathScene;
			GameOverS.tempRevivePosition = newTempDeathPos;
		}

		if (dontDoCountUp){
			PlayerStatsS.dontDoCountUp = true;
		}

		if (newFadeColor != Color.black){
			CameraEffectsS.E.ChangeFadeColor(newFadeColor);
		}
	}
}
