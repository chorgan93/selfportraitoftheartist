using UnityEngine;
using System.Collections;

public class MeleeBuddyS : BuddyS {

	public GameObject myProjectile;
	public int numShots = 6;

	private EnemyDetectS myEnemyDetect;
	public SimpleEnemyDetectS shootDetect;

	public float lungeSpeed = 1000f;

	public float shootRate;
	private float shootCountdown;

	public float shotDelay = 0.08f;
	private float shotDelayCountdown = 0f;
	private bool shotTriggered = false;

	private bool chargeButtonUp = true;


	public string chargeAnimatorTrigger;
	public string fireAnimatorTrigger;

	private SpriteRenderer myRender;
	private bool flashReset = false;
	public int flashFrames = 8;
	private int flashFramesMax;

	// Use this for initialization
	void Start () {

		Initialize();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		ShootControl();
		FollowPlayer();
		FaceDirection();
	
	}

	public override void Initialize(){

		base.Initialize();

		myEnemyDetect = playerRef.myDetect;
		myRender = GetComponent<SpriteRenderer>();
		flashFramesMax = flashFrames;
		myRender.material.SetFloat("_FlashAmount", 0);
	}

	public override void FaceDirection(){

		if (!myEnemyDetect){
			
			myEnemyDetect = playerRef.myDetect;
		}

		if (playerRef.myLockOn.myEnemy != null && charging){
			Vector3 fScale = transform.localScale;
			if (playerRef.myLockOn.myEnemy.transform.position.x > transform.position.x){
				fScale.x = sScale;
			}
			
			if (playerRef.myLockOn.myEnemy.transform.position.x < transform.position.x){
				fScale.x = -sScale;
			}
			transform.localScale = fScale;
			
		}
		else if (myEnemyDetect.closestEnemy != null && charging){
			Vector3 fScale = transform.localScale;
			if (myEnemyDetect.closestEnemy.transform.position.x > transform.position.x){
				fScale.x = sScale;
			}
			
			if (myEnemyDetect.closestEnemy.transform.position.x < transform.position.x){
				fScale.x = -sScale;
			}
			transform.localScale = fScale;

		}else if (shootCountdown > 0){
			Vector3 fScale = transform.localScale;
			if (myRigid.velocity.x > 0){
				fScale.x = -sScale;
			}
			
			if (myRigid.velocity.x < 0){
				fScale.x = sScale;
			}
			transform.localScale = fScale;
		}else{
			base.FaceDirection();
		}

	}

	private void ShootControl(){

		if (flashReset){
			flashFrames--;
			if (flashFrames <= 0){
				flashReset = false;
				myRender.material.SetFloat("_FlashAmount", 0);
			}
		}

		if (shotTriggered){
			canSwitch = false;
			shotDelayCountdown -= Time.deltaTime;
			if (shotDelayCountdown <= 0 || (shootDetect.closestEnemy != null && shotDelayCountdown < shotDelay*0.5f)){
			
				FireProjectile();
				shotTriggered = false;
				shotDelayCountdown = 0;
			}
		}
		else{
			shootCountdown -= Time.deltaTime;
	
			if (!playerRef.talking && !playerRef.myStats.PlayerIsDead()){
	
				if (!playerRef.myControl.FamiliarControl()){
					chargeButtonUp = true;
				}else{
					if (chargeButtonUp){
						if (shootCountdown <= 0 && playerRef.myStats.ChargeCheck(costPerUse)){
							myAnimator.SetTrigger(fireAnimatorTrigger);
							if (shotDelay <= 0){
								FireProjectile();
							}else{
								shotDelayCountdown = shotDelay;
								shotTriggered = true;
								LungeAtEnemy();
							}
						}
						chargeButtonUp = false;
					}
				}
			}
		}

	}

	private void LungeAtEnemy(){
		
		myAnimator.SetTrigger(chargeAnimatorTrigger);
		if (buddySound){
			Instantiate(buddySound);
		}

		Vector3 aimDir = Vector3.zero;

		if (playerRef.myLockOn.myEnemy != null){
			aimDir.x = playerRef.myLockOn.myEnemy.transform.position.x - transform.position.x;
			aimDir.y = playerRef.myLockOn.myEnemy.transform.position.y - transform.position.y;
		}
		else if (myEnemyDetect.closestEnemy != null){
			aimDir.x = myEnemyDetect.closestEnemy.transform.position.x - transform.position.x;
			aimDir.y = myEnemyDetect.closestEnemy.transform.position.y - transform.position.y;
		}else if (playerRef.myRigidbody.velocity.x != 0 || playerRef.myRigidbody.velocity.y != 0){
			aimDir.x = playerRef.myRigidbody.velocity.x;
			aimDir.y = playerRef.myRigidbody.velocity.y;
		}else{
			aimDir.x = myRigid.velocity.x;
			aimDir.y = myRigid.velocity.y;
		}

		myRigid.velocity = Vector3.zero;
		myRigid.AddForce(aimDir.normalized*Time.deltaTime*lungeSpeed, ForceMode.Impulse);
	}

	private void FireProjectile(){

		canSwitch = true;
		myAnimator.SetTrigger(fireAnimatorTrigger);

		Vector3 aimDir = Vector3.zero;

		myRigid.velocity = Vector3.zero;

		if (playerRef.myLockOn.myEnemy != null){
			aimDir.x = playerRef.myLockOn.myEnemy.transform.position.x - transform.position.x;
			aimDir.y = playerRef.myLockOn.myEnemy.transform.position.y - transform.position.y;
		}
		else if (myEnemyDetect.closestEnemy != null){
			aimDir.x = myEnemyDetect.closestEnemy.transform.position.x - transform.position.x;
			aimDir.y = myEnemyDetect.closestEnemy.transform.position.y - transform.position.y;
		}else if (playerRef.myRigidbody.velocity.x != 0 || playerRef.myRigidbody.velocity.y != 0){
			aimDir.x = playerRef.myRigidbody.velocity.x;
			aimDir.y = playerRef.myRigidbody.velocity.y;
		}else{
			aimDir.x = myRigid.velocity.x;
			aimDir.y = myRigid.velocity.y;
		}

		GameObject myProj;
		for (int i = 0; i < numShots; i++){
		myProj = Instantiate(myProjectile, transform.position, Quaternion.identity)
			as GameObject;
		myProj.transform.position += aimDir.normalized*myProj.GetComponent<BuddyProjectileS>().attackSpawnDistance;

		myProj.GetComponent<BuddyProjectileS>().Fire(aimDir.normalized, this);
		}

		charging = false;
		shootCountdown = shootRate;
		
		flashFrames = flashFramesMax;
		flashReset = true;
		myRender.material.SetFloat("_FlashAmount", 1);


	}
}
