using UnityEngine;
using System.Collections;

public class BuddyProjectileS : MonoBehaviour {
	
	
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
	
	[Header("Attack Properties")]
	public float range;
	public float turnOffColliderTime = 0.1f;
	public float shotSpeed;
	public float selfKnockbackMult;
	public float attackSpawnDistance;
	public float accuracyMult = 0f;
	public bool stopBuddy = false;
	
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
	
	private float fadeThreshold = 0.1f;
	private Color fadeColor;
	
	private Collider myCollider;
	
	
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

		if (soundObj){
			Instantiate(soundObj);
		}
		
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
		
		Vector3 shootForce = transform.right * shotSpeed * Time.deltaTime;
		
		_rigidbody.AddForce(shootForce, ForceMode.Impulse);
		
		if (flipOnX && shootForce.x > 0){
			Vector3 flipSize = transform.localScale;
			flipSize.y *= -1f;
			transform.localScale = flipSize;
		}
		
		Vector3 knockbackForce = -(aimDirection).normalized * shotSpeed * selfKnockbackMult *Time.deltaTime;
		
		if (_myBuddy != null){
			if (stopBuddy){
				_myBuddy.myRigid.velocity = Vector3.zero;
			}
			
			_myBuddy.myRigid.AddForce(knockbackForce, ForceMode.Impulse);
		}
		
		
		DoShake();
		
		
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
		
		if (other.gameObject.tag == "Enemy"){
			
			EnemyS hitEnemy = other.gameObject.GetComponent<EnemyS>();

			if (!hitEnemy.isDead){
			
				float actingKnockbackSpeed = shotSpeed*knockbackMult;
	
				
				hitEnemy.TakeDamage
					(actingKnockbackSpeed*_rigidbody.velocity.normalized*Time.deltaTime, 
					 damage, damage);
				
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
		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/3f;
		newHitObj.transform.position = hitObjSpawn;

		Instantiate(hitEffect, hitObjSpawn, Quaternion.identity);
	}
}
