using UnityEngine;
using System.Collections;
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

	private EnemyS myEnemy;
	private bool showing = false;

	private LockOnS _lockOnRef;

	// Use this for initialization
	void Start () {

		startPos = borderImage.rectTransform.anchoredPosition;

		startBorderLength = borderImage.rectTransform.sizeDelta.x;
		startBarLength = barBGImage.rectTransform.sizeDelta.x;

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

		myEnemy = newEnemy;
		myEnemy.SetUIReference(this);
		enemyNameText.text = myEnemy.enemyName;

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

		// resize ui elements
		Vector2 resizeRect = borderImage.rectTransform.sizeDelta;
		resizeRect.x = startBorderLength + lengthPerHealth*myEnemy.maxHealth;
		borderImage.rectTransform.sizeDelta = resizeRect;

		resizeRect = barBGImage.rectTransform.sizeDelta;
		resizeRect.x = startBarLength + lengthPerHealth*myEnemy.maxHealth;
		barBGImage.rectTransform.sizeDelta = resizeRect;

		resizeRect = barDamageImage.rectTransform.sizeDelta;
		damageTargetLength = resizeRect.x = 
			(startBarLength + lengthPerHealth*myEnemy.maxHealth)*(myEnemy.currentHealth/myEnemy.maxHealth);
		barDamageImage.rectTransform.sizeDelta = barFullImage.rectTransform.sizeDelta = resizeRect;

		
		// turn on ui elements
		borderImage.gameObject.SetActive(true);
		barBGImage.gameObject.SetActive(true);
		barDamageImage.gameObject.SetActive(true);
		barFullImage.gameObject.SetActive(true);
		enemyNameText.gameObject.SetActive(true);

		showing = true;

		if (addDamage > 0){
			addedDamage = addDamage;
			ResizeForDamage();
		}
	}

	public void ResizeForDamage(bool extraShake = false){

		if (myEnemy != null){

			ShakeBar(extraShake);
	
			Vector2 resizeRect = barFullImage.rectTransform.sizeDelta;
	
			if (addedDamage > 0){
				resizeRect.x = 
					startBarLength+(myEnemy.currentHealth+addedDamage)/myEnemy.maxHealth*(lengthPerHealth*myEnemy.maxHealth);
			}

			if (damageDecreasing || addedDamage > 0){
				barDamageImage.rectTransform.sizeDelta = resizeRect;
			}

			float healthMult = myEnemy.currentHealth/myEnemy.maxHealth;
			if (healthMult < 0f){
				healthMult = 0f;
			}
		
			resizeRect = barFullImage.rectTransform.sizeDelta;
			resizeRect.x = (startBarLength + lengthPerHealth*myEnemy.maxHealth)*healthMult;
			barFullImage.rectTransform.sizeDelta = resizeRect;
					
			damageDecreasing = false;
			if (myEnemy.currentHealth > 0){
			decreaseDelay = startDecreaseDelay;
			}else{
				decreaseDelay = 0f;
				barDamageImage.rectTransform.sizeDelta = resizeRect;
			}
			tookDamage = true;
			addedDamage = 0f;
		}
			
	}

	public void EndLockOn(){

		currentFade = fadeTimeMax;
		fadingOut = true;

	}

	public void TurnOff(){

		if (myEnemy != null){
			myEnemy.RemoveUIReference();
			myEnemy = null;
		}

		currentFade = 0f;
		fadingOut = false;
		flashing = false;
		
		borderImage.gameObject.SetActive(false);
		barBGImage.gameObject.SetActive(false);
		barDamageImage.gameObject.SetActive(false);
		barFullImage.gameObject.SetActive(false);
		enemyNameText.gameObject.SetActive(false);

		showing = false;
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
}
