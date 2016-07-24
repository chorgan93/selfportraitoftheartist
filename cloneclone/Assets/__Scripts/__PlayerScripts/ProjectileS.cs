using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileS : MonoBehaviour {

	public static float EXTRA_FORCE_MULT = 2.2f;

	// public float rangeLvl;
	private float powerLvl;
	private float speedLvl;
	public GameObject hitObj;
	public GameObject endObj;

	[Header("Shot Effects")]
	public bool isAutomatic = true;
	public bool isPiercing = false;
	public bool canInterruptDash = false;

	[Header("Control Type")]
	public bool lock4Directional = false;
	public bool lock8Directional = false;

	[Header("Shot Stats")]
	public float rateOfFire = 0.12f;
	public float delayShotTime = 0.8f;

	public float shotSpeed = 1000f;
	public float maxShotSpeed;
	private float dashAttackSpeedMult = 1.6f;
	public float spawnRange = 1f;
	public float range = 1f;
	private float currentRange;
	public float rangeRef { get { return currentRange; } }
	//public float minDrag;
	//public float maxDrag;

	public float accuracyMult = 0.1f;

	[Header("Weapon Stats")]
	public float dmg = 1;
	public float critDmg = 2f;
	public float staminaCost = 1;
	public float reloadTime = 1f;
	public int numShots = 1;
	public int numAttacks = 1;
	public float numTimeBetweenAttacks = 0.08f;

	[Header("Knockback Stats")]
	public float delayColliderTime = -1f;
	public float dashDelayAdd = 0.2f;
	private float delayColliderTimeCountdown;
	public float colliderTurnOffTime = -1f;
	private float colliderCutoff;
	public bool stopPlayer = false;
	public bool stopOnEnemyContact = false;
	public float knockbackSpeed = 1200f;
	public float knockbackMult = 1.25f;
	public float enemyKnockbackMult = 1.25f;
	public float knockbackTime = 0.2f;
	private bool colliderTurnedOn = false;
	private bool colliderTurnedOff = false;

	[Header("Effect Properties")]
	public int shakeAmt = 0;
	private float maxSizeMult = 0.5f;
	private float maxKnockbackMult = 0.5f;

	private Rigidbody _rigidbody;
	private SpriteRenderer myRenderer;
	public SpriteRenderer projRenderer { get { return myRenderer; } }
	private Collider myCollider;
	private PlayerController myPlayer;

	private bool hitAllTargets = false;



	void FixedUpdate () {

		currentRange -= Time.deltaTime;

		/*
		delayColliderTimeCountdown -= Time.deltaTime;
		if (delayColliderTimeCountdown <= 0 && !colliderTurnedOn){
			myCollider.enabled = true;
			colliderTurnedOn = true;
		}**/


		if (colliderTurnOffTime > 0){
			if (currentRange <= colliderCutoff && !colliderTurnedOff){
				myCollider.enabled = false;
				colliderTurnedOff = true;
			}
		}
		

		if (currentRange <= 0){

			/*Vector3 endObjSpawn = transform.position;
			GameObject newEndObj = Instantiate(endObj, endObjSpawn, transform.rotation)
				as GameObject;
			SpriteRenderer endRender = newEndObj.GetComponent<SpriteRenderer>();
			endRender.sprite = myRenderer.sprite;
			endRender.color = myRenderer.color;
			newEndObj.transform.localScale = myRenderer.transform.localScale*transform.localScale.x;*/

			Destroy(gameObject);

		}


	
	}

	public void Fire(Vector3 aimDirection, Vector3 knockbackDirection, PlayerController playerReference, bool extraTap, bool doKnockback = true){
		
		_rigidbody = GetComponent<Rigidbody>();
		myRenderer = GetComponentInChildren<SpriteRenderer>();
		myCollider = GetComponent<Collider>();
		myPlayer = playerReference;
		powerLvl = playerReference.myStats.strengthAmt;
		if (extraTap){
			powerLvl++;
		}

		//_rigidbody.drag = minDrag + (1f-((rangeLvl-1f)/4f))*(maxDrag-minDrag);

		/*if (delayColliderTime > 0){
			myCollider.enabled = false;
		}

		delayColliderTimeCountdown = delayColliderTime;
		if (colliderTurnOffTime > 0){
			colliderCutoff = colliderTurnOffTime;
		}

		colliderTurnedOn = false;**/
		colliderTurnedOff = false;
		
		FaceDirection((aimDirection).normalized);



			Vector3 shootForce = transform.right * shotSpeed * Time.deltaTime;
		
		if (extraTap){
			shootForce *= dashAttackSpeedMult;
			Debug.Log("DASH ATTACK!!");
		}

			_rigidbody.AddForce(shootForce, ForceMode.Impulse);
		

		Vector3 knockbackForce = -(aimDirection).normalized * knockbackSpeed * (1f + maxKnockbackMult *(powerLvl-1f)/(4f)) * knockbackMult *Time.deltaTime;

		if (extraTap){
			//knockbackForce *= dashAttackSpeedMult;
		}

		if (stopPlayer){
			if (extraTap){
				myPlayer.myRigidbody.velocity *= 0.6f;
			}else{
			myPlayer.myRigidbody.velocity = Vector3.zero;
			}
		}

		if (doKnockback){

			// attack cooldown formula
			float actingKnockbackTime = knockbackTime - knockbackTime*0.12f*(playerReference.myStats.speedAmt-1f)/4f;

			myPlayer.Knockback(knockbackForce, actingKnockbackTime, true);

		}

		currentRange = range;

		transform.localScale += transform.localScale*(maxSizeMult*(powerLvl-1f)/(4f));

		//myRenderer.enabled = false;


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
		Vector3 currentStartPos = myPlayer.transform.position;

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
						                          dmg*powerLvl, critDmg*myPlayer.myStats.critAmt);
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
		case(-1):
			break;

		}

	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Enemy"){

			EnemyS hitEnemy = other.gameObject.GetComponent<EnemyS>();

			if (stopOnEnemyContact && myPlayer != null){
				if (!myPlayer.myStats.PlayerIsDead()){
					myPlayer.myRigidbody.velocity *= 0.6f;
				}
			}


			hitEnemy.TakeDamage
				(knockbackSpeed*Mathf.Abs(enemyKnockbackMult)*_rigidbody.velocity.normalized*Time.deltaTime, 
				 dmg*powerLvl, critDmg*myPlayer.myStats.critAmt);

			if (!isPiercing){

				_rigidbody.velocity = Vector3.zero;

			}


			HitEffect(other.transform.position,hitEnemy.bloodColor,(hitEnemy.currentHealth <= 0 || hitEnemy.isCritical));
			

		}

	}

	void HitEffect(Vector3 spawnPos, Color bloodCol,bool bigBlood = false){
		Vector3 hitObjSpawn = spawnPos;
		GameObject newHitObj = Instantiate(hitObj, hitObjSpawn, transform.rotation)
			as GameObject;

		newHitObj.transform.Rotate(new Vector3(0,0,Random.Range(-20f, 20f)));
		if (transform.localScale.y < 0){
			newHitObj.transform.Rotate(new Vector3(0,0,180f));
		}

		SpriteRenderer hitRender = newHitObj.GetComponent<SpriteRenderer>();
		hitRender.color = bloodCol;

		if (bigBlood){
			newHitObj.transform.localScale = myRenderer.transform.localScale*transform.localScale.x*2f;
		}else{
			newHitObj.transform.localScale = myRenderer.transform.localScale*transform.localScale.x*1.3f;
		}

		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/3f;
		newHitObj.transform.position = hitObjSpawn;
	}
}
