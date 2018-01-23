using UnityEngine;
using System.Collections;

public class EnemyChargeAttackS : MonoBehaviour {

	private EnemyS myEnemy;
	public Renderer _myRenderer;
	private Collider _myCollider;
	private float _animateRate = 0.033f;
	private float animateCountdown;
	private Vector2 startTiling;
	private float tilingRandomMult = 0.5f;

	private float visibleTime = 0.4f;
	private float startAlpha = 1f;
	private float fadeRate;
	private Color fadeColor;

	private float capturedChargeTime;
	public float chargeUpTime;
	public float maxChargeAlpha;
	private bool charging = false;

	public Texture startFlash;
	private Texture startTexture;
	private int flashFrames = 3;
	private int colliderFrames = 6;
	private int flashMax = 3;
	private int blackFlashFrames = 4;

	public GameObject hitObj;
	public GameObject soundObj;

	[Header("Attack Properties")]
	public float knockbackForce = 1000f;
	public float dmg = 5f;
	private Vector3 knockBackDir;
	public float knockbackTime = 0.8f;
	public float offsetChargeRange = 0f;
	private Vector3 chargeStartPos;
	private Vector3 renderStartPos;
	private Vector3 chargeOffsetPos;

	public bool standalone = false;
	public bool shakeOverride = false;
	public float standaloneTurnOnTime = 0.6f;

	[Header("Spawn Properties")]
	public GameObject spawnOnCharge;
	private Vector3 spawnPos;
	public Vector3 spawnOffset;
	public float spawnZ = 3f;
	public float spawnRadius;
	public int numToSpawn = 1;

	private bool doKill = false;
	[Header("Friendly Properties")]
	public bool isFriendly = false;


	// Use this for initialization
	void Start () {

		//_myRenderer = GetComponent<Renderer>();
		_myCollider = GetComponent<Collider>();
		animateCountdown = _animateRate;

		if (!standalone){
		myEnemy = GetComponentInParent<EnemyS>();
			chargeStartPos = transform.position;
			renderStartPos = _myRenderer.transform.position;
		if (myEnemy){
			isFriendly = myEnemy.isFriendly;
		}
		}

		startTiling = _myRenderer.material.GetTextureScale("_MainTex");
		startTexture = _myRenderer.material.GetTexture("_MainTex");


		fadeRate = startAlpha/visibleTime;

		fadeColor = _myRenderer.material.color;
		if (!standalone){
			_myRenderer.enabled = false;
		}
		else{
			TurnOn(standaloneTurnOnTime);
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_myRenderer.enabled){

			if (charging){
				chargeUpTime -= Time.deltaTime;
					fadeColor.a = maxChargeAlpha*(1f-chargeUpTime/capturedChargeTime);
				_myRenderer.material.color = fadeColor;
				if (chargeUpTime <= 0){
					TriggerAttack();
				}
			}
			else{

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
							if (myEnemy){
								if (!myEnemy.isCritical && !myEnemy.isDead){
									_myCollider.enabled = true;
									if (doKill){
										myEnemy.KillWithoutXP();
									}
								}
							}else if (!myEnemy){
							_myCollider.enabled = true;
							}
							SpawnObjects();
					}
				}else{


			fadeColor.a -= Time.deltaTime*fadeRate;
			if (fadeColor.a <= 0){
				fadeColor.a = 0;
				_myRenderer.enabled = false;
					_myCollider.enabled = false;
							if (standalone){
								Destroy(transform.parent.gameObject);
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
	
	}

	public void TriggerAttack(){

		fadeColor = _myRenderer.material.color;
		fadeColor.a = startAlpha;

		_myRenderer.material.color = Color.black;

		_myRenderer.material.SetTexture("_MainTex", startFlash);
		_myRenderer.enabled = true;
		flashFrames = flashMax;

		if (!standalone && !shakeOverride){
		CameraShakeS.C.TimeSleep(0.06f);
		}
		if (soundObj){
			Instantiate(soundObj);
		}
		if (shakeOverride){
			CameraShakeS.C.LargeShake();
		}else{
		CameraShakeS.C.SmallShake();
		}

		charging = false;


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
		
		if (other.gameObject.tag == "Player" && !isFriendly){
		
			knockBackDir = (other.transform.position-transform.position).normalized;
			knockBackDir.z = 1f;

			float actingDamage = dmg*Random.Range(1f - EnemyS.DAMAGE_VARIANCE, 1f + EnemyS.DAMAGE_VARIANCE);

			if (myEnemy){
			other.gameObject.GetComponent<PlayerController>().myStats.TakeDamage
				(myEnemy, actingDamage, knockBackDir*knockbackForce*Time.deltaTime, knockbackTime);
			}
			else{
				other.gameObject.GetComponent<PlayerController>().myStats.TakeDamage
				(null, actingDamage, knockBackDir*knockbackForce*Time.deltaTime, knockbackTime);
			}

			//HitEffect(other.transform.position, other.gameObject.GetComponent<EnemyS>().bloodColor);
		}

		if (other.gameObject.tag == "Enemy"){


			EnemyS hitEnemy = other.gameObject.GetComponent<EnemyS>();

			if (hitEnemy != null && !myEnemy != null){
			if (hitEnemy.enemyName != myEnemy.enemyName && !hitEnemy.isDead && (hitEnemy.isFriendly != isFriendly)){

			knockBackDir = (other.transform.position-transform.position).normalized;
			knockBackDir.z = 1f;

			float actingDamage = dmg*Random.Range(1f - EnemyS.DAMAGE_VARIANCE, 1f + EnemyS.DAMAGE_VARIANCE);


				hitEnemy.TakeDamage
					(other.transform, knockBackDir*knockbackForce*Time.deltaTime, actingDamage, 1f, 1f, 0.1f, knockbackTime, true);
			
			}
			}

			//HitEffect(other.transform.position, other.gameObject.GetComponent<EnemyS>().bloodColor);
		}

		
	}

	/*void HitEffect(Vector3 spawnPos, Color bloodCol){

		Vector3 hitObjSpawn = spawnPos;
		GameObject newHitObj = Instantiate(hitObj, hitObjSpawn, Quaternion.identity)
			as GameObject;
		
		newHitObj.transform.Rotate(new Vector3(0,0,FaceDirection((spawnPos-transform.position).normalized)));
		
		SpriteRenderer hitRender = newHitObj.GetComponent<SpriteRenderer>();
		hitRender.color = bloodCol;

		newHitObj.transform.localScale = Vector3.one*10f;

		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/2f;
		newHitObj.transform.position = hitObjSpawn;
	}*/

	public void TurnOn(float attackWarmup, bool killOnCast = false){

		if (!standalone){
			chargeOffsetPos = offsetChargeRange*Random.insideUnitSphere;
			chargeOffsetPos.z = 0f;
			transform.position = chargeOffsetPos+chargeStartPos;
			_myRenderer.transform.position = renderStartPos+chargeOffsetPos;
		}
		doKill = killOnCast;
		capturedChargeTime = chargeUpTime = attackWarmup;
		_myRenderer.material.SetTexture("_MainTex", startFlash);
		fadeColor.a = 0;
		_myRenderer.material.color = fadeColor;
		_myCollider.enabled = false;
		_myRenderer.enabled = true;
		charging = true;

		/*if (soundObj && standalone){
			Instantiate(soundObj);
		}**/

	}

	void SpawnObjects(){
		if (spawnOnCharge){
			for (int i = 0; i < numToSpawn; i++){
				spawnPos = transform.position+spawnOffset;
				spawnPos += Random.insideUnitSphere*spawnRadius;
				spawnPos.z = spawnZ;
				Instantiate(spawnOnCharge, spawnPos, Quaternion.identity);
			}
		}
	}

	public void SetEnemy(EnemyS myRef){
		myEnemy = myRef;
		isFriendly = myEnemy.isFriendly;
	}
}
