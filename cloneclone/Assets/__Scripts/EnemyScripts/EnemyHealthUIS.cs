using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyHealthUIS : MonoBehaviour {

	public Image borderImage;
	public Image barBGImage;
	public Image barDamageImage;
	public Image barFullImage;
	public Text enemyNameText;

	private Color borderStartColor;
	private Color fullBarStartColor;
	private Color damageBarStartColor;

	private bool shaking = false;
	private Vector2 startPos;
	private Vector2 shakeOffset;
	private float shakeAmount;
	private float shakeDecay;

	private float barMaxFade = 1f;
	private float borderMaxFade = 0.77f;
	private int flashFrames = 0;
	private bool flashing = false;

	private float fadeTimeMax = 0.18f;
	private float currentFade;
	private bool fadingOut = false;

	private float lengthPerHealth = 5f;
	private float startBorderLength;
	private float startBarLength;

	private float damageTargetLength;
	private float startDecreaseDelay = 0.4f;
	private float decreaseDelay;
	private float reduceRate = 300f;
	private bool tookDamage = false;
	private bool damageDecreasing = false;

	private float addedDamage = 0f;

	private List<EnemyS> myEnemies;
	private float totalMaxHealth;
	private float currentCombinedHealth;
	private float numEnemiesDivider;
	private bool showing = false;

	private LockOnS _lockOnRef;

	// Use this for initialization
	void Start () {

		startPos = borderImage.rectTransform.anchoredPosition;

		startBorderLength = borderImage.rectTransform.sizeDelta.x;
		startBarLength = barBGImage.rectTransform.sizeDelta.x;

		myEnemies = new List<EnemyS>();

		borderStartColor = borderImage.color;
		borderStartColor.a = borderMaxFade;
		damageBarStartColor = barDamageImage.color;
		damageBarStartColor.a = barMaxFade;
		fullBarStartColor = barFullImage.color;
		fullBarStartColor.a = barMaxFade;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (showing){

			if (flashing){
				flashFrames--;
				if (flashFrames <= 0){
					borderImage.color = enemyNameText.color = borderStartColor;
					barFullImage.color = fullBarStartColor;
					//barDamageImage.color = damageBarStartColor;
					flashing = false;
				}
			}
			else if (fadingOut){
				currentFade -= Time.deltaTime;
				if (currentFade <= 0f){
					TurnOff();
				}else{
					float fadeMult = currentFade/fadeTimeMax;

					Color fadeCol = borderImage.color;
					fadeCol.a = borderMaxFade*fadeMult;
					borderImage.color = enemyNameText.color = fadeCol;
					
					fadeCol = barBGImage.color;
					fadeCol.a = borderMaxFade*fadeMult;
					barBGImage.color = fadeCol;
					
					
					fadeCol = barDamageImage.color;
					fadeCol.a = barMaxFade*fadeMult;
					barDamageImage.color = fadeCol;
					
					
					fadeCol = barFullImage.color;
					fadeCol.a = barMaxFade*fadeMult;
					barFullImage.color = fadeCol;
				}
			}

			if (tookDamage){
				decreaseDelay -= Time.deltaTime;
				if (decreaseDelay <= 0){
					damageDecreasing = true;
				}
				if (damageDecreasing){
					Vector2 damageResize = barDamageImage.rectTransform.sizeDelta;
					damageResize.x -= reduceRate*Time.deltaTime;
					if (damageResize.x <= barFullImage.rectTransform.sizeDelta.x){
						damageResize.x = barFullImage.rectTransform.sizeDelta.x;
						damageDecreasing = false;
						tookDamage = false;

						if (!_lockOnRef.lockedOn){
							EndLockOn();
						}
					}
					barDamageImage.rectTransform.sizeDelta = damageResize;
				}
			}
		}
	
	}

	void FixedUpdate(){
		if (showing){
			if (shaking){
				shakeAmount -= shakeDecay*Time.deltaTime;
				if (shakeAmount <= 0){
					shaking = false;
					borderImage.rectTransform.anchoredPosition = startPos;
				}else{
					shakeOffset = Random.insideUnitCircle*shakeAmount;
					borderImage.rectTransform.anchoredPosition = startPos+shakeOffset;
				}
			}
		}
	}

	public void NewTarget(EnemyS newEnemy, float addDamage = 0){

		if (!myEnemies.Contains(newEnemy)){
			myEnemies.Add(newEnemy);
		}
		newEnemy.SetUIReference(this);
		enemyNameText.text = newEnemy.enemyName;

		damageDecreasing = false;
		currentFade = 0f;
		fadingOut = false;

		borderImage.color = enemyNameText.color = borderStartColor;
		barFullImage.color = fullBarStartColor;
		barDamageImage.color = damageBarStartColor;

		// reset colors
		Color resetCol = borderImage.color;
		resetCol.a = borderMaxFade;
		borderImage.color = enemyNameText.color = resetCol;

		resetCol = barBGImage.color;
		resetCol.a = borderMaxFade;
		barBGImage.color = resetCol;

		
		resetCol = barDamageImage.color;
		resetCol.a = barMaxFade;
		barDamageImage.color = resetCol;

		
		resetCol = barFullImage.color;
		resetCol.a = barMaxFade;
		barFullImage.color = resetCol;

		totalMaxHealth = getCurrentMaxHealth();
		currentCombinedHealth = getCurrentCombinedHealth();
		lengthPerHealth = getCurrentXPerHealth();
		numEnemiesDivider = getNumEnemiesDivider();

		// resize ui elements
		Vector2 resizeRect = borderImage.rectTransform.sizeDelta;
		resizeRect.x = startBorderLength + lengthPerHealth/numEnemiesDivider*totalMaxHealth;
		borderImage.rectTransform.sizeDelta = resizeRect;

		resizeRect = barBGImage.rectTransform.sizeDelta;
		resizeRect.x = startBarLength + lengthPerHealth/numEnemiesDivider*totalMaxHealth;
		barBGImage.rectTransform.sizeDelta = resizeRect;

		resizeRect = barDamageImage.rectTransform.sizeDelta;
		damageTargetLength = resizeRect.x = 
			(startBarLength + lengthPerHealth/numEnemiesDivider*totalMaxHealth)*(currentCombinedHealth/totalMaxHealth);
		barDamageImage.rectTransform.sizeDelta = barFullImage.rectTransform.sizeDelta = resizeRect;

		if (PlayerController.equippedTech.Contains(1) && !PlayerStatDisplayS.RECORD_MODE){
		// turn on ui elements
		borderImage.gameObject.SetActive(true);
		barBGImage.gameObject.SetActive(true);
		barDamageImage.gameObject.SetActive(true);
		barFullImage.gameObject.SetActive(true);
		enemyNameText.gameObject.SetActive(true);
		}

		showing = true;

		if (addDamage > 0){
			addedDamage = addDamage;
			ResizeForDamage();
		}
	}

	float getCurrentMaxHealth(){
		float newMaxHealth = 0f;
		for (int i = 0; i< myEnemies.Count; i++){
			newMaxHealth += myEnemies[i].actingMaxHealth;
		}
		return newMaxHealth;
	}

	float getCurrentCombinedHealth(){
		float combinedHealth = 0f;
		for (int i = 0; i < myEnemies.Count; i++){
			combinedHealth += myEnemies[i].currentHealth;
		}
		return combinedHealth;
	}

	float getCurrentXPerHealth(){
		float combinedXPerHealth = 0f;
		for (int i = 0; i < myEnemies.Count; i++){
            combinedXPerHealth += myEnemies[i].healthBarXSize/myEnemies[i].actingMaxHealth;
		}
		if (myEnemies.Count > 0){
			combinedXPerHealth/=(myEnemies.Count*1f);
		}
		return combinedXPerHealth;
	}

	float getNumEnemiesDivider(){
		float div = 1f;
		if (myEnemies.Count <= 0){
			return div;
		}else{
			div = myEnemies.Count*1f;
			return div;
		}
	}

	void CheckFadeOut(){
		if (currentCombinedHealth <= 0f){
			currentFade = fadeTimeMax;
			fadingOut = true;
		}else{
			fadingOut = false;
		}
	}

	public void ResizeForDamage(bool extraShake = false){

		if (myEnemies.Count > 0){

			ShakeBar(extraShake);
	
			Vector2 resizeRect = barFullImage.rectTransform.sizeDelta;

			currentCombinedHealth = getCurrentCombinedHealth();
			totalMaxHealth = getCurrentMaxHealth();
			numEnemiesDivider = getNumEnemiesDivider();
	
			if (addedDamage > 0){
				resizeRect.x = 
					startBarLength+(currentCombinedHealth+addedDamage)/totalMaxHealth*
						(lengthPerHealth/numEnemiesDivider*totalMaxHealth);
			}

			if (damageDecreasing || addedDamage > 0){
				barDamageImage.rectTransform.sizeDelta = resizeRect;
			}

			float healthMult = currentCombinedHealth/totalMaxHealth;
			if (healthMult < 0f){
				healthMult = 0f;
			}
		
			resizeRect = barFullImage.rectTransform.sizeDelta;
			resizeRect.x = (startBarLength + lengthPerHealth/numEnemiesDivider*totalMaxHealth)*healthMult;
			barFullImage.rectTransform.sizeDelta = resizeRect;
					
			damageDecreasing = false;
			if (currentCombinedHealth > 0){
			decreaseDelay = startDecreaseDelay;
			}else{
				decreaseDelay = 0f;
				barDamageImage.rectTransform.sizeDelta = resizeRect;
			}
			tookDamage = true;
			addedDamage = 0f;

			CheckFadeOut();
		}
			
	}

	public void EndLockOn(){

		currentFade = fadeTimeMax;
		//fadingOut = true;

	}

	public void TurnOff(){

		/*if (myEnemy != null){
			myEnemy.RemoveUIReference();
			myEnemy = null;
		}**/

		currentFade = 0f;
		fadingOut = false;
		flashing = false;
		
		borderImage.gameObject.SetActive(false);
		barBGImage.gameObject.SetActive(false);
		barDamageImage.gameObject.SetActive(false);
		barFullImage.gameObject.SetActive(false);
		enemyNameText.gameObject.SetActive(false);

		if (myEnemies != null){
		myEnemies.Clear();
		}

		showing = false;
	}

	void HideAll(){
		borderImage.gameObject.SetActive(false);
		barBGImage.gameObject.SetActive(false);
		barDamageImage.gameObject.SetActive(false);
		barFullImage.gameObject.SetActive(false);
		enemyNameText.gameObject.SetActive(false);
	}
	void ShowAll(){
		if (showing){
			borderImage.gameObject.SetActive(true);
			barBGImage.gameObject.SetActive(true);
			barDamageImage.gameObject.SetActive(true);
			barFullImage.gameObject.SetActive(true);
			enemyNameText.gameObject.SetActive(true);
		}
	}

	private void ShakeBar(bool extra){

		shaking = true;
		if (extra){
			shakeAmount = 8f;
			shakeDecay = 40f;
		}else{
			shakeAmount = 6f;
			shakeDecay = 30f;
		}

	}

	public void SetLockOnRef(LockOnS newRef){
		_lockOnRef = newRef;
	}
	
	public void Show(){
		ShowAll();
	}
	public void Hide(){
		HideAll();
	}
}
