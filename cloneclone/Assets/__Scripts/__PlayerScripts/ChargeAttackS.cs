﻿using UnityEngine;
using System.Collections;

public class ChargeAttackS : MonoBehaviour {


	private Renderer _myRenderer;
	private Collider _myCollider;
	private float _animateRate = 0.033f;
	private float animateCountdown;
	private Vector2 startTiling;
	private float tilingRandomMult = 0.5f;

	private float visibleTime = 0.4f;
	private float startAlpha = 1f;
	private float fadeRate;
	private Color fadeColor;

	public Texture startFlash;
	private Texture startTexture;
	private int flashFrames = 3;
	private int colliderFrames = 3;
	private int flashMax = 3;
	private int blackFlashFrames = 4;

	public GameObject soundObj;
	public GameObject hitObj;

	private PlayerController myPlayer;

	[Header("Attack Properties")]
	public float spawnRange = 1f;
	public float knockbackForce = 1000f;
	public float dmg = 5f;
	public float stunMult = 2f;
	public float absorbPercent = 0.1f;
	private Vector3 knockBackDir;
	public int shakeAmt = 1;

	private bool _firstSpawned = false;

	private bool standAlone = false;

	private FlashEffectS flashEffect;


	// Use this for initialization
	void Start () {

		_myRenderer = GetComponentInChildren<Renderer>();
		_myCollider = GetComponent<Collider>();
		if (transform.parent != null){
			myPlayer = GetComponentInParent<PlayerController>();
		}else{
			standAlone = true;
		}
		animateCountdown = _animateRate;

		startTiling = _myRenderer.material.GetTextureScale("_MainTex");
		startTexture = _myRenderer.material.GetTexture("_MainTex");
		_myRenderer.enabled = false;

		fadeRate = startAlpha/visibleTime;

		fadeColor = _myRenderer.material.color;

		flashEffect = CameraEffectsS.E.specialFlash;

		if (standAlone){
			TriggerAttack(transform.position, Vector3.zero, myPlayer.myRenderer.color);
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (transform.parent){
			transform.parent = null;
		}

		if (_myRenderer.enabled){
		

			flashFrames--;
			if (flashFrames < 0){
				
				if (_myCollider.enabled && flashFrames < -colliderFrames){
					_myCollider.enabled = false;
				}

				if (_myRenderer.material.GetTexture("_MainTex") == startFlash){
					if (_myRenderer.material.color == Color.black){
						_myRenderer.material.color = Color.white;
						flashFrames = blackFlashFrames;
					}else{
					_myRenderer.material.SetTexture("_MainTex", startTexture);
						_myCollider.enabled = true;
						if (_firstSpawned){
							flashEffect.NewColor(fadeColor);
							flashEffect.Flash();
						}
					}
				}else{


			fadeColor.a -= Time.deltaTime*fadeRate;
			if (fadeColor.a <= 0){
						if (!standAlone){
				fadeColor.a = 0;
				_myRenderer.enabled = false;
					_myCollider.enabled = false;
						}else{
							Destroy(gameObject);
						}
			}else{
				_myRenderer.material.color = fadeColor;
			}
				animateCountdown -= Time.deltaTime;
				if (animateCountdown <= 0){
					animateCountdown = _animateRate;
					Vector2 newTiling = startTiling;
					newTiling.x += Random.insideUnitCircle.x*tilingRandomMult;
					newTiling.y += Random.insideUnitCircle.y*tilingRandomMult;
					_myRenderer.material.SetTextureScale("_MainTex", newTiling);
				}
			}
			}
		}
	
	}

	public void TriggerAttack(Vector3 startPos, Vector3 attackDir, Color fadeCol){

		Vector3 spawnPos = attackDir.normalized;
		spawnPos*=spawnRange;
		spawnPos.z = transform.position.z;
		transform.position = startPos + spawnPos;

		fadeColor = fadeCol;
		fadeColor.a = startAlpha;

		_myRenderer.material.color = Color.black;

		_myRenderer.material.SetTexture("_MainTex", startFlash);
		_myRenderer.enabled = true;
		flashFrames = flashMax;

		if (shakeAmt == 1){
		CameraShakeS.C.TimeSleep(0.1f);
		CameraShakeS.C.LargeShake();
		}
		else if (shakeAmt == 0){
			CameraShakeS.C.SpecialAttackShake();
			CameraShakeS.C.TimeSleep(0.04f);
		}else{
			CameraShakeS.C.SmallShake();
		}

		
		if (soundObj){
			Instantiate(soundObj);
		}


	}

	private float FaceDirection(Vector3 direction){
		
		float rotateZ = 0;
		
		Vector3 targetDir = direction.normalized;
		
		if(targetDir.x == 0){
			if (targetDir.y > 0){
				rotateZ = 90;
			}
			else{
				rotateZ = -90;
			}
		}
		else{
			rotateZ = Mathf.Rad2Deg*Mathf.Atan((targetDir.y/targetDir.x));
		}	
		
		
		if (targetDir.x < 0){
			rotateZ += 180;
		}
		
		rotateZ += -90f+25f*Random.insideUnitCircle.x;

		return rotateZ;
	}

	void OnTriggerEnter(Collider other){
		
		if (other.gameObject.tag == "Enemy"){

			if (!other.gameObject.GetComponent<EnemyS>().isDead){
		
			knockBackDir = (other.transform.position-transform.position).normalized;
			knockBackDir.z = 1f;

			//float actingDmg = dmg*myPlayer.myStats.strengthAmt;

			other.gameObject.GetComponent<EnemyS>().TakeDamage
				(knockBackDir*knockbackForce*Time.deltaTime, 
				 dmg, stunMult, 2f);

				if (myPlayer.playerAug.lunaAug){
					myPlayer.myStats.RecoverCharge(absorbPercent*PlayerAugmentsS.lunaAugAmt);
				}else{
					myPlayer.myStats.RecoverCharge(absorbPercent);
				}

			HitEffect(other.transform.position, other.gameObject.GetComponent<EnemyS>().bloodColor);
			}
		}
		
	}

	void HitEffect(Vector3 spawnPos, Color bloodCol){

		Vector3 hitObjSpawn = spawnPos;
		GameObject newHitObj = Instantiate(hitObj, hitObjSpawn, Quaternion.identity)
			as GameObject;
		
		newHitObj.transform.Rotate(new Vector3(0,0,FaceDirection((spawnPos-transform.position).normalized)));
		
		SpriteRenderer hitRender = newHitObj.GetComponent<SpriteRenderer>();
		hitRender.color = bloodCol;

		newHitObj.transform.localScale = Vector3.one*10f;

		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/2f;
		newHitObj.transform.position = hitObjSpawn;
	}

	public void SetPlayer(PlayerController pRef){
		myPlayer = pRef;
	}

	public void SetFirstSpawned(){
		_firstSpawned = true;
	}

	public void TurnOffStun(){
		stunMult = 0f;
	}
}
