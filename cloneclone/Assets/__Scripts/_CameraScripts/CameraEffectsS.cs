using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraEffectsS : MonoBehaviour {

	public static CameraEffectsS E;

	private bool _initialized;
	public bool isFading = false;

	private FadeScreenUI fadeScreen;
	public FadeScreenUI fadeRef { get { return fadeScreen; } }

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

	private float staticMaxAlpha = 1f;

	public SpriteRenderer resetStatic;
	public Color resetColor;
	public Color healColor = Color.magenta;
	public Color endCombatColor;
	private bool endCombatEffect;
	Color staticCol;
	public GameObject staticSound;
	public GameObject healSound;
	public GameObject endCombatSound;

	private ContrastStretch contrastEffect;
	private Tonemapping toneEffect;
	private SunShafts sunEffect;
	private BloomOptimized bloomEffect;

	private Antialiasing antiAliasEffect;
	public static bool aliasOn = false;

	[Header("Special Scene Properties")]
	public bool arcadeMode = false;

	#if UNITY_EDITOR_OSX
	private static bool debugEffects = false;
	#endif

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
		#if UNITY_EDITOR_OSX
		if(Input.GetKeyDown(KeyCode.P)){
			debugEffects = !debugEffects;
			Debug.Log("Setting camera effects: " + debugEffects);
		}
		#endif
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

			contrastEffect = GetComponent<ContrastStretch>();

			antiAliasEffect = GetComponent<Antialiasing>();
			antiAliasEffect.enabled = aliasOn;

			#if UNITY_EDITOR_OSX
			if (!debugEffects){
			toneEffect = GetComponent<Tonemapping>();
			bloomEffect = GetComponent<BloomOptimized>();
			sunEffect = GetComponent<SunShafts>();
			sunEffect.enabled = false;
			bloomEffect.enabled = false;
			toneEffect.enabled = false;
			contrastEffect.enabled = false;
			}

			#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE
			if (QualitySettings.GetQualityLevel() < 1 && !arcadeMode){
			toneEffect = GetComponent<Tonemapping>();
			bloomEffect = GetComponent<BloomOptimized>();
			sunEffect = GetComponent<SunShafts>();
			sunEffect.enabled = false;
			bloomEffect.enabled = false;
			toneEffect.enabled = false;
			contrastEffect.enabled = false;
			}
			#endif
		}
	}

	public void SetContrast(bool onOff){
		if (contrastEffect){
			contrastEffect.enabled = onOff;
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
						staticCol.a = Mathf.Lerp(staticMaxAlpha, 0f, blurT);
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
	public void HealEffect(){
		blurEnabled = true;
		blurEffect.enabled = true;
		blurEffectTime = blurEffectTimeMax/2f;
		blurT = 0;


		staticCol = healColor;


		endCombatEffect = false;
		staticCol.a = staticMaxAlpha = 0.7f;
		resetStatic.color = staticCol;
		resetStatic.gameObject.SetActive(true);

		if (healSound){
			Instantiate(healSound);
		}
	}
	public void ResetEffect(bool endCombat = false, bool noColor = false){
		blurEnabled = true;
		blurEffect.enabled = true;
		blurEffectTime = 0;
		blurT = 0;

		if (endCombat){
			staticCol = endCombatColor;
		}else if (noColor){
			staticCol = Color.white;
		}else{
			staticCol = resetColor;
		}

		endCombatEffect = endCombat;
		staticCol.a = staticMaxAlpha = 1f;
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
	}

	public void MatchAlias(){
		antiAliasEffect.enabled = aliasOn;
	}
}
