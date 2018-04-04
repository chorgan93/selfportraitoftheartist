using UnityEngine;
using System.Collections;

public class PlayerDashEffectS : MonoBehaviour {

	private PlayerController myController;

	public float spawnDistance = 2f;
	private float distanceTraveled;
	private Vector2 prevDashPos;
	private Vector2 currentDashPos;

	public int maxShadows = 4;
	private int currentShadow = 0;

	private bool newDash = true;
	private float dist = 0f;

	private bool doingAttackEffect = false;
	private float attackEffectTime = 0.1f;
	private float attackEffectCountdown = 0f;
	private bool useAltColor = false;
	private Color mainCol;
	private Color altCol;

	private float disconnectEffectNum = 0.6f;
	private int disconnectedSpriteToUse = 0;

	Vector3 spawnPos;
	GameObject newSpawn;
	private SpriteRenderer newSprite;

	private EffectSpawnManagerS spawnManager;
	public Sprite[] disconnectedSprites;

	private bool disconnectAltCol = true;


	// Use this for initialization
	void Start () {
		myController = GetComponentInParent<PlayerController>();
		myController.SetAttackEffectRef(this);
		spawnManager = GameObject.Find("EffectsManager").GetComponent<EffectSpawnManagerS>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!doingAttackEffect){
		if (!myController.myStats.PlayerIsDead() && !myController.isBlocking && 
		    (myController.isDashing)){
			//if (currentShadow < maxShadows || !myController.isDashing){
			currentDashPos.x = myController.transform.position.x;
			currentDashPos.y = myController.transform.position.y;
			if (newDash){
				SpawnShadow();
				prevDashPos = currentDashPos;
				newDash = false;
			}else{
				dist = Vector2.Distance(prevDashPos, currentDashPos);
				distanceTraveled += dist;

				if (distanceTraveled >= spawnDistance){
					SpawnShadow();
					myController.SpawnAttackPuff();
					prevDashPos = currentDashPos;
				}

			}
			//}
		}else{
			distanceTraveled = 0;
			currentShadow = 0;
			newDash = true;
			}}
		else{
			attackEffectCountdown -= Time.deltaTime;
			if (attackEffectCountdown <= 0){
				SpawnShadow();
			}
		}
	
	}

	void SpawnShadow(){

		spawnPos = myController.transform.position;
		spawnPos.z += 1f;

		newSpawn = spawnManager.SpawnPlayerFade(spawnPos);

		newSprite = newSpawn.GetComponent<SpriteRenderer>();
		newSprite.sprite = myController.myRenderer.sprite;

		newSpawn.transform.localScale = myController.transform.localScale.x * myController.myRenderer.transform.localScale;

		if (doingAttackEffect){
			if (useAltColor){
				newSprite.material.SetColor("_FlashColor", altCol);
			}else{
				newSprite.material.SetColor("_FlashColor", mainCol);
			}
			useAltColor = !useAltColor;
			attackEffectCountdown = attackEffectTime;
			newSpawn.transform.localScale*=1.25f;
		}
		else{
			newSprite.material.SetColor("_FlashColor", myController.EquippedWeapon().flashSubColor);
		distanceTraveled = 0f;
		currentShadow++;
		}

	}

	void StandaloneShadowSpawn(Vector3 shadowPos,bool useAlt, float addFade){
		spawnPos = shadowPos;
		spawnPos.z += 1f;


		newSpawn = spawnManager.SpawnPlayerFade(spawnPos, 0.3f*addFade);

		newSprite = newSpawn.GetComponent<SpriteRenderer>();
		newSprite.sprite = disconnectedSprites[disconnectedSpriteToUse];
		disconnectedSpriteToUse++;
		if (disconnectedSpriteToUse > disconnectedSprites.Length-1){
			disconnectedSpriteToUse = 0;
		}

		newSpawn.transform.localScale = myController.transform.localScale.x * myController.myRenderer.transform.localScale;
		newSpawn.transform.localScale*=0.9f;

		if (useAlt){
		newSprite.material.SetColor("_FlashColor", myController.EquippedWeapon().flashSubColor);
		}else{
			newSprite.material.SetColor("_FlashColor", myController.EquippedWeapon().swapColor);
		}
	}

	public void StartAttackEffect(Color mCol, Color aCol){
		attackEffectCountdown = 0f;
		useAltColor = false;
		mainCol = mCol;
		altCol = aCol;
		doingAttackEffect = true;
	}

	public void EndAttackEffect(){
		doingAttackEffect = false;
	}

	public void DisconnectEffect(Vector3 startPos, Vector3 endPos){
		float effectDistance = Vector3.Distance(startPos, endPos);
		int numEffects = Mathf.CeilToInt(effectDistance*disconnectEffectNum);
		disconnectAltCol = true;
		disconnectedSpriteToUse = 0;
		for (int i = 0; i < numEffects; i++){
			StandaloneShadowSpawn(startPos+(endPos-startPos).normalized*effectDistance*((i*1f)/(numEffects*1f)),disconnectAltCol, i+1);
			disconnectAltCol = !disconnectAltCol;
		}
	}
}
