using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

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

	private BlurOptimized blurEffect;
	private float blurEffectTimeMax = 0.4f;
	private float blurEffectTime;
	private float blurT;
	private bool blurEnabled = true;
	private float blurSizeMax = 9.99f;

	public SpriteRenderer resetStatic;
	public Color resetColor;
	public Color endCombatColor;
	private bool endCombatEffect;
	Color staticCol;
	public GameObject staticSound;
	public GameObject endCombatSound;

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

		HandleBlur();
	}


	void Initialize(){

		if (!_initialized){
			_initialized = true;
			E = this;

			fadeScreen = GetComponentInChildren<FadeScreenUI>();
			fadeScreen.FadeOut();

			vignetteFade = vignetteSprites[0].color;
			vignetteFadeMax = vignetteFade.a;

			blurEffect = GetComponent<BlurOptimized>();
			blurEffect.enabled = false;

			resetStatic.gameObject.SetActive(false);
		}
	}

	void HandleBlur(){

		if (blurEnabled){
			if (blurEffectTime < blurEffectTimeMax){
				blurEffectTime += Time.deltaTime;
				if (endCombatEffect){
					blurEffectTime+=Time.deltaTime;
				}
				blurT = blurEffectTime / blurEffectTimeMax;
				//blurT = Mathf.Sin(blurT * Mathf.PI * 0.5f);
				blurT = 1f - Mathf.Cos(blurT * Mathf.PI * 0.5f);
				blurEffect.blurSize = Mathf.Lerp(blurSizeMax, 0, blurT);
				blurEffect.blurIterations = Mathf.FloorToInt(Mathf.Lerp(4, 0, blurT));
				blurEffect.downsample = Mathf.FloorToInt(Mathf.Lerp(2, 0, blurT));

				// static fade
				if (resetStatic.gameObject.activeSelf){
					if (blurEffectTime < blurEffectTimeMax*0.4f){
						blurT = blurEffectTime / blurEffectTimeMax*0.4f;
						blurT = Mathf.Sin(blurT * Mathf.PI * 0.5f);
						staticCol = resetStatic.color;
						staticCol.a = Mathf.Lerp(1f, 0f, blurT);
						resetStatic.color = staticCol;
					}
					else{
						resetStatic.gameObject.SetActive(false);
					}
				}

			}else{
				blurEnabled = false;
				blurEffect.enabled = false;
			}
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
	public void SetNextScene(int newDestinationIndex){
		fadeScreen.SetNewDestination(newDestinationIndex);
	}
	public void VignetteDeathEffect(){
		vignetteFading = true;
		vignetteFade = vignetteSprites[0].color;
		vignetteFade.a = 0f;
		foreach(SpriteRenderer f in vignetteSprites){
			f.color = vignetteFade;
		}
	}
	public void ResetEffect(bool endCombat = false){
		blurEnabled = true;
		blurEffect.enabled = true;
		blurEffectTime = 0;
		blurT = 0;

		if (endCombat){
			staticCol = endCombatColor;
		}else{
			staticCol = resetColor;
		}

		endCombatEffect = endCombat;
		staticCol.a = 1f;
		resetStatic.color = staticCol;
		resetStatic.gameObject.SetActive(true);

		if (staticSound){
			Instantiate(staticSound);
		}
	}

	public void ResetSound(){
		if (endCombatSound){
			Instantiate(endCombatSound);
		}
	}

	public void SetRaysColor(Color rayColor){
		GetComponent<SunShafts>().sunColor = rayColor;
		Debug.Log("sun color changed!");
	}
}
