using UnityEngine;
using System.Collections;

public class HealBuddyS : BuddyS {

	public float healAmount = 3f;

	public float healDelay = 0.08f;
	public float healDuration = 1f;
	private float healCountdown;
	private float healDelayCountdown = 0f;
	private bool healTriggered = false;

	private bool chargeButtonUp = true;
	public GameObject chargeEffect;
	public GameObject healEffect;

	public int flashFrames = 8;
	private int flashFramesMax;

	public string healAnimatorTrigger;
	private SpriteRenderer myRender;
	private bool flashReset = false;

	// Use this for initialization
	void Start () {

		Initialize();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		HealControl();
		FollowPlayer();
		FaceDirection();
	
	}

	public override void Initialize ()
	{
		base.Initialize ();

		myRender = GetComponent<SpriteRenderer>();
		flashFramesMax = flashFrames;
		myRender.material.SetFloat("_FlashAmount", 0);

	}

	public override void FaceDirection(){


		if (healCountdown > 0 || healDelayCountdown > 0){
			Vector3 fScale = transform.localScale;
			if (transform.position.x > playerRef.transform.position.x){
				fScale.x = -sScale;
			}
			
			if (transform.position.x < playerRef.transform.position.x){
				fScale.x = sScale;
			}
			transform.localScale = fScale;
		}else{
			base.FaceDirection();
		}

	}

	private void HealControl(){

		if (flashReset){
			flashFrames--;
			if (flashFrames <= 0){
				flashReset = false;
				myRender.material.SetFloat("_FlashAmount", 0);
			}
		}

		if (healTriggered){
			healDelayCountdown -= Time.deltaTime;
			if (healDelayCountdown <= 0){
				HealPlayer();
				healTriggered = false;
			}
		}
		else{
			healCountdown -= Time.deltaTime;
	
			if (!playerRef.talking && !playerRef.myStats.PlayerIsDead()){
	
				if (!playerRef.myControl.GetCustomInput(2)){
					chargeButtonUp = true;
				}else{
					if (chargeButtonUp){
						if (healCountdown <= 0 && playerRef.myStats.ManaCheck(costPerUse)){

							myAnimator.SetTrigger(healAnimatorTrigger);

							healDelayCountdown = healDelay;
							healTriggered = true;

							Vector3 effectSpawn = transform.position;
							effectSpawn.z += 1f;
							GameObject newSpawn = Instantiate(chargeEffect, effectSpawn, Quaternion.identity)
								as GameObject;
							newSpawn.transform.parent = transform;
							
						}
						chargeButtonUp = false;
					}
				}
			}
		}

	}

	private void HealPlayer(){

		healCountdown = healDuration;

		charging = false;

		playerRef.myStats.Heal(healAmount);

		CameraShakeS.C.LargeShakeCustomDuration(0.3f);
		CameraShakeS.C.TimeSleep(0.1f);

		Vector3 effectSpawn = playerRef.transform.position;
		effectSpawn.z += 1f;
		GameObject newSpawn = Instantiate(healEffect, effectSpawn, Quaternion.identity)
			as GameObject;
		newSpawn.transform.parent = playerRef.transform;

		flashFrames = flashFramesMax;
		flashReset = true;
		myRender.material.SetFloat("_FlashAmount", 1);

	}
}
