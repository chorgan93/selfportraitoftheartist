using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileS : MonoBehaviour {

	public static float EXTRA_FORCE_MULT = 2.2f;
	private float enragedMult = 1.75f;
	
	[Header("Projectile Properties")]
	public GameObject soundObj;
	public GameObject hitSoundObj;
	public GameObject hitObj;
	public GameObject hitObjInanimate;
	public GameObject endObj;
	public bool useAltAnim = false;

	[Header("Shot Effects")]
	public bool isAutomatic = true;
	public bool isPiercing = false;
	public bool canInterruptDash = false;
	public float comboDuration = 0.5f;
	public float chainAllow = 0.18f;

	[Header("Control Type")]
	public bool lock4Directional = false;
	public bool lock8Directional = false;

	[Header("Shot Stats")]
	public float delayShotTime = 0.8f;

	public float shotSpeed = 1000f;
	public float maxShotSpeed;
	private float dashAttackSpeedMult = 1.4f;
	private float delayAttackSpeedMult = 1.6f;
	public float spawnRange = 1f;
	public float range = 1f;
	private float currentRange;
	public float rangeRef { get { return currentRange; } }
	//public float minDrag;
	//public float maxDrag;

	public float accuracyMult = 0.1f;

	[Header("Weapon Stats")]
	public bool dashAttack = false;
	public bool delayAttack = false;
	public float dmg = 1;
	public float stunMult = 1f;
	public float critDmg = 2f;
	public float staminaCost = 1;
	public float absorbPercent = 0.1f;
	public float reloadTime = 1f;
	public float numAttacks = 1;
	public float timeBetweenAttacks = 0.1f;

	[Header("Collider Properties")]
	public float delayColliderTime = -1f;
	public float dashDelayAdd = 0.2f;
	public float delayDelayAdd = 0.12f;
	private float delayColliderTimeCountdown;
	public float colliderTurnOffTime = -1f;
	private float colliderCutoff;
	
	[Header("Knockback Stats")]
	public bool stopPlayer = false;
	public bool stopOnEnemyContact = false;
	public float startKnockbackSpeed = 1200f;
	public float knockbackSpeed = 1200f;
	public float knockbackMult = 1.25f;
	public float enemyKnockbackMult = 1.25f;
	public float knockbackTime = 0.2f;
	private float delayAttackKnockbackMult = 1.8f;
	private float delayAttackEnemyKnockbackMult = 3.2f;
	private float dashAttackKnockbackMult = 1.4f;
	private bool colliderTurnedOn = false;
	private bool colliderTurnedOff = false;

	[Header("Effect Properties")]
	public string attackAnimationTrigger;
	public float animationSpeedMult = 1f;
	public int shakeAmt = 0;
	private float maxSizeMult = 0.5f;
	private float maxKnockbackMult = 0.5f;

	private Rigidbody _rigidbody;
	public SpriteRenderer myRenderer;
	public SpriteRenderer projRenderer { get { return myRenderer; } }
	private Collider myCollider;
	private PlayerController _myPlayer;
	public PlayerController myPlayer { get { return _myPlayer; } }

	private bool hitAllTargets = false;

	private bool isDashAttack = false;
	private bool isDelayAttack = false;

	[Header("Charge Attack Properties")]
	public float chargeAttackTime;
	public GameObject chargeAttackPrefab;

	[Header("Extend Properties")]
	public Collider extraRangeCollider;
	public GameObject extraRangeSprite;


	void FixedUpdate () {

		currentRange -= Time.deltaTime;


		delayColliderTimeCountdown -= Time.deltaTime;
		if (delayColliderTimeCountdown <= 0 && !colliderTurnedOn && dmg > 0){
			if (_myPlayer.playerAug.aeroAug){
				extraRangeCollider.enabled = true;
			}else{
				myCollider.enabled = true;
			}
			colliderTurnedOn = true;
		}


		/*if (colliderTurnOffTime > 0){
			if (currentRange <= colliderCutoff && !colliderTurnedOff){
				myCollider.enabled = false;
				colliderTurnedOff = true;
			}
		}**/
		

		if (currentRange <= 0){


			Destroy(gameObject);

		}


	
	}

	public void StartKnockback(PlayerController playerReference, Vector3 aimDirection){
		if (startKnockbackSpeed > 0){
			Vector3 startKForce = aimDirection.normalized*startKnockbackSpeed*Time.unscaledDeltaTime;
			playerReference.myRigidbody.AddForce(startKForce, ForceMode.Impulse);
		}
	}

	public void Fire(bool tooCloseForKnockback, Vector3 aimDirection, Vector3 knockbackDirection, PlayerController playerReference, bool doKnockback = true){
		
		_rigidbody = GetComponent<Rigidbody>();
		myCollider = GetComponent<Collider>();
		_myPlayer = playerReference;
		// powerLvl = dmg;
		dmg *= _myPlayer.myStats.strengthAmt;
		if (_myPlayer.playerAug.enragedAug && _myPlayer.myStats.currentHealth <= _myPlayer.myStats.maxHealth/3f){
			dmg*=enragedMult;
		}

		if (soundObj){
			Instantiate(soundObj);
		}

		if (dmg > 0){ // exclude charge spawners
			if (_myPlayer.playerAug.aeroAug){
				extraRangeSprite.SetActive(true);
				myCollider.enabled = false;
				extraRangeCollider.enabled = true;
			}else{
				extraRangeCollider.enabled = false;
				extraRangeSprite.SetActive(false);
			}
		


		//_rigidbody.drag = minDrag + (1f-((rangeLvl-1f)/4f))*(maxDrag-minDrag);

		if (delayColliderTime > 0 && !dashAttack && !delayAttack){
			myCollider.enabled = false;
			if (extraRangeCollider){
				extraRangeCollider.enabled = false;
			}
			colliderTurnedOn = false;
			delayColliderTimeCountdown = delayColliderTime;
		}else{
			if (_myPlayer.playerAug.aeroAug){
				extraRangeCollider.enabled = true;
			}else{
				myCollider.enabled = true;
			}
			colliderTurnedOn = true;
		}
		}

		else{
			myCollider.enabled = false;
			myRenderer.enabled = false;
		}


		/*if (colliderTurnOffTime > 0){
			colliderCutoff = colliderTurnOffTime;
		}

		colliderTurnedOn = false;**/
		
		FaceDirection((aimDirection).normalized);



			Vector3 shootForce = transform.right * shotSpeed * Time.deltaTime;


			_rigidbody.AddForce(shootForce, ForceMode.Impulse);
		

		Vector3 knockbackForce = -(aimDirection).normalized * knockbackSpeed * (1f + maxKnockbackMult *(1f-1f)/(4f)) * knockbackMult *Time.deltaTime;


		if (stopPlayer){
			if (dashAttack){
				_myPlayer.myRigidbody.velocity *= 0.6f;
			}else{
			_myPlayer.myRigidbody.velocity = Vector3.zero;
			}
		}

		if (doKnockback && !tooCloseForKnockback){

			// attack cooldown formula
			float actingKnockbackTime = knockbackTime - knockbackTime*0.12f*(playerReference.myStats.speedAmt-1f)/4f;

			_myPlayer.Knockback(knockbackForce, actingKnockbackTime, true);

		}
		if (tooCloseForKnockback){
			knockbackSpeed*=1.4f;
		}

		currentRange = range;

		transform.localScale += transform.localScale*(maxSizeMult*(1f-1f)/(4f));



	}

	private void HitscanAttack(Vector3 aimDirection){

		// DOESNT WORK FIX LATER

		currentRange = 1000f;
		myRenderer.enabled = false;
		myCollider.enabled = false;

		StartCoroutine(HitTargets(aimDirection));

	}

	private IEnumerator HitTargets(Vector3 aim){

		RaycastHit hitInfo = new RaycastHit();
		bool hitTarget = true;
		EnemyS hitEnemy;
		Vector3 currentStartPos = _myPlayer.transform.position;

		while (!hitAllTargets){

			hitTarget = Physics.Raycast(currentStartPos, aim.normalized, out hitInfo, 50f, 32,
			                            QueryTriggerInteraction.Ignore);

			Debug.DrawRay(currentStartPos, aim.normalized*50f, Color.green);

			if (hitTarget){
				Debug.Log("Hit something! " + hitInfo.collider.gameObject.name);
				if (hitInfo.collider.gameObject.tag == "Wall"){
					hitAllTargets = true;
				}else{
					currentStartPos.x = hitInfo.point.x;
					currentStartPos.y = hitInfo.point.y;
					hitEnemy = hitInfo.collider.gameObject.GetComponent<EnemyS>();
					if (hitEnemy != null){
						hitEnemy.TakeDamage(knockbackSpeed*Mathf.Abs(enemyKnockbackMult)*_rigidbody.velocity.normalized*Time.deltaTime, 
						                          dmg, stunMult, critDmg*SolAugMult());
					}
				}
			}
			else{
				hitAllTargets = true;
			}


			yield return null;

		}

		Destroy(gameObject);

	}

	private void FaceDirection(Vector3 direction){

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

		rotateZ += accuracyMult*Random.insideUnitCircle.x;

		
		transform.rotation = Quaternion.Euler(new Vector3(0,0,rotateZ));

		DoShake();

	}

	private void DoShake(){

		switch (shakeAmt){

		default:
			CameraShakeS.C.MicroShake();
			break;
		case(1):
			CameraShakeS.C.SmallShake();
			break;
		case(2):
			CameraShakeS.C.LargeShake();
			break;

		case(3):
			CameraShakeS.C.SpecialAttackShake();
			break;
		case(-1):
			break;

		}

	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Destructible"){
			DestructibleItemS destructible = other.gameObject.GetComponent<DestructibleItemS>();
			destructible.TakeDamage(dmg,transform.rotation.z,(transform.position+other.transform.position)/2f);
			HitEffectDestructible(destructible.myRenderer, other.transform.position);
		}

		if (other.gameObject.tag == "Enemy"){

			EnemyS hitEnemy = other.gameObject.GetComponent<EnemyS>();

			if (!hitEnemy.isFriendly){

				if (stopOnEnemyContact && _myPlayer != null){
					if (!_myPlayer.myStats.PlayerIsDead()){
						_myPlayer.myRigidbody.velocity *= 0.4f;
					}
				}
	
	
				float actingKnockbackSpeed = knockbackSpeed;
	
	
				if (isDelayAttack){
					actingKnockbackSpeed *= delayAttackEnemyKnockbackMult;
				}
	
				else if (isDashAttack){
					actingKnockbackSpeed *= dashAttackKnockbackMult;
				}
	
				hitEnemy.TakeDamage
					(actingKnockbackSpeed*Mathf.Abs(enemyKnockbackMult)*_rigidbody.velocity.normalized*Time.deltaTime, 
					 dmg, stunMult, critDmg*SolAugMult());

				if (hitSoundObj){
					Instantiate(hitSoundObj);
				}
	
				if (!isPiercing){
	
					_rigidbody.velocity = Vector3.zero;
	
				}
	
				if (_myPlayer.playerAug.lunaAug){
					_myPlayer.myStats.RecoverCharge(absorbPercent*PlayerAugmentsS.lunaAugAmt);
				}else{
					_myPlayer.myStats.RecoverCharge(absorbPercent);
				}
	
	
				HitEffect(hitEnemy, other.transform.position,hitEnemy.bloodColor,(hitEnemy.currentHealth <= 0 || hitEnemy.isCritical));
			}

		}

	}

	void HitEffect(EnemyS enemyRef, Vector3 spawnPos, Color bloodCol,bool bigBlood = false){
		Vector3 hitObjSpawn = spawnPos;
		hitObjSpawn.z -= 1f;
		GameObject newHitObj = Instantiate(hitObj, hitObjSpawn, transform.rotation)
			as GameObject;

		newHitObj.transform.Rotate(new Vector3(0,0,Random.Range(-20f, 20f)));
		if (transform.localScale.y < 0){
			newHitObj.transform.Rotate(new Vector3(0,0,180f));
		}

		enemyRef.GetComponent<BleedingS>().SpawnBlood(newHitObj.transform.up, bigBlood);

		//SpriteRenderer hitRender = newHitObj.GetComponent<SpriteRenderer>();
		//hitRender.color = bloodCol;

		if (bigBlood){
			newHitObj.transform.localScale = myRenderer.transform.localScale*transform.localScale.x*2.25f;
		}else{
			newHitObj.transform.localScale = myRenderer.transform.localScale*transform.localScale.x*1.75f;
		}

		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/3f;
		newHitObj.transform.position = hitObjSpawn;
	}

	void HitEffectDestructible(SpriteRenderer renderRef, Vector3 spawnPos,bool bigBlood = false){
		Vector3 hitObjSpawn = spawnPos;
		hitObjSpawn.z -= 1f;
		GameObject newHitObj = Instantiate(hitObjInanimate, hitObjSpawn, transform.rotation)
			as GameObject;
		
		newHitObj.transform.Rotate(new Vector3(0,0,Random.Range(-20f, 20f)));
		if (transform.localScale.y < 0){
			newHitObj.transform.Rotate(new Vector3(0,0,180f));
		}
		
		
		
		if (bigBlood){
			newHitObj.transform.localScale = myRenderer.transform.localScale*transform.localScale.x*2.25f;
		}else{
			newHitObj.transform.localScale = myRenderer.transform.localScale*transform.localScale.x*1.75f;
		}
		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/3f;
		newHitObj.transform.position = hitObjSpawn;
	}

	private float SolAugMult(){

		if (_myPlayer.playerAug.solAug){
			return PlayerAugmentsS.solAugAmt;
		}else{
			return 1f;
		}

	}
}
