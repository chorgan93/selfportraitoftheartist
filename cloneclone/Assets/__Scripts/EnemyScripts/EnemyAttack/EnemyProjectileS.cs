using UnityEngine;
using System.Collections;

public class EnemyProjectileS : MonoBehaviour {


	private Rigidbody _rigidbody;
	private SpriteRenderer _myRenderer;
	private Renderer _myRenderer3D;
	private EnemyS _myEnemy;
	public EnemyS myEnemy { get { return _myEnemy; } }
	private bool isFriendly = false;

	private float reflectSpeedMult = 1.5f;
	
	[Header("Projectile Properties")]
	public GameObject soundObj;
	public GameObject hitSoundObj;
	public GameObject hitObj;
	public bool flipOnX = false;
	public bool dontRotate = false;
	public float hitStopAmount = 0.2f;

	[Header("Attack Properties")]
	public float range;
	private float _maxRange;
	public float maxRange { get { return _maxRange; } }
	public float turnOffColliderTime = 0.1f;
	public float shotSpeed;
	public float selfKnockbackMult;
	public float attackSpawnDistance;
	public float accuracyMult = 0f;
	public bool stopEnemy = false;
	public bool followEnemy = false;
	public bool dontTriggerWitchTime = false;

	[Header("Player Interaction")]
	public bool aoe = false;
	public bool isPiercing = true;
	private bool allowMultiHit = false;
	public float damage;
	private float startDamage;
	public float knockbackTime;
	public float playerKnockbackMult;
	public bool noCollider = false;
	
	[Header("Effect Properties")]
	public int shakeAmt = 0;
	public int flashFrames = 0;
	public Texture flashTexture;
	private Texture startTexture;
	private Color startColor;
	private bool doFlashLogic;
	private bool didFlashLogic = false;
	public bool disappearOnRoll = false;
	
	public float fadeThreshold = 0.1f;
	private Color fadeColor;

	private bool _fired = false;

	private bool hitPlayer = false;

	private Collider myCollider;
	private float witchTimeMult = 0.1f;
	private bool inWitchTime = false;

	[Header("Tracking Properties")]
	public bool trackPlayer = false;
	public float trackSpeed = 400f;
	public float trackingDelay = 0.5f;
	public float endTrackingTime = -1;
	private float trackingCountdown;
	private Vector3 trackingAcceleration = Vector3.zero;
	private Transform trackingRef;

	void Start(){
		_maxRange = range;

		if (!_fired){
		_rigidbody = GetComponent<Rigidbody>();

		_myRenderer = GetComponentInChildren<SpriteRenderer>();
		myCollider = GetComponent<Collider>();
			startDamage = damage;

		}

		if (trackPlayer && endTrackingTime < 0){
			endTrackingTime = _maxRange;
		}
	}

	// Update is called once per frame
	void Update(){
		if (doFlashLogic){
			if (flashFrames > 0){
				flashFrames--;
			}else{
				if (!didFlashLogic){
					if (_myRenderer){
						_myRenderer.material.SetFloat("_FlashAmount", 0f);
					}
					else{
						_myRenderer3D.material.SetTexture("_MainTex", startTexture);
						_myRenderer3D.material.color = startColor;
					}
					didFlashLogic = true;
				}
			}
		}

		if (trackPlayer){
			TrackTarget();
		}
	}

	void FixedUpdate () {

		if (followEnemy){
			transform.localPosition = Vector3.zero;
			if (_myEnemy != null){
				if (_myEnemy.isDead || _myEnemy.isCritical){
					myCollider.enabled = false;
					range = 0;
				}
			}
		}
		if (!inWitchTime){
		range -= Time.deltaTime;
		}else{
			range -= Time.deltaTime*0.1f;
		}
		if (myCollider.enabled){
			if (range <= turnOffColliderTime){
				myCollider.enabled = false;
			}
		}

		if (range < fadeThreshold){
			if (_myRenderer){
			fadeColor = _myRenderer.color;
			fadeColor.a = range/fadeThreshold;
			_myRenderer.color = fadeColor;
			}else{
				fadeColor = _myRenderer3D.material.color;
				fadeColor.a = range/fadeThreshold;
				_myRenderer3D.material.color = fadeColor;
			}
		}
		
		if (range <= 0){
			Destroy(gameObject);
		}
		
	}
	
	public void Fire(Vector3 aimDirection, EnemyS enemyReference, float difficultyModifier = 1f){

		_fired = true;
		startDamage = damage;
		if (soundObj){
			Instantiate(soundObj);
		}
		_rigidbody = GetComponent<Rigidbody>();

		_myRenderer = GetComponentInChildren<SpriteRenderer>();
		myCollider = GetComponent<Collider>();
		if (noCollider){
			myCollider.enabled = false;
		}

		_maxRange = range/difficultyModifier;

		if (_myRenderer == null){
			_myRenderer3D = GetComponentInChildren<Renderer>();
			if (flashFrames > 0){
				doFlashLogic = true;
				startColor = _myRenderer3D.material.color;
				startTexture = _myRenderer3D.material.GetTexture("_MainTex");
				_myRenderer3D.material.color = Color.white;
				_myRenderer3D.material.SetTexture("_MainTex", flashTexture);
			}
		}else{
			if (flashFrames > 0){
				doFlashLogic = true;
				_myRenderer.material.SetFloat("_FlashAmount", 1f);
			}
		}

		if (enemyReference != null){
			_myEnemy = enemyReference;
			isFriendly = _myEnemy.isFriendly;
			if (trackPlayer){
				trackingCountdown  = trackingDelay;
				trackingRef = _myEnemy.GetTargetReference();
			}
		}

		if (!aoe){
			FaceDirection((aimDirection).normalized);
		}

		if (followEnemy){
			transform.parent=_myEnemy.transform;
		}else{
			
			Vector3 shootForce = transform.right * shotSpeed * Time.fixedDeltaTime;

			// if drag = 0, this is a projectile, modify the speed
			if (_rigidbody.drag <= 0){
				shootForce *= difficultyModifier;
			}
			
			_rigidbody.AddForce(shootForce, ForceMode.Impulse);
	
			if (flipOnX && shootForce.x > 0){
				Vector3 flipSize = transform.localScale;
				flipSize.y *= -1f;
				transform.localScale = flipSize;
			}
	
			Vector3 knockbackForce = -(aimDirection).normalized * shotSpeed * selfKnockbackMult *Time.fixedDeltaTime;
	
			if (_myEnemy != null){
				if (stopEnemy){
					_myEnemy.myRigidbody.velocity = Vector3.zero;
				}
		
				_myEnemy.AttackKnockback(knockbackForce);
			}
		}

		damage *= Random.Range(1f - EnemyS.DAMAGE_VARIANCE, 1f + EnemyS.DAMAGE_VARIANCE);
			
		
		DoShake();
		
		
	}

	private void DoShake(){
		
		switch (shakeAmt){
			
		default:
			CameraShakeS.C.MicroShake();
			break;
		case(0):
			break;
		case(1):
			CameraShakeS.C.SmallShake();
			break;
		case(2):
			CameraShakeS.C.LargeShake();
			break;
			
		}
		
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

		if (dontRotate){
			_myRenderer.transform.rotation = Quaternion.Euler(new Vector3(0,0,-rotateZ));
		}

		
	}

	private void TrackTarget(){
		if (trackingCountdown > 0){
			trackingCountdown -= Time.deltaTime;
		}else{
			if (endTrackingTime > 0 && !inWitchTime){
				trackingAcceleration = (trackingRef.position-transform.position).normalized*trackSpeed*Time.deltaTime;
			trackingAcceleration.z = 0f;
			_rigidbody.AddForce(trackingAcceleration, ForceMode.Acceleration);
				endTrackingTime -= Time.deltaTime;
			}
		}

	}

	
	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "WitchTime"){
			damage = 0;
			if (!_rigidbody){
				_rigidbody = GetComponent<Rigidbody>();
			}
			_rigidbody.velocity *= witchTimeMult;
			inWitchTime = true;
		}

		if (other.gameObject.tag == "Wall"){
			if (hitSoundObj){
				Instantiate(hitSoundObj);
			}
			HitEffectDestructible(_myRenderer, transform.position);
			if (!_rigidbody){
				_rigidbody = GetComponent<Rigidbody>();
			}
			if (!myCollider){
				myCollider = GetComponent<Collider>();
			}
			if (!isPiercing){
				_rigidbody.velocity = Vector3.zero;
				range = fadeThreshold;
				myCollider.enabled = false;
			}
		}

		if (other.gameObject.tag == "Destructible"){
			DestructibleItemS destructible = other.gameObject.GetComponent<DestructibleItemS>();
			destructible.TakeDamage(damage,transform.rotation.z,(transform.position+other.transform.position)/2f, -1);
			HitEffectDestructible(destructible.myRenderer, other.transform.position);
			if (!isPiercing){
				
				_rigidbody.velocity = Vector3.zero;
				range = fadeThreshold;
				myCollider.enabled = false;
				
			}
			if (hitSoundObj){
				Instantiate(hitSoundObj);
			}
		}

		if (!isFriendly){
		if (other.gameObject.tag == "Player" && (!hitPlayer || (hitPlayer && allowMultiHit))){


			PlayerController playerRef = other.gameObject.GetComponent<PlayerController>();

				if (!_rigidbody){
					_rigidbody = GetComponent<Rigidbody>();
				}

			if (!playerRef.myStats.PlayerIsDead()){
			if (_myEnemy != null){
					if (followEnemy){
						playerRef.myStats.
							TakeDamage(_myEnemy, damage, _myEnemy.myRigidbody.velocity.normalized*playerKnockbackMult*Time.fixedDeltaTime, knockbackTime, dontTriggerWitchTime);
						
					}
					else{
							playerRef.myStats.TakeDamage(_myEnemy, damage, _rigidbody.velocity.normalized*playerKnockbackMult*Time.fixedDeltaTime, knockbackTime, dontTriggerWitchTime);

					}
				}

			else{

				playerRef.myStats.
						TakeDamage(null, damage, _rigidbody.velocity.normalized*playerKnockbackMult*Time.fixedDeltaTime, knockbackTime, dontTriggerWitchTime);
				

				}

				if (hitSoundObj){
					Instantiate(hitSoundObj);
				}

			if (!playerRef.isDashing && !playerRef.isBlocking){
				HitEffect(other.gameObject, other.transform.position,playerRef.myStats.currentHealth<=1f);
						playerRef.myStats.DamageEffect(_myRenderer.transform.rotation.eulerAngles.z);
						if (!isPiercing){
							range = fadeThreshold;
							_rigidbody.velocity = Vector3.zero;
						}
			}
					if ((playerRef.isDashing || playerRef.isBlocking) && disappearOnRoll){
						range = fadeThreshold;
					}
				
			hitPlayer = true;
			}
		}
			if (other.gameObject.tag == "Enemy"){
				
				EnemyS hitEnemy = other.gameObject.GetComponent<EnemyS>();

				if (!hitEnemy){
					hitEnemy = other.GetComponentInParent<EnemyS>();
				}
				
				if (hitEnemy.isFriendly && !hitEnemy.isDead && !hitEnemy.invulnerable){
					
					
					hitEnemy.TakeDamage
					(other.transform, selfKnockbackMult*_rigidbody.velocity.normalized*Time.fixedDeltaTime, 
						 0, 1f, 1.5f);
					
					if (hitSoundObj){
						Instantiate(hitSoundObj);
					}
					
					if (!isPiercing){
						range = fadeThreshold;
						_rigidbody.velocity = Vector3.zero;
						
					}
					
					
					HitEffectEnemy(hitEnemy, other.transform.position,hitEnemy.bloodColor,(hitEnemy.currentHealth <= 0 || hitEnemy.isCritical));
				}
				
			}
			
		}

		else{

			if (other.gameObject.tag == "Enemy"){
				
				EnemyS hitEnemy = other.gameObject.GetComponent<EnemyS>();
				
				if (!hitEnemy.isFriendly && !hitEnemy.isDead && !hitEnemy.invulnerable){

					
					hitEnemy.TakeDamage
					(other.transform, selfKnockbackMult*_rigidbody.velocity.normalized*Time.fixedDeltaTime, 
						damage, 1f, 1.5f, hitStopAmount, 0f, true);
					
					if (hitSoundObj){
						Instantiate(hitSoundObj);
					}
					
					if (!isPiercing){
						range = fadeThreshold;
						_rigidbody.velocity = Vector3.zero;
						
					}
					
					
					HitEffectEnemy(hitEnemy, other.transform.position,hitEnemy.bloodColor,(hitEnemy.currentHealth <= 0 || hitEnemy.isCritical));
				}
				
			}

		}
		
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "WitchTime"){
			damage = startDamage;
			_rigidbody.velocity /= witchTimeMult;
			inWitchTime = false;
		}

	}

	void HitEffect(GameObject p, Vector3 spawnPos, bool bigBlood = false){
		Vector3 hitObjSpawn = spawnPos;
		GameObject newHitObj = Instantiate(hitObj, hitObjSpawn, transform.rotation)
			as GameObject;
		
		newHitObj.transform.Rotate(new Vector3(0,0,Random.Range(-20f, 20f)));
		if (transform.localScale.y < 0){
			newHitObj.transform.Rotate(new Vector3(0,0,180f));
		}

		if (!_myRenderer){
			_myRenderer = GetComponentInChildren<SpriteRenderer>();
		}
		
		p.GetComponent<BleedingS>().SpawnBlood(newHitObj.transform.up, bigBlood);
		
		if (bigBlood){
			newHitObj.transform.localScale = _myRenderer.transform.localScale*transform.localScale.x;
		}else{
			newHitObj.transform.localScale = _myRenderer.transform.localScale*transform.localScale.x*0.75f;
		}
		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/3f;
		newHitObj.transform.position = hitObjSpawn;
	}

	void HitEffectEnemy(EnemyS enemyRef, Vector3 spawnPos, Color bloodCol,bool bigBlood = false){
		Vector3 hitObjSpawn = spawnPos;
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
			newHitObj.transform.localScale = _myRenderer.transform.localScale*transform.localScale.x;
		}else{
			newHitObj.transform.localScale = _myRenderer.transform.localScale*transform.localScale.x*0.75f;
		}
		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/3f;
		newHitObj.transform.position = hitObjSpawn;
	}

	void HitEffectDestructible(SpriteRenderer renderRef, Vector3 spawnPos,bool bigBlood = false){
		Vector3 hitObjSpawn = spawnPos;
		hitObjSpawn.z -= 1f;
		GameObject newHitObj = Instantiate(hitObj, hitObjSpawn, transform.rotation)
			as GameObject;
		
		newHitObj.transform.Rotate(new Vector3(0,0,Random.Range(-20f, 20f)));
		if (transform.localScale.y < 0){
			newHitObj.transform.Rotate(new Vector3(0,0,180f));
		}
		
		if (!_myRenderer){
			_myRenderer = GetComponentInChildren<SpriteRenderer>();
		}
		
		if (bigBlood){
			newHitObj.transform.localScale = _myRenderer.transform.localScale*transform.localScale.x*2.25f;
		}else{
			newHitObj.transform.localScale = _myRenderer.transform.localScale*transform.localScale.x*1.75f;
		}
		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/3f;
		newHitObj.transform.position = hitObjSpawn;
	}

	public void AllowMultiHit(){
		allowMultiHit = true;
	}

	private IEnumerator ReflectCoroutine(Vector3 newForce){
		if (myCollider){
			myCollider.enabled = false;
		}
		if (_myRenderer3D){
			doFlashLogic = true;
			_myRenderer3D.material.color = Color.white;
			_myRenderer3D.material.SetTexture("_MainTex", flashTexture);
		}
		if (_myRenderer){
			doFlashLogic = true;
			_myRenderer.material.SetFloat("_FlashAmount", 1f);
		}
		didFlashLogic = false;
		flashFrames = 4;
		yield return new WaitForSeconds(0.2f);

		if (myCollider){
			myCollider.enabled = true;
		}
		CameraShakeS.C.SmallShake();
		Vector3 rotateForce = Vector3.zero;
		if (_myEnemy){
			rotateForce = (_myEnemy.transform.position-transform.position).normalized*shotSpeed*Time.fixedDeltaTime*reflectSpeedMult;
			_rigidbody.AddForce(rotateForce,
				ForceMode.Impulse);
		}else{
			rotateForce = newForce*shotSpeed*Time.fixedDeltaTime*reflectSpeedMult;
			_rigidbody.AddForce(rotateForce, ForceMode.Impulse);
		}
		FaceDirection(rotateForce.normalized);

	}

	public void ReflectProjectile(Vector3 aimDir, ProjectileS myReflector){
		if (!isFriendly){
		isFriendly = true;
		range = _maxRange+0.2f;
			damage = startDamage*1.5f;
		_rigidbody.velocity = Vector3.zero;
		StartCoroutine(ReflectCoroutine(aimDir));
		CameraShakeS.C.MicroShake();
			myReflector.StartReflect(this);
			AnimObjS[] myAnims = GetComponentsInChildren<AnimObjS>();
			for (int i = 0; i < myAnims.Length; i++){
				myAnims[i].ActivateReflect();
			}
		}

	}
}
