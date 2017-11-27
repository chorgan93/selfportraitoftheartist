using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileS : MonoBehaviour {

	private const float stopAtWallMult = 0.5f;
	private bool stopAtWallTime = false;
	private Vector3 hitWallPos = Vector3.zero;
	private bool touchingWall = false;

	private float bioMult = 0.5f;

	public static float EXTRA_FORCE_MULT = 2.2f;
	public int projectileID = -1;

	private const float damageVariance = 0.2f;
	
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
	public bool momsEye = false;
	public float hitStopAmt = 0.1f;

	[Header("Control Type")]
	public bool lock4Directional = false;
	public bool lock8Directional = false;
	public bool lockFaceDirection = false;

	[Header("Shot Stats")]
	public float delayShotTime = 0.8f;

	public float shotSpeed = 1000f;
	public float maxShotSpeed;
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
	public bool counterAttack = false;
	public bool isFinisher = false;
	public float dmg = 1;
	public float killAtLessThan = 0f;
	public bool addOnDetermined = false;
	private float startDmg;
	public float stunMult = 1f;
	public float critDmg = 2f;
	public float staminaCost = 1;
	public float absorbPercent = 0.1f;
	public float reloadTime = 1f;
	public float numAttacks = 1;
	public float timeBetweenAttacks = 0.1f;

	[Header("Combo Properties")]
	public bool allowChainHeavy = true;
	public bool allowChainLight = true;
	public bool activateBiosEffect;

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
	public float reduceHitVelocityMult = 0.5f;
	public float startKnockbackSpeed = 1200f;
	public float knockbackSpeed = 1200f;
	public float knockbackMult = 1.25f;
	public float enemyKnockbackMult = 1.25f;
	public float knockbackTime = 0.2f;
	private float delayAttackEnemyKnockbackMult = 3.2f;
	private float dashAttackKnockbackMult = 1.4f;
	private bool colliderTurnedOn = false;

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
	public bool useAllCharge = false;

	[Header("Extend Properties")]
	public Collider extraRangeCollider;
	public GameObject extraRangeSprite;
	private AnimObjS extraRangeAnim;
	public List<AnimObjS> biosAnimators;

	private int weaponNum = 0;
	private List<EnemyS> enemiesHit = new List<EnemyS>();

	private bool firedOnce = false;
	private AnimObjS[] allAnimators;
	private ProjectilePoolS myPool;
	private Vector3 startScale;
	public ChargeProjectileS chargeProjectileRef;
	private bool useBios = false;

	// hitStopTest
	bool isStopped = false;
	float stopTime = 0f;
	Vector3 savedVelocity;


	void FixedUpdate () {

		if (!isStopped){
		currentRange -= Time.deltaTime;


		delayColliderTimeCountdown -= Time.deltaTime;
		if (delayColliderTimeCountdown <= 0 && !colliderTurnedOn && dmg > 0){
			if (_myPlayer.playerAug.aeroAug && extraRangeCollider){
				extraRangeCollider.enabled = true;
			}else{
				myCollider.enabled = true;
			}
			colliderTurnedOn = true;
		}

			if (stopAtWallTime && currentRange <= range*stopAtWallMult){

				HitWallEffect();

			}


		/*if (colliderTurnOffTime > 0){
			if (currentRange <= colliderCutoff && !colliderTurnedOff){
				myCollider.enabled = false;
				colliderTurnedOff = true;
			}
		}**/
		

		if (currentRange <= 0){

			if (projectileID < 0){
			Destroy(gameObject);
			}else{
				myPool.AddProjectile(this);	
			}

		}
		}else{
			stopTime -= Time.deltaTime;
			if (stopTime <= 0){
				isStopped = false;
				_rigidbody.velocity = savedVelocity;
				SetAnimationEnable(true);
			}
		}


	
	}

	public void StartKnockback(PlayerController playerReference, Vector3 aimDirection){
		if (startKnockbackSpeed > 0){
			Vector3 startKForce = aimDirection.normalized*startKnockbackSpeed*Time.fixedDeltaTime;
			if (playerReference.superCloseEnemyDetect.allEnemiesInRange.Count > 0){
				startKForce *= 0.1f;
			}
			playerReference.myRigidbody.AddForce(startKForce, ForceMode.Impulse);
		}
	}

	public void Fire(bool tooCloseForKnockback, Vector3 aimDirection, Vector3 knockbackDirection, PlayerController playerReference, bool doKnockback = true, 
		int activeBio = 0){

		if (activeBio > 0){
			useBios = true;
		}else{
			useBios = false;
		}

		if (!firedOnce){
		_rigidbody = GetComponent<Rigidbody>();
		myCollider = GetComponent<Collider>();
			_myPlayer = playerReference;
			myPool = _myPlayer.projectilePool;
			weaponNum = _myPlayer.EquippedWeapon().weaponNum;
			startDmg = dmg;
			stopTime = 0f;
			isStopped = false;

			for (int i=0;i < biosAnimators.Count; i++){
				biosAnimators[i].gameObject.SetActive(false);
			}

			if (extraRangeSprite != null){
				extraRangeAnim = extraRangeSprite.GetComponent<AnimObjS>();
			}
		}else{
			dmg = startDmg;
			transform.localScale = startScale;
		}
		stopAtWallTime = false;
		touchingWall = false;
		enemiesHit.Clear();
		// powerLvl = dmg;

		playerReference.SetFace(lockFaceDirection);

		// calculate attack power
		dmg *= _myPlayer.myStats.strengthAmt;
		dmg *= _myPlayer.playerAug.GetParanoidMult();
		if (dashAttack || counterAttack && _myPlayer.playerAug.incensedAug){
			dmg *= _myPlayer.playerAug.incensedPowerMult;
		}
		dmg *= Random.Range(1f-damageVariance, 1f+damageVariance);

		if (_myPlayer.playerAug.enragedAug){
			dmg*= _myPlayer.playerAug.GetEnragedMult();
		}
		if (_myPlayer.adaptiveAugBonus){
			dmg*= PlayerAugmentsS.ADAPTIVE_DAMAGE_BOOST;
		}
		if (useBios){
			dmg*= 1f+(activeBio*bioMult);
		}

		if (soundObj){
			Instantiate(soundObj);
		}

		if (dmg > 0){ // exclude charge spawners
			if (_myPlayer.playerAug.aeroAug){
				if (extraRangeSprite){
				extraRangeSprite.SetActive(true);
				}
				myCollider.enabled = false;
				if (extraRangeCollider){
					extraRangeCollider.enabled = true;
				}
			}else{
				myCollider.enabled = true;
				if (!chargeProjectileRef){
					extraRangeCollider.enabled = false;
					extraRangeSprite.SetActive(false);
				}
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
				if (_myPlayer.playerAug.aeroAug && extraRangeCollider){
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



			Vector3 shootForce = transform.right * shotSpeed * Time.fixedDeltaTime;


			_rigidbody.AddForce(shootForce, ForceMode.Impulse);
		

		Vector3 knockbackForce = -(aimDirection).normalized * knockbackSpeed * (1f + maxKnockbackMult *(1f-1f)/(4f)) * knockbackMult *Time.fixedDeltaTime;


		if (stopPlayer && !_myPlayer.isSprinting){
			if (dashAttack){
				_myPlayer.myRigidbody.velocity *= 0.6f;
			}else{
			_myPlayer.myRigidbody.velocity = Vector3.zero;
			}
		}

		if (doKnockback){

			// attack cooldown formula
			float actingKnockbackTime = knockbackTime - knockbackTime*0.12f*(playerReference.myStats.speedAmt-1f)/4f;

			if (tooCloseForKnockback && knockbackMult < 0){
				knockbackForce *= 0.1f;
			}

			_myPlayer.Knockback(knockbackForce, actingKnockbackTime, true);

		}
		//if (tooCloseForKnockback){
		//	knockbackSpeed*=1.4f;
		//}

		currentRange = range;
		InitializeSprites(_myPlayer.playerAug.aeroAug, activeBio);

		if (!firedOnce){
			transform.localScale += transform.localScale*(maxSizeMult*(1f-1f)/(4f));
			startScale = transform.localScale;
			firedOnce = true;
		}


		if (activateBiosEffect && playerReference){
			playerReference.ActivateBios();
		}


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
						hitEnemy.TakeDamage(knockbackSpeed*enemyKnockbackMult*_rigidbody.velocity.normalized*Time.fixedDeltaTime, 
							dmg, stunMult, critDmg*SolAugMult(), hitStopAmt, 0f, false, killAtLessThan*DeterminedMult());
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
			if (stopOnEnemyContact && _myPlayer != null){
				if (!_myPlayer.myStats.PlayerIsDead() && !_myPlayer.isDashing && !_myPlayer.isStunned && !_myPlayer.isSprinting){
					_myPlayer.myRigidbody.velocity *= reduceHitVelocityMult;
				}
			}
			DestructibleItemS destructible = other.gameObject.GetComponent<DestructibleItemS>();
			//DoShake();
			destructible.TakeDamage(dmg,transform.rotation.z,(transform.position+other.transform.position)/2f, weaponNum);
			HitEffectDestructible(destructible.myRenderer, other.transform.position);

			if (chargeProjectileRef != null){
				chargeProjectileRef.TriggerHit();
			}
		}

		if (other.gameObject.tag == "Wall"){
			touchingWall = true;
			hitWallPos = transform.position;
			if (currentRange <= stopAtWallMult*range){
				HitWallEffect();
			}
			else{
				stopAtWallTime = true;
			}
		}

		if (other.gameObject.tag == "Enemy"){

			EnemyS hitEnemy = other.gameObject.GetComponent<EnemyS>();

			if (!hitEnemy.isFriendly && !enemiesHit.Contains(hitEnemy) && !hitEnemy.invulnerable){

				if (stopOnEnemyContact && _myPlayer != null){
					if (!_myPlayer.myStats.PlayerIsDead() && !_myPlayer.isDashing && !_myPlayer.isStunned && !_myPlayer.isSprinting){
						_myPlayer.myRigidbody.velocity *= reduceHitVelocityMult;
					}
				}
	
	
				float actingKnockbackSpeed = knockbackSpeed;
	
	
				if (isDelayAttack){
					actingKnockbackSpeed *= delayAttackEnemyKnockbackMult;
				}
	
				else if (isDashAttack){
					actingKnockbackSpeed *= dashAttackKnockbackMult;
				}

				//DoShake();
				float dmgDealt = hitEnemy.TakeDamage
					(actingKnockbackSpeed*enemyKnockbackMult*_rigidbody.velocity.normalized*Time.fixedDeltaTime, 
					dmg, stunMult*_myPlayer.playerAug.GetGaeaAug(), critDmg*_myPlayer.playerAug.GetErebosAug(), hitStopAmt, 0f,
					false, killAtLessThan*DeterminedMult());

				_myPlayer.myStats.DesperateRecover(dmgDealt);

				if (isFinisher || counterAttack || hitEnemy.isCritical){
					RankManagerS.R.ScoreHit(1, dmgDealt);
				}else{
					RankManagerS.R.ScoreHit(0, dmgDealt);
				}

				StartMoveStop(hitStopAmt);
				_myPlayer.AnimationStop(hitStopAmt);
				_myPlayer.ExtendWitchTime();

				if (!hitEnemy.isDead){
					_myPlayer.AddEnemyHit(hitEnemy);
				}

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
	
				if (chargeProjectileRef != null){
					chargeProjectileRef.TriggerHit();
				}
				enemiesHit.Add(hitEnemy);
				HitEffect(hitEnemy, other.transform.position,hitEnemy.bloodColor,(hitEnemy.currentHealth <= 0 || hitEnemy.isCritical));
			}

		}

	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Wall"){
			touchingWall = false;
		}
	}

	void HitWallEffect(){
		if (touchingWall){
		if (stopOnEnemyContact && _myPlayer != null){
			if (!_myPlayer.myStats.PlayerIsDead() && !_myPlayer.isDashing && !_myPlayer.isStunned && !_myPlayer.isSprinting){

				_myPlayer.myRigidbody.velocity *= reduceHitVelocityMult;

			}
		}
		HitEffectDestructible(myRenderer, hitWallPos);
		CameraShakeS.C.SmallShake();
		if (hitSoundObj){
			Instantiate(hitSoundObj);
		}
		if (!isPiercing){
			_rigidbody.velocity = Vector3.zero;
			currentRange = 0f;
			myCollider.enabled = false;
		}else{
			_rigidbody.velocity*=reduceHitVelocityMult;
		}
		}
		stopAtWallTime = false;
	}

	void StartMoveStop(float sTime){
		if (!isStopped){
			SetAnimationEnable(false);
			savedVelocity = _rigidbody.velocity*reduceHitVelocityMult;
		}
		isStopped = true;
		_rigidbody.velocity = Vector3.zero;
		stopTime = sTime;
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

	private void InitializeSprites(bool aeroOn = false, int biosActive = 0){
		int numBiosToUse = biosActive;
		if (!firedOnce){
			allAnimators = transform.GetComponentsInChildren<AnimObjS>();
			for (int i = 0; i < allAnimators.Length; i++){
				allAnimators[i].destroyOnEnd = false;
			}
			if (useBios){
				for (int i = 0; i < biosAnimators.Count; i++){
					if (numBiosToUse > 0){
					biosAnimators[i].ResetAnimation();
						numBiosToUse--;
					}else{
						biosAnimators[i].gameObject.SetActive(false);
					}
				}
			}
		}else{
			for (int i = 0; i < allAnimators.Length; i++){
				
					if (allAnimators[i] == extraRangeAnim){
						if (aeroOn){
							allAnimators[i].ResetAnimation();
						}
					}
					else{
						allAnimators[i].ResetAnimation();
					}
				}
				for (int i = 0; i < biosAnimators.Count; i++){

				if (useBios){
					if (numBiosToUse > 0){
						biosAnimators[i].ResetAnimation();
						numBiosToUse--;
					}else{
						biosAnimators[i].gameObject.SetActive(false);
					}
				}
				else{
					biosAnimators[i].gameObject.SetActive(false);
				}
			}


		}
	}

	private void SetAnimationEnable(bool newEnable){
		for (int i = 0; i < allAnimators.Length; i++){
			allAnimators[i].enabled = newEnable;
		}
	}

	float DeterminedMult(){
		if (_myPlayer.playerAug.determinedAug && addOnDetermined){
			return 1.4f;
		}else{
			return 1f;
		}
	}


}
