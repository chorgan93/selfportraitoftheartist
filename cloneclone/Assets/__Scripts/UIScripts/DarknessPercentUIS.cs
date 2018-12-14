﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DarknessPercentUIS : MonoBehaviour {

	public static bool demoMode = false;

	[Header("Text Display")]
	public Image wholeDisplay;
	public Image displayBG;
	public Image wholeNumDisplay;
	public Image displayBGForNum;
	public Text mainTextDisplay;
	public Text redTextDisplay;
	public Text purpleTextDisplay;
    public Text reduceTextDisplay;
	Vector2 redTextStartPos;
	Vector2 redTextOffset;
	Vector2 purpleTextStartPos;
	Vector2 purpleTextOffset;
	private float redTextShakeTriggerCount;
	private float purpleTextShakeTriggerCount;

	private int numShakesMin = 3;
	private int numShakesMax = 6;
	private float shakeTimeMin = 0.2f;
	private float shakeTimeMax = 0.48f;
	private float triggerShakeTimeMin = 1f;
	private float triggerShakeTimeMax = 6f;

	private float xShakeMult = 20f;
	private float yShakeMult = 10f;

	private bool redShaking;
	private int redShakeNum;
	private bool purpleShaking;
	private int purpleShakeNum;
	private int currentRedShake;
	private int currentPurpleShake;
	private float redShakeDuration;
	private float purpleShakeDuration;
	private float redShakeCountdown;
	private float purpleShakeCountdown;

	private float displayAmt;
	private string displayString;

	[Header("Image Display")]
	public Image mainBarDisplay;
	public Image redBarDisplay;
	public Image purpleBarDisplay;
	private Vector2 maxBarSize;
	private Vector2 currentBarSize;
	private Color fadeOutElementCol;
	private float fadeOutElementRate = 2f;

	private float barOffsetCountdown;
	private float barChangeTime = 0.1f;
	private float barOffsetMin = 0.2f;
	private float barOffsetMax = 2f;
	private Vector2 redBarSizeOffset;
	private Vector2 purpleBarSizeOffset;

	[Header("Death Display")]
	public Text deathTextDisplay;
	public Text deathRedTextDisplay;
	private float saveDeathAmt;
	private float adjustTime = 1.2f;
	private float adjustCount = 0f;
	private float delayFadeOut = 1f;
	private float adjustT;
	private bool adjustingNum;
	public RetryFightUI allowRetryUI;

	private PlayerStatsS pStats;
	public PlayerStatsS pStatRef { get { return pStats; } }

	private float delayFadeInTime = 1.2f;
	private float delayFadeOutTime = 1.4f;
	private bool deathSequence;
    private bool postCombatSequence = false;
	private bool fadeInDeathNumbers;
	private bool fadeOutRegNumbers;
	private bool fadeOutDeathNumber;
	private float fadeOutTime = 1.6f;
	private float fadeInTime = 1.4f;
	private float fadeT;
	private float fadeCount;
	private Color fadeColorRed;
	private float fadeRedMaxAlpha;
	private Color fadeColorWhite;
	private float fadeWhiteMaxAlpha;

	private bool inRecordMode = false;

	private bool _allowAdvance = true;
	public bool allowAdvance { get { return _allowAdvance; } }

	[Header("Special Case Properties")]
	public bool doNotShowInScene = false;

	public static bool hasReached100 = false;
	private string firstTime100Scene = "DarknessReviveScene";
	private string terribleFateScene = "EndingB_00_ColinFarewell";
    private string descentFailScene = "EndingD_00_DescentFail";

    private bool checkedForNatalie = false;

    bool sentLoadingMessage = false;

	public static DarknessPercentUIS DPERCENT = null;

    public bool standaloneInScene = false;
    public static float savedDarknessNum = 0;
    public static bool resetToZero = false;
    public static bool setTo100 = false;
    float cutsceneTargetNum = 0;

    private bool useDescent = false;
    public bool UseDescent { get { return useDescent; }}

    [Header("Transform Properties")]
    public GameObject transformEffect;

    // TODO add in descent darkness stuff
	void Awake(){
		DPERCENT = this;
	}

	// Use this for initialization
	void Start () {

		maxBarSize = currentBarSize = mainBarDisplay.rectTransform.sizeDelta;
        if (!standaloneInScene)
        {
            pStats = GameObject.Find("Player").GetComponent<PlayerStatsS>();
        }
		_allowAdvance = true;

		fadeColorRed = deathRedTextDisplay.color;
		fadeRedMaxAlpha = fadeColorRed.a;
		fadeColorRed.a = 0;
		deathRedTextDisplay.color = fadeColorRed;

		fadeColorWhite = deathTextDisplay.color;
		fadeWhiteMaxAlpha = fadeColorWhite.a;
		fadeColorWhite.a = 0;
		deathTextDisplay.color = fadeColorWhite;

		barOffsetCountdown = barChangeTime;

		StartShake();

		if (allowRetryUI){
			allowRetryUI.Initialize(this);
		}

		inRecordMode = PlayerStatDisplayS.RECORD_MODE;
        if (inRecordMode || doNotShowInScene){
			TurnOffCornerDisplay();
		}

        if (standaloneInScene){
            if (resetToZero){
                cutsceneTargetNum = 0;
            }
            if (setTo100){
                hasReached100 = true;
                cutsceneTargetNum = 100;
            }
            checkedForNatalie = true;
        }

        SetTransform(false);
	
	}
	
	// Update is called once per frame
	void Update () {

        if (!checkedForNatalie){
            if (pStats.pRef.isNatalie){
                TurnOffCornerDisplay();
            }
            checkedForNatalie = true;
        }
	
		if (!inRecordMode){
		if (!fadeOutRegNumbers){
			SetText();
			BarOffset();
			SetMainSize();
		}else{
			/*if (wholeDisplay.enabled && fadeOutDeathNumber){
				fadeOutElementCol = wholeDisplay.color;
				fadeOutElementCol.a = fadeColorWhite.a;
				wholeDisplay.color = wholeNumDisplay.color = fadeOutElementCol;

				fadeOutElementCol = displayBG.color;
				fadeOutElementCol.a = fadeColorWhite.a;
				displayBG.color = displayBGForNum.color = fadeOutElementCol;

				fadeOutElementCol = mainBarDisplay.color;
				fadeOutElementCol.a = fadeColorWhite.a;
				mainBarDisplay.color = fadeOutElementCol;

				fadeOutElementCol = redBarDisplay.color;
				fadeOutElementCol.a = fadeColorWhite.a;
				redBarDisplay.color = fadeOutElementCol;

				fadeOutElementCol = purpleBarDisplay.color;
				fadeOutElementCol.a = fadeColorWhite.a;
				purpleBarDisplay.color = fadeOutElementCol;

				fadeOutElementCol = mainTextDisplay.color;
				fadeOutElementCol.a = fadeColorWhite.a;
				mainTextDisplay.color = fadeOutElementCol;

				fadeOutElementCol = redTextDisplay.color;
				fadeOutElementCol.a = fadeColorWhite.a;
				redTextDisplay.color = fadeOutElementCol;

				fadeOutElementCol = purpleTextDisplay.color;
				fadeOutElementCol.a = fadeColorWhite.a;
				purpleTextDisplay.color = fadeOutElementCol;

				if (wholeDisplay.color.a <= 0){
					wholeDisplay.enabled = displayBG.enabled = mainBarDisplay.enabled = redBarDisplay.enabled
						= purpleBarDisplay.enabled = mainTextDisplay.enabled = redTextDisplay.enabled
							= purpleTextDisplay.enabled = wholeNumDisplay.enabled = displayBGForNum.enabled = false;
				}
			}**/
			BarOffset();
			SetMainSizeAdjustNum();
			TextShake();

		}
		}

		if (fadeInDeathNumbers){
			if (delayFadeOut <= 0){
				fadeCount += Time.deltaTime;
				fadeT = fadeCount/fadeInTime;
				
				if (fadeCount >= fadeInTime){
					fadeT = 1f;
					fadeInDeathNumbers = false;
					adjustingNum = true;
					adjustCount = 0f;
				}
				fadeColorRed = deathRedTextDisplay.color;
				fadeColorRed.a = Mathf.SmoothStep(0, fadeRedMaxAlpha, fadeT);
				deathRedTextDisplay.color = fadeColorRed;
				
				
				fadeColorWhite = deathTextDisplay.color;
				fadeColorWhite.a = Mathf.SmoothStep(0, fadeWhiteMaxAlpha, fadeT);
				deathTextDisplay.color = fadeColorWhite;
			}else{
				delayFadeOut -= Time.deltaTime;
			}
		}
		else if (fadeOutDeathNumber){
			fadeCount += Time.deltaTime;
			fadeT = fadeCount/fadeOutTime;
			
			if (fadeCount >= fadeOutTime){
				fadeT = 1f;
                if (!standaloneInScene)
                {
                    if (RetryFightUI.allowRetry && (pStats.currentDarkness < 100f || FadeScreenUI.PostDarkScene))
                    {
                        //Debug.LogError("Darkness Trying to turn on Retry!!");
                        allowRetryUI.TurnOn();
                    }
                    else
                    {
                        _allowAdvance = true;
                        if (RetryFightUI.allowRetry && pStats.currentDarkness >= 100f && !sentLoadingMessage)
                        {
                            CameraEffectsS.E.fadeRef.StartLoading();
                            RetryFightUI.allowRetry = false;
                            sentLoadingMessage = true;
                        }
                    }
                }else{
                    _allowAdvance = true;
                }
			}
			fadeColorRed = deathRedTextDisplay.color;
			fadeColorRed.a = Mathf.SmoothStep(fadeRedMaxAlpha, 0, fadeT);
			deathRedTextDisplay.color = fadeColorRed;

			
			fadeColorWhite = deathTextDisplay.color;
			fadeColorWhite.a = Mathf.SmoothStep(fadeWhiteMaxAlpha, 0, fadeT);
			deathTextDisplay.color = fadeColorWhite;
		}else if (deathSequence){
			if (adjustingNum){
				adjustCount += Time.deltaTime;
				adjustT = adjustCount/adjustTime;
				adjustT = Mathf.Sin(adjustT * Mathf.PI * 0.5f);

				if (adjustCount >= adjustTime){
					adjustT = 1f;
					adjustingNum = false;
					delayFadeOut = delayFadeOutTime;
				}

                if (standaloneInScene)
                {
                    if (useDescent)
                    {
                        if (resetToZero)
                        {
                            displayAmt = Mathf.Lerp(PlayerStatsS._currentDarkness, 0f, adjustT) / PlayerStatsS.DARKNESS_MAX * 100f;
                        }
                        else
                        {
                            displayAmt = Mathf.Lerp(PlayerStatsS._descentDarkness, 100f, adjustT) / PlayerStatsS.DARKNESS_MAX * 100f;
                        }
                    }
                    else
                    {
                        if (resetToZero)
                        {
                            displayAmt = Mathf.Lerp(savedDarknessNum, 0f, adjustT) / PlayerStatsS.DARKNESS_MAX * 100f;
                        }
                        else
                        {
                            displayAmt = Mathf.Lerp(savedDarknessNum, 100f, adjustT) / PlayerStatsS.DARKNESS_MAX * 100f;
                        }
                    }
                }
                else
                {
                    if (useDescent)
                    {
                        displayAmt = Mathf.Lerp(saveDeathAmt, pStats.descentDarkness, adjustT) / PlayerStatsS.DARKNESS_MAX * 100f;
                    }
                    else
                    {
                        displayAmt = Mathf.Lerp(saveDeathAmt, pStats.currentDarkness, adjustT) / PlayerStatsS.DARKNESS_MAX * 100f;
                    }
                }
				if (displayAmt < 10){
					displayString = "0" + displayAmt.ToString("F2") + "%";
				}else{
					displayString = displayAmt.ToString("F2") + "%";
				}
				redTextDisplay.text = mainTextDisplay.text = purpleTextDisplay.text
					= deathTextDisplay.text = deathRedTextDisplay.text = displayString;
			}else{
				delayFadeOut -= Time.deltaTime;
				if (delayFadeOut <= 0){
					fadeOutDeathNumber = true;
					fadeCount = 0f;
				}
			}
		}

	}

	public string Return100Scene(){
		if (!hasReached100){
			return firstTime100Scene;
		}else{
			return terribleFateScene;
		}

	}

    public void SetTransform(bool onOff){
        if (transformEffect)
        {
            transformEffect.SetActive(onOff);
        }
    }

	void BarOffset(){
		barOffsetCountdown -= Time.deltaTime;
		if (barOffsetCountdown <= 0){
			barOffsetCountdown = barChangeTime;
			purpleBarSizeOffset = redBarSizeOffset = Vector2.zero;
			purpleBarSizeOffset.x = Random.Range(barOffsetMin, barOffsetMax);
			redBarSizeOffset.x = Random.Range(barOffsetMin, barOffsetMax);
		}
	}

	void SetMainSize(){

		currentBarSize = maxBarSize;
        if (standaloneInScene) {
            currentBarSize.x *= savedDarknessNum / PlayerStatsS.DARKNESS_MAX; }
        else
        {
            if (useDescent)
            {
                currentBarSize.x *= pStats.descentDarkness / PlayerStatsS.DARKNESS_MAX;
            }
            else
            {
                currentBarSize.x *= pStats.currentDarkness / PlayerStatsS.DARKNESS_MAX;
            }
        }
		mainBarDisplay.rectTransform.sizeDelta = purpleBarDisplay.rectTransform.sizeDelta 
			= redBarDisplay.rectTransform.sizeDelta = currentBarSize;

		purpleBarDisplay.rectTransform.sizeDelta += purpleBarSizeOffset;
		redBarDisplay.rectTransform.sizeDelta += redBarSizeOffset;

	}

	void SetMainSizeAdjustNum(){
		
		currentBarSize = maxBarSize;
		currentBarSize.x *= displayAmt/100f;
		mainBarDisplay.rectTransform.sizeDelta = purpleBarDisplay.rectTransform.sizeDelta 
			= redBarDisplay.rectTransform.sizeDelta = currentBarSize;
		
		purpleBarDisplay.rectTransform.sizeDelta += purpleBarSizeOffset;
		redBarDisplay.rectTransform.sizeDelta += redBarSizeOffset;
		
	}

	void SetText(){

        if (standaloneInScene)
        {
            displayAmt = savedDarknessNum / PlayerStatsS.DARKNESS_MAX * 100f;
        }else if (postCombatSequence){
            displayAmt = saveDeathAmt / PlayerStatsS.DARKNESS_MAX * 100f;
        }else{
            if (useDescent)
            {

                displayAmt = pStats.descentDarkness / PlayerStatsS.DARKNESS_MAX * 100f;
            }
            else
            {
                displayAmt = pStats.currentDarkness / PlayerStatsS.DARKNESS_MAX * 100f;
            }
        }
		if (displayAmt < 10){
			displayString = "0" + displayAmt.ToString("F2") + "%";
		}else{
			displayString = displayAmt.ToString("F2") + "%";
		}
		mainTextDisplay.text = purpleTextDisplay.text = redTextDisplay.text = displayString;
	}

	void StartShake(){
		redTextStartPos = redTextDisplay.rectTransform.anchoredPosition;
		purpleTextStartPos = purpleTextDisplay.rectTransform.anchoredPosition;
		redShaking = false;
		purpleShaking = false;
		purpleTextShakeTriggerCount = Random.Range(triggerShakeTimeMin, triggerShakeTimeMax);
		redTextShakeTriggerCount = Random.Range(triggerShakeTimeMin, triggerShakeTimeMax);
	}
	void TextShake(){

		if (!redShaking){
			redTextShakeTriggerCount -= Time.deltaTime;
			if (redTextShakeTriggerCount <= 0){
				redShaking = true;
				redShakeNum = currentRedShake = Mathf.RoundToInt(Random.Range(numShakesMin, numShakesMax));
				redShakeCountdown = redShakeDuration = Random.Range(shakeTimeMin, shakeTimeMax)/(currentRedShake*1f);
				redTextOffset = Random.insideUnitCircle;
				redTextOffset.x *= xShakeMult*(currentRedShake*1f)/(redShakeNum*1f);
				redTextOffset.y *= yShakeMult*(currentRedShake*1f)/(redShakeNum*1f);
			}
		}else{

			redShakeCountdown-=Time.deltaTime;
			if (redShakeCountdown <= 0){
				currentRedShake--;
				redShakeCountdown = redShakeDuration;
				if (currentRedShake <= 0){
					redShaking = false;
					redTextShakeTriggerCount = Random.Range(triggerShakeTimeMin, triggerShakeTimeMax);
					redTextOffset = Vector2.zero;
				}else{
					redTextOffset = Random.insideUnitCircle;
					redTextOffset.x *= xShakeMult*(currentRedShake*1f)/(redShakeNum*1f);
					redTextOffset.y *= yShakeMult*(currentRedShake*1f)/(redShakeNum*1f);
				}
			}
			redTextDisplay.rectTransform.anchoredPosition = redTextStartPos+redTextOffset;

		}

		if (!purpleShaking){
			purpleTextShakeTriggerCount -= Time.deltaTime;
			if (purpleTextShakeTriggerCount <= 0){
				purpleShaking = true;
				purpleShakeNum = currentPurpleShake = Mathf.RoundToInt(Random.Range(numShakesMin, numShakesMax));
				purpleShakeCountdown = purpleShakeDuration = Random.Range(shakeTimeMin, shakeTimeMax)/(currentPurpleShake*1f);
				purpleTextOffset = Random.insideUnitCircle;
				purpleTextOffset.x *= xShakeMult*(currentPurpleShake*1f)/(purpleShakeNum*1f);
				purpleTextOffset.y *= yShakeMult*(currentPurpleShake*1f)/(purpleShakeNum*1f);
			}
		}else{
			
			purpleShakeCountdown-=Time.deltaTime;
			if (purpleShakeCountdown <= 0){
				currentPurpleShake--;
				purpleShakeCountdown = purpleShakeDuration;
				if (currentPurpleShake <= 0){
					purpleShaking = false;
					purpleTextShakeTriggerCount = Random.Range(triggerShakeTimeMin, triggerShakeTimeMax);
					purpleTextOffset = Vector2.zero;
				}else{
					purpleTextOffset = Random.insideUnitCircle;
					purpleTextOffset.x *= xShakeMult*(currentPurpleShake*1f)/(purpleShakeNum*1f);
					purpleTextOffset.y *= yShakeMult*(currentPurpleShake*1f)/(purpleShakeNum*1f);
				}
			}
			purpleTextDisplay.rectTransform.anchoredPosition = purpleTextStartPos+purpleTextOffset;
			
		}

	}



	public void ActivateDeathCountUp(){
		delayFadeOut = delayFadeInTime;
        if (standaloneInScene)
        {
            saveDeathAmt = displayAmt = savedDarknessNum;
        }
        else
        {
            if (useDescent)
            {
                saveDeathAmt = displayAmt = pStats.descentDarkness;
            }
            else
            {
                saveDeathAmt = displayAmt = pStats.currentDarkness;
            }
        }
		_allowAdvance = false;
        fadeInDeathNumbers = true;
        //Debug.LogError("Trying to fade in Death Numbers!");

		deathTextDisplay.text = deathRedTextDisplay.text = displayString;

		deathTextDisplay.enabled = true;
		deathRedTextDisplay.enabled = true;
		deathSequence = true;
		fadeOutRegNumbers = true;

		fadeCount = 0f;

	}

	public void ActivateColinReset(){
		delayFadeOut = delayFadeInTime;
		saveDeathAmt = displayAmt = pStats.currentDarkness;
		pStats.DeathColinReset();
		_allowAdvance = false;
		fadeInDeathNumbers = true;

		deathTextDisplay.text = deathRedTextDisplay.text = displayString;

		deathTextDisplay.enabled = true;
		deathRedTextDisplay.enabled = true;
		deathSequence = true;
		fadeOutRegNumbers = true;

        hasReached100 = true;

		fadeCount = 0f;
	}
    public void ActivateDescentReset(bool resetToNormal = false)
    {
        delayFadeOut = delayFadeInTime;
        if (resetToNormal)
        {
            saveDeathAmt = displayAmt = pStats.descentDarkness;
        }
        else
        {
            saveDeathAmt = displayAmt = pStats.currentDarkness;
        }
        pStats.StartDescentDarkness();
        _allowAdvance = false;
        fadeInDeathNumbers = true;

        deathTextDisplay.text = deathRedTextDisplay.text = displayString;

        deathTextDisplay.enabled = true;
        deathRedTextDisplay.enabled = true;
        deathSequence = true;
        fadeOutRegNumbers = true;

        useDescent = !resetToNormal;
        fadeCount = 0f;
    }

    public void SetAdvance(){
		_allowAdvance = true;
	}

	void TurnOffCornerDisplay(){

		mainBarDisplay.enabled =
			redBarDisplay.enabled =
				purpleBarDisplay.enabled =
					wholeDisplay.enabled =
						displayBG.enabled =
							wholeNumDisplay.enabled =
								displayBGForNum.enabled =
									mainTextDisplay.enabled =
		redTextDisplay.enabled =
		purpleTextDisplay.enabled = false;
	}

    public void StartDarknessReduce(float reduceAmt, RankManagerS rankRef)
    {
        if (useDescent)
        {
            if (PlayerStatsS._descentDarkness < 100f)
            {
                postCombatSequence = true;
                saveDeathAmt = pStatRef.descentDarkness;
                PlayerStatsS._descentDarkness += reduceAmt;
                if (PlayerStatsS._descentDarkness < 0f)
                {
                    PlayerStatsS._descentDarkness = 0f;
                }
                reduceTextDisplay.text = (reduceAmt * PlayerStatsS.DARKNESS_MAX / 100f).ToString("F2") + "%";
                StartCoroutine(CombatDarknessReduce(rankRef));
            }
            else
            {
                postCombatSequence = false;
                rankRef.delayLoad = false;
            }
        }
        else
        {
            if (PlayerStatsS._currentDarkness < 100f)
            {
                postCombatSequence = true;
                saveDeathAmt = pStatRef.currentDarkness;
                PlayerStatsS._currentDarkness += reduceAmt;
                if (PlayerStatsS._currentDarkness < 0f)
                {
                    PlayerStatsS._currentDarkness = 0f;
                }
                reduceTextDisplay.text = (reduceAmt * PlayerStatsS.DARKNESS_MAX / 100f).ToString("F2") + "%";
                StartCoroutine(CombatDarknessReduce(rankRef));
            }
            else
            {
                postCombatSequence = false;
                rankRef.delayLoad = false;
            }
        }
    }

    IEnumerator CombatDarknessReduce(RankManagerS R){
        Color fadeInCol = reduceTextDisplay.color;
        fadeInCol.a = 0;
        while (fadeInCol.a < 1f){
            reduceTextDisplay.color = fadeInCol;
            fadeInCol.a += Time.deltaTime*R.myUI.SpeedUpMultiplier();
            if (fadeInCol.a >= 1f){
                fadeInCol.a = 1f;
                reduceTextDisplay.color = fadeInCol;
            }
            yield return null;
        }

        yield return new WaitForSeconds(1f/R.myUI.SpeedUpMultiplier());

        float reduceT = 0f;
        float reduceTimeMax = 0.5f;
        float reduceTimeCount = 0f;

        float startNum = saveDeathAmt;

        while (reduceTimeCount < reduceTimeMax){
            if (useDescent)
            {
                saveDeathAmt = Mathf.Lerp(startNum, pStats.descentDarkness, reduceT);
            }
            else
            {
                saveDeathAmt = Mathf.Lerp(startNum, pStats.currentDarkness, reduceT);
            }
            reduceTimeCount += Time.deltaTime* R.myUI.SpeedUpMultiplier();
            reduceT = reduceTimeCount / reduceTimeMax;
            if (reduceT >= 1f)
            {
                reduceT = 1f;
                if (useDescent)
                {
                    saveDeathAmt = Mathf.Lerp(startNum, pStats.descentDarkness, reduceT);
                }
                else
                {
                    saveDeathAmt = Mathf.Lerp(startNum, pStats.currentDarkness, reduceT);
                }
            }
            if (useDescent)
            {
                reduceTextDisplay.text = "- " + (saveDeathAmt - pStats.descentDarkness * PlayerStatsS.DARKNESS_MAX / 100f).ToString("F2") + "%";
            }
            else
            {
                reduceTextDisplay.text = "- " + (saveDeathAmt - pStats.currentDarkness * PlayerStatsS.DARKNESS_MAX / 100f).ToString("F2") + "%";
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f/R.myUI.SpeedUpMultiplier());

        while (fadeInCol.a > 0f)
        {
            reduceTextDisplay.color = fadeInCol;
            fadeInCol.a -= Time.deltaTime*2f* R.myUI.SpeedUpMultiplier();
            if (fadeInCol.a <= 0f)
            {
                fadeInCol.a = 0f;
                reduceTextDisplay.color = fadeInCol;
            }
            yield return null;
        }
        postCombatSequence = false;
        R.delayLoad = false;
    }

    public void SetDescentState(bool newDescent){
        useDescent = newDescent;
    }
}
