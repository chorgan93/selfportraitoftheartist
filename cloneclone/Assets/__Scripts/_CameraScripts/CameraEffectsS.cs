using UnityEngine;
using System.Collections;

public class CameraEffectsS : MonoBehaviour {

	public static CameraEffectsS E;

	private bool _initialized;
	public bool isFading = false;

	private FadeScreenUI fadeScreen;

	public FlashEffectS hurtFlash;
	public FlashEffectS killFlash;
	public FlashEffectS deathFlash;
	public FlashEffectS critFlash;
	public FlashEffectS specialFlash;

	public SpriteRenderer[] vignetteSprites;
	private Color vignetteFade;
	private float vignetteFadeMax;
	private float vignetteFadeRate = 1.6f;
	private bool vignetteFading = true;

	// Use this for initialization
	void Awake () {

		Initialize();
	
	}

	void Update(){
		if (vignetteFading){
			vignetteFade.a += Time.deltaTime*vignetteFadeRate;
			if (vignetteFade.a >= vignetteFadeMax){
				vignetteFade.a = vignetteFadeMax;
				vignetteFading = false;
			}
			foreach(SpriteRenderer f in vignetteSprites){
				f.color = vignetteFade;
			}
		}
	}


	void Initialize(){

		if (!_initialized){
			_initialized = true;
			E = this;

			fadeScreen = GetComponentInChildren<FadeScreenUI>();
			fadeScreen.FadeOut();

			vignetteFade = vignetteSprites[0].color;
			vignetteFadeMax = vignetteFade.a;
		}
	}

	public void ChangeFadeColor (Color newCol){
		fadeScreen.ChangeColor(newCol);
	}

	public void FadeIn(float newRate = 0){
		fadeScreen.FadeIn("", newRate);
		isFading = true;
	}
	public void SetNextScene(string newDestination){
		fadeScreen.SetNewDestination(newDestination);
	}
	public void VignetteDeathEffect(){
		vignetteFading = true;
		vignetteFade = vignetteSprites[0].color;
		vignetteFade.a = 0f;
		foreach(SpriteRenderer f in vignetteSprites){
			f.color = vignetteFade;
		}
	}
}
