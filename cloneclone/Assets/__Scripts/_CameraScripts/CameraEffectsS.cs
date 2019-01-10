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
	public SpriteRenderer transformFilter;
	private Color transformFilterColor;
	public Color transformFilterNoEffect;

	public SpriteRenderer[] vignetteSprites;
	private Color vignetteFade;
	private float vignetteFadeMax;
	private float vignetteFadeRate = 1.6f;
	private bool vignetteFading = true;

    private Blur blurEffect;
	private float blurEffectTimeMax = 0.4f;
	private float blurEffectTime;
	private float blurT;
	private bool blurEnabled = true;
	private float blurSizeMax = 1f;

    private float enemyResetEffectTimeMax = 1f;
    private float enemyResetEffectTime;
    private float enemyResetEffectT;
    private bool enemyEffectEnabled = true;

    private float staticMaxAlpha = 1f;

	public SpriteRenderer resetStatic;
    public SpriteRenderer resetStaticEnemyEffect;
	public Color resetColor;
	public Color healColor = Color.magenta;
	public Color endCombatColor;
	private bool endCombatEffect;
	Color staticCol;
    Color enemyStaticCol;
	public GameObject staticSound;
	public GameObject healSound;
	public GameObject endCombatSound;

	private ContrastStretch contrastEffect;
	private Tonemapping toneEffect;
	private SunShafts sunEffect;
    private Bloom bloomEffect;
	private ColorCorrectionCurves colorCorrection;

	private Antialiasing antiAliasEffect;
	public static bool aliasOn = false;

	[Header("Special Scene Properties")]
	public bool arcadeMode = false;

	//#if UNITY_EDITOR_OSX
	public static bool cameraEffectsEnabled = true;
	//#endif

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
			cameraEffectsEnabled = !cameraEffectsEnabled;
			Debug.Log("Setting camera effects: " + cameraEffectsEnabled);
			if (!sunEffect){
				toneEffect = GetComponent<Tonemapping>();
				bloomEffect = GetComponent<Bloom>();
				sunEffect = GetComponent<SunShafts>();
				colorCorrection = GetComponent<ColorCorrectionCurves>();
			}
			if (cameraEffectsEnabled){

				sunEffect.enabled = true;
				bloomEffect.enabled = true;
				toneEffect.enabled = true;
				contrastEffect.enabled = true;
				colorCorrection.enabled = true;
			}else{
				sunEffect.enabled = false;
				bloomEffect.enabled = false;
				toneEffect.enabled = false;
				contrastEffect.enabled = false;
				colorCorrection.enabled = false;
			}
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

			transformFilterColor = transformFilter.color;
			transformFilter.gameObject.SetActive(false);

			blurEffect = GetComponent<Blur>();
			blurEffect.enabled = false;

			resetStatic.gameObject.SetActive(false);
            resetStaticEnemyEffect.gameObject.SetActive(false);

			contrastEffect = GetComponent<ContrastStretch>();

			antiAliasEffect = GetComponent<Antialiasing>();
			antiAliasEffect.enabled = aliasOn;

			#if UNITY_EDITOR_OSX
			if (!cameraEffectsEnabled){
			toneEffect = GetComponent<Tonemapping>();
			bloomEffect = GetComponent<Bloom>();
				sunEffect = GetComponent<SunShafts>();
				colorCorrection = GetComponent<ColorCorrectionCurves>();
			sunEffect.enabled = false;
			bloomEffect.enabled = false;
			toneEffect.enabled = false;
			contrastEffect.enabled = false;
				colorCorrection.enabled = false;
			}

			#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE
			if (QualitySettings.GetQualityLevel() < 1 && !arcadeMode){
			toneEffect = GetComponent<Tonemapping>();
			bloomEffect = GetComponent<Bloom>();
			sunEffect = GetComponent<SunShafts>();
			colorCorrection = GetComponent<ColorCorrectionCurves>();
			sunEffect.enabled = false;
			bloomEffect.enabled = false;
			toneEffect.enabled = false;
			contrastEffect.enabled = false;
			colorCorrection.enabled = false;
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

        if (enemyEffectEnabled){
            enemyResetEffectTime += Time.deltaTime;
            enemyResetEffectT = enemyResetEffectTime / enemyResetEffectTimeMax;
            enemyResetEffectT = Mathf.Sin(enemyResetEffectT * Mathf.PI * 0.5f);
            if (enemyResetEffectTime < enemyResetEffectTimeMax)
            {
                staticCol = resetStaticEnemyEffect.color;
                staticCol.a = Mathf.Lerp(staticMaxAlpha, 0f, enemyResetEffectT)*2f;
                if (staticCol.a > 1f) { staticCol.a = 1f; }
                resetStaticEnemyEffect.color = staticCol;
            }
            else
            {
                resetStaticEnemyEffect.gameObject.SetActive(false);
                enemyEffectEnabled = false;
            }
        }

		if (blurEnabled){
			if (blurEffectTime < blurEffectTimeMax){
				blurEffectTime += Time.deltaTime;
				if (endCombatEffect){
					blurEffectTime+=Time.deltaTime;
				}
				blurT = blurEffectTime / blurEffectTimeMax;
				//blurT = Mathf.Sin(blurT * Mathf.PI * 0.5f);
				blurT = 1f - Mathf.Cos(blurT * Mathf.PI * 0.5f);
                blurEffect.blurSpread = Mathf.Lerp(blurSizeMax, 0, blurT);
                blurEffect.iterations = Mathf.FloorToInt(Mathf.Lerp(3, 0, blurT));

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
	public void SetTransformFilter(bool onOff){
		#if UNITY_EDITOR_OSX
		if (onOff){
			if (cameraEffectsEnabled){
				transformFilter.color = transformFilterColor;
			}else{
				transformFilter.color = transformFilterNoEffect;
			}
			transformFilter.gameObject.SetActive(true);
		}else{
			transformFilter.gameObject.SetActive(false);
		}
		#else
		if (onOff){
			transformFilter.color = transformFilterColor;

			transformFilter.gameObject.SetActive(true);
		}else{
			transformFilter.gameObject.SetActive(false);
		}
		#endif
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
			staticCol =
            enemyStaticCol = Color.white;
		}else{
			staticCol = 
            enemyStaticCol = resetColor;
        }

		endCombatEffect = endCombat;
		staticCol.a = staticMaxAlpha = 1f;
		resetStatic.color = staticCol;
		resetStatic.gameObject.SetActive(true);
        if (!endCombat){
            resetStaticEnemyEffect.gameObject.SetActive(true);
            enemyEffectEnabled = true;
            enemyResetEffectTime = enemyResetEffectT = 0f;
        }

		if (staticSound){
			Instantiate(staticSound);
		}
	}

    public void BlurEffect(){
        blurEnabled = true;
        blurEffect.enabled = true;
        blurEffectTime = blurEffectTimeMax*0.5f;
        blurT = 0;
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

    public static void ChangeEffectSetting(bool newSet){
       
        cameraEffectsEnabled = newSet;
        if (E != null)
        {
            //Debug.Log("Setting camera effects: " + debugEffects);
            if (!E.sunEffect)
            {
                E.toneEffect = E.GetComponent<Tonemapping>();
                E.bloomEffect = E.GetComponent<Bloom>();
                E.sunEffect = E.GetComponent<SunShafts>();
                E.colorCorrection = E.GetComponent<ColorCorrectionCurves>();
            }
            if (cameraEffectsEnabled)
            {

                E.sunEffect.enabled = true;
                E.bloomEffect.enabled = true;
                E.toneEffect.enabled = true;
                E.contrastEffect.enabled = true;
                E.colorCorrection.enabled = true;
            }
            else
            {
                E.sunEffect.enabled = false;
                E.bloomEffect.enabled = false;
                E.toneEffect.enabled = false;
                E.contrastEffect.enabled = false;
                E.colorCorrection.enabled = false;
            }

            GameObject filterReset = GameObject.Find("CameraFilterSetter");
            if (filterReset != null){
                filterReset.GetComponent<ColorFilterSetter>().RefreshFilter();
            }
        }


    }

}
