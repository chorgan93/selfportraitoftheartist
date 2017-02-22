using UnityEngine;
using System.Collections;

public class PlayerDodgeEffect : MonoBehaviour {

	[Header("Effect References")]
	public SpriteRenderer spriteOne;
	public SpriteRenderer spriteTwo;
	public SpriteRenderer spriteThree;

	[Header("Effect Timings")]
	public float sleepTime = 0.14f;
	public float effectDuration = 0.8f;
	public float effectDistance = 0.75f;
	public float effectSpawnDistance = 0.1f;
	private SpriteRenderer playerSprite;
	private PlayerController playerController;

	private float allowCounterTime = 0.12f;
	private float allowCounter;

	Vector3 oneStartPos;
	Vector3 oneEndPos;
	Vector3 twoStartPos;
	Vector3 twoEndPos;

	private Color startColor;
	private Color endColor;

	private float effectTime;
	private float effectT;
	public AnimationCurve movementCurve;

	private Vector3 spriteOneDirection = Vector3.left;
	private Vector3 spriteTwoDirection = Vector3.right;

	// Use this for initialization
	void Start () {

		spriteOne.gameObject.SetActive(false);
		spriteTwo.gameObject.SetActive(false);
		spriteThree.gameObject.SetActive(false);
		playerSprite = GetComponent<SpriteRenderer>();
		playerController = GetComponentInParent<PlayerController>();


		startColor = spriteOne.color;
		endColor = startColor;
		endColor.a = 0f;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (spriteOne.gameObject.activeSelf){
			allowCounter -= Time.unscaledDeltaTime;

			effectTime += Time.deltaTime;
			if (effectTime > effectDuration){
				spriteOne.gameObject.SetActive(false);
				spriteTwo.gameObject.SetActive(false);
				spriteThree.gameObject.SetActive(false);
			}else{
				//spriteOne.sprite = spriteTwo.sprite = playerSprite.sprite;
				effectT = effectTime/effectDuration;
				spriteOne.transform.position = Vector3.Lerp(oneStartPos, oneEndPos, movementCurve.Evaluate(effectT));
				spriteTwo.transform.position = Vector3.Lerp(twoStartPos, twoEndPos, movementCurve.Evaluate(effectT));
				spriteOne.color = spriteTwo.color = spriteThree.color =
					Color.Lerp(startColor, endColor, movementCurve.Evaluate(effectT));
			}
		}
	
	}

	public void FireEffect(){
		spriteOne.material.SetColor("_FlashColor", playerController.EquippedWeapon().swapColor);
		spriteOne.material.SetFloat("_FlashAmount", 1f);
		spriteThree.material.SetColor("_FlashColor", playerController.EquippedWeapon().swapColor);
		spriteThree.material.SetFloat("_FlashAmount", 1f);
		spriteTwo.material.SetColor("_FlashColor", playerController.EquippedWeapon().flashSubColor);
		spriteTwo.material.SetFloat("_FlashAmount", 1f);
		spriteOne.sprite = spriteTwo.sprite = playerSprite.sprite;
		spriteOne.transform.position = playerSprite.transform.position + spriteOneDirection * effectSpawnDistance;
		oneStartPos = spriteOne.transform.position;
		oneStartPos.z -= 1f;
		spriteOne.transform.position = oneStartPos;
		oneEndPos = oneStartPos+effectDistance*spriteOneDirection;
		spriteTwo.transform.position = playerSprite.transform.position + spriteTwoDirection * effectSpawnDistance;
		twoStartPos = spriteTwo.transform.position;
		twoStartPos.z -= 1f;
		spriteTwo.transform.position = twoStartPos;
		twoEndPos = twoStartPos+effectDistance*spriteTwoDirection;
		spriteThree.transform.position = playerSprite.transform.position;
		spriteThree.transform.localScale = spriteOne.transform.localScale = 
			spriteTwo.transform.localScale = playerSprite.transform.localScale;
		effectT = effectTime = 0f;
		spriteOne.gameObject.SetActive(true);
		spriteTwo.gameObject.SetActive(true);
		spriteThree.gameObject.SetActive(true);
		CameraShakeS.C.TimeSleep(sleepTime);

		allowCounter = allowCounterTime;
	}

	public bool AllowAttackTime(){
		return(allowCounter <= 0f);
	}
}
