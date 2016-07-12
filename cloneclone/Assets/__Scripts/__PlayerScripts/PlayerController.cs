﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//_________________________________________CONSTANTS

	private static float DASH_THRESHOLD = 0.65f;
	private static float DASH_RESET_THRESHOLD = 0.15f;
	private static float SMASH_TIME_ALLOW = 0.2f;
	private static float SMASH_MIN_SPEED = 0.042f;
	private static float ATTACK_CHAIN_TIMING = 0.1f;
	
	//_________________________________________CLASS PROPERTIES

	private PlayerStatsS _myStats;

	[Header("Movement Variables")]
	public float walkSpeed;
	public float walkSpeedMax;
	private float walkSpeedBlockMult = 0.5f;
	public float runSpeed;
	public float runSpeedMax;
	public float walkThreshold = 0.8f;

	[Header("Dash Variables")]
	public float dashSpeed;
	private bool _isDashing;
	public float dashDuration;
	public float dashDragMult;
	private float dashDurationTime;

	private bool _isShooting;
	private bool _lastInClip;
	private bool _isAiming;

	private bool _shoot4Dir;
	private bool _shoot8Dir;

	private Vector2 _inputDirectionLast;
	private Vector2 _inputDirectionCurrent;

	
	//_________________________________________INSTANCE PROPERTIES

	private Rigidbody _myRigidbody;
	private ControlManagerS controller;
	private Camera mainCamera;
	[Header ("Instance Objects")]
	public SpriteRenderer myRenderer;
	private Animator _myAnimator;

	private float startDrag;

	private Vector3 inputDirection;
	private bool blockButtonUp;
	private bool shootButtonUp;
	private bool reloadButtonUp;
	private bool aimButtonUp;

	// Status Properties
	private bool _isStunned = false;
	private bool attackTriggered;
	private float stunTime;

	// Animation Properties
	private bool _facingDown = true;
	private bool _facingUp = true;

	// Weapon Properites
	private GameObject equippedProjectile;
	[Header("Equipment Properties")]
	public GameObject mainProjectile;
	private bool isAutomatic;
	private float rateOfFireMax;
	private float rateOfFire;
	private float reloadTimeMax;
	private float currentReloadMaxTime;
	private float reloadTime;
	private int numberShotsPerAmmo;
	private bool attackDashInterrupt;
	private float spawnRange;
	private float staminaCost;
	private float delayAttackTime;
	private float delayAttackCountdown;
	private float attackDuration = 0;
	private bool useAltAnim = false;

	private Vector3 capturedShootDirection;
	private EnemyDetectS enemyDetect;

	private int numAttacksPerShot;
	private float timeBetweenAttacks;
	private float timeBetweenAttacksCountdown;
	private int attacksRemaining = 0;

	private bool _isBlocking;
	private bool _stickReset = false;
	private float _smashReset = 0;

	private MuzzleFlareS muzzleFlare;

	public bool _inCombat = true;

	
	//_________________________________________GETTERS AND SETTERS
	
	public bool isBlocking		{get { return _isBlocking; } }
	public bool isDashing		{get { return _isDashing; } }
	public bool isShooting		{get { return _isShooting; } }
	public bool isStunned		{get { return _isStunned; } }
	public Rigidbody myRigidbody	{ get { return _myRigidbody; } }
	public Animator myAnimator		{ get { return _myAnimator; } }
	public EnemyDetectS myDetect	{ get { return enemyDetect; } }
	public PlayerStatsS myStats		{ get { return _myStats; } }
	public bool inCombat		{ get { return _inCombat; } }

	public bool facingDown		{ get { return _facingDown; } }
	public bool facingUp		{ get { return _facingUp; } }

	
	//_________________________________________UNITY METHODS

	// Use this for initialization
	void Start () {

		InitializePlayer();
	
	}

	void FixedUpdate () {

		PlayerFixedUpdate();

	}

	//_________________________________________PUBLIC METHODS

	public void Knockback(Vector3 knockbackForce, float knockbackTime, bool attackTime = false){

		myRigidbody.AddForce(knockbackForce, ForceMode.Impulse);

		if (!attackTime){
			Stun(knockbackTime);
		}
		else{
			AttackDuration(knockbackTime);
		}

	}

	//_________________________________________PRIVATE METHODS

	void InitializePlayer(){

		_myRigidbody = GetComponent<Rigidbody>();
		startDrag = _myRigidbody.drag;
		_myAnimator = myRenderer.GetComponent<Animator>();

		mainCamera = CameraShakeS.C.GetComponent<Camera>();

		SetWeapon();


		muzzleFlare = GetComponentInChildren<MuzzleFlareS>();

		controller = GetComponent<ControlManagerS>();

		inputDirection = new Vector3(1,0,0);

		_inputDirectionLast = new Vector2(0,0);
		_inputDirectionCurrent = new Vector2(0,0);

	}

	void PlayerFixedUpdate(){

		ButtonCheck();
		StatusCheck();

		// Control Methods
		if (!_myStats.PlayerIsDead()){

			if (_inCombat){
				BlockControl();
				DashControl();
				ShootControl();
			}

			MovementControl();
		}

		StickResetCheck();

	}

	private void SetWeapon(){

		// for initializing projectile without changing main/alt
		equippedProjectile = mainProjectile;
		ProjectileS newProjectileStats = equippedProjectile.GetComponent<ProjectileS>();

		isAutomatic = newProjectileStats.isAutomatic;
		rateOfFireMax = newProjectileStats.rateOfFire;
		reloadTimeMax = newProjectileStats.reloadTime;
		numberShotsPerAmmo = newProjectileStats.numShots;
		attackDashInterrupt = newProjectileStats.canInterruptDash;
		_shoot4Dir = newProjectileStats.lock4Directional;
		_shoot8Dir = newProjectileStats.lock8Directional;
		spawnRange = newProjectileStats.spawnRange;
		staminaCost = newProjectileStats.staminaCost;
		numAttacksPerShot = newProjectileStats.numAttacks;
		timeBetweenAttacks = newProjectileStats.numTimeBetweenAttacks;
		delayAttackTime = newProjectileStats.delayShotTime;

	}

	public void Stun(float sTime){

		stunTime = sTime;
		_isStunned = true;

	}

	public void AttackDuration(float aTime){
		attackDuration = aTime;
	}

	//_________________________________________CONTROL METHODS
	private void MovementControl(){

		if (CanInputMovement()){
			Vector2 input2 = Vector2.zero;
			input2.x = controller.Horizontal();
			input2.y = controller.Vertical();

	
			Vector3 moveVelocity = _myRigidbody.velocity;
	
			if (input2.x != 0 || input2.y != 0){
				moveVelocity.x = inputDirection.x = input2.x;
				moveVelocity.y = inputDirection.y = input2.y;

				if (Mathf.Abs(moveVelocity.x) <= 0.6f && moveVelocity.y < 0){
					FaceDown();
				}else if (Mathf.Abs(moveVelocity.x) <= 0.6f && moveVelocity.y > 0){
					FaceUp();
				}else{
					FaceLeftRight();
				}

				if (_isBlocking){
					moveVelocity *= walkSpeedBlockMult;
					RunAnimationCheck(input2.magnitude*walkSpeedBlockMult);
				}else{
					RunAnimationCheck(input2.magnitude);
				}
		
				if (moveVelocity.magnitude < walkThreshold){
					moveVelocity *= walkSpeed;
					if (_myRigidbody.velocity.magnitude < walkSpeedMax){
						_myRigidbody.AddForce( moveVelocity*Time.deltaTime, ForceMode.Acceleration );
					}
				}
				else{
					moveVelocity *= runSpeed;
					if (_myRigidbody.velocity.magnitude < runSpeedMax){
						_myRigidbody.AddForce( moveVelocity*Time.deltaTime, ForceMode.Acceleration );
					}
				}
		
			}else{
				RunAnimationCheck(input2.magnitude);
			}
		}

	}

	private void BlockControl(){

		if (BlockInputPressed() && !_isDashing && _myStats.currentMana > 0){
			_isBlocking = true;
			PrepBlockAnimation();
		}else{
			_isBlocking = false;
		}

	}

	private void DashControl(){

		if (CanInputDash() && (DashInputPressed() || _smashReset > 0) && StaminaCheck(1) && _isBlocking){

			_isDashing = true;
			_myAnimator.SetTrigger("Dash");
			_myAnimator.SetBool("Evading", true);
			_myRigidbody.velocity = Vector3.zero;
			
			if (controller.Horizontal() != 0 || controller.Vertical() != 0){
				inputDirection = Vector3.zero;
				inputDirection.x = controller.Horizontal();
				inputDirection.y = controller.Vertical();
			}

				dashDurationTime = 0;
				
				_myRigidbody.drag = startDrag*dashDragMult;
				_myRigidbody.AddForce(inputDirection.normalized*dashSpeed*Time.deltaTime, ForceMode.Impulse);
			
			_smashReset =0;

		}

		if (_isDashing){

			dashDurationTime += Time.deltaTime;

			//if (dashDurationTime >= dashDuration || (dashDurationTime >= triggerSprintMult*dashDuration && !blockButtonUp)){
			if (dashDurationTime >= dashDuration){
				// check if holding sprint, else end dash
				if (!blockButtonUp){
					// _isSprinting = true;
				}
				
				_myAnimator.SetBool("Evading", false);
				_isDashing = false;
				_myRigidbody.drag = startDrag;
			}
		}

	}

	private void ShootControl(){

		delayAttackCountdown -= Time.deltaTime;
		if (delayAttackCountdown <= 0 && attackTriggered){
			for(int i = 0; i < numberShotsPerAmmo; i++){
				GameObject newProjectile = (GameObject)Instantiate(equippedProjectile, transform.position+ShootDirection()*spawnRange, Quaternion.identity);
				if (i == 0){
					newProjectile.GetComponent<ProjectileS>().Fire(ShootDirection(), ShootDirectionUnlocked(), this, (DashInputPressed() || _smashReset > 0));
				}
				else{
					newProjectile.GetComponent<ProjectileS>().Fire(ShootDirection(), ShootDirectionUnlocked(), this, (DashInputPressed() || _smashReset > 0), false);
				}

				// subtract mana cost
				_myStats.ManaCheck(staminaCost);

				if (myRenderer.transform.localScale.x > 0){
					if (!useAltAnim){
						Vector3 flip = newProjectile.transform.localScale;
						flip.y *= -1f;
						newProjectile.transform.localScale = flip;
					}
				}else{
					if (useAltAnim){
						Vector3 flip = newProjectile.transform.localScale;
						flip.y *= -1f;
						newProjectile.transform.localScale = flip;
					}
				}
			}
			muzzleFlare.Fire(rateOfFireMax, ShootDirection(), equippedProjectile.transform.localScale.x);
			rateOfFire = rateOfFireMax;
			
			if (_lastInClip){
				_lastInClip = false;
			}
			
			if (numAttacksPerShot > 1){
				capturedShootDirection = ShootDirection();
				attacksRemaining = numAttacksPerShot-1;
				timeBetweenAttacksCountdown = timeBetweenAttacks;
			}
			attackTriggered = false;
		}
		else if (attacksRemaining > 0){
			// for multi attacks
			timeBetweenAttacksCountdown -= Time.deltaTime;
			if (timeBetweenAttacksCountdown <= 0){
				for(int i = 0; i < numberShotsPerAmmo; i++){
					GameObject newProjectile = (GameObject)Instantiate(equippedProjectile, transform.position+capturedShootDirection*spawnRange, Quaternion.identity);
					if (i == 0){
						newProjectile.GetComponent<ProjectileS>().Fire(capturedShootDirection, ShootDirectionUnlocked(), this, false);
					}
					else{
						newProjectile.GetComponent<ProjectileS>().Fire(capturedShootDirection, ShootDirectionUnlocked(), this, false, false);
					}
				}
				muzzleFlare.Fire(rateOfFireMax, capturedShootDirection, equippedProjectile.transform.localScale.x);
				rateOfFire = rateOfFireMax;
				attacksRemaining--;
				if (attacksRemaining > 0){
					timeBetweenAttacksCountdown = timeBetweenAttacks;
				}
			}

		}
		else{
			attackDuration -= Time.deltaTime;
		// once roF is less than 0, allow shooting
		if (CanInputShoot()){
				if (ShootInputPressed() && StaminaCheck(staminaCost, false) && shootButtonUp){


				shootButtonUp = false;
					
					delayAttackCountdown = delayAttackTime;
					attackTriggered = true;
					_isShooting = true;

					AttackAnimationTrigger();

			
				}
			else{
				_isShooting = false;
					TurnOffAttackAnimation();
				}}
		}

	}

	private bool StaminaCheck(float cost, bool takeAway = true){

		return _myStats.ManaCheck(cost, takeAway);

	}

	//___________________________________________VARIABLE CHECKS
	private void ButtonCheck(){

		_inputDirectionLast = _inputDirectionCurrent;
		_inputDirectionCurrent.x = controller.Horizontal();
		_inputDirectionCurrent.y = controller.Vertical();

		if (!controller.ShootButton() && !controller.ShootTrigger()){
			shootButtonUp = true;
		}


	}

	private void StickResetCheck(){

		
		_smashReset -= Time.deltaTime;
		if (DashInputPressed() && _stickReset){
			_smashReset = SMASH_TIME_ALLOW;
		}

		if (ShootDirectionUnlocked().magnitude < DASH_RESET_THRESHOLD){
			_stickReset = true;
		}
		else{
			_stickReset = false;
		}


	}


	private void StatusCheck(){

		if (_isStunned){
			stunTime -= Time.deltaTime;
			if (stunTime <= 0){
				_isStunned = false;
			}
		}


	}

	private void RunAnimationCheck(float inputMagnitude){
		_myAnimator.SetFloat("Speed", inputMagnitude);
	}

	private void AttackAnimationTrigger(){
		if (useAltAnim){
			_myAnimator.SetTrigger("Attack2");
		}else{
			_myAnimator.SetTrigger("Attack1");
		}
		useAltAnim = !useAltAnim;
		if (_myAnimator.GetBool("Attacking")){
			_myAnimator.SetBool("Chaining", true);
			delayAttackCountdown = 0f;
		}else{
			_myAnimator.SetBool("Attacking", true);
			_myAnimator.SetBool("Chaining", false);
		}
	}

	private void PrepBlockAnimation(){
		
		_myAnimator.SetBool("Attacking", false);
		_myAnimator.SetBool("Chaining", false);
		FaceLeftRight();
	}

	private void TurnOffAttackAnimation(){
		_myAnimator.SetBool("Attacking", false);
		_myAnimator.SetBool("Chaining", false);
	}

	private void FaceDown(){
		if (!_isBlocking){
		_myAnimator.SetLayerWeight(1, 1f);
		_myAnimator.SetLayerWeight(2, 0f);
		_facingDown = true;
		_facingUp = false;
		}

	}
	private void FaceUp(){
		if (!_isBlocking){
		_myAnimator.SetLayerWeight(2, 1f);
		_facingUp = true;
		_facingDown = false;
		}
		
	}
	private void FaceLeftRight(){
		_myAnimator.SetLayerWeight(1, 0f);
		_myAnimator.SetLayerWeight(2, 0f);
		_facingDown = false;
		_facingUp = false;
	}

	private bool CanInputMovement(){

		if (!_isDashing && !_isStunned && !_isAiming && attacksRemaining <= 0 && !attackTriggered){
			return true;
		}
		else{
			return false;
		}

	}

	private bool CanInputDash(){

		bool dashAllow = false;

		/*
		if (_isBlocking && !_isDashing && !_isStunned){
			dashAllow = true;
		}
		else{
			dashAllow = false;
		}*/

		return dashAllow;
	}

	private bool CanInputShoot(){

		if ((!_isDashing || (_isDashing && attackDashInterrupt)) && !_isBlocking && !attackTriggered && !_isStunned
		    && attackDuration <= ATTACK_CHAIN_TIMING){
			return true;
		}
		else{
			return false;
		}

	}

	private Vector3 ShootDirectionUnlocked(){

		Vector3 inputDirection = Vector3.zero;

			
		inputDirection.x = controller.Horizontal();
		inputDirection.y = controller.Vertical();

		
		return inputDirection.normalized;

	}

	private Vector3 GetMouseDirection(){

		Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = transform.position.z;

		return (mousePosition-transform.position).normalized;

	}

	private Vector3 ShootDirection(){

		Vector3 inputDirection = Vector3.zero;

		// read left analogue input
		if (Input.GetJoystickNames().Length > 0){
		inputDirection.x = controller.Horizontal();
		inputDirection.y = controller.Vertical();
		}
		else{
			inputDirection = GetMouseDirection();
		}


		// now check 4/8 directional, if applicable
		if (_shoot4Dir && !_isAiming){

			float directionZ = FindDirectionOfVector(inputDirection.normalized);


			if (directionZ > 315 || directionZ <= 45){
				inputDirection.x = 1;
				inputDirection.y = 0;
			}
			else if (directionZ > 45 && directionZ <= 135){
				inputDirection.x = 0;
				inputDirection.y = 1;
			}
			else if (directionZ > 135 && directionZ <= 225){
				inputDirection.x = -1;
				inputDirection.y = 0;
			}
			else {
				inputDirection.x = 0;
				inputDirection.y = -1;
			}

		}
		else if (_shoot8Dir || _isAiming){

			float directionZ = FindDirectionOfVector(inputDirection.normalized);

			if (directionZ > 337.5f || directionZ <= 22.5f){
				inputDirection.x = 1;
				inputDirection.y = 0;
				FaceLeftRight();
			}
			else if (directionZ > 22.5f && directionZ <= 67.5f){
				inputDirection.x = 1;
				inputDirection.y = 1;
				FaceLeftRight();
			}
			else if (directionZ > 67.5f && directionZ <= 112.5f){
				inputDirection.x = 0;
				inputDirection.y = 1;
				FaceLeftRight();
			}
			else if (directionZ > 112.5f && directionZ <= 157.5f){
				inputDirection.x = -1;
				inputDirection.y = 1;
				FaceLeftRight();
			}
			else if (directionZ > 157.5f && directionZ <= 202.5f){
				inputDirection.x = -1;
				inputDirection.y = 0;
				FaceLeftRight();
			}
			else if (directionZ > 202.5f && directionZ <= 247.5f){
				inputDirection.x = -1;
				inputDirection.y = -1;
				FaceDown();
			}
			else if (directionZ > 247.5f && directionZ <= 292.5f){
				inputDirection.x = 0;
				inputDirection.y = -1;
				FaceDown();
			}
			else {
				inputDirection.x = 1;
				inputDirection.y = -1;
				FaceDown();
			}

		}

		return inputDirection.normalized;
		

	}

	private Vector3 ShootDirectionAssisted(){

		if (enemyDetect.closestEnemy != null){
			return (enemyDetect.closestEnemy.transform.position - transform.position).normalized;
		}
		else{
			return ShootDirection();
		}

	}

	private bool DashInputPressed(){

		if (controller.ControllerAttached()){
		if ((ShootDirectionUnlocked().magnitude > DASH_THRESHOLD && _stickReset 
		     && Mathf.Abs(_inputDirectionCurrent.magnitude-_inputDirectionLast.magnitude) > SMASH_MIN_SPEED)){

			if (_smashReset <= 0){
				//_smashReset = SMASH_TIME_ALLOW;
			}

			return true;
		}else{
			return false;
		}
		}
		else{
			if (controller.DashKey()){
				return true;
			}else{
				return false;
			}
		}

	}

	private bool BlockInputPressed(){

		return (controller.BlockButton() || controller.BlockTrigger());

	}

	private bool ShootInputPressed(){
		return (controller.ShootButton() || controller.ShootTrigger());
	}
	

	private bool ReloadInputPressed(){

		return (controller.ReloadButton());

	}

	private float FindDirectionOfVector(Vector3 direction){
		
		float rotateZ = 0;
		
		Vector3 targetDir = direction.normalized;
		
		if(targetDir.x == 0){
			if (targetDir.y > 0){
				rotateZ = 90;
			}
			else{
				rotateZ = 270;
			}
		}
		else{
			rotateZ = Mathf.Rad2Deg*Mathf.Atan((targetDir.y/targetDir.x));
		}	
		
		
		if (targetDir.x < 0){
			rotateZ += 180;
		}

		if (rotateZ < 0){
			rotateZ += 360f;
		}
		
		return(rotateZ);
		
	}

	//_______________________________________PUBLIC VARIABLE METHODS


	public void SetStatReference(PlayerStatsS stat){
		_myStats = stat;
	}

	public void SetDetect(EnemyDetectS newDetect){

		enemyDetect = newDetect;

	}

	public bool InAttack(){
		if (attackTriggered || attackDuration > 0){
			return true;
		}
		else{
			return false;
		}
	}

	public Vector3 ShootPosition(){
		return (ShootDirectionUnlocked());
	}

	public void SetCombat(bool combat){
		_inCombat = combat;
	}
}
