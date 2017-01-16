using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DarknessPercentUIS : MonoBehaviour {

	[Header("Text Display")]
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

	private float xShakeMult = 25f;
	private float yShakeMult = 15f;

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

	private PlayerStatsS pStats;

	private bool deathSequence;
	private bool fadeInDeathNumbers;
	private bool fadeOutDeathNumbers;

	private bool _allowAdvance = true;
	public bool allowAdvance { get { return _allowAdvance; } }

	// Use this for initialization
	void Start () {

		maxBarSize = currentBarSize = mainBarDisplay.rectTransform.sizeDelta;
		pStats = GameObject.Find("Player").GetComponent<PlayerStatsS>();
		_allowAdvance = true;

		barOffsetCountdown = barChangeTime;

		StartShake();
	
	}
	
	// Update is called once per frame
	void Update () {
	
		BarOffset();
		SetMainSize();
		SetText();
		TextShake();

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
		saveDeathAmt = pStats.currentDarkness;
		_allowAdvance = false;
		fadeInDeathNumbers = true;

		deathTextDisplay.enabled = true;
		deathRedTextDisplay.enabled = true;
		deathSequence = true;
	}

	public void EndDeathCountup(){

		fadeInDeathNumbers = false;
		fadeOutDeathNumbers = true;
	}
}
