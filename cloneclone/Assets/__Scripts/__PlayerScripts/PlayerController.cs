using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	//_________________________________________CONSTANTS

	private static float DASH_THRESHOLD = 0.24f;
	private static float DASH_RESET_THRESHOLD = 0.15f;
	private static float SMASH_TIME_ALLOW = 0.2f;
	private static float SMASH_MIN_SPEED = 0.042f;
	private static float CHAIN_DASH_THRESHOLD = 0.2f;

	private static float SMASH_THRESHOLD = 0.75f;
	
	//_________________________________________CLASS PROPERTIES

	private PlayerStatsS _myStats;

	private static bool _doWakeUp = true;
	private bool wakingUp = false;
	public bool isWaking  { get { return  wakingUp; } }
	private float wakeUpTime = 3f;
	private float wakeUpCountdown;

	[Header("Movement Variables")]
	public float walkSpeed;
	public float walkSpeedMax;
	private float walkSpeedBlockMult = 0.4f;
	public float runSpeed;
	public float runSpeedMax;
	public float walkThreshold = 0.8f;
	private float maxSpeedStatAdd = 0.3f;
	private bool _isSprinting = false;
	public float sprintMult = 1.4f;
	public float sprintDuration = 1f;
	private float sprintDurationCountdown = 0f;

	[Header("Dash Variables")]
	public float dashSpeed;
	private bool _isDashing;
	public float dashDuration;
	public float dashSlideTime;
	public float dashDragMult;
	public float dashDragSlideMult;
	private float dashDurationTime;
	private float bigDashMult = 1.6f;
	private float speedDashMult = 0.1f;
	private bool preppingSecondDash = false;

	private bool _isShooting;
	private bool _lastInClip;
	private bool _isAiming;

	private bool _shoot4Dir;
	private bool _shoot8Dir = true;
	private Vector3 savedDir = Vector3.zero;

	private bool _doingDashAttack = false;
	public bool doingSpecialAttack { get { return _doingDashAttack; } }

	private Vector2 _inputDirectionLast;
	private Vector2 _inputDirectionCurrent;

	
	//_________________________________________INSTANCE PROPERTIES

	private Rigidbody _myRigidbody;
	private ControlManagerS controller;
	private Camera mainCamera;
	[Header ("Instance Objects")]
	public SpriteRenderer myRenderer;
	public Material damageFlashMat;
	public Material manaFlashMat;
	public Material healFlashMat;
	private Material startMat;
	private Animator _myAnimator;
	private int flashHealFrames;
	private int flashManaFrames;
	private int flashDamageFrames;

	private float startDrag;

	private Vector3 inputDirection;
	private bool blockButtonUp;
	private bool shootButtonUp;
	private bool reloadButtonUp;
	private bool aimButtonUp;
	private bool switchButtonUp;

	// Status Properties
	private bool _isStunned = false;
	private bool attackTriggered;
	private float stunTime;
	private List<GameObject> queuedAttacks;
	private List<float> queuedAttackDelays;
	private bool newAttack = true;

	// Charging Properties
	private bool _chargingAttack;
	private float _chargeAttackTime;
	private float _chargeAttackTrigger = 0.4f;
	private float _chargeAttackDuration = 0.8f;
	private ChargeAttackS _chargeCollider;
	private bool _chargeAttackTriggered = false;
	private bool allowChargeAttack = true;

	// Buddy Properties
	public BuddyS myBuddy;
	public BuddyS altBuddy;

	// Animation Properties
	private bool _facingDown = true;
	private bool _facingUp = true;
	private bool triggerBlockAnimation = true;
	private bool doingBlockTrigger = false;
	private float blockPrepCountdown = 0;
	private float timeInBlock;
	private float blockPrepMax = 0.18f;
		
	// Attack Properties
	public GameObject[] attackChain;
	public GameObject dashAttack;
	private ProjectileS currentAttackS;
	private int currentChain = 0;
	private float comboDuration = 0f;
	private float attackDelay;
	private float attackDuration;

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
	private bool _examining = false;
	private string _overrideExamineString = "";
	private bool _isTalking = false;

	private PlayerSoundS _playerSound;

	
	//_________________________________________GETTERS AND SETTERS

	public bool showBlock		{ get { return _isBlocking; } }
	public bool isBlocking		{get { return _isBlocking || doingBlockTrigger; } }
	public bool doDeflect		{get { return _isBlocking;}}
	public bool isDashing		{get { return _isDashing; } }
	public bool isSprinting		{get { return _isSprinting; } }
	public bool isShooting		{get { return _isShooting; } }
	public bool isStunned		{get { return _isStunned; } }
	public Rigidbody myRigidbody	{ get { return _myRigidbody; } }
	public Animator myAnimator		{ get { return _myAnimator; } }
	public EnemyDetectS myDetect	{ get { return enemyDetect; } }
	public PlayerStatsS myStats		{ get { return _myStats; } }
	public ControlManagerS myControl { get { return controller; } }
	public bool inCombat		{ get { return _inCombat; } }

	public PlayerSoundS playerSound { get { return _playerSound; } }

	public bool facingDown		{ get { return _facingDown; } }
	public bool facingUp		{ get { return _facingUp; } }

	public bool chargingAttack { get { return _chargingAttack;}}

	public bool examining { get { return _examining; } }
	public string overrideExamineString { get { return _overrideExamineString; } }
	public bool talking { get { return _isTalking; } }

	
	//_________________________________________UNITY METHODS

	// Use this for initialization
	void Start () {

		InitializePlayer();
	
	}

	void Update(){
		ManageFlash();

		if (myControl.ControllerAttached()){
			Cursor.visible = false;
		}else{
			Cursor.visible = true;
		}

		if (Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
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
		enemyDetect = GetComponentInChildren<EnemyDetectS>();
		startDrag = _myRigidbody.drag;
		_myAnimator = myRenderer.GetComponent<Animator>();
		startMat = myRenderer.material;
		_playerSound = GetComponent<PlayerSoundS>();

		mainCamera = CameraShakeS.C.GetComponent<Camera>();

		currentChain = -1;
		comboDuration = 0f;

		queuedAttacks = new List<GameObject>();
		queuedAttackDelays = new List<float>();


		_chargeCollider = GetComponentInChildren<ChargeAttackS>();

		muzzleFlare = GetComponentInChildren<MuzzleFlareS>();

		controller = GetComponent<ControlManagerS>();

		inputDirection = new Vector3(1,0,0);

		_inputDirectionLast = new Vector2(0,0);
		_inputDirectionCurrent = new Vector2(0,0);

		if (_doWakeUp){
			TriggerWakeUp();
		}

		currentAttackS = attackChain[0].GetComponent<ProjectileS>();

	}

	void PlayerFixedUpdate(){

		ButtonCheck();
		StatusCheck();

		// Control Methods
		if (!_myStats.PlayerIsDead() && !_isTalking){

			if (_inCombat){
				BlockControl();
				DashControl();
				AttackControl();
				SwapControl();
			}

			MovementControl();
		}

		StickResetCheck();

	}

	void PlayerUpdate(){
		ManageFlash();
	}

	public void EquipBuddy(BuddyS newBud){
		myBuddy = newBud;
	}

	public void Stun(float sTime){

		stunTime = sTime;
		_isStunned = true;

	}

	public void AttackDuration(float aTime){
		attackDuration = aTime;
	}

	public void FlashDamage(){
		flashDamageFrames = 5;
		myRenderer.material = damageFlashMat;
	}

	public void FlashHeal(){
		flashHealFrames = 8;
		myRenderer.material = healFlashMat;
	}

	public void FlashMana(){
		flashManaFrames = 5;
		myRenderer.material = manaFlashMat;
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
				
				_playerSound.SetWalking(true);

				if (Mathf.Abs(moveVelocity.x) <= 0.6f && moveVelocity.y < 0){
					FaceDown();
				}else if (Mathf.Abs(moveVelocity.x) <= 0.6f && moveVelocity.y > 0){
					FaceUp();
				}else{
					FaceLeftRight();
				}

				if (_isBlocking || _chargingAttack){
					moveVelocity *= walkSpeedBlockMult;
					RunAnimationCheck(input2.magnitude*walkSpeedBlockMult);
				}else if (_isSprinting){
					RunAnimationCheck(input2.magnitude * 10f);
				}else{
					RunAnimationCheck(input2.magnitude);
				}
		
				if (moveVelocity.magnitude < walkThreshold){

					//_isSprinting = false;

					float actingWalkSpeed = walkSpeed + walkSpeed*maxSpeedStatAdd*(_myStats.speedAmt-1f)/4f; 

					moveVelocity *= actingWalkSpeed;
					if (_myRigidbody.velocity.magnitude < walkSpeedMax){
						_myRigidbody.AddForce( moveVelocity*Time.deltaTime, ForceMode.Acceleration );
					}
				}
				else{
					
					float actingRunSpeed = runSpeed + runSpeed*maxSpeedStatAdd*(_myStats.speedAmt-1f)/4f; 

					moveVelocity *= actingRunSpeed;

					if (_isSprinting){
						if (_myRigidbody.velocity.magnitude < runSpeedMax*sprintMult){
							_myRigidbody.AddForce( sprintMult*moveVelocity*Time.deltaTime, ForceMode.Acceleration );
						}
					}
					else{
						if (_myRigidbody.velocity.magnitude < runSpeedMax){
							_myRigidbody.AddForce( moveVelocity*Time.deltaTime, ForceMode.Acceleration );
						}
					}
				}
		
			}else{
				RunAnimationCheck(input2.magnitude);
				_playerSound.SetWalking(false);
			}


		}else{
			_playerSound.SetWalking(false);
		}

	}

	private void BlockControl(){

		if (_isBlocking){
			timeInBlock += Time.deltaTime;
		}

		if (BlockInputPressed() && CanInputBlock()){
			if (!_isDashing){
				blockButtonUp = false;
			}
			if (!_isDashing && _myStats.currentDefense > 0 && !_isStunned){
				if (_myStats.ManaCheck(1, false)){
					blockPrepCountdown -= Time.deltaTime;
				}
			if (blockPrepCountdown <= 0 && doingBlockTrigger && _myStats.ManaCheck(1)){
				_isBlocking = true;
					_isSprinting = false;
				doingBlockTrigger = false;
				_myAnimator.SetBool("Blocking", false);
					_playerSound.PlayShieldSound();
				CameraShakeS.C.MicroShake();
					FlashMana();
			}
			if (triggerBlockAnimation && _myStats.ManaCheck(1, false)){
				PrepBlockAnimation();
				triggerBlockAnimation = false;
				doingBlockTrigger = true;
					timeInBlock = 0;
			}
			}

			// shield break
			if (_myStats.currentDefense <= 0){
				TurnOffBlockAnimation();
				
				_myStats.ActivateDefense();
				blockButtonUp = true;
				blockPrepCountdown = blockPrepMax;
				triggerBlockAnimation = true;
				doingBlockTrigger = false;
				timeInBlock = 0;
				_isBlocking = false;
			}
		}else{
			// check for dash tap
			if  (!blockButtonUp && CanInputDash() && _myStats.ManaCheck(1) && !_isSprinting){
				TriggerDash();
			}

			TurnOffBlockAnimation();
			
			_myStats.ActivateDefense();
			blockButtonUp = true;
			blockPrepCountdown = blockPrepMax;
			triggerBlockAnimation = true;
			doingBlockTrigger = false;
			_isBlocking = false;

		}

	}

	private void TriggerDash(){

		_myAnimator.SetBool("Evading", true);
		TurnOffBlockAnimation();
		_myRigidbody.velocity = Vector3.zero;


		FlashMana();

		_playerSound.PlayRollSound();


		if (_myStats.speedAmt >= 5f){
			myRenderer.enabled = false;
		}

		inputDirection = Vector3.zero;
		inputDirection.x = controller.Horizontal();
		inputDirection.y = controller.Vertical();

		
		dashDurationTime = 0;
		
		_myRigidbody.drag = startDrag*dashDragMult;


		if (_isDashing){
			/*_myAnimator.SetTrigger("Dash");
			_myRigidbody.AddForce(inputDirection.normalized*dashSpeed*Time.deltaTime, ForceMode.Impulse);**/
			_myAnimator.SetTrigger("Roll");
			_myRigidbody.AddForce(inputDirection.normalized*dashSpeed*0.6f*Time.deltaTime, ForceMode.Impulse);
			dashDurationTime = dashDuration*0.4f;
		}
		else{
			_myAnimator.SetTrigger("Roll");
			_myRigidbody.AddForce(inputDirection.normalized*dashSpeed*0.6f*Time.deltaTime, ForceMode.Impulse);
			dashDurationTime = dashDuration*0.4f;
			blockButtonUp = true;
		}
		_isDashing = true;

	}

	private void TriggerSprint(){
		CameraShakeS.C.SmallShake();
		_isSprinting = true;
		sprintDurationCountdown = sprintDuration;
		_myAnimator.SetBool("Evading", false);
		_isDashing = false;
		_myRigidbody.drag = startDrag;
		Debug.Log("Trigger sprint!");
	}

	private void DashControl(){

		if (_isDashing){


			// allow for second dash
			if (BlockInputPressed()){
				if (blockButtonUp && ((dashDurationTime >= dashDuration-CHAIN_DASH_THRESHOLD && _myStats.ManaCheck(1)))){
					if ((controller.Horizontal() != 0 || controller.Vertical() != 0)){
						TriggerDash();
					}
				}
				/*else if (blockButtonUp && ((dashDurationTime < dashDuration-CHAIN_DASH_THRESHOLD))){
					if ((controller.Horizontal() != 0 || controller.Vertical() != 0)){
						TriggerSprint();
					}
				}**/
				blockButtonUp = false;
			}
			else{
				blockButtonUp = true;
			}


			dashDurationTime += Time.deltaTime;
			if (dashDurationTime >= dashDuration-dashSlideTime){
				_myRigidbody.drag = startDrag*dashDragSlideMult;
			}

			if (dashDurationTime >= dashDuration){
				
				_myAnimator.SetBool("Evading", false);
				_isDashing = false;
				_myRigidbody.drag = startDrag;

				if (!myRenderer.enabled){
					myRenderer.enabled = true;
				}
			}
		}else{
			preppingSecondDash = false;
		}

		if (_isSprinting){
			sprintDurationCountdown -= Time.deltaTime;
			if (_myRigidbody.velocity.magnitude <= 0.1f || sprintDurationCountdown <= 0f){
				_isSprinting = false;
			}
		}

	}

	private void AttackControl(){

		if (!_isTalking&&!_isBlocking && !_isDashing && !_chargingAttack && !InAttack()){
			comboDuration -= Time.deltaTime;
			if (comboDuration <= 0 && currentChain != 0){
				currentChain = -1;
			}
		}

		if (_chargingAttack && (ShootInputPressed() || _chargeAttackTriggered)){
			_chargeAttackTime+= Time.deltaTime;
			if (!_chargeAttackTriggered && _chargeAttackTime >= _chargeAttackTrigger){
				_chargeAttackTriggered = true;
				_chargeCollider.TriggerAttack(transform.position, ShootDirection());
				_myStats.ManaCheck(3);
				_playerSound.PlayChargeSound();
			}
			if (_chargeAttackTime >= _chargeAttackDuration){
				_chargingAttack = false;
				_chargeAttackTriggered = false;
				_myAnimator.SetBool("Charging", false);
			}

		}
		if (_chargingAttack && !ShootInputPressed() && !_chargeAttackTriggered){
			_chargingAttack = false;
			_myAnimator.SetBool("Charging", false);
			shootButtonUp = false;
			allowChargeAttack = true;
		}

		attackDelay -= Time.deltaTime;
		if (attackDelay <= 0 && attackTriggered){

			GameObject newProjectile;

			if (queuedAttacks.Count > 0){
				newAttack = false;
				newProjectile = Instantiate(queuedAttacks[0], transform.position, Quaternion.identity)
					as GameObject;
				queuedAttacks.RemoveAt(0);
			}
			else{
				newAttack = true;
			if (_doingDashAttack){
				
				newProjectile = (GameObject)Instantiate(dashAttack, 
				                                        transform.position, 
				                                        Quaternion.identity);
			}else{

					currentChain++;
					if (currentChain > attackChain.Length-1){
						currentChain = 0;
					}

				newProjectile = (GameObject)Instantiate(attackChain[currentChain], 
				                                        transform.position, 
				                                        Quaternion.identity);
				
			}
			}

			comboDuration = currentAttackS.comboDuration;

			currentAttackS = newProjectile.GetComponent<ProjectileS>();

			if (newAttack && currentAttackS.numAttacks > 1){
				for (int i = 0; i < currentAttackS.numAttacks - 1; i++){
					if (_doingDashAttack){
						queuedAttacks.Add(dashAttack);
						queuedAttackDelays.Add(currentAttackS.timeBetweenAttacks);
					}else{
						queuedAttacks.Add(attackChain[currentChain]);
						queuedAttackDelays.Add(currentAttackS.timeBetweenAttacks);
					}
				}
			}

			newProjectile.transform.position += savedDir.normalized*currentAttackS.spawnRange;

			if (newAttack){
						currentAttackS.Fire(ShootDirection(),
				                                                  ShootDirectionUnlocked(), this);
			}else{
				currentAttackS.Fire(savedDir,
				                    savedDir, this);
			}


				// subtract mana cost
				_myStats.ManaCheck(1f, newAttack);
				FlashMana();

				if (myRenderer.transform.localScale.x > 0){
					if (!currentAttackS.useAltAnim){
						Vector3 flip = newProjectile.transform.localScale;
						flip.y *= -1f;
						newProjectile.transform.localScale = flip;
					}
				}else{
					if (currentAttackS.useAltAnim){
						Vector3 flip = newProjectile.transform.localScale;
						flip.y *= -1f;
						newProjectile.transform.localScale = flip;
					}
				}

			muzzleFlare.Fire(currentAttackS.rateOfFire, ShootDirection(), newProjectile.transform.localScale.x);

			if (queuedAttackDelays.Count > 0){
				attackDelay = queuedAttackDelays[0];
				queuedAttackDelays.RemoveAt(0);
			}else{
				attackTriggered = false;
			}
		}
		else{
			attackDuration -= Time.deltaTime;

		if (CanInputShoot()){
				if (ShootInputPressed() && StaminaCheck(1f, false) && shootButtonUp){


				shootButtonUp = false;
					_doingDashAttack = false;



					if (_isDashing || _isSprinting){

						currentAttackS = dashAttack.GetComponent<ProjectileS>();

						_isSprinting = false;
						_doingDashAttack = true;

					}else{
						int nextAttack = currentChain+1;
						if (nextAttack > attackChain.Length-1){
							nextAttack = 0;
						}
						currentAttackS = attackChain[nextAttack].GetComponent<ProjectileS>();
					}
					
					attackDelay = currentAttackS.delayShotTime;
					attackTriggered = true;
					_isShooting = true;
					allowChargeAttack = true;

					AttackAnimationTrigger();

			
				}else if (ShootInputPressed() && !shootButtonUp && allowChargeAttack){
					if (_myStats.ManaCheck(1, false)){
					// charge attack
					_chargingAttack = true;
					_chargeAttackTriggered = false;
					_chargeAttackTime = 0;
					_myAnimator.SetBool("Charging", true);
					_myAnimator.SetTrigger("Charge Attack");
					allowChargeAttack = false;
					}else{
						allowChargeAttack = false;
					}
				}
			else{if (attackDuration <= 0){
				_isShooting = false;

						if (_doingDashAttack){
						_doingDashAttack = false;
						}
						TurnOffAttackAnimation();
						
					}

					if (_chargingAttack && (_chargeAttackTime < _chargeAttackTrigger 
					                        || _chargeAttackTime > _chargeAttackDuration)){
						_chargingAttack = false;
						_chargeAttackTime = 0;
						_myAnimator.SetBool("Charging", false);
					}
				}}
		}

	}

	private void SwapControl(){

		if (!myControl.SwitchButton()){
			switchButtonUp = true;
		}

		if (!myStats.PlayerIsDead() && myBuddy.canSwitch && switchButtonUp){
			if (myControl.SwitchButton()){
				BuddyS tempSwap = myBuddy;
				myBuddy = altBuddy;
				myBuddy.transform.position = tempSwap.transform.position;
				myBuddy.gameObject.SetActive(true);
				Instantiate(myBuddy.buddySound);
				altBuddy = tempSwap;
				altBuddy.gameObject.SetActive(false);
			}
		}
		if (myControl.SwitchButton()){
			switchButtonUp = false;
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
		if (SmashInputPressed() && _stickReset){
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

		if (_myStats.PlayerIsDead()){
			_doWakeUp = true;
		}

		if (wakingUp){
			wakeUpCountdown -= Time.deltaTime;
			if (wakeUpCountdown <= 0){
				wakingUp = false;
				_isTalking = false;
			}
		}

		if (_isStunned){
			stunTime -= Time.deltaTime;
			if (stunTime <= 0){
				_isStunned = false;
			}
		}


	}

	private void ManageFlash(){

		if (flashDamageFrames > 0){
			if (myRenderer.material != damageFlashMat){
				myRenderer.material = damageFlashMat;
			}
		}
		else if (flashHealFrames > 0){
			if (myRenderer.material != healFlashMat){
				myRenderer.material = healFlashMat;
			}
		}
		else if (flashManaFrames > 0){
			if (myRenderer.material != manaFlashMat){
				myRenderer.material = manaFlashMat;
			}

		}else{
			if (myRenderer.material != startMat){
				myRenderer.material = startMat;
				if (myRenderer.enabled && _isDashing && _myStats.speedAmt >= 5f){
					myRenderer.enabled = false;
				}
			}
		}

		
		flashDamageFrames--;
		flashManaFrames--;
		flashHealFrames--;

	}

	private void RunAnimationCheck(float inputMagnitude){
		_myAnimator.SetFloat("Speed", inputMagnitude);
		if (inputMagnitude > 0.8f){
			_playerSound.SetRunning(true);
		}else{
			_playerSound.SetRunning(false);
		}
	}

	private void TriggerWakeUp(){
		_isTalking = true;
		wakingUp = true;
		wakeUpCountdown = wakeUpTime;
		_myAnimator.SetTrigger("Wake");
		_doWakeUp = false;
	}

	private void AttackAnimationTrigger(){
		if (_doingDashAttack){
			_myAnimator.SetBool("Attacking", true);
			//_myAnimator.SetBool("Chaining", false);
			_myAnimator.SetTrigger("DashAttack");
			
		}else{
		


			_myAnimator.SetTrigger(currentAttackS.attackAnimationTrigger);
			_myAnimator.SetBool("Attacking", true);
			//_myAnimator.SetBool("Chaining", false);
		
		}
	}

	private void PrepBlockAnimation(){
		
		_myAnimator.SetBool("Attacking", false);
		//_myAnimator.SetBool("Chaining", false);
		_myAnimator.SetBool("Blocking", true);
		_myAnimator.SetLayerWeight(3, 1f);
		FaceLeftRight();
	}
	
	private void TurnOffBlockAnimation(){
		_myAnimator.SetLayerWeight(3, 0f);
		_myAnimator.SetBool("Blocking", false);
	}

	private void TurnOffAttackAnimation(){
		_myAnimator.SetBool("Attacking", false);
		//_myAnimator.SetBool("Chaining", false);
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

		if (!_isDashing && !_isStunned && !_isAiming && attacksRemaining <= 0 && !attackTriggered
		    && !doingBlockTrigger && attackDuration <= 0 && !_chargeAttackTriggered){
			return true;
		}
		else{
			return false;
		}

	}

	private bool CanInputDash(){

		bool dashAllow = false;

		if (blockPrepMax-blockPrepCountdown+timeInBlock < DASH_THRESHOLD && blockPrepCountdown > 0 &&
		    (controller.Horizontal() != 0 || controller.Vertical() != 0) && !_isDashing
		    && !_isStunned && _myStats.currentDefense > 0 && (!_examining || enemyDetect.closestEnemy)){
			dashAllow = true;
		}

		return dashAllow;
	}

	private bool CanInputShoot(){

		if (!doingBlockTrigger && !_isBlocking && !attackTriggered && !_isStunned
		    && attackDuration <= currentAttackS.chainAllow && !_chargingAttack){
			return true;
		}
		else{
			return false;
		}

	}

	private bool CanInputBlock(){
		if ((!_examining || enemyDetect.closestEnemy)){
			return true;
		}else{
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

	private Vector3 ShootDirection(bool moveOverride = false){

		Vector3 inputDirection = Vector3.zero;

		// read left analogue input
		if (Input.GetJoystickNames().Length > 0 || moveOverride){
		inputDirection.x = controller.Horizontal();
		inputDirection.y = controller.Vertical();
		}
		else{
			inputDirection = GetMouseDirection();
		}


		// now check 4/8 directional, if applicable
		if (Mathf.Abs(inputDirection.x) <= 0.04f && Mathf.Abs(inputDirection.y) <= 0.04f){
			inputDirection = savedDir;
		}
		else if (_shoot4Dir && !_isAiming){

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

			//Debug.Log("8 dir!");

			float directionZ = FindDirectionOfVector(inputDirection.normalized);

			if (directionZ > 348.75f || directionZ <= 11.25f){
				inputDirection.x = 1;
				inputDirection.y = 0;
				FaceLeftRight();
				//Debug.Log("Look 1!");
			}
			else if (directionZ > 11.25f && directionZ <= 33.75f){
				inputDirection.x = 1f;
				inputDirection.y = 0.5f;
				FaceLeftRight();
				//Debug.Log("Look 2!");
			}
			else if (directionZ > 33.75f && directionZ <= 56.25f){
				inputDirection.x = 1;
				inputDirection.y = 1;
				FaceLeftRight();
				//Debug.Log("Look 3!");
			}
			else if (directionZ > 56.25f && directionZ <= 78.75f){
				inputDirection.x = 0.5f;
				inputDirection.y = 1;
				FaceUp();
				//Debug.Log("Look 4!");
			}
			else if (directionZ > 78.75f && directionZ <= 101.25f) {
				inputDirection.x = 0;
				inputDirection.y = 1;
				FaceUp();
				//Debug.Log("Look 5!");
			}
			else if (directionZ > 101.25f && directionZ <= 123.75f) {
				inputDirection.x = -0.5f;
				inputDirection.y = 1;
				FaceLeftRight();
				//Debug.Log("Look 6!");
			}
			else if (directionZ > 123.75f && directionZ <= 146.25f) {
				inputDirection.x = -1;
				inputDirection.y = 1;
				FaceLeftRight();
				//Debug.Log("Look 7!");
			}
			else if (directionZ > 146.25f && directionZ <= 168.75f) {
				inputDirection.x = -1;
				inputDirection.y = 0.5f;
				FaceLeftRight();
				//Debug.Log("Look 8!");
			}
			else if (directionZ > 168.75f && directionZ <= 191.25f) {
				inputDirection.x = -1;
				inputDirection.y = 0;
				FaceLeftRight();
				//Debug.Log("Look 9!");
			}
			else if (directionZ > 191.25f && directionZ <= 213.75f) {
				inputDirection.x = -1;
				inputDirection.y = -0.5f;
				FaceLeftRight();
				//Debug.Log("Look 10!");
			}
			else if (directionZ > 213.75f && directionZ <= 236.25f) {
				inputDirection.x = -1;
				inputDirection.y = -1;
				FaceLeftRight();
				//Debug.Log("Look 11!");
			}
			else if (directionZ > 236.25f && directionZ <= 258.75f){
				inputDirection.x = -0.5f;
				inputDirection.y = -1;
				FaceDown();
				//Debug.Log("Look 12!");
			}
			else if (directionZ > 258.75f && directionZ <= 281.25f)  {
				inputDirection.x = 0;
				inputDirection.y = -1;
				FaceDown();
				//Debug.Log("Look 13!");
			}
			else if (directionZ > 281.25f && directionZ <= 303.75f) {
				inputDirection.x = 0.5f;
				inputDirection.y = -1;
				FaceLeftRight();
				//Debug.Log("Look 14!");
			}
			else if (directionZ > 303.75f && directionZ <= 326.25f) {
				inputDirection.x = 1;
				inputDirection.y = -1;
				FaceLeftRight();
				//Debug.Log("Look 15!");
			}
			else{
				inputDirection.x = 1;
				inputDirection.y = -0.5f;
				FaceLeftRight();
				//Debug.Log("Look 16!");
			}

		}

		savedDir = inputDirection.normalized;
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

	private bool SmashInputPressed(){

		if (controller.ControllerAttached()){
		if ((ShootDirectionUnlocked().magnitude > SMASH_THRESHOLD && _stickReset 
		     && Mathf.Abs(_inputDirectionCurrent.magnitude-_inputDirectionLast.magnitude) > SMASH_MIN_SPEED)){

			if (_smashReset <= 0){
				_smashReset = SMASH_TIME_ALLOW;
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

	public bool IsRunning(){
		return (_myAnimator.GetFloat("Speed") > 0.8f);
	}

	public void SetCombat(bool combat){
		_inCombat = combat;
	}

	public void SetExamining(bool nEx, string newExString = ""){
		_examining = nEx;
		_overrideExamineString = newExString;
	}

	public void SetTalking(bool nEx){
		_isTalking = nEx;
		if (_isTalking){
			_myRigidbody.velocity = Vector3.zero;
		}
	}

}
