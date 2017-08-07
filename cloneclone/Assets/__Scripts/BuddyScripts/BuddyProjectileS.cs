using UnityEngine;
using System.Collections;

public class BuddyProjectileS : MonoBehaviour {

	private const float DAMAGE_VARIANCE = 0.125f;

	private const float stopAtWallMult = 0.5f;
	private bool stopAtWallTime = false;
	private Vector3 hitWallPos = Vector3.zero;
	private bool touchingWall = false;
	
	private Rigidbody _rigidbody;
	private SpriteRenderer _myRenderer;
	private Renderer _myRenderer3D;
	private BuddyS _myBuddy;

	[Header("Projectile Properties")]
	public GameObject soundObj;
	public GameObject hitSoundObj;
	public GameObject hitObj;
	public GameObject muzzleFlash;
	public GameObject hitEffect;
	public bool flipOnX = false;
	public float hitEffectSizeMult = 1f;
	
	[Header("Attack Properties")]
	public float range;
	private float _maxRange;
	public float maxRange { get { return _maxRange; } }
	public float turnOffColliderTime = 0.1f;
	public float shotSpeed;
	public float selfKnockbackMult;
	public float attackSpawnDistance;
	public float accuracyMult = 0f;
	public bool stopBuddy = false;
	public bool stopTime = false;
	
	[Header("Enemy Interaction")]
	public bool isPiercing = false;
	public float damage;
	public float knockbackTime;
	public float knockbackMult;
	
	[Header("Effect Properties")]
	public int shakeAmt = 0;
	public int flashFrames = 0;
	public Texture flashTexture;
	private Texture startTexture;
	private Color startColor;
	private bool doFlashLogic;
	private bool didFlashLogic = false;
	
	public float fadeThreshold = 0.1f;
	private Color fadeColor;
	
	private Collider myCollider;

	void Start(){
		_maxRange = range;
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
	}
	
	void FixedUpdate () {
		
		range -= Time.deltaTime;
		if (myCollider.enabled){
			if (range <= turnOffColliderTime){
				myCollider.enabled = false;
			}
		}
		if (stopAtWallTime && range <= maxRange*stopAtWallMult){

			HitWallEffect();

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
	
	public void Fire(Vector3 aimDirection, BuddyS buddyReference){

		_maxRange = range;
		if (soundObj){
			Instantiate(soundObj);
		}

		touchingWall = false;
		stopAtWallTime = false;
		
		_rigidbody = GetComponent<Rigidbody>();
		_myRenderer = GetComponentInChildren<SpriteRenderer>();
		myCollider = GetComponent<Collider>();
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
		
		if (buddyReference != null){
			_myBuddy = buddyReference;
		}
		
		FaceDirection((aimDirection).normalized);

		Instantiate(muzzleFlash, transform.position, Quaternion.identity);
		
		Vector3 shootForce = transform.right * shotSpeed * Time.fixedDeltaTime;
		
		_rigidbody.AddForce(shootForce, ForceMode.Impulse);
		
		if (flipOnX && shootForce.x > 0){
			Vector3 flipSize = transform.localScale;
			flipSize.y *= -1f;
			transform.localScale = flipSize;
		}
		
		Vector3 knockbackForce = -(aimDirection).normalized * shotSpeed * selfKnockbackMult *Time.fixedDeltaTime;
		
		if (_myBuddy != null){
			if (stopBuddy){
				_myBuddy.myRigid.velocity = Vector3.zero;
			}
			
			_myBuddy.myRigid.AddForce(knockbackForce, ForceMode.Impulse);
		}

		damage *= Random.Range(1f - DAMAGE_VARIANCE, 1f + DAMAGE_VARIANCE);

		DoShake();
		if (stopTime){
			CameraShakeS.C.TimeSleep(0.1f);
		}
		
		
	}
	
	private void DoShake(){
		
		switch (shakeAmt){
			
		default:
			CameraShakeS.C.MicroShake();
			break;
		case(0):
			CameraShakeS.C.SpecialAttackShake();
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
		
		
	}
	
	
	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Wall"){
			touchingWall = true;
			if (range <= stopAtWallMult*_maxRange){
				HitWallEffect();
			}else{
				stopAtWallTime = true;
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
		
		if (other.gameObject.tag == "Enemy"){
			
			EnemyS hitEnemy = other.gameObject.GetComponent<EnemyS>();

			if (!hitEnemy.isDead && !hitEnemy.isFriendly && !hitEnemy.invulnerable){
			
				float actingKnockbackSpeed = shotSpeed*knockbackMult;
				float actingStunMult = 1f;
				if (_myBuddy.playerRef.playerAug.trustingAug){
					actingStunMult = 1.5f;
				}
				
				hitEnemy.TakeDamage
					(actingKnockbackSpeed*_rigidbody.velocity.normalized*Time.fixedDeltaTime, 
					 damage, actingStunMult, 2f);

				if (_myBuddy){
					_myBuddy.playerRef.ExtendWitchTime();
				}
				
				if (!isPiercing){
					
					_rigidbody.velocity = Vector3.zero;
					range = fadeThreshold;
					myCollider.enabled = false;
					
				}
				if (hitSoundObj){
					Instantiate(hitSoundObj);
				}
			

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
		if (hitSoundObj){
			Instantiate(hitSoundObj);
		}
		HitEffectDestructible(_myRenderer, transform.position);
		if (!isPiercing){
			_rigidbody.velocity = Vector3.zero;
			range = fadeThreshold;
			myCollider.enabled = false;
		}
		stopAtWallTime = false;
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
		
		
		
		if (bigBlood){
			newHitObj.transform.localScale = _myRenderer.transform.localScale*transform.localScale.x*2.25f;
		}else{
			newHitObj.transform.localScale = _myRenderer.transform.localScale*transform.localScale.x*1.75f;
		}
		newHitObj.transform.localScale *= hitEffectSizeMult;
		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/3f;
		newHitObj.transform.position = hitObjSpawn;
	}
	
	void HitEffect(EnemyS enemyRef, Vector3 spawnPos, Color bloodCol,bool bigBlood = false){
		Vector3 hitObjSpawn = spawnPos;
		GameObject newHitObj = Instantiate(hitObj, hitObjSpawn, transform.rotation)
			as GameObject;
		
		newHitObj.transform.Rotate(new Vector3(0,0,Random.Range(-20f, 20f)));
		if (transform.localScale.y < 0){
			newHitObj.transform.Rotate(new Vector3(0,0,180f));
		}
		
		enemyRef.GetComponent<BleedingS>().SpawnBlood(newHitObj.transform.up, bigBlood);
		
		SpriteRenderer hitRender = newHitObj.GetComponent<SpriteRenderer>();
		hitRender.color = bloodCol;
		
		if (bigBlood){
			newHitObj.transform.localScale = transform.localScale*transform.localScale.x*2f;
		}else{
			newHitObj.transform.localScale = transform.localScale*transform.localScale.x*1.3f;
		}
		newHitObj.transform.localScale *= hitEffectSizeMult;
		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/3f;
		newHitObj.transform.position = hitObjSpawn;

		hitObjSpawn = transform.position;
		hitObjSpawn.z -= 1f;

		Instantiate(hitEffect, hitObjSpawn, Quaternion.identity);
	}
}
