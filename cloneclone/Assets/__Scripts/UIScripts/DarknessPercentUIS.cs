using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DarknessPercentUIS : MonoBehaviour {

	[Header("Text Display")]
	public Image wholeDisplay;
	public Image displayBG;
	public Text mainTextDisplay;
	public Text redTextDisplay;
	public Text purpleTextDisplay;
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

	private PlayerStatsS pStats;

	private float delayFadeInTime = 1.2f;
	private float delayFadeOutTime = 1.4f;
	private bool deathSequence;
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

	private bool _allowAdvance = true;
	public bool allowAdvance { get { return _allowAdvance; } }

	// Use this for initialization
	void Start () {

		maxBarSize = currentBarSize = mainBarDisplay.rectTransform.sizeDelta;
		pStats = GameObject.Find("Player").GetComponent<PlayerStatsS>();
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
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!fadeOutRegNumbers){
			BarOffset();
			SetMainSize();
			SetText();
			TextShake();
		}else{
			if (wholeDisplay.enabled){
				fadeOutElementCol = wholeDisplay.color;
				fadeOutElementCol.a -= Time.deltaTime*fadeOutElementRate;
				wholeDisplay.color = fadeOutElementCol;

				fadeOutElementCol = displayBG.color;
				fadeOutElementCol.a -= Time.deltaTime*fadeOutElementRate;
				displayBG.color = fadeOutElementCol;

				fadeOutElementCol = mainBarDisplay.color;
				fadeOutElementCol.a -= Time.deltaTime*fadeOutElementRate;
				mainBarDisplay.color = fadeOutElementCol;

				fadeOutElementCol = redBarDisplay.color;
				fadeOutElementCol.a -= Time.deltaTime*fadeOutElementRate;
				redBarDisplay.color = fadeOutElementCol;

				fadeOutElementCol = purpleBarDisplay.color;
				fadeOutElementCol.a -= Time.deltaTime*fadeOutElementRate;
				purpleBarDisplay.color = fadeOutElementCol;

				fadeOutElementCol = mainTextDisplay.color;
				fadeOutElementCol.a -= Time.deltaTime*fadeOutElementRate;
				mainTextDisplay.color = fadeOutElementCol;

				fadeOutElementCol = redTextDisplay.color;
				fadeOutElementCol.a -= Time.deltaTime*fadeOutElementRate;
				redTextDisplay.color = fadeOutElementCol;

				fadeOutElementCol = purpleTextDisplay.color;
				fadeOutElementCol.a -= Time.deltaTime*fadeOutElementRate;
				purpleTextDisplay.color = fadeOutElementCol;

				if (wholeDisplay.color.a <= 0){
					wholeDisplay.enabled = displayBG.enabled = mainBarDisplay.enabled = redBarDisplay.enabled
						= purpleBarDisplay.enabled = mainTextDisplay.enabled = redTextDisplay.enabled
							= purpleTextDisplay.enabled = false;
				}
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
				_allowAdvance = true;
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

				displayAmt = Mathf.Lerp(saveDeathAmt, pStats.currentDarkness, adjustT)/PlayerStatsS.DARKNESS_MAX*100f;
				if (displayAmt < 10){
					displayString = "0" + displayAmt.ToString("F2") + "%";
				}else{
					displayString = displayAmt.ToString("F2") + "%";
				}
				deathTextDisplay.text = deathRedTextDisplay.text = displayString;
			}else{
				delayFadeOut -= Time.deltaTime;
				if (delayFadeOut <= 0){
					fadeOutDeathNumber = true;
					fadeCount = 0f;
				}
			}
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
		currentBarSize.x *= pStats.currentDarkness/PlayerStatsS.DARKNESS_MAX;
		mainBarDisplay.rectTransform.sizeDelta = purpleBarDisplay.rectTransform.sizeDelta 
			= redBarDisplay.rectTransform.sizeDelta = currentBarSize;

		purpleBarDisplay.rectTransform.sizeDelta += purpleBarSizeOffset;
		redBarDisplay.rectTransform.sizeDelta += redBarSizeOffset;

	}

	void SetText(){

		displayAmt = pStats.currentDarkness/PlayerStatsS.DARKNESS_MAX*100f;
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
		saveDeathAmt = pStats.currentDarkness;
		_allowAdvance = false;
		fadeInDeathNumbers = true;

		deathTextDisplay.text = deathRedTextDisplay.text = displayString;

		deathTextDisplay.enabled = true;
		deathRedTextDisplay.enabled = true;
		deathSequence = true;
		fadeOutRegNumbers = true;

		fadeCount = 0f;

	}
}
