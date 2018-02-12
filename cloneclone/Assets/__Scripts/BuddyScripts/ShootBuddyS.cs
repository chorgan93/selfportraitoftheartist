using UnityEngine;
using System.Collections;

public class ShootBuddyS : BuddyS {

	public GameObject myProjectile;
	public GameObject shootEffect;
	private float effectDir = 1f;

	private EnemyDetectS myEnemyDetect;
	public SimpleEnemyDetectS shootDetect;

	public float shootRate;
	private float shootCountdown;
	public int numShots = 1;
	public float timeBetweenShots = 0.1f;
	private int currentShot = 0;

	public float shotDelay = 0.08f;
	private float shotDelayCountdown = 0f;
	private bool shotTriggered = false;
	public bool lookOnShoot = true;
	public float allowChainTime = 0f;

	private bool chargeButtonUp = true;
	private Vector3 aimDir = Vector3.zero;


	public string chargeAnimatorTrigger;
	public string fireAnimatorTrigger;


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
		myEnemyDetect = playerRef.enemyDetect;
		shootDetect = GetComponentInChildren<SimpleEnemyDetectS>();
	}

	public override void FaceDirection(){

		if (!myEnemyDetect){
			
			myEnemyDetect = playerRef.myDetect;
		}

		if (playerRef.targetEnemy != null && charging){
			Vector3 fScale = transform.localScale;
			if (playerRef.targetEnemy.transform.position.x > transform.position.x){
				fScale.x = sScale;
			}
			
			if (playerRef.targetEnemy.transform.position.x < transform.position.x){
				fScale.x = -sScale;
			}
			transform.localScale = fScale;
			
		}
		else if (playerRef.myDetect.closestEnemy != null && charging){
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
			if (lookOnShoot){
				transform.localScale = fScale;
			}
		}else{
			base.FaceDirection();
		}

	}

	private void ShootControl(){

		if (shotTriggered){
			shotDelayCountdown -= Time.deltaTime;
			if (shotDelayCountdown <= 0){
				FireProjectile();
			}
		}
		else{
			shootCountdown -= Time.deltaTime;

	
			if (!playerRef.talking && !playerRef.myStats.PlayerIsDead()){
	
				if (!playerRef.myControl.GetCustomInput(2)){
					chargeButtonUp = true;
				}else{
					if (chargeButtonUp){
						if (shootCountdown <= allowChainTime && playerRef.myStats.ChargeCheck(costPerUse)){
							myAnimator.SetTrigger(fireAnimatorTrigger);

							if (shotDelay <= 0){
								FireProjectile();
							}else{
								shotDelayCountdown = shotDelay;
								shotTriggered = true;
								canSwitch = false;
							}
						}
						chargeButtonUp = false;

						if (playerRef.tutorialReference != null){
							playerRef.tutorialReference.AddFamiliarAttack();
						}
					}
				}
			}else{
				chargeButtonUp = false;
			}
		}

	}

	private void FireProjectile(){

		if (buddySound){
			Instantiate(buddySound);
		}

		canSwitch = true;
		aimDir = Vector3.zero;


		if (playerRef.targetEnemy != null){
			//Debug.Log("using player target enemy!");
			aimDir.x = playerRef.targetEnemy.transform.position.x - transform.position.x;
			aimDir.y = playerRef.targetEnemy.transform.position.y - transform.position.y;
		}
		else if (playerRef.enemyDetect.closestEnemy != null){
			//Debug.Log("using player closest enemy!");
			aimDir.x = playerRef.enemyDetect.closestEnemy.transform.position.x - transform.position.x;
			aimDir.y = playerRef.enemyDetect.closestEnemy.transform.position.y - transform.position.y;
		}else if (shootDetect.closestEnemy != null){
			//Debug.Log("using my closest enemy!");
			aimDir.x = shootDetect.closestEnemy.transform.position.x - transform.position.x;
			aimDir.y = shootDetect.closestEnemy.transform.position.y - transform.position.y;
		}else if (playerRef.myRigidbody.velocity.x != 0 || playerRef.myRigidbody.velocity.y != 0){
			//Debug.Log("using player velocity!");
			aimDir.x = playerRef.myRigidbody.velocity.x;
			aimDir.y = playerRef.myRigidbody.velocity.y;
		}else{
			//Debug.Log("using my velocity!");
			aimDir.x = myRigid.velocity.x;
			aimDir.y = myRigid.velocity.y;
		}

		GameObject myProj = Instantiate(myProjectile, transform.position, Quaternion.identity)
			as GameObject;
		myProj.transform.position += aimDir.normalized*myProj.GetComponent<BuddyProjectileS>().attackSpawnDistance;

		myProj.GetComponent<BuddyProjectileS>().Fire(aimDir.normalized, this);

		charging = false;
		shootCountdown = shootRate;

		currentShot++;
		if (currentShot >= numShots){
			shotTriggered = false;
			currentShot = 0;
		}else{
			shotDelayCountdown = timeBetweenShots;
		}

		if (shootEffect){
			Vector3 pRotation = myProj.transform.rotation.eulerAngles;
			pRotation.z += 225f*effectDir;
			effectDir *= -1f;
			Instantiate(shootEffect, transform.position, Quaternion.Euler(pRotation));
		}

	}
}
