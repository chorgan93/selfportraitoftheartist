using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStatDisplayS : MonoBehaviour {

	private const float barAddSize = 8f;
	private const float staminaBarAddSize = 7f;
	private const float chargeAddSize = 4f;

	public const bool RECORD_MODE = false; // TODO turn off after build!
	
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
	public Image[] healthIconsArcade;
	private static bool usingArcadeIcons = false;

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
	private float chargeFillMaxHeight;
	private float notEnoughChargeMult = 0.5f;
	private float rechargeFillMaxHeight;
	public Image chargeBar;
	private float rechargeTimeMax = 0.4f;
	private float rechargeTime;
	private bool refillingCharge = false;
	private float refillSizeRate = 50f;
	public Image rechargeRecoveryBar;
	public Image minChargeUseBar;
	private float chargeMinStartX;


	private Vector2 backgroundMaxSize = new Vector2(520,130);
	private Vector2 backgroundCurrentSize;
	private Image background; 
	public Image mantraHighlight;
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

	void Awake(){
		playerStats = GameObject.Find("Player").GetComponent<PlayerStatsS>();
		playerStats.AddUIReference(this);
	}

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
		if (chargeBarMaxSize.x <= 0){
		chargeBarMaxSize = chargeBar.rectTransform.sizeDelta;
		chargeBarMaxSize.x += playerStats.addedCharge*chargeAddSize;
			Debug.Log("setting charge bar max size! " + chargeBarMaxSize.x);
		chargeFillMaxHeight = chargeFill.rectTransform.sizeDelta.y;
			rechargeFillMaxHeight = rechargeRecoveryBar.rectTransform.sizeDelta.y;
			chargeBorderMaxSize = chargeBorder.rectTransform.sizeDelta;
			chargeBorderMaxSize.x+= playerStats.addedCharge*chargeAddSize;
		}

		//chargeMinStartX = minChargeUseBar.rectTransform.anchoredPosition.x;

		healthBorderMaxSize = healthBorder.rectTransform.sizeDelta;
		staminaBorderMaxSize = staminaBorder.rectTransform.sizeDelta;

		

		playerTransform = playerStats.transform;
		pController = playerStats.GetComponent<PlayerController>();
		playerRender = pController.myRenderer;
		mantraHighlight.color = playerRender.color;

		followRef = Camera.main;
		orthoRef = followRef.orthographicSize;

		parentRect = transform.parent.GetComponent<RectTransform>();

		//UpdateMaxSizes();
		UpdateFills(true);

		if (!PlayerController.equippedUpgrades.Contains(0) || hideInScene || RECORD_MODE){
			//Debug.Log("HIDE!");
			DisableUI ();
		}
		if (playerStats.arcadeMode){
			if (!usingArcadeIcons){
				playerStats.ResetArcadeHP();
				usingArcadeIcons = true;
			}
			RefreshArcadeIcons();
		}else{
			usingArcadeIcons = false;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (PlayerStatsS.godMode){
			//TurnOffAll();
		}
		else{
			TurnOnAll();
			updateChargeFills();
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

	public void RefreshArcadeIcons(){
		for (int i = 0; i < healthIconsArcade.Length; i++){
			if (i+1 <= PlayerStatsS.currentArcadeHealth){
				healthIconsArcade[i].enabled = true;
			}else{
				healthIconsArcade[i].enabled = false;
			}
		}
	}



	public void UpdateFills(bool chargeRefill = false, bool levelUp = false){

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
		fillSize.x += playerStats.addedMana*staminaBarAddSize;
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
		borderSize.x += playerStats.addedMana*staminaBarAddSize;
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
		if (levelUp){
			chargeBarMaxSize.x += chargeAddSize;
			chargeBorderMaxSize.x += chargeAddSize;
		}
		if (chargeRefill){
			updateChargeFills(true);
		}
		
		borderSize = chargeBorderMaxSize;
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

	private void updateChargeFills(bool singleRefill = false, float startRefill = -1f){
		if (singleRefill){
			if (playerStats.currentCharge >= playerStats.maxCharge){
				chargeBarCurrentSize.x = chargeBarMaxSize.x;
			}else{
			chargeBarCurrentSize.x = chargeBarMaxSize.x* playerStats.currentCharge/playerStats.maxCharge;
			}
			chargeBarCurrentSize.y = chargeFillMaxHeight;
			if (!playerStats.EnoughChargeForBuddy()){
				chargeBarCurrentSize.y *= notEnoughChargeMult;
			}
			chargeFill.rectTransform.sizeDelta = chargeBarCurrentSize;
			chargeBarCurrentSize.x = 0f;
			chargeBarCurrentSize.y = rechargeFillMaxHeight;
			if (chargeFill.rectTransform.sizeDelta.x/chargeBarMaxSize.x < playerStats.EnoughChargeForBuddyPercent()){
				chargeBarCurrentSize.y *= notEnoughChargeMult;
			}
			rechargeRecoveryBar.rectTransform.sizeDelta = chargeBarCurrentSize;
			chargeFill.color = Color.Lerp(chargeEmptyColor, chargeFullColor, playerStats.currentCharge/playerStats.maxCharge);
		}else if (startRefill > 0){
			chargeBarCurrentSize = chargeFill.rectTransform.sizeDelta;

				chargeBarCurrentSize.y = chargeFillMaxHeight;
				if (chargeBarCurrentSize.x/chargeBarMaxSize.x < playerStats.EnoughChargeForBuddyPercent()){
					chargeBarCurrentSize.y *= notEnoughChargeMult;
				}
				chargeFill.rectTransform.sizeDelta = chargeBarCurrentSize;
				chargeFill.color = Color.Lerp(chargeEmptyColor, chargeFullColor, chargeBarCurrentSize.x/chargeBarMaxSize.x);
				chargeBarCurrentSize.y = rechargeFillMaxHeight;
				if (chargeFill.rectTransform.sizeDelta.x/chargeBarMaxSize.x < playerStats.EnoughChargeForBuddyPercent()){
					chargeBarCurrentSize.y *= notEnoughChargeMult;
				}
			chargeBarCurrentSize.x += startRefill/playerStats.maxCharge*chargeBarMaxSize.x;
				rechargeRecoveryBar.rectTransform.sizeDelta = chargeBarCurrentSize;

			
		}else if (refillingCharge){
			if (rechargeTime > 0){
				rechargeTime -= Time.deltaTime;

			}else{
				chargeBarCurrentSize = chargeFill.rectTransform.sizeDelta;
				chargeBarCurrentSize.x += refillSizeRate*Time.deltaTime;
				if (chargeBarCurrentSize.x >= rechargeRecoveryBar.rectTransform.sizeDelta.x){
					refillingCharge = false;
					chargeBarCurrentSize.x = chargeBarMaxSize.x*playerStats.currentCharge/playerStats.maxCharge;
					chargeBarCurrentSize.y = chargeFillMaxHeight;
					chargeFill.rectTransform.sizeDelta = chargeBarCurrentSize;
					chargeFill.color = Color.Lerp(chargeEmptyColor, chargeFullColor, chargeBarCurrentSize.x/chargeBarMaxSize.x);
					chargeBarCurrentSize.x = 0f;
					chargeBarCurrentSize.y = rechargeFillMaxHeight;
					rechargeRecoveryBar.rectTransform.sizeDelta = chargeBarCurrentSize;
				}else{
				chargeBarCurrentSize.y = chargeFillMaxHeight;
				if (chargeBarCurrentSize.x/chargeBarMaxSize.x < playerStats.EnoughChargeForBuddyPercent()){
					chargeBarCurrentSize.y *= notEnoughChargeMult;
				}
				chargeFill.rectTransform.sizeDelta = chargeBarCurrentSize;
				chargeFill.color = Color.Lerp(chargeEmptyColor, chargeFullColor, chargeBarCurrentSize.x/chargeBarMaxSize.x);
				chargeBarCurrentSize.y = rechargeFillMaxHeight;
					if (chargeFill.rectTransform.sizeDelta.x/chargeBarMaxSize.x < playerStats.EnoughChargeForBuddyPercent()){
					chargeBarCurrentSize.y *= notEnoughChargeMult;
				}
				chargeBarCurrentSize.x = rechargeRecoveryBar.rectTransform.sizeDelta.x;
				rechargeRecoveryBar.rectTransform.sizeDelta = chargeBarCurrentSize;
				}
			}
		}
	}

	public void SetChargeImmediate(){
		/*chargeBarCurrentSize = chargeBarMaxSize;
		chargeBarCurrentSize.x *= playerStats.currentCharge/playerStats.maxCharge;
		chargeFill.rectTransform.sizeDelta =  chargeBarCurrentSize;
		chargeBarCurrentSize.y = rechargeRecoveryBar.rectTransform.sizeDelta.y;
		rechargeRecoveryBar.rectTransform.sizeDelta = chargeBarCurrentSize;
		chargeFill.color = Color.Lerp(chargeEmptyColor, chargeFullColor, playerStats.currentCharge/playerStats.maxCharge);**/
		updateChargeFills(true);
	}

	public void ChargeUseEffect(float chargeUsed){

		refillingCharge = false;

		SetChargeImmediate();

		float chargeUseWidth =  chargeBarMaxSize.x;
		chargeUseWidth *= chargeUsed/playerStats.maxCharge;

		float usePosX = chargeBarMaxSize.x;
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

		updateChargeFills(false, chargeAdded);

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

	public void SetChargeMin(float newMin){
		Vector2 newMinPos = minChargeUseBar.rectTransform.anchoredPosition;
		if (chargeBarMaxSize.x <= 0){
			chargeMinStartX = minChargeUseBar.rectTransform.anchoredPosition.x;
			chargeBarMaxSize = chargeBar.rectTransform.sizeDelta;
			chargeBarMaxSize.x += playerStats.addedCharge*chargeAddSize;

			//Debug.Log("setting charge bar max size! " + chargeBarMaxSize.x);

			chargeFillMaxHeight = chargeFill.rectTransform.sizeDelta.y;
			rechargeFillMaxHeight = rechargeRecoveryBar.rectTransform.sizeDelta.y;
			chargeBorderMaxSize = chargeBorder.rectTransform.sizeDelta;
			chargeBorderMaxSize.x += playerStats.addedCharge*chargeAddSize;
		}
		newMinPos.x = chargeBarMaxSize.x * newMin + chargeMinStartX;
		minChargeUseBar.rectTransform.anchoredPosition = newMinPos;
		if (!refillingCharge){
			SetChargeImmediate();
		}
		if (playerRender){

			mantraHighlight.color = playerRender.color;
		}
	}
}
