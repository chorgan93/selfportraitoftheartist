using UnityEngine;
using System.Collections;

public class EnemyProjectileS : MonoBehaviour {


	private Rigidbody _rigidbody;
	private SpriteRenderer _myRenderer;
	private Renderer _myRenderer3D;
	private EnemyS _myEnemy;
	
	[Header("Projectile Properties")]
	public GameObject soundObj;
	public GameObject hitObj;
	public bool flipOnX = false;

	[Header("Attack Properties")]
	public float range;
	public float turnOffColliderTime = 0.1f;
	public float shotSpeed;
	public float selfKnockbackMult;
	public float attackSpawnDistance;
	public float accuracyMult = 0f;
	public bool stopEnemy = false;
	public bool followEnemy = false;

	[Header("Player Interaction")]
	public bool aoe = false;
	public bool isPiercing = true;
	private bool allowMultiHit = false;
	public float damage;
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
	
	public float fadeThreshold = 0.1f;
	private Color fadeColor;

	private bool hitPlayer = false;

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

		if (followEnemy){
			transform.localPosition = Vector3.zero;
		}
		
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
	
	public void Fire(Vector3 aimDirection, EnemyS enemyReference){

		if (soundObj){
			Instantiate(soundObj);
		}
		
		_rigidbody = GetComponent<Rigidbody>();
		_myRenderer = GetComponentInChildren<SpriteRenderer>();
			myCollider = GetComponent<Collider>();
		if (noCollider){
			myCollider.enabled = false;
		}
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
		}

		if (!aoe){
			FaceDirection((aimDirection).normalized);
		}

		if (followEnemy){
			transform.parent=_myEnemy.transform;
		}else{
			
			Vector3 shootForce = transform.right * shotSpeed * Time.deltaTime;
			
			_rigidbody.AddForce(shootForce, ForceMode.Impulse);
	
			if (flipOnX && shootForce.x > 0){
				Vector3 flipSize = transform.localScale;
				flipSize.y *= -1f;
				transform.localScale = flipSize;
			}
	
			Vector3 knockbackForce = -(aimDirection).normalized * shotSpeed * selfKnockbackMult *Time.deltaTime;
	
			if (_myEnemy != null){
				if (stopEnemy){
					_myEnemy.myRigidbody.velocity = Vector3.zero;
				}
		
				_myEnemy.AttackKnockback(knockbackForce);
			}
		}
			
		
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

		
	}

	
	void OnTriggerEnter(Collider other){
		
		if (other.gameObject.tag == "Player" && (!hitPlayer || (hitPlayer && allowMultiHit))){

			PlayerController playerRef = other.gameObject.GetComponent<PlayerController>();

			if (!playerRef.myStats.PlayerIsDead()){
			if (_myEnemy != null){
					if (followEnemy){
						playerRef.myStats.
							TakeDamage(_myEnemy, damage, _myEnemy.myRigidbody.velocity.normalized*playerKnockbackMult*Time.deltaTime, knockbackTime);
					}
					else{
			playerRef.myStats.TakeDamage(_myEnemy, damage, _rigidbody.velocity.normalized*playerKnockbackMult*Time.deltaTime, knockbackTime);	
					}
				}

			else{

				playerRef.myStats.
					TakeDamage(null, damage, _rigidbody.velocity.normalized*playerKnockbackMult*Time.deltaTime, knockbackTime);

				}

			if (!playerRef.isDashing && !playerRef.isBlocking){
				HitEffect(other.gameObject, other.transform.position,playerRef.myStats.currentHealth<=1f);
			}

				if (!isPiercing){
					range = fadeThreshold;
					_rigidbody.velocity = Vector3.zero;
				}
			hitPlayer = true;
			}
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
		
		p.GetComponent<BleedingS>().SpawnBlood(newHitObj.transform.up, bigBlood);
		
		if (bigBlood){
			newHitObj.transform.localScale = _myRenderer.transform.localScale*transform.localScale.x*1.3f;
		}else{
			newHitObj.transform.localScale = _myRenderer.transform.localScale*transform.localScale.x*0.8f;
		}
		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/3f;
		newHitObj.transform.position = hitObjSpawn;
	}

	public void AllowMultiHit(){
		allowMultiHit = true;
	}
}
