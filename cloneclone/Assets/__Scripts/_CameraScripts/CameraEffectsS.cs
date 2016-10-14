using UnityEngine;
using System.Collections;

public class CameraEffectsS : MonoBehaviour {

	public static CameraEffectsS E;

	private bool _initialized;
	public bool isFading = false;

	private FadeScreenUI fadeScreen;

	public FlashEffectS hurtFlash;
	public FlashEffectS killFlash;
	public FlashEffectS critFlash;
	public FlashEffectS specialFlash;

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
		isFading = true;
	}
	public void SetNextScene(string newDestination){
		fadeScreen.SetNewDestination(newDestination);
	}
}
