using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStatDisplayS : MonoBehaviour {

	private const float barAddSize = 3f;
	private const float chargeAddSize = 3f;

	public const bool RECORD_MODE = false;
	
	public Color healthFullColor;
	public Color healthEmptyColor;
	public Color staminaFullColor;
	public Color staminaEmptyColor;
	public Color staminaRecoveryColor = Color.yellow;
	public Color staminaExhaustedColor = Color.red;
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
	private Vector2 healthBarDesperateCurrentSize;
	public Image healthBarDesperate;

	private Vector2 staminaBarMaxSize;
	private Vector2 staminaStartPos;
	private float staminaStartYMult;
	private Vector2 staminaBarCurrentSize;
	public Image staminaBar;

	private Vector2 overchargeBarCurrentSize;
	public Image overchargeBar;
	
	private Vector2 recoveryBarMaxSize;
	private Vector2 recoveryStartPos;
	private float recoveryStartYMult;
	private Vector2 recoveryBarCurrentSize;
	public Image recoveryBar;

	private float prevStaminaSize = 0f;

	private Vector2 chargeBarMaxSize;
	private Vector2 chargeStartPos;
	private float chargeStartYMult;
	private Vector2 chargeBarCurrentSize;
	public Image chargeBar;
	private float rechargeTimeMax = 0.4f;
	private float rechargeTime;
	private bool refillingCharge = false;
	private float refillSizeRate = 50f;
	public Image rechargeRecoveryBar;


	private Vector2 backgroundMaxSize = new Vector2(520,130);
	private Vector2 backgroundCurrentSize;
	private Image background; 
	//public Image backgroundFill;

	private float xPositionMultiplier = 0.1f;
	private float yPositionMultiplier = -0.05f;
	private float xPositionMeterMultiplier = 0.1f;


	//___________________________DISPLAY STUFF
	
	public Image healthBorder;
	public Image healthBorderBG;
	public Image healthFill;
	public Image savedHealthFill;
	public Image condemnedHealthFill;
	private Vector2 healthBorderMaxSize;

	public Image staminaFill;
	public Image staminaBorder;
	private Vector2 staminaBorderMaxSize;

	public Vector2 staminaOffset = new Vector2(0,-1f);
	Vector2 viewportPosition;
	Vector2 proportionalPosition;

	public Image recoveryFill;
	
	public Image chargeFill;
	public Image chargeBorder;
	public Image chargeBorderBG;
	private Vector2 chargeBorderMaxSize;

	public ChargeUseUIS[] chargeUses;
	private int currentUseIndex = 0;

	public Text lvText;

//	public Text healthText;
//	public Text staminaText;
//	public Text chargeText;

	private PlayerStatsS playerStats;
	private SpriteRenderer playerRender;
	private PlayerController pController;
	public PlayerController pConRef { get { return pController; } }
	private Transform playerTransform;
	private RectTransform parentRect;

	private Camera followRef;
	private float orthoRef;

	public bool hideInScene;
	private bool allTurnedOn = false;
	public bool statsAreOn { get { return allTurnedOn; } }

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

		healthBorderMaxSize = healthBorder.rectTransform.sizeDelta;
		staminaBorderMaxSize = staminaBorder.rectTransform.sizeDelta;
		chargeBorderMaxSize = chargeBorder.rectTransform.sizeDelta;

		
		playerStats = GameObject.Find("Player").GetComponent<PlayerStatsS>();
		playerStats.AddUIReference(this);
		playerTransform = playerStats.transform;
		pController = playerStats.GetComponent<PlayerController>();
		playerRender = pController.myRenderer;

		followRef = Camera.main;
		orthoRef = followRef.orthographicSize;

		parentRect = transform.parent.GetComponent<RectTransform>();

		//UpdateMaxSizes();
		UpdateFills();

		if (!PlayerController.equippedUpgrades.Contains(0) || hideInScene || RECORD_MODE){
			DisableUI ();
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (PlayerStatsS.godMode){
			//TurnOffAll();
		}
		else{
			TurnOnAll();
		//UpdateMaxSizes();
		//UpdateFills();
			//PositionStamina();
			//UpdateText();
		}
	
	}

	private void UpdateText(){

		if (playerStats.currentLevel < 10){
			lvText.text = "0"+playerStats.currentLevel;
		}else{
			lvText.text = playerStats.currentLevel.ToString();
		}

	}

	private void PositionStamina(){
		viewportPosition = Camera.main.WorldToViewportPoint(playerTransform.position);
		proportionalPosition =new Vector2(
			((viewportPosition.x*parentRect.sizeDelta.x)-(parentRect.sizeDelta.x*0.5f)),
			((viewportPosition.y*parentRect.sizeDelta.y)-(parentRect.sizeDelta.y*0.5f)));
		staminaBorder.rectTransform.anchoredPosition = proportionalPosition+staminaOffset*(orthoRef/followRef.orthographicSize);
	}

	private void UpdateMaxSizes(){

		Vector2 reposition = Vector2.zero;

		// bg stuff
		backgroundCurrentSize = backgroundMaxSize*ScreenMultiplier();
		background.rectTransform.sizeDelta=backgroundCurrentSize;
		//backgroundFill.rectTransform.sizeDelta = backgroundCurrentSize*0.99f;

		// recover stuff
		recoveryBarCurrentSize = recoveryBarMaxSize;
		recoveryBarCurrentSize.x = playerStats.addedMana*barAddSize;
		recoveryBarCurrentSize.y = recoveryBar.rectTransform.sizeDelta.y;
		recoveryBar.rectTransform.sizeDelta=recoveryBarCurrentSize;

		
		reposition = recoveryBar.rectTransform.anchoredPosition;
		reposition.x = recoveryStartPos.x*ScreenMultiplier();
		reposition.y = backgroundCurrentSize.y*recoveryStartYMult;
		recoveryBar.rectTransform.anchoredPosition = reposition;

		// health stuff
		healthBarCurrentSize = healthBarMaxSize*ScreenMultiplier();
		healthBarCurrentSize.x = playerStats.addedHealth*barAddSize;
		healthBar.rectTransform.sizeDelta=healthBarCurrentSize;

		healthBorder.rectTransform.sizeDelta= healthBorderBG.rectTransform.sizeDelta 
			=new Vector2(healthBorderMaxSize.x+playerStats.addedHealth*barAddSize, healthBorderMaxSize.y);

		
		/*reposition = healthBar.rectTransform.anchoredPosition;
		reposition.x = healthStartPos.x*ScreenMultiplier();
		reposition.y = backgroundCurrentSize.y*healthStartYMult;
		healthBar.rectTransform.anchoredPosition = reposition;**/

		// stamina stuff
		staminaBarCurrentSize = staminaBarMaxSize*ScreenMultiplier();
		staminaBarCurrentSize.x = playerStats.addedMana*barAddSize;
		staminaBarCurrentSize.y = staminaFill.rectTransform.sizeDelta.y;
		staminaBar.rectTransform.sizeDelta=staminaBarCurrentSize;

		/*reposition = staminaBar.rectTransform.anchoredPosition;
		reposition.x = staminaStartPos.x*ScreenMultiplier();
		reposition.y = backgroundCurrentSize.y*staminaStartYMult;
		staminaBar.rectTransform.anchoredPosition = reposition;**/

		overchargeBar.rectTransform.anchoredPosition = reposition;
		overchargeBarCurrentSize = staminaBarCurrentSize;
		overchargeBarCurrentSize.x = 0;
		overchargeBar.rectTransform.sizeDelta = overchargeBarCurrentSize;

		// charge stuff
		chargeBarCurrentSize = chargeBarMaxSize*ScreenMultiplier();
		chargeBarCurrentSize.x = playerStats.addedCharge*barAddSize;
		chargeBar.rectTransform.sizeDelta=chargeBarCurrentSize;


		chargeBorder.rectTransform.sizeDelta= chargeBorderBG.rectTransform.sizeDelta 
			=new Vector2(chargeBorderMaxSize.x+playerStats.addedCharge*chargeAddSize, chargeBorderMaxSize.y);

		/*reposition = chargeBar.rectTransform.anchoredPosition;
		reposition.x = chargeStartPos.x*ScreenMultiplier();
		reposition.y = backgroundCurrentSize.y*chargeStartYMult;
		chargeBar.rectTransform.anchoredPosition = reposition;**/

	}

	public void UpdateFills(){

		if (!healthBarDesperate.enabled){
			if (playerStats.canRecoverHealth > 0){
				healthBarDesperate.enabled = true;
			}
		}

		// health fill
		Vector2 fillSize = healthBarMaxSize;
		fillSize.x += playerStats.addedHealth*barAddSize;
		fillSize.x *= playerStats.currentHealth/playerStats.maxHealth;
		fillSize.y = healthFill.rectTransform.sizeDelta.y;
		healthFill.rectTransform.sizeDelta = fillSize;
		//if (playerStats.currentHealth > playerStats.maxHealth*0.3f){
			healthFill.color = Color.Lerp(healthEmptyColor, healthFullColor, playerStats.currentHealth/playerStats.maxHealth);
		//}else{
		//	healthFill.color = staminaExhaustedColor;
		//}

		if (pController._inCombat && PlayerInventoryS.I.GetItemCount(0) > 0){
			fillSize.x = healthBarMaxSize.x;
			fillSize.x += playerStats.addedHealth*barAddSize;
			fillSize.x *= playerStats.savedHealth/playerStats.maxHealth;
			fillSize.y = savedHealthFill.rectTransform.sizeDelta.y;
			savedHealthFill.rectTransform.sizeDelta = fillSize;
			if (!savedHealthFill.enabled){
				savedHealthFill.enabled = true;
			}
		}else{
			if (savedHealthFill.enabled){
				savedHealthFill.enabled = false;
			}
		}

		if (pController.myStats != null){
		if (pController.myStats.InCondemnedState()){
			if (!condemnedHealthFill.enabled){
				condemnedHealthFill.enabled = true;
				}
				fillSize.y = condemnedHealthFill.rectTransform.sizeDelta.y;
				fillSize.x = (pController.myStats.condemnedCurrentTime/PlayerAugmentsS.CONDEMNED_TIME)
					*(healthBarMaxSize.x+ playerStats.addedHealth*barAddSize);
				condemnedHealthFill.rectTransform.sizeDelta = fillSize;
		}else{
			if (condemnedHealthFill.enabled){
				condemnedHealthFill.enabled = false;
			}
		}
		}

		Vector2 borderSize = healthBorderMaxSize;
		borderSize.y = healthBorder.rectTransform.sizeDelta.y;
		borderSize.x += playerStats.addedHealth*barAddSize;
		healthBorder.rectTransform.sizeDelta = healthBorderBG.rectTransform.sizeDelta = borderSize;

		// desperate fill
		if (healthBarDesperate.enabled){
			fillSize = healthBarMaxSize;
			fillSize.x += playerStats.addedHealth*barAddSize;
			fillSize.x *= (playerStats.currentHealth+playerStats.canRecoverHealth)/playerStats.maxHealth;
			fillSize.y = healthFill.rectTransform.sizeDelta.y;
			healthBarDesperate.rectTransform.sizeDelta = fillSize;
			if (fillSize.x <= healthFill.rectTransform.sizeDelta.x || playerStats.canRecoverHealth <= 0){
				healthBarDesperate.enabled = false;
			}
		}

		// stamina fill & recharge fill
		fillSize = staminaBarMaxSize;
		fillSize.x += playerStats.addedMana*barAddSize;
		fillSize.x *= playerStats.currentMana/playerStats.maxMana;
		fillSize.y = staminaFill.rectTransform.sizeDelta.y;
		staminaFill.rectTransform.sizeDelta = fillSize;
		if(playerStats.ManaUnlocked() && playerStats.currentMana > 0){
		staminaFill.color = Color.Lerp(staminaEmptyColor, staminaFullColor, playerStats.currentMana/playerStats.maxMana);
			//staminaFill.color = playerRender.color;
			recoveryFill.color = staminaRecoveryColor;
		}else{
			staminaFill.color = recoveryFill.color = staminaExhaustedColor;
		}

		if (playerStats.currentMana <= 0){
			recoveryFill.color = staminaExhaustedColor;
		}

		if (playerStats.currentCooldownTimer <= 0){
			prevStaminaSize = fillSize.x;
		}

		fillSize = staminaBarMaxSize;
		fillSize.y = recoveryFill.rectTransform.sizeDelta.y;
		if (playerStats.currentCooldownTimer > 0 && !playerStats.PlayerIsDead()){
			if (playerStats.currentMana > 0){
				fillSize.x = (prevStaminaSize-staminaFill.rectTransform.sizeDelta.x)*
					playerStats.currentCooldownTimer/playerStats.recoveryCooldownMax;
				if (playerStats.currentCooldownTimer < playerStats.recoveryCooldownMax){
					prevStaminaSize = fillSize.x + staminaFill.rectTransform.sizeDelta.x;
				}
			}else{
				fillSize.x = staminaBarMaxSize.x*(playerStats.OverCooldownMult());
			}
		}
		else{
			fillSize.x = 0;
		}
		//fillSize.x = 0;
		recoveryFill.rectTransform.sizeDelta = fillSize;

		borderSize = staminaBorderMaxSize;
		borderSize.x += playerStats.addedMana*barAddSize;
		staminaBorder.rectTransform.sizeDelta =  borderSize;

		fillSize = staminaBarMaxSize;
		if (playerStats.overchargeMana > 0){
			fillSize.x *= playerStats.overchargeMana/playerStats.maxMana;
			fillSize.y *= 3f/4f;
		}else{
			fillSize.x = 0;
		}
		overchargeBar.rectTransform.sizeDelta = overchargeBarCurrentSize = fillSize;

		// charge fill
		updateChargeFills();
		
		borderSize = chargeBorderMaxSize;
		borderSize.x += playerStats.addedCharge*chargeAddSize;
		borderSize.y = chargeBorder.rectTransform.sizeDelta.y;
		chargeBorder.rectTransform.sizeDelta = chargeBorderBG.rectTransform.sizeDelta = borderSize;


		//healthText.fontSize = Mathf.RoundToInt(startFontSize*ScreenMultiplier());
		//staminaText.fontSize = Mathf.RoundToInt(startFontSizeSmall*ScreenMultiplier());
		//chargeText.fontSize = Mathf.RoundToInt(startFontSizeSmallest*ScreenMultiplier());

		// old text ui
	/*	healthText.text = "[ " + Mathf.RoundToInt(playerStats.currentHealth*10f) + " ]";
		staminaText.text = "< " + Mathf.RoundToInt(playerStats.currentMana*10f) + " >";
		chargeText.text = ""+ playerStats.currentCharge/100*1.0f;
		if (chargeText.text == "1"){
			chargeText.text = "1.0";
		}
		if (chargeText.text == "0"){
			chargeText.text = "0.0";
		}**/

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

			UpdateFills();
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

	private void updateChargeFills(){
		if (!refillingCharge){
			chargeBarCurrentSize = chargeBarMaxSize;
			chargeBarCurrentSize.x += playerStats.addedCharge*chargeAddSize;
			chargeBarCurrentSize.x *= playerStats.currentCharge/playerStats.maxCharge;
			chargeBarCurrentSize.y = chargeFill.rectTransform.sizeDelta.y;
			chargeFill.rectTransform.sizeDelta = chargeBarCurrentSize;
			chargeBarCurrentSize.y = rechargeRecoveryBar.rectTransform.sizeDelta.y;
			chargeBarCurrentSize.x = 0f;
			rechargeRecoveryBar.rectTransform.sizeDelta = chargeBarCurrentSize;
			chargeFill.color = Color.Lerp(chargeEmptyColor, chargeFullColor, playerStats.currentCharge/playerStats.maxCharge);
		}else{
			if (rechargeTime > 0){
				rechargeTime -= Time.deltaTime;
			}else{
				chargeBarCurrentSize.x += refillSizeRate*Time.deltaTime;
				if (chargeBarCurrentSize.x >= rechargeRecoveryBar.rectTransform.sizeDelta.x){
					refillingCharge = false;
					chargeBarCurrentSize.x = rechargeRecoveryBar.rectTransform.sizeDelta.x;
				}
				chargeBarCurrentSize.y = chargeFill.rectTransform.sizeDelta.y;
				chargeFill.rectTransform.sizeDelta = chargeBarCurrentSize;
				chargeFill.color = Color.Lerp(chargeEmptyColor, chargeFullColor, chargeBarCurrentSize.x/chargeBarMaxSize.x);
			}
		}
	}

	public void SetChargeImmediate(){
		chargeBarCurrentSize = chargeBarMaxSize;
		chargeBarCurrentSize.x += playerStats.addedCharge*chargeAddSize;
		chargeBarCurrentSize.x *= playerStats.currentCharge/playerStats.maxCharge;
		chargeFill.rectTransform.sizeDelta =  chargeBarCurrentSize;
		chargeBarCurrentSize.y = rechargeRecoveryBar.rectTransform.sizeDelta.y;
		rechargeRecoveryBar.rectTransform.sizeDelta = chargeBarCurrentSize;
		chargeFill.color = Color.Lerp(chargeEmptyColor, chargeFullColor, playerStats.currentCharge/playerStats.maxCharge);
	}

	public void ChargeUseEffect(float chargeUsed){

		refillingCharge = false;

		SetChargeImmediate();

		float chargeUseWidth =  chargeBarMaxSize.x;
		chargeUseWidth += playerStats.addedCharge*chargeAddSize;
		chargeUseWidth *= chargeUsed/playerStats.maxCharge;

		float usePosX = chargeBarMaxSize.x;
		usePosX += playerStats.addedCharge*chargeAddSize;
		usePosX *= playerStats.currentCharge/playerStats.maxCharge;
		usePosX+=chargeFill.rectTransform.anchoredPosition.x;

		chargeUses[currentUseIndex].StartUse(chargeUseWidth, usePosX);

		currentUseIndex++;
		if (currentUseIndex > chargeUses.Length-1){
			currentUseIndex = 0;
		}
	}

	public void ChargeAddEffect(float chargeAdded){

		if (refillingCharge){
			SetChargeImmediate();
		}

		rechargeTime = rechargeTimeMax;

		Vector2 fillSize = chargeBarMaxSize;
		fillSize.x += playerStats.addedCharge*chargeAddSize;
		fillSize.x *= (playerStats.currentCharge+chargeAdded)/playerStats.maxCharge;
		fillSize.y = rechargeRecoveryBar.rectTransform.sizeDelta.y;
		rechargeRecoveryBar.rectTransform.sizeDelta = fillSize;

		fillSize =chargeBarMaxSize;
		fillSize.x += playerStats.addedCharge*chargeAddSize;
		fillSize.x *= (playerStats.currentCharge)/playerStats.maxCharge;
		fillSize.y = chargeFill.rectTransform.sizeDelta.y;
		chargeFill.rectTransform.sizeDelta = chargeBarCurrentSize = fillSize;
		chargeFill.color = Color.Lerp(chargeEmptyColor, chargeFullColor, playerStats.currentCharge/playerStats.maxCharge);

		refillingCharge = true;


	}

	public void DisableUI(){
		for (int i = 0; i < transform.childCount; i++){
			transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	public void EnableUI(){
		if (!hideInScene){
		for (int i = 0; i < transform.childCount; i++){
			transform.GetChild(i).gameObject.SetActive(true);
		}
		}
	}
}
