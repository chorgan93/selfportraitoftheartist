using UnityEngine;
using System.Collections;

public class PlayerDodgeEffect : MonoBehaviour {
	
	[Header("Effect References")]
	public SpriteRenderer spriteOne;
	public SpriteRenderer spriteTwo;
	public SpriteRenderer spriteThree;
	public SpriteRenderer spriteFour;
	public SpriteRenderer spriteFive;
	public DodgeLinesS dodgeLines;
	private SpawnOnProjectileS particleSpawner;
	
	[Header("Effect Timings")]
	public float sleepTime = 0.14f;
	public float effectDuration = 0.8f;
	public float effectDistance = 0.75f;
	public float effectSpawnDistance = 0.1f;
	private SpriteRenderer playerSprite;
	private PlayerController playerController;
	
	private float allowCounterTime = 0.22f;
	private float allowCounter;
	
	Vector3 oneStartPos;
	Vector3 oneEndPos;
	Vector3 twoStartPos;
	Vector3 twoEndPos;
	Vector3 fourStartPos;
	Vector3 fourEndPos;
	Vector3 fiveStartPos;
	Vector3 fiveEndPos;
	Vector3 spriteThreePos;
	
	private bool cantFire;
	
	private bool matchSprites = false;
	
	private Color startColor;
	private Color endColor;
	private Color halfCol;
	
	private Color currentMainCol = Color.white;
	private Color currentAltCol = Color.white;
	
	private float effectTime;
	private float effectT;
	public AnimationCurve movementCurve;
	
	private Vector3 spriteOneDirection = Vector3.left;
	private Vector3 spriteTwoDirection = Vector3.right;
	private Vector3 spriteFourDirection = Vector3.left;
	private Vector3 spriteFiveDirection = Vector3.right;
	
	// Use this for initialization
	void Start () {
		
		spriteOne.gameObject.SetActive(false);
		spriteTwo.gameObject.SetActive(false);
		spriteThree.gameObject.SetActive(false);
		spriteFour.gameObject.SetActive(false);
		spriteFive.gameObject.SetActive(false);
		playerSprite = GetComponent<SpriteRenderer>();
		playerController = GetComponentInParent<PlayerController>();
		particleSpawner = GetComponent<SpawnOnProjectileS>();
		particleSpawner.enabled = false;
		
		spriteOne.material.SetFloat("_FlashAmount", 1f);
		spriteTwo.material.SetFloat("_FlashAmount", 1f);
		spriteThree.material.SetFloat("_FlashAmount", 1f);
		spriteFour.material.SetFloat("_FlashAmount", 1f);
		spriteFive.material.SetFloat("_FlashAmount", 1f);
		
		startColor = spriteOne.color;
		endColor = startColor;
		endColor.a = 0f;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (spriteOne.gameObject.activeSelf){
			allowCounter -= Time.unscaledDeltaTime;
			
			effectTime += Time.deltaTime;
			
			if (cantFire){
				cantFire = false;
			}
			
			if (effectTime > effectDuration){
				spriteOne.gameObject.SetActive(false);
				spriteTwo.gameObject.SetActive(false);
				spriteThree.gameObject.SetActive(false);
				spriteFour.gameObject.SetActive(false);
				spriteFive.gameObject.SetActive(false);
				particleSpawner.enabled = false;
			}else{
				if (matchSprites){
					spriteOne.sprite = spriteTwo.sprite = spriteThree.sprite 
						= spriteFour.sprite = spriteFive.sprite = playerSprite.sprite;
				}
				effectT = effectTime/effectDuration;
				spriteOne.transform.position = Vector3.Lerp(oneStartPos, oneEndPos, movementCurve.Evaluate(effectT));
				spriteTwo.transform.position = Vector3.Lerp(twoStartPos, twoEndPos, movementCurve.Evaluate(effectT));
				spriteThree.transform.position = spriteThreePos;
				
				if (spriteFour.gameObject.activeSelf){
					
					spriteFour.transform.position = Vector3.Lerp(fourStartPos, fourEndPos, movementCurve.Evaluate(effectT));
					spriteFive.transform.position = Vector3.Lerp(fiveStartPos, fiveEndPos, movementCurve.Evaluate(effectT));
				}
				
				spriteOne.color = spriteTwo.color = spriteFour.color = spriteFive.color =
					Color.Lerp(startColor, endColor, movementCurve.Evaluate(effectT));
				halfCol = spriteOne.color;
				halfCol.a *= 0.5f;
				spriteThree.color = halfCol;
			}
		}
		
	}
	
	public void FireEffect(bool fromParry = false){
		
		if (cantFire){
			cantFire = false;
		}else{
			SetSpriteAppearance(fromParry);
			
			// set up sprite one
			if (!fromParry){
				spriteOneDirection = Vector3.Cross(playerController.counterNormal, Vector3.back);
				spriteOneDirection.z = 0f;
				spriteOneDirection = Quaternion.Euler(0,0,-160f) * spriteOneDirection;
			}else{
				spriteOneDirection = Vector3.Cross(playerController.counterNormal, Vector3.forward);
				spriteOneDirection = Quaternion.Euler(0,0,-30f) * spriteOneDirection;
			}
			spriteOneDirection.z = 0f;
			spriteOne.transform.position = playerSprite.transform.position + spriteOneDirection * effectSpawnDistance;
			oneStartPos = spriteOne.transform.position;
			if (fromParry){
				oneEndPos = oneStartPos+effectDistance*spriteOneDirection*1.5f;
			}else{
				oneEndPos = oneStartPos+effectDistance*spriteOneDirection;
			}
			
			// set up sprite two
			if (!fromParry){
				spriteTwoDirection = Vector3.Cross(playerController.counterNormal, Vector3.forward);
				spriteTwoDirection.z = 0f;
				spriteTwoDirection = Quaternion.Euler(0,0,-160f) * spriteTwoDirection;
			}else{
				spriteTwoDirection = Vector3.Cross(playerController.counterNormal, Vector3.back);
				spriteTwoDirection = Quaternion.Euler(0,0,30f) * spriteTwoDirection;
			}
			spriteTwoDirection.z = 0f;
			spriteTwo.transform.position = playerSprite.transform.position + spriteTwoDirection * effectSpawnDistance;
			twoStartPos = spriteTwo.transform.position;
			if (fromParry){
				twoEndPos = twoStartPos+effectDistance*spriteTwoDirection*1.5f;
			}else{
				twoEndPos = twoStartPos+effectDistance*spriteTwoDirection;
			}
			
			spriteThree.transform.position = spriteThreePos = playerSprite.transform.position;
			spriteFive.transform.localScale = spriteFour.transform.localScale = spriteThree.transform.localScale = 
				spriteOne.transform.localScale = spriteTwo.transform.localScale = playerSprite.transform.localScale;
			effectT = effectTime = 0f;
			spriteOne.gameObject.SetActive(true);
			spriteTwo.gameObject.SetActive(true);
			spriteThree.gameObject.SetActive(true);
			
			if (!fromParry){
				// set up sprite four and five for dodge effect
				spriteFourDirection = -playerController.counterNormal;
				spriteFourDirection = Quaternion.Euler(0,0,-160f) * spriteFourDirection;
				spriteFourDirection.z = 0f;
				spriteFour.transform.position = playerSprite.transform.position + spriteFourDirection * effectSpawnDistance;
				fourStartPos = spriteFour.transform.position;
				fourEndPos = fourStartPos+effectDistance*spriteFourDirection*1.8f;
				
				/*spriteFiveDirection = playerController.counterNormal;
			spriteFiveDirection = Quaternion.Euler(0,0,40f) * spriteFiveDirection;
			spriteFiveDirection.z = 0f;
			spriteFive.transform.position = playerSprite.transform.position + spriteFiveDirection * effectSpawnDistance;
			fiveStartPos = spriteFive.transform.position;
			fiveEndPos = fiveStartPos+effectDistance*spriteFiveDirection*1.8f;*/
				
				spriteFour.gameObject.SetActive(true);
				//spriteFive.gameObject.SetActive(true);
			}
			
			CameraShakeS.C.TimeSleep(sleepTime);
			
			matchSprites = fromParry;
			
			allowCounter = allowCounterTime;

			if (!fromParry){
				dodgeLines.TriggerEffect(playerController.EquippedWeapon().swapColor, playerController.EquippedWeapon().flashSubColor,
					playerSprite.transform.position, playerController.myRigidbody.velocity);
				particleSpawner.enabled = true;
			}
		}
	}
	
	public bool AllowAttackTime(){
		return(allowCounter <= 0f);
	}
	
	void SetSpriteAppearance(bool fromParry = false){
		
		if (currentMainCol != playerController.EquippedWeapon().swapColor){
			
			currentMainCol = playerController.EquippedWeapon().swapColor;
			currentAltCol = playerController.EquippedWeapon().flashSubColor;
			spriteThree.material.SetColor("_FlashColor", currentMainCol);
			if (!fromParry){
				
				spriteOne.material.SetColor("_FlashColor", currentMainCol);
				spriteTwo.material.SetColor("_FlashColor", currentAltCol);
				spriteFour.material.SetColor("_FlashColor", currentMainCol);
				spriteFive.material.SetColor("_FlashColor", currentMainCol);
				
			}else{
				spriteOne.material.SetColor("_FlashColor", currentMainCol);
				spriteTwo.material.SetColor("_FlashColor", currentAltCol);
			}
		}
		
		spriteOne.sprite = spriteTwo.sprite = spriteThree.sprite 
			= spriteFour.sprite = spriteFive.sprite = playerSprite.sprite;
	}
}
