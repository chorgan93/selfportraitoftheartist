using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStatDisplayS : MonoBehaviour {
	
	public Color healthFullColor;
	public Color healthEmptyColor;
	public Color staminaFullColor;
	public Color staminaEmptyColor;
	public Color chargeFullColor;
	public Color chargeEmptyColor;

	private float referenceScreenWidth = 1920f;
	private float referenceScreenHeight = 1080f;

	public PlayerCurrencyDisplayS cDisplay;

	private Vector2 healthBarMaxSize;

	private Vector2 healthStartPos;
	private float healthStartYMult;
	private Vector2 healthBarCurrentSize;
	public Image healthBar;

	private Vector2 staminaBarMaxSize;
	private Vector2 staminaStartPos;
	private float staminaStartYMult;
	private Vector2 staminaBarCurrentSize;
	public Image staminaBar;
	
	private Vector2 recoveryBarMaxSize;
	private Vector2 recoveryStartPos;
	private float recoveryStartYMult;
	private Vector2 recoveryBarCurrentSize;
	public Image recoveryBar;

	private Vector2 chargeBarMaxSize;
	private Vector2 chargeStartPos;
	private float chargeStartYMult;
	private Vector2 chargeBarCurrentSize;
	public Image chargeBar;


	private Vector2 backgroundMaxSize = new Vector2(520,130);
	private Vector2 backgroundCurrentSize;
	private Image background; 
	//public Image backgroundFill;

	private float xPositionMultiplier = 0.1f;
	private float yPositionMultiplier = -0.05f;
	private float xPositionMeterMultiplier = 0.1f;

	//___________________________DISPLAY STUFF

	public Image healthFill;
	public Image staminaFill;
	public Image recoveryFill;
	public Image chargeFill;

	public Text healthText;
	public Text staminaText;
	public Text chargeText;

	private PlayerStatsS playerStats;

	private bool allTurnedOn = false;

	// Use this for initialization
	void Start () {
	
		background = GetComponent<Image>();

		healthStartPos = healthBar.rectTransform.anchoredPosition;
		healthStartYMult = healthStartPos.y/backgroundMaxSize.y;
		healthBarMaxSize = healthBar.rectTransform.sizeDelta;
		staminaStartPos = staminaBar.rectTransform.anchoredPosition;
		staminaStartYMult = staminaStartPos.y/backgroundMaxSize.y;
		staminaBarMaxSize = staminaBar.rectTransform.sizeDelta;
		recoveryStartPos = recoveryBar.rectTransform.anchoredPosition;
		recoveryStartYMult = recoveryStartPos.y/backgroundMaxSize.y;
		recoveryBarMaxSize = recoveryBar.rectTransform.sizeDelta;
		chargeStartPos = chargeBar.rectTransform.anchoredPosition;
		chargeStartYMult = chargeStartPos.y/backgroundMaxSize.y;
		chargeBarMaxSize = chargeBar.rectTransform.sizeDelta;
		
		playerStats = GameObject.Find("Player").GetComponent<PlayerStatsS>();
		playerStats.AddUIReference(this);

		UpdateMaxSizes();
		UpdateFills();

	}
	
	// Update is called once per frame
	void Update () {

		if (PlayerStatsS.godMode){
			//TurnOffAll();
		}
		else{
			TurnOnAll();
		//UpdateMaxSizes();
		UpdateFills();
		}
	
	}

	private void UpdateMaxSizes(){

		Vector2 reposition = Vector2.zero;

		// bg stuff
		backgroundCurrentSize = backgroundMaxSize*ScreenMultiplier();
		background.rectTransform.sizeDelta=backgroundCurrentSize;
		//backgroundFill.rectTransform.sizeDelta = backgroundCurrentSize*0.99f;

		// recover stuff
		recoveryBarCurrentSize = recoveryBarMaxSize*ScreenMultiplier();
		recoveryBar.rectTransform.sizeDelta=recoveryBarCurrentSize;

		
		reposition = recoveryBar.rectTransform.anchoredPosition;
		reposition.x = recoveryStartPos.x*ScreenMultiplier();
		reposition.y = backgroundCurrentSize.y*recoveryStartYMult;
		recoveryBar.rectTransform.anchoredPosition = reposition;

		// health stuff
		healthBarCurrentSize = healthBarMaxSize*ScreenMultiplier();
		healthBar.rectTransform.sizeDelta=healthBarCurrentSize;

		
		reposition = healthBar.rectTransform.anchoredPosition;
		reposition.x = healthStartPos.x*ScreenMultiplier();
		reposition.y = backgroundCurrentSize.y*healthStartYMult;
		healthBar.rectTransform.anchoredPosition = reposition;

		// stamina stuff
		staminaBarCurrentSize = staminaBarMaxSize*ScreenMultiplier();
		staminaBar.rectTransform.sizeDelta=staminaBarCurrentSize;

		reposition = staminaBar.rectTransform.anchoredPosition;
		reposition.x = staminaStartPos.x*ScreenMultiplier();
		reposition.y = backgroundCurrentSize.y*staminaStartYMult;
		staminaBar.rectTransform.anchoredPosition = reposition;

		// charge stuff
		chargeBarCurrentSize = chargeBarMaxSize*ScreenMultiplier();
		chargeBar.rectTransform.sizeDelta=chargeBarCurrentSize;

		reposition = chargeBar.rectTransform.anchoredPosition;
		reposition.x = chargeStartPos.x*ScreenMultiplier();
		reposition.y = backgroundCurrentSize.y*chargeStartYMult;
		chargeBar.rectTransform.anchoredPosition = reposition;

	}

	private void UpdateFills(){

		// health fill
		Vector2 fillSize = healthBarMaxSize;
		fillSize.x *= playerStats.currentHealth/playerStats.maxHealth;
		healthFill.rectTransform.sizeDelta = fillSize;
		healthFill.color = Color.Lerp(healthEmptyColor, healthFullColor, playerStats.currentHealth/playerStats.maxHealth);

		// stamina fill
		fillSize = staminaBarMaxSize;
		fillSize.x *= playerStats.currentMana/playerStats.maxMana;
		staminaFill.rectTransform.sizeDelta = fillSize;
		staminaFill.color = Color.Lerp(staminaEmptyColor, staminaFullColor, playerStats.currentMana/playerStats.maxMana);

		// charge fill
		fillSize = chargeBarMaxSize;
		fillSize.x *= playerStats.currentCharge/playerStats.maxCharge;
		chargeFill.rectTransform.sizeDelta = fillSize;
		chargeFill.color = Color.Lerp(chargeEmptyColor, chargeFullColor, playerStats.currentCharge/playerStats.maxCharge);

		// recharge fill
		fillSize = recoveryBarMaxSize;
		if (playerStats.currentRegenCount > 0){
			fillSize.x *= playerStats.currentCooldownTimer/playerStats.recoveryCooldownMax;
		}
		else{
			fillSize.x = 0;
		}
		recoveryFill.rectTransform.sizeDelta = fillSize;

		//healthText.fontSize = Mathf.RoundToInt(startFontSize*ScreenMultiplier());
		//staminaText.fontSize = Mathf.RoundToInt(startFontSizeSmall*ScreenMultiplier());
		//chargeText.fontSize = Mathf.RoundToInt(startFontSizeSmallest*ScreenMultiplier());

		healthText.text = "[ " + Mathf.RoundToInt(playerStats.currentHealth*10f) + " ]";
		staminaText.text = "< " + Mathf.RoundToInt(playerStats.currentMana*10f) + " >";
		chargeText.text = ""+ playerStats.currentCharge/100*1.0f;
		if (chargeText.text == "1"){
			chargeText.text = "1.0";
		}
		if (chargeText.text == "0"){
			chargeText.text = "0.0";
		}

	}

	private float ScreenMultiplier(){

		//return (Screen.width*1f)/referenceScreenWidth;
		return 1f;

	}

	private float ScreenMultiplierY(){
		
		//return (Screen.height*1f)/referenceScreenHeight;
		return 1f;
		
	}

		
		
	private float GetLeftAnchorPos(){
			
		return 0;
		//return (Screen.width-Screen.height*4f/3f)/2f;
			
	}

	private void TurnOnAll(){

		if (!allTurnedOn){
			healthBar.enabled = true;
			recoveryBar.enabled = true;
			staminaBar.enabled = true;
			//background.enabled = true;
			allTurnedOn = true;
		}

	}

	private void TurnOffAll(){
		if (allTurnedOn){
			healthBar.enabled = false;
			recoveryBar.enabled = false;
			staminaBar.enabled = false;
			background.enabled = false;
			allTurnedOn = false;
		}
	}
}
