using UnityEngine;
using System.Collections;

public class CameraEffectsS : MonoBehaviour {

	public static CameraEffectsS E;

	private bool _initialized;

	private FadeScreenUI fadeScreen;

	// Use this for initialization
	void Awake () {

		Initialize();
	
	}


	void Initialize(){

		if (!_initialized){
			_initialized = true;
			E = this;

			fadeScreen = GetComponentInChildren<FadeScreenUI>();
			fadeScreen.FadeOut();
		}
	}

	public void FadeIn(){
		fadeScreen.FadeIn("");
	}
}
