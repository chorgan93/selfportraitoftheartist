using UnityEngine;
using System.Collections;

public class ProjectileS : MonoBehaviour {

	public static float EXTRA_FORCE_MULT = 2.2f;

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
	public float spawnRange = 1f;
	public float range = 1f;
	private float currentRange;

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
	private float delayColliderTimeCountdown;
	public float colliderTurnOffTime = -1f;
	private float colliderCutoff;
	public bool stopPlayer = false;
	public bool stopOnEnemyContact = false;
	public float knockbackMult = 1.25f;
	public float enemyKnockbackMult = 1.25f;
	public float knockbackTime = 0.2f;
	private bool colliderTurnedOn = false;
	private bool colliderTurnedOff = false;

	[Header("Effect Properties")]
	public int shakeAmt = 0;

	private Rigidbody _rigidbody;
	private SpriteRenderer myRenderer;
	private Collider myCollider;
	private PlayerController myPlayer;
	private float fadeThreshold = 0.1f;
	private Color fadeColor;

	// Update is called once per frame
	void FixedUpdate () {

		currentRange -= Time.deltaTime;

		delayColliderTimeCountdown -= Time.deltaTime;
		if (delayColliderTimeCountdown <= 0 && !colliderTurnedOn){
			myCollider.enabled = true;
			colliderTurnedOn = true;
		}

		if (colliderTurnOffTime > 0){
			if (currentRange <= colliderCutoff && !colliderTurnedOff){
				myCollider.enabled = false;
				colliderTurnedOff = true;
			}
		}
		

		if (currentRange < fadeThreshold){
			fadeColor = myRenderer.color;
			fadeColor.a = range/fadeThreshold;
			myRenderer.color = fadeColor;
		}

		if (currentRange <= 0){
			Destroy(gameObject);
		}
	
	}

	public void Fire(Vector3 aimDirection, Vector3 knockbackDirection, PlayerController playerReference, bool extraTap, bool doKnockback = true){
		
		_rigidbody = GetComponent<Rigidbody>();
		myRenderer = GetComponentInChildren<SpriteRenderer>();
		myCollider = GetComponent<Collider>();
		myPlayer = playerReference;

		if (delayColliderTime > 0){
			myCollider.enabled = false;
		}

		delayColliderTimeCountdown = delayColliderTime;
		if (colliderTurnOffTime > 0){
			colliderCutoff = colliderTurnOffTime;
		}

		colliderTurnedOn = false;
		colliderTurnedOff = false;
		currentRange = range;
		
		FaceDirection((aimDirection).normalized, playerReference);
		
		if (extraTap){
			shotSpeed *= EXTRA_FORCE_MULT;
		}

		Vector3 shootForce = transform.right * shotSpeed * Time.deltaTime;


		_rigidbody.AddForce(shootForce, ForceMode.Impulse);

		Vector3 knockbackForce = -(aimDirection).normalized * shotSpeed * knockbackMult *Time.deltaTime;

		if (stopPlayer){
			myPlayer.myRigidbody.velocity = Vector3.zero;
		}

		if (doKnockback){
		
			myPlayer.Knockback(knockbackForce, knockbackTime, true);

		}



	}

	private void FaceDirection(Vector3 direction, PlayerController player){

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

			if (stopOnEnemyContact && myPlayer != null){
				if (!myPlayer.myStats.PlayerIsDead()){
					myPlayer.myRigidbody.velocity *= 0.6f;
				}
			}


			other.gameObject.GetComponent<EnemyS>().TakeDamage
				(shotSpeed*Mathf.Abs(enemyKnockbackMult)*_rigidbody.velocity.normalized*Time.deltaTime, 
				 dmg*myPlayer.myStats.strengthAmt, critDmg*myPlayer.myStats.critAmt);

			if (!isPiercing){

				_rigidbody.velocity = Vector3.zero;

			}

		}

	}
}
