using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	//_________________________________________CONSTANTS

	private static float SPRINT_THRESHOLD = 0.66f;
	private static float DASH_THRESHOLD = 0.34f;
	private static float DASH_RESET_THRESHOLD = 0.15f;
	private static float SMASH_TIME_ALLOW = 0.2f;
	private static float SMASH_MIN_SPEED = 0.042f;
	private static float CHAIN_DASH_THRESHOLD = 0.2f;

	private static float SMASH_THRESHOLD = 0.75f;
	
	//_________________________________________CLASS PROPERTIES

	private PlayerStatsS _myStats;

	public static bool doWakeUp = true;
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
	private bool _isDoingMovement = false;
	public bool isDoingMovement { get { return _isDoingMovement; } }

	[Header("Dash Variables")]
	public float dashSpeed;
	private bool _isDashing;
	public float dashDuration;
	public float dashSlideTime;
	public float dashDragMult;
	public float dashDragSlideMult;
	private float dashDurationTime;
	private float dashDurationTimeMax;
	private float dashCooldown = 0.4f;
	private float dashCooldownMax = 0.2f;
	private float dashHoldTime = 0f;
	private float bigDashMult = 1.6f;
	private float speedDashMult = 0.1f;
	private float _dashCost = 2f;
	private float _dodgeCost = 1.75f;
	public GameObject dashObj;

	private bool _isShooting;
	private bool _lastInClip;
	private bool _isAiming;
	private bool _canSwap;

	private bool _shoot4Dir;
	private bool _shoot8Dir = true;
	private Vector3 savedDir = Vector3.zero;

	private bool _doingDashAttack = false;
	private bool _doingHeavyAttack = false;
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
	public Material chargeFlashMat;
	private Material startMat;
	private Animator _myAnimator;
	private int flashHealFrames;
	private int flashManaFrames;
	private int flashChargeFrames;
	private int flashDamageFrames;

	private float startDrag;

	private Vector3 inputDirection;
	private bool dashButtonUp = true;
	private bool blockButtonUp;
	private bool shootButtonUp;
	private bool reloadButtonUp;
	private bool aimButtonUp;
	private bool switchButtonUp;
	private bool switchBuddyButtonUp;
	private bool lockInputReset = false;
	private float lockDownTime = 0f;
	private float lockTurnOffThreshold = 0.5f;

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
	private float _chargeAttackTrigger = 0.6f;
	private float _chargeAttackDuration = 1f;
	private string _chargeAnimationTrigger;
	private float _chargeAnimationSpeed;
	//private ChargeAttackS _chargeCollider;
	private GameObject _chargePrefab;
	private bool _chargeAttackTriggered = false;
	private bool allowChargeAttack = true;
	private float _chargeAttackCost = 5f;

	// Buddy Properties
	private BuddyS _myBuddy;
	private BuddyS _altBuddy;
	private bool altBuddyCreated = false;
	[Header("Buddy Properties")]
	public List<GameObject> equippedBuddies;
	public Transform buddyPos;
	public Transform buddyPosLower;
	private BuddySwitchEffectS _buddyEffect;

	// Virtue Properties
	public List<int> equippedVirtues;

	// Animation Properties
	private bool _facingDown = true;
	private bool _facingUp = true;
	private bool triggerBlockAnimation = true;
	private bool doingBlockTrigger = false;
	private float blockPrepCountdown = 0;
	private float timeInBlock;
	private float blockPrepMax = 0.18f;
		
	// Attack Properties
	//public GameObject[] attackChain;
	//public GameObject dashAttack;
	private PlayerWeaponS equippedWeapon;
	public PlayerWeaponS getEWeapon { get { return equippedWeapon; } }
	public List<PlayerWeaponS> equippedWeapons;
	public List<PlayerWeaponS> subWeapons;
	private WeaponSwitchFlashS weaponSwitchIndicator;
	public static int _currentParadigm = 0;
	public int currentParadigm { get { return _currentParadigm; } }
	private static int _subParadigm = 1;
	public int subParadigm { get { return _subParadigm; } }
	private static int currentBuddy = 0;
	private static int subBuddy = 1;
	private ProjectileS currentAttackS;
	private int currentChain = 0;
	private int prevChain = 0;
	private float comboDuration = 0f;
	private float attackDelay;
	private float attackDuration;
	public GameObject attackEffectObj;

	private Vector3 capturedShootDirection;
	private EnemyDetectS enemyDetect;
	public EnemyDetectS superCloseEnemyDetect;

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
	private float delayTurnOffTalk;
	private bool delayTalkTriggered = false;

	private bool _usingItem = false;
	public bool usingitem { get { return _usingItem; } }
	private float usingItemTime = 0f;
	private float usingItemTimeMax = 0.8f;

	private PlayerSoundS _playerSound;
	private PlayerAugmentsS _playerAug;
	public PlayerAugmentsS playerAug { get { return _playerAug; } }
	private LockOnS _myLockOn;
	private bool _lockButtonDown = false;
	public LockOnS myLockOn { get { return _myLockOn; } }

	private BlockDisplay3DS _blockRef;
	//private FlashEffectS _specialFlash;
	private CombatManagerS _currentCombatManager;
	public CombatManagerS currentCombatManager { get { return _currentCombatManager; } }

	//_________________________________________AUGMENT-SPECIFIC

	private float staggerBonusTimeMax = 1f;
	private float staggerBonusTime;

	
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
	public BuddyS myBuddy { get { return _myBuddy; } }

	public bool facingDown		{ get { return _facingDown; } }
	public bool facingUp		{ get { return _facingUp; } }

	public bool chargingAttack { get { return _chargingAttack;}}

	public bool examining { get { return _examining; } }
	public string overrideExamineString { get { return _overrideExamineString; } }
	public bool talking { get { return _isTalking; } }

	
	//_________________________________________UNITY METHODS

	void Awake(){

		CinematicHandlerS.inCutscene = false;

	}

	// Use this for initialization
	void Start () {

		InitializePlayer();
	
	}

	void Update(){
		ManageFlash();
		ManageAugments();

		if (myControl.ControllerAttached()){
			Cursor.visible = false;
		}else{
			Cursor.visible = true;
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

		PlayerInventoryS.I.dManager.SpawnBlood();
		//_specialFlash = CameraEffectsS.E.specialFlash;

		weaponSwitchIndicator = GetComponentInChildren<WeaponSwitchFlashS>();

		mainCamera = CameraShakeS.C.GetComponent<Camera>();

		if (PlayerInventoryS.I.EquippedWeapons() != null){
		equippedWeapons = PlayerInventoryS.I.EquippedWeapons();
			subWeapons = PlayerInventoryS.I.SubWeapons();
		}
		if (PlayerInventoryS.I.EquippedBuddies() != null){
			equippedBuddies = PlayerInventoryS.I.EquippedBuddies();
		}

		equippedWeapon = equippedWeapons[_currentParadigm];

		
		_playerAug = GetComponent<PlayerAugmentsS>();
		_playerAug.SetPlayerRef(this);

		if (_blockRef){
			_blockRef.ChangeColors(equippedWeapon.swapColor);
		}

		GameObject startBuddy = Instantiate(equippedBuddies[currentBuddy], transform.position, Quaternion.identity)
			as GameObject;
		startBuddy.transform.parent = transform;
		_myBuddy = startBuddy.gameObject.GetComponent<BuddyS>();
		_myBuddy.SetPositions(buddyPos, buddyPosLower);
		_myBuddy.gameObject.SetActive(true);
		_myAnimator.SetInteger("WeaponNumber", equippedWeapon.weaponNum);

		_buddyEffect = GetComponentInChildren<BuddySwitchEffectS>();

		currentChain = -1;
		comboDuration = 0f;

		queuedAttacks = new List<GameObject>();
		queuedAttackDelays = new List<float>();



		muzzleFlare = GetComponentInChildren<MuzzleFlareS>();

		controller = GetComponent<ControlManagerS>();

		inputDirection = new Vector3(1,0,0);

		_inputDirectionLast = new Vector2(0,0);
		_inputDirectionCurrent = new Vector2(0,0);

		if (doWakeUp){
			TriggerWakeUp();
		}

		currentAttackS = equippedWeapon.attackChain[0].GetComponent<ProjectileS>();
		myRenderer.color = equippedWeapon.swapColor;

	}

	void PlayerFixedUpdate(){

		ButtonCheck();
		StatusCheck();

		// Control Methods
		if (!_myStats.PlayerIsDead() && !_isTalking){

			//if (_inCombat){
				//LockOnControl();
				SwapControl();
				BlockControl();
				DashControl();
				AttackControl();
			//}

			MovementControl();
		}

		StickResetCheck();

	}

	void PlayerUpdate(){
		ManageFlash();
	}


	public void EquipBuddy(BuddyS newBud){
		_myBuddy = newBud;
	}

	public void Stun(float sTime){

		stunTime = sTime;
		_isStunned = true;
		CancelAttack();

	}

	private void CancelAttack(){
		attackTriggered = false;
		attackDuration = 0f;
		currentChain = 0;
		_chargingAttack = false;
		allowChargeAttack = false;
		_chargeAttackTriggered = false;
		_chargeAttackTime = 0f;
		_myAnimator.SetBool("Charging", false);
		TurnOffAttackAnimation();
	}

	public void AttackDuration(float aTime){
		attackDuration = aTime;
		if (_playerAug.animaAug){
			attackDuration*=PlayerAugmentsS.animaAugAmt;
		}
	}

	public void FlashDamage(){
		flashDamageFrames = 5;
		myRenderer.material = damageFlashMat;
	}

	public void FlashHeal(){
		flashHealFrames = 8;
		myRenderer.material = healFlashMat;
		VignetteEffectS.V.Flash(healFlashMat.color);
	}

	public void FlashMana(bool doEffect = false){
		flashManaFrames = 5;
		myRenderer.material = manaFlashMat;
		if (doEffect){
			VignetteEffectS.V.Flash(manaFlashMat.color);
		}
	}
	public void FlashCharge(){
		flashChargeFrames = 5;
		myRenderer.material = chargeFlashMat;
		VignetteEffectS.V.Flash(chargeFlashMat.color);
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
				_isDoingMovement = true;

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

					_isSprinting = false;


					float actingWalkSpeed = walkSpeed*equippedWeapon.speedMult; 

					moveVelocity *= actingWalkSpeed;
					if (_myRigidbody.velocity.magnitude < walkSpeedMax){
						_myRigidbody.AddForce( moveVelocity*Time.deltaTime, ForceMode.Acceleration );
					}
				}
				else{
				
					float actingRunSpeed = runSpeed*equippedWeapon.speedMult; 

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
				_isDoingMovement = false;
			}


		}else{
			_playerSound.SetWalking(false);
			_isDoingMovement = false;
		}

	}

	private void BlockControl(){

		if (_isBlocking){
			timeInBlock += Time.deltaTime;
			dashHoldTime = 0f;
		}

		//if (BlockInputPressed() && CanInputBlock()){
		if (controller.BlockTrigger() && CanInputBlock()){
			if (!_isDashing){
				blockButtonUp = false;
			}
			if (!_isDashing && _myStats.currentDefense > 0 && !_isStunned){
				if (_myStats.ManaCheck(1, false)){
					blockPrepCountdown -= Time.deltaTime;
				}
			if (blockPrepCountdown <= 0 && doingBlockTrigger && _myStats.ManaCheck(1)){
				_isBlocking = true;
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
		dashHoldTime = DASH_THRESHOLD;
		
		_myRigidbody.drag = startDrag*dashDragMult;

		// if you want to differentiate bt lock on and not, uncomment this and remove solo roll code
		/*if (!myControl.LockOnButton()){
			_myStats.ManaCheck(_dashCost);
			_myAnimator.SetTrigger("Dash");
			_myRigidbody.AddForce(inputDirection.normalized*dashSpeed*Time.deltaTime, ForceMode.Impulse);
			dashDurationTime = dashDuration*0.4f;
		}
		else{
			_myStats.ManaCheck(_dodgeCost);
			_myAnimator.SetTrigger("Roll");
			_myRigidbody.AddForce(inputDirection.normalized*dashSpeed*0.6f*Time.deltaTime, ForceMode.Impulse);
			dashDurationTime = dashDuration*0.4f;
		}**/

		if (_playerAug.dashAug){
			_myStats.ManaCheck(_dodgeCost);
			_myAnimator.SetTrigger("Dash");
			_myRigidbody.AddForce(inputDirection.normalized*dashSpeed*0.9f*Time.deltaTime, ForceMode.Impulse);
			dashDurationTimeMax = dashDuration*0.96f;
		}else{
			_myStats.ManaCheck(_dodgeCost);
			_myAnimator.SetTrigger("Roll");
			_myRigidbody.AddForce(inputDirection.normalized*dashSpeed*0.6f*Time.deltaTime, ForceMode.Impulse);
			dashDurationTimeMax = dashDuration*0.4f;
		}

		dashCooldown = dashCooldownMax;

		if (!_isDashing){
			blockButtonUp = true;
			_isDashing = true;
		}

		SpawnDashPuff();

	}

	private void TriggerSprint(){
		//CameraShakeS.C.SmallShake();
		_isSprinting = true;
		sprintDurationCountdown = sprintDuration;
		_myAnimator.SetBool("Evading", false);
		_isDashing = false;
		_myRigidbody.drag = startDrag;
	}

	private void DashControl(){

		//___________________________________________DASH VERSION WITHOUT SPRINTING
		//control for first dash

		if (!_isDashing){
			dashCooldown -= Time.deltaTime;
			if (myControl.DashTrigger() && dashButtonUp && CanInputDash() && _myStats.ManaCheck(1, false) &&
			    (controller.Horizontal() != 0 || controller.Vertical() != 0)){
					
				TriggerDash();
				dashButtonUp = false;
			}
		}
			
		
		else{
			
			// allow for second dash
			if (controller.DashTrigger()){
				if (dashButtonUp && ((dashDurationTime >= dashDurationTimeMax-CHAIN_DASH_THRESHOLD) 
				                     && CanInputDash() && _myStats.ManaCheck(1, false))){
					if ((controller.Horizontal() != 0 || controller.Vertical() != 0)){
						TriggerDash();
					}
				}
				dashButtonUp = false;
			}
			
			
			dashDurationTime += Time.deltaTime;
			if (dashDurationTime >= dashDurationTimeMax-dashSlideTime){
				_myRigidbody.drag = startDrag*dashDragSlideMult;
			}
			
			if (dashDurationTime >= dashDurationTimeMax){
				
				_myAnimator.SetBool("Evading", false);
				_isDashing = false;
				_myRigidbody.drag = startDrag;
				
				if (!myRenderer.enabled){
					myRenderer.enabled = true;
				}
			}
		}

		if (_isTalking){
			dashButtonUp = false;
		}else{
			if (!myControl.DashTrigger()){
				dashButtonUp = true;
			}
		}

	}

	private void AttackControl(){

		if (!_isTalking&&!_isBlocking && !_isDashing && !_chargingAttack && !InAttack()){
			comboDuration -= Time.deltaTime;
			if (comboDuration <= 0 && currentChain != -1){
				currentChain = -1;
			}
		}

		if (_chargingAttack && (ShootInputPressed() || _chargeAttackTriggered)){
			_chargeAttackTime+= Time.deltaTime;
			if (!_chargeAttackTriggered && _chargeAttackTime >= _chargeAttackTrigger){
				_chargeAttackTriggered = true;
				//_chargeCollider.TriggerAttack(transform.position, ShootDirection());

				GameObject newCharge = Instantiate(_chargePrefab, transform.position, Quaternion.identity)
					as GameObject;
				newCharge.GetComponent<ProjectileS>().Fire(superCloseEnemyDetect.allEnemiesInRange.Count > 0,
				                                           ShootDirection(), ShootDirection(), this);
				SpawnAttackPuff();

				_myStats.ManaCheck(_chargeAttackCost);
				_myStats.ChargeCheck(_chargeAttackCost);
				_playerSound.PlayChargeSound();

				//_specialFlash.Flash();
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
				
				newProjectile = (GameObject)Instantiate(equippedWeapon.dashAttack, 
				                                        transform.position, 
				                                        Quaternion.identity);
			}else{

					currentChain++;
					if (_doingHeavyAttack){
						if (currentChain > equippedWeapon.heavyChain.Length-1){
							currentChain = 0;
						}

						// Opportunistic effect
						if (_playerAug.opportunisticAug && staggerBonusTime > 0){
							currentChain = equippedWeapon.heavyChain.Length-1;
						}

						newProjectile = (GameObject)Instantiate(equippedWeapon.heavyChain[currentChain], 
						                                        transform.position, 
						                                        Quaternion.identity);
						// for now, heavy attacks do not combo
						prevChain = currentChain;
						//currentChain = -1;
					}
					else{
					if (currentChain > equippedWeapon.attackChain.Length-1){
						currentChain = 0;
					}

						// Opportunistic effect
						if (_playerAug.opportunisticAug && staggerBonusTime > 0){
							currentChain = equippedWeapon.attackChain.Length-1;
						}
				newProjectile = (GameObject)Instantiate(equippedWeapon.attackChain[currentChain], 
				                                        transform.position, 
						                                        Quaternion.identity);
						prevChain = currentChain;
					}
				
			}
			}


			comboDuration = currentAttackS.comboDuration;

			currentAttackS = newProjectile.GetComponent<ProjectileS>();

			if (_playerAug.thanaAug){
				currentAttackS.dmg *= PlayerAugmentsS.thanaAugAmt;
			}

			if (newAttack && currentAttackS.numAttacks > 1){
				for (int i = 0; i < currentAttackS.numAttacks - 1; i++){
					if (_doingDashAttack){
						queuedAttacks.Add(equippedWeapon.dashAttack);
						if (_playerAug.animaAug){
							queuedAttackDelays.Add(currentAttackS.timeBetweenAttacks*PlayerAugmentsS.animaAugAmt);
						}else{
							queuedAttackDelays.Add(currentAttackS.timeBetweenAttacks);
						}
					}else{
						if (_doingHeavyAttack){
							queuedAttacks.Add(equippedWeapon.heavyChain[prevChain]);
						}else{
						queuedAttacks.Add(equippedWeapon.attackChain[currentChain]);
						}
						if (_playerAug.animaAug){
							queuedAttackDelays.Add(currentAttackS.timeBetweenAttacks*PlayerAugmentsS.animaAugAmt);
						}else{
							queuedAttackDelays.Add(currentAttackS.timeBetweenAttacks);
						}
					}
				}
			}

			newProjectile.transform.position += savedDir.normalized*currentAttackS.spawnRange;

			if (newAttack){
				currentAttackS.Fire(superCloseEnemyDetect.allEnemiesInRange.Count > 0, savedDir,
				                                                 savedDir, this);
			}else{
				currentAttackS.Fire(superCloseEnemyDetect.allEnemiesInRange.Count > 0, savedDir,
				                    savedDir, this);
			}
			SpawnAttackPuff();


				// subtract mana cost
			if (_playerAug.gaeaAug){
				_myStats.ManaCheck(currentAttackS.staminaCost*PlayerAugmentsS.gaeaAugAmt, newAttack);
			}else{
				_myStats.ManaCheck(currentAttackS.staminaCost, newAttack);
			}
				FlashMana();

			if (_myStats.currentMana <= 0){
				comboDuration = 0f;
			}

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

			//muzzleFlare.Fire(currentAttackS.knockbackTime, savedDir, newProjectile.transform.localScale.x, equippedWeapon.swapColor);

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
					_doingHeavyAttack = false;


					if (_isDashing || _isSprinting){

						currentAttackS = equippedWeapon.dashAttack.GetComponent<ProjectileS>();

						_isSprinting = false;
						dashHoldTime = 0f;
						_doingDashAttack = true;


					}else{
						int nextAttack = currentChain+1;
						if (myControl.HeavyButton()){

							if (nextAttack > equippedWeapon.heavyChain.Length-1){
								nextAttack = 0;
							}
							// Opportunistic effect
							if (_playerAug.opportunisticAug && staggerBonusTime > 0){
								nextAttack = equippedWeapon.heavyChain.Length-1;
							}
							currentAttackS = equippedWeapon.heavyChain[nextAttack].GetComponent<ProjectileS>();
							_doingHeavyAttack = true;
						}else{
							if (nextAttack > equippedWeapon.attackChain.Length-1){
								nextAttack = 0;
							}
							// Opportunistic effect
							if (_playerAug.opportunisticAug && staggerBonusTime > 0){
								nextAttack = equippedWeapon.attackChain.Length-1;
							}
							currentAttackS = equippedWeapon.attackChain[nextAttack].GetComponent<ProjectileS>();
						}
					}
					
					attackDelay = currentAttackS.delayShotTime;
					if (_playerAug.animaAug){
						attackDelay*=PlayerAugmentsS.animaAugAmt;
					}
					currentAttackS.StartKnockback(this, ShootDirection());
					equippedWeapon.AttackFlash(transform.position, ShootDirection(), transform, attackDelay);
					attackTriggered = true;
					_isShooting = true;
					if (currentAttackS.chargeAttackTime <=  0){
						allowChargeAttack = true;
					}
					else{
						allowChargeAttack = false;
					}

					AttackAnimationTrigger(_doingHeavyAttack);

			
				}else if (ShootInputPressed() && !shootButtonUp && allowChargeAttack){
					if (_myStats.ManaCheck(1, false) && _myStats.ChargeCheck(1, false)){
					// charge attack

						if (prevChain < 0){
							prevChain = 0;
						}

						ProjectileS chargeAttackRef;
						if (_doingHeavyAttack){
							if (prevChain > equippedWeapon.heavyChain.Length-1){
								prevChain = equippedWeapon.heavyChain.Length-1;
							}
							chargeAttackRef = 
								equippedWeapon.heavyChain[prevChain].GetComponent<ProjectileS>()
									.chargeAttackPrefab.GetComponent<ProjectileS>();
						}else{
							if (prevChain > equippedWeapon.attackChain.Length-1){
								prevChain = equippedWeapon.attackChain.Length-1;
							}
							chargeAttackRef = 
								equippedWeapon.attackChain[prevChain].GetComponent<ProjectileS>()
									.chargeAttackPrefab.GetComponent<ProjectileS>();
						}
						ChargeAttackSet(chargeAttackRef.gameObject, 
						                chargeAttackRef.chargeAttackTime, 
						                chargeAttackRef.staminaCost, 
						                (chargeAttackRef.chargeAttackTime+chargeAttackRef.knockbackTime),
						                chargeAttackRef.animationSpeedMult, chargeAttackRef.attackAnimationTrigger);

					_chargingAttack = true;
						//EquippedWeapon().AttackFlash(transform.position, ShootDirection(), transform, _chargeAttackTrigger,1);
					_chargeAttackTriggered = false;
					_chargeAttackTime = 0;
						ChargeAnimationTrigger();
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

		if (!myStats.PlayerIsDead() && SubWeapon() != null && _canSwap){
		
			if (switchButtonUp && _myBuddy.canSwitch){
				if (myControl.SwitchButton()){
	
					_currentParadigm++;
					if (_currentParadigm > equippedWeapons.Count-1){
						_currentParadigm = 0;
						_subParadigm = 1;
					}else{
						_subParadigm = 0;
					}
					SwitchParadigm(_currentParadigm);

					currentBuddy++;
					if (currentBuddy > equippedBuddies.Count-1){
						currentBuddy = 0;
						subBuddy = 1;
					}else{
						subBuddy = 0;
					}
					
					BuddyS tempSwap = _myBuddy;
					if (!altBuddyCreated){
						altBuddyCreated = true;
						GameObject newBuddy = Instantiate(equippedBuddies[currentBuddy], transform.position,Quaternion.identity)
							as GameObject;
						newBuddy.transform.parent = transform;
						_altBuddy = newBuddy.GetComponent<BuddyS>();
					}	
					_altBuddy.SetPositions(buddyPos, buddyPosLower);
					_myBuddy = _altBuddy;
					_myBuddy.transform.position = tempSwap.transform.position;
					_myBuddy.gameObject.SetActive(true);
					Instantiate(_myBuddy.buddySound);
					_altBuddy = tempSwap;
					_altBuddy.gameObject.SetActive(false);
					
					_buddyEffect.ChangeEffect(_myBuddy.shadowColor, _myBuddy.transform);

					_playerAug.RefreshAll();
	
				}
			}
		}

		if (myControl.SwitchButton()){
			switchButtonUp = false;
		}

	}

	private void LockOnControl(){

		// controller only for the moment
		// figure out mouse control scheme (middle click & scroll wheel should work)
		if (myControl.ControllerAttached()){

			if (_myLockOn.lockedOn){

				if (myDetect.allEnemiesInRange.Count <= 0){
					_myLockOn.EndLockOn();
				}else{

					// allow change of target
					if (myDetect.allEnemiesInRange.Count > 1 && lockInputReset){
						if (myControl.RightHorizontal() > 0.1f || myControl.RightVertical() > 0.1f){
						int currentLockedEnemy = myDetect.allEnemiesInRange.IndexOf(_myLockOn.myEnemy);
						currentLockedEnemy++;
						if (currentLockedEnemy > myDetect.allEnemiesInRange.Count-1){
							currentLockedEnemy = 0;
						}
						_myLockOn.LockOn(myDetect.allEnemiesInRange[currentLockedEnemy]);
							lockInputReset = false;
						}
						if (myControl.RightHorizontal() < -0.1f || myControl.RightVertical() < -0.1f){
							int currentLockedEnemy = myDetect.allEnemiesInRange.IndexOf(_myLockOn.myEnemy);
							currentLockedEnemy--;
							if (currentLockedEnemy < 0){
								currentLockedEnemy = myDetect.allEnemiesInRange.Count-1;
							}
							_myLockOn.LockOn(myDetect.allEnemiesInRange[currentLockedEnemy]);
							lockInputReset = false;
						}
					}

				/*if (myControl.LockOnButton()){
					_lockButtonDown = true;
					lockDownTime+=Time.deltaTime;
					if (lockDownTime >= lockTurnOffThreshold){
						_myLockOn.EndLockOn();
							lockInputReset = false;
					}**/
					if (!myControl.LockOnButton()){
						_lockButtonDown = false;
							_myLockOn.EndLockOn();
							lockInputReset = false;

				}/*else{
					if (_lockButtonDown && lockDownTime < lockTurnOffThreshold && myDetect.allEnemiesInRange.Count > 1){
						int currentLockedEnemy = myDetect.allEnemiesInRange.IndexOf(_myLockOn.myEnemy);
						currentLockedEnemy++;
						if (currentLockedEnemy > myDetect.allEnemiesInRange.Count-1){
							currentLockedEnemy = 0;
						}
						_myLockOn.LockOn(myDetect.allEnemiesInRange[currentLockedEnemy]);
					}
						lockDownTime = 0f;
						_lockButtonDown = false;
				}**/

			}
				
			}else{
				if (myControl.LockOnButton()){
					_lockButtonDown = true;
					if (myDetect.allEnemiesInRange.Count > 0){
						_myLockOn.LockOn(myDetect.closestEnemy);
						_isSprinting = false;
						_lockButtonDown = false;
					}
				}/*else{
					if (_lockButtonDown && lockInputReset){
						if (myDetect.allEnemiesInRange.Count > 0){
							_myLockOn.LockOn(myDetect.closestEnemy);
							_isSprinting = false;
							lockDownTime = 0f;
						}
						_lockButtonDown = false;
					}
				}**/
			}

			if (!myControl.LockOnButton()){
				_lockButtonDown = false;
			}

			if (Mathf.Abs(myControl.RightVertical()) <= 0.1f && Mathf.Abs(myControl.RightHorizontal()) <= 0.1f){
				lockInputReset = true;
			}
			
		}
	}

	private void ManageAugments(){

		if (staggerBonusTime > 0){
			staggerBonusTime -= Time.deltaTime;
		}

	}

	public void SendCritMessage(){
		staggerBonusTime = staggerBonusTimeMax;
	}

	private void ChargeAttackSet(GameObject chargePrefab, float chargeTime, float chargeCost, float cDuration,
	                             float animationSpeed, string animationTrigger){
		_chargePrefab = chargePrefab;
		_chargeAttackTrigger = chargeTime;
		_chargeAttackCost = chargeCost;
		_chargeAttackDuration = cDuration;
		_chargeAnimationSpeed = animationSpeed;
		_chargeAnimationTrigger = animationTrigger;

		if (_playerAug.animaAug){
			_chargeAttackTrigger *= PlayerAugmentsS.animaAugAmt;
			_chargeAttackDuration *= PlayerAugmentsS.animaAugAmt;
			_chargeAnimationSpeed *= PlayerAugmentsS.animaAugAmt;
		}
	}

	public void ParadigmCheck(){
		SwitchParadigm(_currentParadigm);
	}
	public void BuddyLoad(int buddyIndex, GameObject buddyPrefab){

		if (buddyIndex > 0){
			if (altBuddyCreated){
				Destroy(_altBuddy.gameObject);
				altBuddyCreated = false;
			}
			equippedBuddies[_subParadigm] = buddyPrefab;
		}
		else{
		
			GameObject oldBuddy = _myBuddy.gameObject;

			equippedBuddies[_currentParadigm] = buddyPrefab;

			GameObject newBuddy = Instantiate(equippedBuddies[_currentParadigm], oldBuddy.transform.position,Quaternion.identity)
				as GameObject;
			newBuddy.transform.parent = transform;
			_myBuddy = newBuddy.GetComponent<BuddyS>();
			_myBuddy.SetPositions(buddyPos, buddyPosLower);
			_myBuddy.gameObject.SetActive(true);
			Instantiate(_myBuddy.buddySound);
			Destroy(oldBuddy);
			
			_buddyEffect.ChangeEffect(_myBuddy.shadowColor, _myBuddy.transform);

		}
	}

	private void SwitchParadigm (int newPara){

		_currentParadigm = newPara;
		if (newPara == 1){
			_subParadigm = 0;
		}else{
			_subParadigm = 1;
		}

		// switch buddy
		/*BuddyS tempSwap = _myBuddy;
		_myBuddy = equippedBuddies[_currentParadigm];
		_myBuddy.transform.position = tempSwap.transform.position;
		_myBuddy.gameObject.SetActive(true);
		Instantiate(_myBuddy.buddySound);
		tempSwap.gameObject.SetActive(false);*/

		
		// switchWeapon
		equippedWeapon = equippedWeapons[_currentParadigm];
		if (currentChain > equippedWeapon.attackChain.Length-1){
			currentChain = -1;
		}

		_myAnimator.SetInteger("WeaponNumber",equippedWeapon.weaponNum);
		_myLockOn.SetSprite();
		weaponSwitchIndicator.Flash(equippedWeapon);
		myRenderer.color = equippedWeapon.swapColor;

		if (_blockRef){
			_blockRef.ChangeColors(equippedWeapon.swapColor);
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

		if (_isTalking){
			shootButtonUp = false;
			allowChargeAttack = false;
		}

		if (!controller.ShootButton() && !controller.HeavyButton() && !_isTalking){
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
			doWakeUp = true;
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

		if (_usingItem){
			usingItemTime -= Time.deltaTime;
			if (usingItemTime <= 0){
				_usingItem = false;
			}
		}

		if (delayTalkTriggered){
			delayTurnOffTalk -= Time.deltaTime;
			if (delayTurnOffTalk <= 0.5f){
				TurnOffResting();
			}
			if (delayTurnOffTalk <= 0){
				delayTalkTriggered = false;
				_isTalking = false;
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

		}else if (flashChargeFrames > 0){
			if (myRenderer.material != chargeFlashMat){
				myRenderer.material = chargeFlashMat;
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
		flashChargeFrames--;
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
			doWakeUp = false;
	}

	public void TriggerItemAnimation(){
		_myAnimator.SetTrigger("Item");
		_usingItem = true;
		usingItemTime = usingItemTimeMax;
	}

	public void ResetCombat(){
		if (_currentCombatManager != null){
			_currentCombatManager.Initialize(true);
			FlashMana();
			CameraEffectsS.E.ResetEffect();
			CameraShakeS.C.SmallShakeCustomDuration(0.6f);
			CameraShakeS.C.TimeSleep(0.08f);
			_myStats.ResetCombatStats();
		}
	}

	private void AttackAnimationTrigger(bool heavy = false){

		if (heavy){
			_myAnimator.SetBool("HeavyAttacking", true);
		}else{
			_myAnimator.SetBool("HeavyAttacking", false);
		}
		_myAnimator.SetTrigger("AttackTrigger");

		if (_playerAug.animaAug){
			_myAnimator.SetFloat("AttackAnimationSpeed", currentAttackS.animationSpeedMult/PlayerAugmentsS.animaAugAmt);
		}else{
			_myAnimator.SetFloat("AttackAnimationSpeed", currentAttackS.animationSpeedMult);
		}
		_myAnimator.SetTrigger(currentAttackS.attackAnimationTrigger);
		_myAnimator.SetBool("Attacking", true);
		

	}

	private void ChargeAnimationTrigger(){

		
		_myAnimator.SetBool("Charging", true);
		_myAnimator.SetTrigger("Charge Attack");

		ProjectileS currentProj = _chargePrefab.GetComponent<ProjectileS>();

		_myAnimator.SetTrigger(currentProj.attackAnimationTrigger);
		_myAnimator.SetFloat("AttackAnimationSpeed", currentProj.animationSpeedMult);

	}

	private void PrepBlockAnimation(){
		
		_myAnimator.SetBool("Attacking", false);
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
		_myAnimator.SetBool("HeavyAttacking", false);
	}

	public void TurnOffResting(){
		_myAnimator.SetBool("Resting", false);
	}
	public void TriggerResting(float delayTalk = 0f){
		if (delayTalk > 0f){
			_isTalking = true;
			delayTurnOffTalk = delayTalk;
			delayTalkTriggered = true;
		}
		_myAnimator.SetTrigger("Rest");
		_myAnimator.SetBool("Resting", true);
		_playerSound.SetWalking(false);
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
		    && !doingBlockTrigger && attackDuration <= 0 && !_chargeAttackTriggered && !_isTalking && !_usingItem){
			return true;
		}
		else{
			return false;
		}

	}

	private bool CanInputDash(){

		bool dashAllow = false;

		/*if (blockPrepMax-blockPrepCountdown+timeInBlock < DASH_THRESHOLD && blockPrepCountdown > 0 &&
		    (controller.Horizontal() != 0 || controller.Vertical() != 0) && !_isDashing
		    && !_isStunned && _myStats.currentDefense > 0 && (!_examining || enemyDetect.closestEnemy)){**/
		if (!_isTalking && CanInputShoot() && dashCooldown <= 0){
			dashAllow = true;
		}

		return dashAllow;
	}

	private bool CanInputShoot(){

		if (!attackTriggered && !_isStunned
		    && attackDuration <= currentAttackS.chainAllow && !_chargingAttack && !_usingItem){
			return true;
		}
		else{
			return false;
		}

	}

	private bool CanInputBlock(){
		if (!_isDashing && !_isTalking && !_isShooting && !_chargingAttack && !_usingItem){
			return true;
		}else{
			return false;
		}
	}

	public bool CanUseItem(){
		if (CanInputShoot() && !_isDashing && !_isTalking){
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

		// first, do lock on
		if (_myLockOn.lockedOn){
			inputDirection = (_myLockOn.myEnemy.transform.position-transform.position).normalized;
		}
		// now check 4/8 directional, if applicable
		else if (Mathf.Abs(inputDirection.x) <= 0.04f && Mathf.Abs(inputDirection.y) <= 0.04f){
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
				FaceLeftRight();
				//FaceUp();
				//Debug.Log("Look 4!");
			}
			else if (directionZ > 78.75f && directionZ <= 101.25f) {
				inputDirection.x = 0;
				inputDirection.y = 1;
				FaceLeftRight();
				//FaceUp();
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
				FaceLeftRight();
				//FaceDown();
				//Debug.Log("Look 12!");
			}
			else if (directionZ > 258.75f && directionZ <= 281.25f)  {
				inputDirection.x = 0;
				inputDirection.y = -1;
				FaceLeftRight();
				//FaceDown();
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
		FaceAttackDirection(savedDir);
		return inputDirection.normalized;
		

	}

	private void FaceAttackDirection(Vector3 aimDir){
		float directionZ = FindDirectionOfVector(aimDir);
		
		if (directionZ > 348.75f || directionZ <= 11.25f){
			FaceLeftRight();
			//Debug.Log("Look 1!");
		}
		else if (directionZ > 11.25f && directionZ <= 33.75f){
			FaceLeftRight();
			//Debug.Log("Look 2!");
		}
		else if (directionZ > 33.75f && directionZ <= 56.25f){
			FaceLeftRight();
			//Debug.Log("Look 3!");
		}
		else if (directionZ > 56.25f && directionZ <= 78.75f){
			FaceUp();
			//Debug.Log("Look 4!");
		}
		else if (directionZ > 78.75f && directionZ <= 101.25f) {
			FaceUp();
			//Debug.Log("Look 5!");
		}
		else if (directionZ > 101.25f && directionZ <= 123.75f) {
			FaceLeftRight();
			//Debug.Log("Look 6!");
		}
		else if (directionZ > 123.75f && directionZ <= 146.25f) {
			FaceLeftRight();
			//Debug.Log("Look 7!");
		}
		else if (directionZ > 146.25f && directionZ <= 168.75f) {
			FaceLeftRight();
			//Debug.Log("Look 8!");
		}
		else if (directionZ > 168.75f && directionZ <= 191.25f) {
			FaceLeftRight();
			//Debug.Log("Look 9!");
		}
		else if (directionZ > 191.25f && directionZ <= 213.75f) {
			FaceLeftRight();
			//Debug.Log("Look 10!");
		}
		else if (directionZ > 213.75f && directionZ <= 236.25f) {
			FaceLeftRight();
			//Debug.Log("Look 11!");
		}
		else if (directionZ > 236.25f && directionZ <= 258.75f){
			FaceDown();
			//Debug.Log("Look 12!");
		}
		else if (directionZ > 258.75f && directionZ <= 281.25f)  {
			FaceDown();
			//Debug.Log("Look 13!");
		}
		else if (directionZ > 281.25f && directionZ <= 303.75f) {
			FaceLeftRight();
			//Debug.Log("Look 14!");
		}
		else if (directionZ > 303.75f && directionZ <= 326.25f) {
			FaceLeftRight();
			//Debug.Log("Look 15!");
		}
		else{
			FaceLeftRight();
			//Debug.Log("Look 16!");
		}
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
		return (controller.ShootButton() || controller.HeavyButton());
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

	public void SetBlockReference(BlockDisplay3DS bd){
		_blockRef = bd;
	}

	public void SetDetect(EnemyDetectS newDetect){

		enemyDetect = newDetect;

	}

	public void SetLockOnIndicator(LockOnS newLock){
		_myLockOn = newLock;
		//_myLockOn.SetSprite();
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
		if (combat = false){
			_currentCombatManager = null;
		}
	}

	public void SetCombatManager(CombatManagerS m){
		_currentCombatManager = m;
		_myStats.SaveStats();
	}

	public void SetExamining(bool nEx, string newExString = ""){
		_examining = nEx;
		_overrideExamineString = newExString;
	}

	public void SetTalking(bool nEx){
		_isTalking = nEx;
		if (_isTalking){
			_myRigidbody.velocity = Vector3.zero;
			_myAnimator.SetFloat("Speed", 0f);
			_myAnimator.SetBool("Attacking", false);
			_playerSound.SetWalking(false);
		}
	}

	public void SetBuddy(bool onOff){
		if (!onOff){
			EquippedBuddy().gameObject.SetActive(false);
		}else{
			EquippedBuddy().gameObject.SetActive(true);
			_buddyEffect.ChangeEffect(_myBuddy.shadowColor, _myBuddy.transform);
		}
	}

	public void SetCanSwap (bool newCanSwap)
	{
		_canSwap = newCanSwap;
	}

	public PlayerWeaponS EquippedWeapon(){
		return (equippedWeapons[_currentParadigm]);
	}
	public PlayerWeaponS SubWeapon(){
		if (equippedWeapons.Count > 1){
			return (equippedWeapons[_subParadigm]);
		}else{
			return null;
		}
	}

	public PlayerWeaponS EquippedWeaponAug(){
			
		return (subWeapons[_currentParadigm]);

	}
	public PlayerWeaponS SubWeaponAug(){
		if (subWeapons.Count > 1){
			return (subWeapons[_subParadigm]);
		}else{
			return null;
		}
	}

	public BuddyS EquippedBuddy(){
		return (_myBuddy);
	}
	public BuddyS SubBuddy(){
		if (equippedBuddies.Count > 1){
			if (!altBuddyCreated){
				return (equippedBuddies[_subParadigm].GetComponent<BuddyS>());
			}else{
				return (_altBuddy);
			}
		}
		else{
			return null;
		}
	}

	//_________________________________________________________________VISUAL EFFECTS
	public void SpawnDashPuff(){
		Vector3 spawnPos = transform.position;
		spawnPos.y -= 0.5f;
		spawnPos.z += 1f;
		GameObject dashEffect = Instantiate(dashObj, spawnPos, Quaternion.identity)
			as GameObject;
		float rotateFix = 0f;
		if (myRenderer.transform.localScale.x < 0){
			Vector3 flipsize = dashEffect.transform.localScale;
			flipsize.x *= -1f;
			dashEffect.transform.localScale = flipsize;
			rotateFix = 180f;
		}
		dashEffect.GetComponent<SpriteRenderer>().color = myRenderer.color;
		dashEffect.transform.GetChild(1).GetComponent<SpriteRenderer>().color = myRenderer.color;
	}

	public void SpawnAttackPuff(){
		Vector3 spawnPos = transform.position;
		spawnPos.z += 1f;
		GameObject attackEffect = Instantiate(attackEffectObj, spawnPos, Quaternion.identity)
			as GameObject;
		float rotateFix = 0f;
		if (myRenderer.transform.localScale.x < 0){
			Vector3 flipsize = attackEffect.transform.localScale;
			flipsize.x *= -1f;
			attackEffect.transform.localScale = flipsize;
			rotateFix = 180f;
		}

		attackEffect.GetComponent<SpriteRenderer>().color = myRenderer.color;
		attackEffect.transform.GetChild(1).GetComponent<SpriteRenderer>().color = myRenderer.color;
	}

}
