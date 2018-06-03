using UnityEngine;
using System.Collections;

public class AllyFollowS : MonoBehaviour {

	public PlayerDetectS playerDetect;
	public PlayerDetectS closePlayerDetect;
	private Rigidbody myRigidbody;
	public SpriteRenderer mySprite;
	private Vector3 startSpriteScale;
	private Vector3 currentSpriteScale;

	Vector3 moveForce;

	public float moveSpeed = 1000f;
	public float runMult = 3f;
	public float stopMult = 0.2f;

	private PlayerController myPlayer;

	public string walkKey = "Walk";
	public string runKey = "Run";
	public string idleKey = "Idle";
	private int animState = 0; // 0 = idle, 1 = walk, 2 = run
	private int currentAnimState = -1;
	private Animator myAnimator;

	public float hasBeenInRangeMax = 0.8f;
	private float hasBeenInRangeTime = 0f;

	// Use this for initialization
	void Start () {
	
		myRigidbody = GetComponent<Rigidbody>();
		startSpriteScale = mySprite.transform.localScale;

		myPlayer = GameObject.Find("Player").GetComponent<PlayerController>();
		myAnimator = mySprite.GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update () {
		FaceDirection();
		Animate();
	}

	void FixedUpdate(){
		
		if (!closePlayerDetect.PlayerInRange() || hasBeenInRangeTime > 0){
			moveForce = myPlayer.transform.position-transform.position;
			moveForce.z = 0f;
			moveForce=moveForce.normalized*moveSpeed;
			animState = 1;

			if (closePlayerDetect.PlayerInRange()){
				hasBeenInRangeTime -= Time.deltaTime;
				moveForce*=stopMult;
			}else if (hasBeenInRangeTime <= 0f){
				hasBeenInRangeTime = hasBeenInRangeMax;
			}

			if (!playerDetect.PlayerInRange()){
				moveForce*=runMult;
				animState = 2;
			}
			myRigidbody.AddForce(moveForce*Time.deltaTime, ForceMode.Force);
		}else{
			animState = 0;
		}
	}

	void FaceDirection(){

		currentSpriteScale = startSpriteScale;
		if (!closePlayerDetect.PlayerInRange() || hasBeenInRangeTime > 0){
			if (myRigidbody.velocity.x < 0){
				currentSpriteScale.x *= -1f;
			}
		}else{
			if (transform.position.x > myPlayer.transform.position.x){
				currentSpriteScale.x *= -1f;
			}
		}
		mySprite.transform.localScale = currentSpriteScale;
	}

	void Animate(){
		if (currentAnimState != animState){
			currentAnimState = animState;
			if (animState == 0){
				myAnimator.SetTrigger(idleKey);
			}else if (animState == 1){
				myAnimator.SetTrigger(walkKey);
			}else{
				myAnimator.SetTrigger(runKey);
			}
		}
	}
}
