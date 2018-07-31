using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	//_________________________________________CONSTANTS

	private static float DASH_RESET_THRESHOLD = 0.15f;
	private static float SMASH_TIME_ALLOW = 0.2f;
	private static float SMASH_MIN_SPEED = 0.042f;
	private static float CHAIN_DASH_THRESHOLD = 0.12f; // was 0.4f
	private static float TELEPORT_INPUT_THRESHOLD = 0.33f;
	private static float ALLOW_DASHATTACK_TIME = 0.14f;
	private static float ENEMY_TOO_CLOSE_DISTANCE = 8f;

	private const float PUSH_ENEMY_MULT = 0.2f;
	private const int START_PHYSICS_LAYER = 8;
	private const int DODGE_PHYSICS_LAYER = 12;


	private const float ADAPTIVE_WINDOW = 0.3f;

	private static float SMASH_THRESHOLD = 0.75f;
	
	//_________________________________________CLASS PROPERTIES

	private PlayerStatsS _myStats;


    public static bool killedFamiliar = false;
	public static bool doWakeUp = true;
	public static bool dontHealWakeUp = false;
	public static bool familiarUnlocked = true;
	private bool wakingUp = false;
	public bool isWaking  { get { return  wakingUp; } }
	private float wakeUpTime = 3f;
	private float wakeUpCountdown;

	[Header("Movement Variables")]
	public float walkSpeed;
	public float walkSpeedMax;
	private float walkSpeedBlockMult = 0.2f;
	public float runSpeed;
	public float runSpeedMax;
	public float walkThreshold = 0.8f;
	private bool _isSprinting = false;
	private bool allowSprint = false;
	public float sprintMult = 1.4f;
	public float sprintStaminaRate = .75f;
	private float sprintNoDrainTime = 0f;
	private float sprintNoDrainMax = 0.33f;
	private bool _isDoingMovement = false;
	public bool isDoingMovement { get { return _isDoingMovement; } }

	[Header("Dash Variables")]
	public float dashSpeed;
	public Collider dashCollider;
	private float evadeSpeed = 2000f;
	private bool _isDashing;
	private bool _triggerBlock;
	private bool _dashStickReset;
	public float dashDuration;
	public float dashSlideTime;
	private float triggerDashSlideTime;
	public float dashDragMult;
	public float dashDragSlideMult;
	private float dashDurationTime;
	private float dashDurationTimeMax;
	private float flourishLockTime = 0.5f;
	private float dashEffectThreshold = 0.2f;
	private float dashCooldown = 0.4f;
	private float dashCooldownMax = 0.2f;
	private float _dodgeCost = 0.75f;
	public GameObject dashObj;
	private bool _allowCounterAttack = false;
	public bool allowCounterAttack { get { return _allowCounterAttack; } }
	private float counterAttackTime;
	private float counterAttackTimeMax = 0.4f;
	private float parryDelayWitchTime = 0.3f;
	private float parryDelayWitchCountdown = 0f;
	private bool _delayWitchTime = false;
	public bool delayWitchTime { get { return _delayWitchTime; } }
	private EnemyS _counterTarget;

	[Header("Disconnected Variables")]
	public LayerMask disconnectedMask;
	private float disconnectedMaxDistance = 15f;
	private float disconnectedMinDistance = 6f;
	private float disconnectedNoTargetDistance = 7f;
	private float disconnectedTeleportDistance = 0.5f;
	private Vector3 disconnectedTeleportTarget = Vector3.zero;
	private float disconnectedChargeCost = 1f;
	private float disconnectedCooldown = 0.4f;
	private float disconnectedEndSpeed = 1200f;
	private float disconnectDragMult = 0.8f;

	[Header("Parry Variables")]
	public float parryForce = 1000f;
	public float parryKnockbackMult = -2f;

	private Vector3 parrySizeStart;
	private Vector3 currentParrySize;
	private bool dontSlowWhenClose = false;

	private PlayerSlowTimeS witchReference;

	private Vector3 _counterNormal = Vector3.zero;
	public Vector3 counterNormal { get { return _counterNormal; } }

	private float dashChargeAllowMult = 0.75f;
	private float dashSprintAllowMult = 0.55f;
	private float attackBufferAllowMult = 0.88f;
	private float sprintStartForce = 1200f;
	private bool speedUpChargeAttack = false;

	private bool _isShooting;
	private bool _lastInClip;
	private bool _canSwap;

	private bool _shoot8Dir = true;
	private Vector3 savedDir = Vector3.zero;
	private Vector3 _attackStartDirection = Vector3.zero;
	public Vector3 attackStartDirection { get { return _attackStartDirection; } }

	private bool allowChainHeavy;
	private bool allowChainLight;

	private bool _doingDashAttack = false;
	private bool _doingCounterAttack = false;
	public bool doingCounterAttack { get { return _doingCounterAttack; } } 
	private bool _allowDashAttack = false;
	private bool _doingHeavyAttack = false;
	private bool _attackBuffered = false;
	public bool doingSpecialAttack { get { return _doingDashAttack; } }

	private Vector2 _inputDirectionLast;
	private Vector2 _inputDirectionCurrent;

	private ProjectilePoolS _projectilePool;
	public ProjectilePoolS projectilePool {get { return _projectilePool; } }

	[Header("Transform Properties")]
	private bool _isTransformed = false;
	public bool isTransformed { get { return _isTransformed; } }
	private CorruptedEffectS transformActiveEffect;
	private TransformStartEffectS transformStartEffect;
	private bool _transformReady = false;
	public bool transformReady { get { return _transformReady; } }
	public Color transformedColor = Color.magenta;
	private float transformMoveSpeedMult = 1.12f;
	private float transformedAttackSpeedMult = 0.8f;
	private float _transformedStaminaMult = 0.3f;
	public float transformedStaminaMult { get { return _transformedStaminaMult; } }
	private float _transformedRecoveryMult = 3f;
	public float transformedRecoverMult { get { return _transformedRecoveryMult; } }
	private float _transformedDamageMult = 3f;
	public float transformedDamageMult { get { return _transformedDamageMult; } }
	private float _transformedAbsorbMult = 3f;
	public float transformedAbsorbMult { get { return _transformedAbsorbMult; } }
	// transform activation properties
	private float _transformRequireHoldTime = 0.3f;
	public float transformRequireHoldTime { get { return _transformRequireHoldTime; } }
	private float _revertRequireHoldTime = 0.04f;
	public float revertRequireHoldTime { get { return _revertRequireHoldTime; } }
	private float _transformHoldTime = 0f;
	public float transformHoldTime { get { return _transformHoldTime; } }
	private float _transformDefenseMult = 3f;
	public float transformDefenseMult { get { return _transformDefenseMult; } }
	public GameObject transformEcho;

	[Header("Taunt Properties")]
	public GameObject tauntPrefab;
	public ChargeRiseEffectS tauntEffect;
	public EnemyDetectS tauntTrigger;
	public TauntDialogueS tauntText;
	private bool tauntButtonUp = true;
	
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

    private bool trackingScorn = false;

	private MantraSwitchS mantraText;

	private TrackingEffectS myTracker;

	private PlayerAnimationFaceS _myFace;

	private float startDrag = 6.5f;
	private float sprintDragMult = 0.9f;

	private Vector3 inputDirection;
	private bool dashButtonUp = true;
	private bool shootButtonUp;
	private bool reloadButtonUp;
	private bool aimButtonUp;
	private bool switchButtonUp;
	private bool switchBuddyButtonUp;
	private bool lockInputReset = false;

	private Vector3 savedHitVelocity;
	private bool hitStopped = false;

	private float momsEyeMult = 1f;

	// Status Properties
	private float delayAttackAllow = 0f;
	private bool _isStunned = false;
	public bool isStunned { get { return _isStunned; } }
	private bool attackTriggered;
	private bool allowItemUse = true;
	//private PlayerWeaponS weaponTriggered;
	private float stunTime;
	private List<GameObject> queuedAttacks;
	private List<float> queuedAttackDelays;
	private bool newAttack = true;
	private bool counterQueued = false;
	private bool heavyCounterQueued;
	private bool preAttackSlowdown = false;
	private float preAttackSlowTime = 0f;
	private float preAttackPunchMult = 1f;
	private float preAttackHangTime = 1f;
	private bool preAttackExtraSlow = false;

	// Charging Properties
	private bool _chargingAttack;
	private float _chargeAttackTime;
	private float _chargeAttackTrigger = 0.6f;
	private float _chargeAttackDuration = 1f;
	private float _chargeAnimationSpeed;
	private bool _chargeAttackUseAll = false;
	//private ChargeAttackS _chargeCollider;
	private GameObject _chargePrefab;
	private bool _chargeAttackTriggered = false; 
	private bool allowChargeAttack = false;
	private float _chargeAttackCost = 5f;

	// Buddy Properties
	private BuddyS _myBuddy;
	private BuddyS _altBuddy;
	private bool altBuddyCreated = false;
	[Header("Bios Properties")]
	public GameObject[] biosDistortions;
	private int activeBios = 0;
	public int ActiveBios { get {return activeBios; } }
	private float activeBiosTime = 5f;
	private float activeBiosCount = 0f;
	[Header("Buddy Properties")]
	public List<GameObject> equippedBuddies;
	public Transform buddyPos;
	public Transform buddyPosLower;
	private BuddySwitchEffectS _buddyEffect;

	// Buddy Garden Properties
	private float embraceOutTime = 0.5f;
	private float embraceOutCount;
	private bool embracing = false;

	// Virtue Properties
	public static List<int> equippedVirtues;
	public static List<int> equippedTech; // tech

	// Animation Properties
	private bool _facingDown = true;
	private bool _facingUp = true;
	private bool triggerBlockAnimation = true;
	private bool doingBlockTrigger = false;
	private float blockPrepCountdown = 0;
	private float timeInBlock;
	private float blockPrepMax = 0.18f;

	private InstructionTrigger tutorialRef;
	public InstructionTrigger tutorialReference { get { return tutorialRef; } }
		
	// Attack Properties
	//public GameObject[] attackChain;
	//public GameObject dashAttack;
	private PlayerWeaponS equippedWeapon,  _attackingWeapon, attackingWeaponAug;
	public PlayerWeaponS attackingWeapon { get { return _attackingWeapon; } }
	public PlayerWeaponS getEWeapon { get { return equippedWeapon; } }
	public List<PlayerWeaponS> equippedWeapons;
	public List<PlayerWeaponS> subWeapons;
	private WeaponSwitchFlashS weaponSwitchIndicator;
	public static int _currentParadigm = 0;
	public int currentParadigm { get { return _currentParadigm; } }
	private static int _subParadigm = 1;
	public int subParadigm { get { return _subParadigm; } }
	private ProjectileS currentAttackS;
	private int currentChain = 0;
	private int prevChain = 0;
	private float comboDuration = 0f;
	private float adaptComboCutoff = 0f;
	private float adaptPostRecover = 0.06f;
	private float attackDelay;
	public bool inAttackDelay { get { return (attackDelay > 0 && attackTriggered);} }
	private float attackDuration;
    public bool inAttackDuration { get { return (attackDuration > 0); } }
	public GameObject attackEffectObj;
	private float allowParryInput = 0.1f;
	private float allowParryCountdown = 0f;

	private Vector3 capturedShootDirection;
	[Header ("Enemy Detection References")]
	public EnemyDetectS enemyDetect;
	private BoxCollider enemyDetectCollider;
	private Vector3 startDetectSize; 
	public EnemyDetectS lockOnEnemyDetect;
	public EnemyDetectS superCloseEnemyDetect;
	public EnemyDetectS dontWalkIntoEnemiesCheck;
	public EnemyDetectS dontGetStuckInEnemiesCheck;
	private PlayerDashEffectS attackEffectRef;
	private EnemyS currentTargetEnemy;
	public EnemyS targetEnemy { get { return currentTargetEnemy; } }

	private List<EnemyS> enemiesHitByLastAttack;
	public List<EnemyS> enemiesHitByAttackRef { get { return enemiesHitByLastAttack; } }

	private int numAttacksPerShot;
	private float timeBetweenAttacks;
	private float timeBetweenAttacksCountdown;
	private int attacksRemaining = 0;

	private bool _isBlocking;
	private bool _stickReset = false;
	private float _smashReset = 0;

	public bool _inCombat = true;
	private bool _examining = false;
	private string _overrideExamineString = "";
	private Vector3 _examineStringPos;
	public Vector3 examineStringPos { get { return _examineStringPos; } }
	private bool _isTalking = false;
	private bool _allowWalk = false;
	private bool _tempTransformAllow = false;
    private bool _tempTauntAllow = false;
	private float delayTurnOffTalk;
	private bool delayTalkTriggered = false;

	private bool _usingItem = false;
	public bool usingitem { get { return _usingItem; } }
	private float usingItemTime = 0f;
	private float usingItemTimeMax = 0.8f;

	private List<ChargeAttackS> fosCharges = new List<ChargeAttackS>();

	private PlayerSoundS _playerSound;
	private PlayerAugmentsS _playerAug;
	public PlayerAugmentsS playerAug { get { return _playerAug; } }
	private LockOnS _myLockOn;
	private bool _lockButtonDown = false;
	public LockOnS myLockOn { get { return _myLockOn; } }

	private BlockDisplay3DS _blockRef;
	private PlayerDodgeEffect _dodgeEffectRef;
	//private FlashEffectS _specialFlash;
	private CombatManagerS _currentCombatManager;
	public CombatManagerS currentCombatManager { get { return _currentCombatManager; } }

	private List<ProjectileS> _activeProjectiles = new List<ProjectileS>();
	public List<ProjectileS> activeProjectiles { get { return _activeProjectiles; } }

	//_________________________________________AUGMENT-SPECIFIC

	private float staggerBonusTimeMax = 1f;
	private float staggerBonusTime;
	private float _adaptiveCountdown = 0f;
	private bool canDoAdaptive = false;
	public bool adaptiveAugBonus { get { return (_adaptiveCountdown > 0); } }

	
	//_________________________________________GETTERS AND SETTERS

	public bool showBlock		{ get { return _isBlocking; } }
	public bool isBlocking		{get { return _isBlocking || doingBlockTrigger; } }
	public bool doDeflect		{get { return _isBlocking;}}
	public bool isDashing		{get { return _isDashing; } }
	public bool isSprinting		{get { return _isSprinting; } }
	public bool isShooting		{get { return _isShooting; } }
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

	[HideInInspector]
	public bool isNatalie = false;

	//_________________________________________DEMO SHOW ONLY
	private float resetTimeMax = 60f;
	private float resetCountdown;
	private bool demoResetTriggered = false;
	private GameOverS resetManager;

	public bool disableTransformInScene = false;

    //_________________________________________UNITY METHODS

    void Awake()
    {

        MainMenuNavigationS.inMain = false;
        CinematicHandlerS.inCutscene = false;

        if (_currentParadigm < 0)
        {
            _currentParadigm = 0;

        }
        if (_currentParadigm > 1){
            _currentParadigm = 1;
        }

	}

	// Use this for initialization
	void Start () {

		if (ControlManagerS.controlProfile != 1){
			Cursor.visible = false;
		}else{
			Cursor.visible = true;
		}
		InitializePlayer();

        PlayerInventoryS.I.AddSceneIveBeenTo(Application.loadedLevel);

	}

	void Update(){

		PlayerUpdate();

		if (!myControl.ControllerAttached()){
			if (!Cursor.visible){
				if (Input.GetKeyDown(KeyCode.Escape) && !_isTalking){
					Cursor.visible = true;
				}
			}else{
				if (Input.GetMouseButtonDown(0) && ControlManagerS.controlProfile != 1){
					Cursor.visible = false;
				}
			}
				
		}

		#if UNITY_EDITOR_OSX
		DebugCommands();
		#endif

	}

	void FixedUpdate () {

		PlayerFixedUpdate();

	}

	void DebugCommands(){

        // add marked
        if (Input.GetKeyDown(KeyCode.M)){
            AddMarked();
        }
	}

	//_________________________________________PUBLIC METHODS

	public void SetWitchObject(PlayerSlowTimeS newSlow){
		witchReference = newSlow;
	}
	public void SetTransformStartEffect(TransformStartEffectS newT){
		transformStartEffect = newT;
	}

	void TriggerWitchTime(){

		if (_playerAug.untetheredAug){
			gameObject.layer = DODGE_PHYSICS_LAYER;
			witchReference.TriggerWitchTime();
		}

		if (_playerAug.agileAug){
			_myStats.ResetStamina(true);
		}

	}

	public void ExtendWitchTime(){
		witchReference.ExtendWitchTime();
	}
	public void EndWitchTime(bool fromWitch = false, bool fromItem = false){

		if (!_isDashing){
			gameObject.layer = START_PHYSICS_LAYER;
		}
		if (!fromWitch){
			witchReference.EndWitchTime(fromItem);
		}
	}

	public void Knockback(Vector3 knockbackForce, float knockbackTime, bool attackTime = false){

		myRigidbody.AddForce(knockbackForce, ForceMode.Impulse);

		if (!attackTime){
			Stun(knockbackTime);
		}
		else{
			AttackDuration(knockbackTime);
		}

	}

	public void SetFace(bool newFace){
		if (newFace){
			_myFace.StopFace();
		}else{
			_myFace.AllowFace();
		}
	}
	public void SetFaceDirection(PlayerAnimationFaceS.PlayerFaceState newFace){
		if (newFace == PlayerAnimationFaceS.PlayerFaceState.faceDown){
			FaceDown();
			if (!_myRigidbody){
				_myRigidbody = GetComponent<Rigidbody>();
			}
			_myRigidbody.velocity = new Vector3(0f,-1f,0f); 
		}
		if (newFace == PlayerAnimationFaceS.PlayerFaceState.faceUp){
			FaceUp();
			if (!_myRigidbody){
				_myRigidbody = GetComponent<Rigidbody>();
			}
			_myRigidbody.velocity = new Vector3(0f,1f,0f); 
		}
		if (newFace == PlayerAnimationFaceS.PlayerFaceState.faceLeft){
			FaceLeftRight();
			if (!_myRigidbody){
				_myRigidbody = GetComponent<Rigidbody>();
			}
			_myRigidbody.velocity = new Vector3(-1f,0f,0f); 
		}
		if (newFace == PlayerAnimationFaceS.PlayerFaceState.faceRight){
			FaceLeftRight();
			if (!_myRigidbody){
				_myRigidbody = GetComponent<Rigidbody>();
			}
			_myRigidbody.velocity = new Vector3(1f,0f,0f); 
		}
	}

	public void StartEmbrace(){
		SetTalking(true);
		_myAnimator.SetTrigger("Embrace");
		_myAnimator.SetBool("Embracing", true);
	}
	public void EndEmbrace(){
		embracing = true;
		embraceOutCount = embraceOutTime;
		_myAnimator.SetBool("Embracing", false);
	}

	public void AddEnemyHit(EnemyS newEnemy){
		if (!enemiesHitByLastAttack.Contains(newEnemy)){
			enemiesHitByLastAttack.Add(newEnemy);
		}
	}

	public void ResetBuddyPos(){
		_myBuddy.transform.position = buddyPos.position;
	}

	//_________________________________________PRIVATE METHODS

	void InitializePlayer(){

		_myRigidbody = GetComponent<Rigidbody>();
		enemyDetectCollider = enemyDetect.GetComponent<BoxCollider>();
		startDetectSize = enemyDetectCollider.size;

        if (_myRigidbody.drag > 0)
        {
            startDrag = _myRigidbody.drag;
        }
		_myAnimator = myRenderer.GetComponent<Animator>();
		_myFace = myRenderer.GetComponent<PlayerAnimationFaceS>();
		_dodgeEffectRef = myRenderer.GetComponent<PlayerDodgeEffect>();
		startMat = myRenderer.material;
		_playerSound = GetComponent<PlayerSoundS>();
		mantraText = GetComponentInChildren<MantraSwitchS>();
		if (!_myStats){
			_myStats = GetComponent<PlayerStatsS>();
		}

		_projectilePool = GetComponent<ProjectilePoolS>();

		myTracker = GetComponentInChildren<TrackingEffectS>();
		dashCollider.enabled = false;

		transformActiveEffect = GetComponentInChildren<CorruptedEffectS>();
		if (!_isTransformed){
			transformActiveEffect.gameObject.SetActive(false);
		}

		PlayerInventoryS.I.dManager.SpawnBlood();
		//_specialFlash = CameraEffectsS.E.specialFlash;

		weaponSwitchIndicator = GetComponentInChildren<WeaponSwitchFlashS>();

		mainCamera = CameraShakeS.C.GetComponent<Camera>();

		enemiesHitByLastAttack = new List<EnemyS>();

		if (PlayerInventoryS.I.EquippedWeapons() != null && !isNatalie){
			if (PlayerInventoryS.I.EquippedWeapons().Count > 0){
		equippedWeapons = PlayerInventoryS.I.EquippedWeapons();
			subWeapons = PlayerInventoryS.I.SubWeapons();
			}else{
				_currentParadigm = 0;
			}
		}
		if (PlayerInventoryS.I.EquippedBuddies() != null){
			if (PlayerInventoryS.I.EquippedBuddies().Count > 0){
			equippedBuddies = PlayerInventoryS.I.EquippedBuddies();
			}else{
				_currentParadigm = 0;
			}
		}

		if (_currentParadigm > 1){
			_currentParadigm = 1;
		}

		equippedWeapon = _attackingWeapon =equippedWeapons[_currentParadigm];
		attackingWeaponAug = subWeapons[_currentParadigm];


		if (equippedVirtues == null){
			equippedVirtues = new List<int>();
			equippedVirtues.Add(0);
		}// marked only check
        else{
            if (equippedVirtues.Count == 1 && equippedVirtues.Contains(15)){
                equippedVirtues.Add(0);
            }
        }

		
		_playerAug = GetComponent<PlayerAugmentsS>();
		_playerAug.SetPlayerRef(this);

		if (_blockRef){
			_blockRef.ChangeColors(equippedWeapon.swapColor);
		}

		GameObject startBuddy = Instantiate(equippedBuddies[_currentParadigm], transform.position, Quaternion.identity)
			as GameObject;
		startBuddy.transform.parent = transform;
		_myBuddy = startBuddy.gameObject.GetComponent<BuddyS>();
		_myBuddy.SetPositions(buddyPos, buddyPosLower);
        if (!InGameCinematicS.turnOffBuddies && familiarUnlocked && !isNatalie && !killedFamiliar){
			_myBuddy.gameObject.SetActive(true);
		}
		_myStats.SetMinChargeUse(_myBuddy.costPerUse, _myBuddy.useAllCharge);
		_myAnimator.SetInteger("WeaponNumber", equippedWeapon.weaponNum);

		_buddyEffect = GetComponentInChildren<BuddySwitchEffectS>();

		currentChain = -1;

		queuedAttacks = new List<GameObject>();
		queuedAttackDelays = new List<float>();



		controller = GetComponent<ControlManagerS>();

		inputDirection = new Vector3(1,0,0);

		_inputDirectionLast = new Vector2(0,0);
		_inputDirectionCurrent = new Vector2(0,0);

		if (doWakeUp){
			TriggerWakeUp();
			PlayerStatsS.PlayerCantDie = false;
		}
		_subParadigm=_currentParadigm+1;
		if (_subParadigm>1){
			_subParadigm=0;
		}

		ResetBios();

		currentAttackS = _attackingWeapon.attackChain[0].GetComponent<ProjectileS>();
		if (!_isTransformed){
		myRenderer.color = equippedWeapon.swapColor;
		}else{
			myRenderer.color = transformedColor;
		}

		resetCountdown = resetTimeMax;

		parrySizeStart = currentParrySize = superCloseEnemyDetect.GetComponent<BoxCollider>().size;

	}

	void PlayerFixedUpdate(){

		StatusCheck();

		// Control Methods
		if (!_myStats.PlayerIsDead()){

			//if (_inCombat){
				//LockOnControl();
			if (!_isTalking){
				SwapControl();
				BlockControl();
				DashControl();
            }
            if (!_isTalking || _tempTauntAllow)
            {
                AttackControl();
            }
			if (!_isTalking || _tempTransformAllow){
				TransformControl();
			}
			//}
			if (!_isTalking || _allowWalk){
				MovementControl();
			}
		}

		StickResetCheck();

	}

	void DemoResetCheck(){

		// for demo show purposes only!! TODO comment out when not making show demo
		if (!demoResetTriggered){
			resetCountdown -= Time.deltaTime;
			if (resetCountdown <= 0){
				if (resetManager){
				SetExamining(false, Vector3.zero, "");
				SetTalking(true);
				resetManager.FakeDeath(true);
				}
				demoResetTriggered = true;
		
			}

		}



	}

	public void SetResetManager(GameOverS newManage){
		resetManager = newManage;
	}

	void PlayerUpdate(){

		ButtonCheck();
		ManageCounterTimer();

		//DemoResetCheck();
		ManageFlash();
		ManageAugments();

		CurrentEnemyCheck();
	}

	void ManageBios(){
		if (activeBios > 0){
			activeBiosCount -= Time.deltaTime;
			if (activeBiosCount <= 0){
				biosDistortions[activeBios-1].SetActive(false);
				activeBios--;
				activeBiosCount = activeBiosTime;
			}
		}
	}


	public void EquipBuddy(BuddyS newBud){
		_myBuddy = newBud;
	}
	public void TurnOnBuddy(){
		_myBuddy.transform.position = buddyPos.position;
		_buddyEffect.ChangeEffect(_myBuddy.shadowColor, _myBuddy.transform);
		_myBuddy.gameObject.SetActive(true);
	}

	public void Stun(float sTime, bool overrideTransform = false){

		if (!_isTransformed && !overrideTransform){
		stunTime = sTime;
		_isStunned = true;
		CancelAttack();
		}

	}

	public void ActivateBios(){
		if (activeBios < biosDistortions.Length){
			biosDistortions[activeBios].SetActive(true);
			activeBios++;
		}
		activeBiosCount = activeBiosTime;
	}

	void ResetBios(){
		for (int i = 0; i < biosDistortions.Length; i++){
			biosDistortions[i].SetActive(false);
		}
		activeBios = 0;
	}

	private void CancelAttack(bool allowChain = false){
		_attackBuffered = false;
		attackTriggered = false;
		_attackingWeapon = equippedWeapon;
        if (EquippedWeaponAug() != null)
        {
            attackingWeaponAug = EquippedWeaponAug();
        }
		attackDuration = 0f;
		if (myTracker){
		myTracker.TurnOffEffect();
		}
		if (!allowChain){
			currentChain = 0;
		}else{
			if (currentAttackS){
				comboDuration = currentAttackS.comboDuration;
				allowChainLight = currentAttackS.allowChainLight;
				allowChainHeavy = currentAttackS.allowChainHeavy;
			}else{
				comboDuration = 0.3f;
			}
			currentChain++;
		}
		if (queuedAttackDelays != null){
		queuedAttackDelays.Clear();
		}
		if (queuedAttacks != null){
		queuedAttacks.Clear();
		}
		attackDelay = 0;
		_chargingAttack = false;
		_canSwap = true;
		allowChargeAttack = false;
		_chargeAttackTriggered = false;
		_chargeAttackTime = 0f;
		_myAnimator.SetBool("Charging", false);
		TurnOffAttackAnimation();
		_isSprinting = false;

	}

	public void AttackDuration(float aTime){
		attackDuration = aTime*TransformedAttackSpeedMult();
		if (_playerAug.animaAug){
			attackDuration*=_playerAug.AnimaAugAmt();
		}
		if (activeBios > 0){
			attackDuration*=1f-(PlayerAugmentsS.addSpeedPerBios*activeBios);
		}
	}

	public void FlashDamage(){
		/*flashDamageFrames = 5;
		myRenderer.material = damageFlashMat;**/
	}

	public void FlashHeal(){
		flashHealFrames = 6;
		myRenderer.material = healFlashMat;
		//VignetteEffectS.V.Flash(healFlashMat.color);
	}

	public void FlashMana(bool doEffect = false){
		if (doEffect){
			flashManaFrames = 8;
		}else{
		flashManaFrames = 4;
		}
		myRenderer.material = manaFlashMat;
		myRenderer.material.SetColor("_FlashColor", equippedWeapon.swapColor);
		/*if (doEffect){
			VignetteEffectS.V.Flash(manaFlashMat.color);
		}**/
	}
	public void FlashCharge(){
		/*flashChargeFrames = 5;
		myRenderer.material = chargeFlashMat;
		VignetteEffectS.V.Flash(chargeFlashMat.color);**/
	}

	public void WitchTime(EnemyS targetEnemy, bool fromParry = false){
		if (!_allowCounterAttack && !_doingCounterAttack && !counterQueued && !_delayWitchTime){
			if (targetEnemy != null){
				CameraPOIS.POI.JumpToMidpoint(transform.position, targetEnemy.transform.position);
			}else{
				CameraPOIS.POI.JumpToPoint(transform.position);
			}
			if (!fromParry){
				if (targetEnemy != null){
					_counterNormal = (targetEnemy.transform.position-transform.position).normalized;
				}else{
					_counterNormal = _myRigidbody.velocity.normalized;
				}
				_myRigidbody.AddForce(-evadeSpeed*Time.deltaTime*_counterNormal, ForceMode.Impulse);
				CameraShakeS.C.DodgeSloMo(0.22f, 0.12f, 0.8f, counterAttackTimeMax*0.3f);
				dashDurationTime = dashDurationTimeMax-flourishLockTime;
				_myAnimator.SetTrigger("Evade");
				_playerSound.PlayFlourishSound();
				_dodgeEffectRef.FireEffect();
				FlashMana();
				TriggerWitchTime();
			}else{
				CameraShakeS.C.DodgeSloMo(0.28f, 0.14f, 0.7f, counterAttackTimeMax*0.4f);
				/*if (_playerAug.agileAug){
					_myStats.ResetStamina(true);
				}**/
			}
			_myStats.WitchStaminaCorrect();
			_playerAug.EnragedTrigger();
			_allowCounterAttack = true;
			counterAttackTime = counterAttackTimeMax;
			_counterTarget = targetEnemy;
			_playerSound.PlaySlowSound();
		}
	}

	//_________________________________________CONTROL METHODS
	private void MovementControl(){

		if (CanInputMovement()){
			Vector2 input2 = Vector2.zero;
			input2.x = controller.Horizontal();
			input2.y = controller.Vertical();

			if (input2.magnitude > 1f){
				input2 = input2.normalized;
			}

			if (input2.x != 0 || input2.y != 0){
				savedDir.x = input2.x;
				savedDir.y = input2.y;
			}
	
			Vector3 moveVelocity = _myRigidbody.velocity;
	
			if (input2.x != 0 || input2.y != 0){

				moveVelocity.x = inputDirection.x = input2.x;
				moveVelocity.y = inputDirection.y = input2.y;
				
				_playerSound.SetWalking(true);
				_isDoingMovement = true;
				resetCountdown = resetTimeMax;

				if (Mathf.Abs(moveVelocity.x) <= 0.6f && moveVelocity.y < 0){
					FaceDown();
				}else if (Mathf.Abs(moveVelocity.x) <= 0.6f && moveVelocity.y > 0){
					FaceUp();
				}else{
					FaceLeftRight();
				}


				if (_isBlocking || _chargingAttack || _allowWalk){
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
					if (_isTransformed){
						moveVelocity *= transformMoveSpeedMult;
					}
					if (!dontWalkIntoEnemiesCheck.NoEnemies() && !dontSlowWhenClose && gameObject.layer == START_PHYSICS_LAYER){
						moveVelocity *= PUSH_ENEMY_MULT;
					}
					if (_myRigidbody.velocity.magnitude < walkSpeedMax){
						_myRigidbody.AddForce( moveVelocity*Time.deltaTime, ForceMode.Acceleration );
					}
				}
				else{
				
					float actingRunSpeed = runSpeed*equippedWeapon.speedMult; 

					moveVelocity *= actingRunSpeed;
					if (_isTransformed){
						moveVelocity *= transformMoveSpeedMult;
					}
					if (!dontWalkIntoEnemiesCheck.NoEnemies() && !dontSlowWhenClose && gameObject.layer == START_PHYSICS_LAYER){
						moveVelocity *= PUSH_ENEMY_MULT;
					}

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
		}

		//if (BlockInputPressed() && CanInputBlock()){
		if (_triggerBlock){
			if (_myStats.currentDefense > 0 && !_isStunned){
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
				blockPrepCountdown = blockPrepMax;
				triggerBlockAnimation = true;
				doingBlockTrigger = false;
				timeInBlock = 0;
				_isBlocking = false;
			}
		}else{

			TurnOffBlockAnimation();
			
			_myStats.ActivateDefense();
			blockPrepCountdown = blockPrepMax;
			triggerBlockAnimation = true;
			doingBlockTrigger = false;
			_isBlocking = false;

		}

	}

	private void DisconnectedTeleport(){
		float testDistance = 0f;
		RaycastHit teleportHit = new RaycastHit();
		if (Physics.Raycast(transform.position, ShootDirectionUnlocked(), out teleportHit, disconnectedMaxDistance, disconnectedMask)){
			//Debug.LogError(teleportHit.collider.gameObject.name, teleportHit.collider.gameObject);
			testDistance = Vector3.Distance(teleportHit.point, transform.position);
			if (testDistance > disconnectedMinDistance){
			float sizeHit = 0f;
			if (teleportHit.collider.tag != "Wall"){
                    sizeHit = Mathf.Abs(transform.localScale.x);

				disconnectedTeleportTarget = teleportHit.point
					-(teleportHit.point-transform.position).normalized*(disconnectedTeleportDistance+Mathf.Abs(sizeHit));
			}else if (testDistance < disconnectedNoTargetDistance){
				disconnectedTeleportTarget = teleportHit.point
					-(teleportHit.point-transform.position).normalized*(disconnectedTeleportDistance+Mathf.Abs(sizeHit));
			}else{
				disconnectedTeleportTarget = transform.position+ShootDirectionUnlocked()*disconnectedNoTargetDistance;
			}
		}}else{
			disconnectedTeleportTarget = transform.position+ShootDirectionUnlocked()*disconnectedNoTargetDistance;
		}


		disconnectedTeleportTarget.z = transform.position.z;
		testDistance = Vector3.Distance(disconnectedTeleportTarget, transform.position);
		if (testDistance <= disconnectedMaxDistance){
		_myAnimator.SetBool("Evading", true);
		_myAnimator.SetTrigger("Dash");
		attackEffectRef.DisconnectEffect(transform.position, disconnectedTeleportTarget);

		_myRigidbody.AddForce((disconnectedTeleportTarget-transform.position).normalized*Time.deltaTime*disconnectedEndSpeed,
			ForceMode.Impulse);
		transform.position = disconnectedTeleportTarget;
		allowSprint = true;
		dashDurationTime = 0;
		dashDurationTimeMax = disconnectedCooldown;
		dashCooldown = dashCooldownMax;
		_myRigidbody.drag = disconnectDragMult*startDrag;

		_isDashing = true;
		_myStats.ChargeCheck(disconnectedChargeCost);
		triggerDashSlideTime = 0f;
			CameraFollowS.F.FasterFollow(1f);
		}
	}


	private void TriggerDash(bool fullDash = false){


		FlashMana();
		resetCountdown = resetTimeMax;
		_myRigidbody.velocity = Vector3.zero;
		canDoAdaptive = true;

		myTracker.TurnOffEffect();

		// first, check for parry, otherwise dodge
		//Debug.Log(superCloseEnemyDetect.allEnemiesInRange.Count + " : " + superCloseEnemyDetect.EnemyToParry() + " : " +(equippedUpgrades.Contains(5)));
		if (superCloseEnemyDetect.EnemyToParry() != null && !_chargingAttack  && !InAttack() 
			&& !_isDashing && !_allowCounterAttack && equippedTech.Contains(5)){

			_myRigidbody.AddForce(ShootDirection().normalized*parryForce*Time.deltaTime, ForceMode.Impulse);
			List<EnemyS> enemiesToParry = superCloseEnemyDetect.EnemyToParry();
			for (int i = 0; i < enemiesToParry.Count; i++){
				enemiesToParry[i].AutoCrit(parryForce*ShootDirection().normalized*Time.deltaTime*parryKnockbackMult, 3f,
					_playerAug.AquaAugAmt());
			}
			if (_playerAug.incensedAug){
				_myStats.ManaCheck(_dodgeCost*_playerAug.incensedStaminaMult);
			}else{
				_myStats.ManaCheck(_dodgeCost);
			}
			CameraShakeS.C.SmallShake();
			CameraShakeS.C.SmallSleep();
			CameraShakeS.C.SloAndPunch(0f, 0.7f, 0.2f);
			DelayWitchTimeActivate(enemiesToParry[0]);
			//shootButtonUp = false;
			PrepParryAnimation();
			_isDashing = false;
			dashDurationTime = dashDurationTimeMax;
			if (_chargingAttack){
			_chargingAttack = false;
				_canSwap = true;
			allowChargeAttack = false;
			_chargeAttackTriggered = false;
				_attackingWeapon = equippedWeapon;
				attackingWeaponAug = EquippedWeaponAug();
			_chargeAttackTime = 0f;
			_myAnimator.SetBool("Charging", false);
			}
		}else{
			if (hitStopped){
				_myAnimator.enabled = true;
				hitStopped = false;
			}
			_myAnimator.ResetTrigger("Evade");
		_myAnimator.SetBool("Evading", true);
		TurnOffBlockAnimation();
		_triggerBlock = false;

			dashCollider.enabled = true;

			_allowCounterAttack = false;
			_delayWitchTime = false;
			parryDelayWitchCountdown = 0f;
			counterAttackTime = 0f;



		if (attackTriggered){
			CancelAttack(true);
		}


		_playerSound.PlayRollSound();

		_allowCounterAttack = false;


		if (_myStats.speedAmt >= 5f){
			myRenderer.enabled = false;
		}

		inputDirection = Vector3.zero;
		inputDirection.x = controller.Horizontal();
		inputDirection.y = controller.Vertical();

		if (inputDirection.x == 0 && inputDirection.y == 0){
			inputDirection = -savedDir;
		}

		if (Mathf.Abs(inputDirection.x) <= 0.6f && inputDirection.y < 0){
			FaceDown();
		}else if (Mathf.Abs(inputDirection.x) <= 0.6f && inputDirection.y > 0){
			FaceUp();
		}else{
			FaceLeftRight();
		}

		
		dashDurationTime = 0;
		
		_myRigidbody.drag = startDrag*dashDragMult;

		gameObject.layer = DODGE_PHYSICS_LAYER;

		if (fullDash){
				if (_playerAug.incensedAug){
					_myStats.ManaCheck(_dodgeCost/2f*_playerAug.incensedStaminaMult);
				}else{
			_myStats.ManaCheck(_dodgeCost/2f);
				}
			_myAnimator.SetTrigger("Dash");
			_myRigidbody.AddForce(inputDirection.normalized*dashSpeed*Time.deltaTime, ForceMode.Impulse);
			dashDurationTimeMax = dashDuration*0.6f;
			_allowDashAttack = true;
		}else{
			_allowDashAttack = true; // change this to false if we dont want roll attacks
				if (_playerAug.incensedAug){
					_myStats.ManaCheck(_dodgeCost*_playerAug.incensedStaminaMult);
				}else{
			_myStats.ManaCheck(_dodgeCost);
				}
			_myAnimator.SetTrigger("Roll");
			_myRigidbody.AddForce(inputDirection.normalized*dashSpeed*0.7f*Time.deltaTime, ForceMode.Impulse);
			dashDurationTimeMax = dashDuration*0.4f;
		}

		triggerDashSlideTime = dashDurationTimeMax*dashSlideTime;

		dashCooldown = dashCooldownMax;
			allowSprint = true;

		if (!_isDashing){
			_isDashing = true;
		}

			if (tutorialRef != null){
				tutorialRef.AddDodge();
			}
		}

		_isSprinting = false;

		SpawnDashPuff();


	}

	private void TriggerSprint(){
		_isSprinting = true;
		allowSprint = false;
		resetCountdown = resetTimeMax;
		_myAnimator.SetBool("Evading", false);
		_isDashing = false;
		_myRigidbody.drag = startDrag*sprintDragMult;

		dashCollider.enabled = false;

		_myRigidbody.AddForce(sprintStartForce*Time.deltaTime*_myRigidbody.velocity.normalized, ForceMode.Impulse);

		sprintNoDrainTime = sprintNoDrainMax;
		
		if (dontGetStuckInEnemiesCheck.NoEnemies()){
			gameObject.layer = START_PHYSICS_LAYER;
		}

		if (tutorialRef != null){
			tutorialRef.AddSprint();
		}
	}

	private void TransformControl(){
		if (myControl.GetCustomInput(9)){
			if ((equippedTech.Contains(8) || _tempTransformAllow) && !disableTransformInScene){ 
			_transformHoldTime += Time.deltaTime;
			transformStartEffect.StartCharge();
			}
		}else{
			if (!_isTransformed){
			if (_transformHoldTime >= _transformRequireHoldTime){
						ActivateTransform();
				}
			}else{
				if (_transformHoldTime >= _revertRequireHoldTime){
						DeactivateTransform();
					}
				}
			if (_transformHoldTime > 0){
			_transformHoldTime = 0f;
			transformStartEffect.TurnOffChargeAnims(true);
			}
		}
	}

	private void ActivateTransform(){
		_isTransformed = true;
		transformStartEffect.ActivateEffect();
		transformActiveEffect.gameObject.SetActive(true);
		SwitchParadigm(currentParadigm);
		_myBuddy.gameObject.SetActive(false);
		CameraEffectsS.E.SetTransformFilter(true);
		Instantiate(transformEcho, transform.position, Quaternion.identity);
		CameraShakeS.C.TimeSleepCustomPunch(0.12f, 0.86f, 0.1f);
		
	}
	public void DeactivateTransform(){
		if (_isTransformed){
			_isTransformed = false;
			transformStartEffect.DeactivateEffect();
		transformActiveEffect.gameObject.SetActive(false);
		_myBuddy.transform.position = buddyPos.position;
		_buddyEffect.ChangeEffect(_myBuddy.shadowColor, _myBuddy.transform);
		_myBuddy.gameObject.SetActive(true);
		SwitchParadigm(currentParadigm);
		CameraEffectsS.E.SetTransformFilter(false);
		CameraShakeS.C.SloAndPunch(0.3f, 0.95f, 0.12f, true, false);
		CameraShakeS.C.SmallShake();
		}
	}

	private void DashControl(){

		//___________________________________________DASH VERSION WITHOUT SPRINTING
		//control for first dash

		if (!_isDashing){
			dashCooldown -= Time.deltaTime;
			if (myControl.GetCustomInput(4) && dashButtonUp && CanInputDash() && StaminaCheck(1f, false)){

				TriggerDash();
				dashButtonUp = false;
			}
		}
			
		
		else{
			
			// allow for second dash
			if (controller.GetCustomInput(4)){
				if (dashButtonUp && (dashDurationTime < TELEPORT_INPUT_THRESHOLD) && CanInputDash(true) 
					&& _myStats.ChargeCheck(disconnectedChargeCost, false) && _playerAug.disconnectedAug){
					DisconnectedTeleport();
				}
				else if (dashButtonUp && ((dashDurationTime >= dashDurationTimeMax-CHAIN_DASH_THRESHOLD) 
				                     && CanInputDash())){
                    if ((Mathf.Abs(controller.Horizontal()) > 0.1f || Mathf.Abs(controller.Vertical()) > 0.1f)){
						TriggerDash();
						_dashStickReset = false;
					}
				}
				dashButtonUp = false;
			}else{
				dashButtonUp = true;
				allowSprint = false;
			}
			
			
			dashDurationTime += Time.deltaTime;
			if (_doingCounterAttack){
				dashDurationTime = dashDurationTimeMax;
			}
			if (dashDurationTime >= dashDurationTimeMax-triggerDashSlideTime){
				_myRigidbody.drag = startDrag*dashDragSlideMult;
			}

			if (dashDurationTime > ALLOW_DASHATTACK_TIME){
				//_triggerBlock = true;
				_allowDashAttack = false;
			}
			
			if ((!chargingAttack && dashDurationTime >= dashDurationTimeMax) ||
			    (!chargingAttack && dashDurationTime >= dashDurationTimeMax*dashSprintAllowMult 
			 && controller.GetCustomInput(4) && !dashButtonUp) ||
			    (chargingAttack && !_chargeAttackTriggered && dashDurationTime >= dashDurationTimeMax*dashChargeAllowMult) ||
			    (chargingAttack && _chargeAttackTriggered && dashDurationTime >= dashDurationTimeMax) ||
			    (controller.GetCustomInput(4) && dashDurationTime >= dashDurationTimeMax*dashChargeAllowMult)
				|| (_attackBuffered && dashDurationTime >= dashDurationTimeMax*attackBufferAllowMult)){
				
				_myAnimator.SetBool("Evading", false);
				_isDashing = false;
				dashCollider.enabled = false;
				_myRigidbody.drag = startDrag;

				if (dontGetStuckInEnemiesCheck.NoEnemies() && !PlayerSlowTimeS.witchTimeActive){
					gameObject.layer = START_PHYSICS_LAYER;
				}
				
				if (!myRenderer.enabled){
					myRenderer.enabled = true;
				}

				if (_chargingAttack){
					if (!_chargeAttackTriggered){
						_chargeAttackTime = 0f;
						ChargeAnimationTrigger(true);
					}else{
						_chargingAttack = false;
						_canSwap = true;
						_myAnimator.SetBool("Charging", false);
						_chargeAttackTriggered = false;
						_isShooting = false;
					}
				}else if (controller.GetCustomInput(4) && !dashButtonUp && !_isSprinting && SprintMoveCondition() && allowSprint){
					TriggerSprint();
				}
			}
		}

		if (_isTalking){
			dashButtonUp = false;
			_triggerBlock = false;
			_isSprinting = false;
			_myRigidbody.drag = startDrag;
		}else{
			if (!myControl.GetCustomInput(4)){
				dashButtonUp = true;
				_triggerBlock = false;
				_dashStickReset = true;
				_isSprinting = false;
				_myRigidbody.drag = startDrag;
			}else if (_isSprinting){
				if (sprintNoDrainTime > 0){
					sprintNoDrainTime -= Time.deltaTime;
				}
				else if (!_myStats.ManaCheck(sprintStaminaRate*GetIncensedSprintMult()*Time.deltaTime)){
					_isSprinting = false;
					_myRigidbody.drag = startDrag;
				}
			}
			if (!_dashStickReset){
				if (Mathf.Abs(controller.Horizontal()) <= 0.1f && Mathf.Abs(controller.Vertical()) <= 0.1f){
					_dashStickReset = true;
				}
			}
		}

	}

	public void CloseCallCheck(EnemyS attemptedSource){
		if (AllowDodgeEffect() && attemptedSource != null){
			WitchTime(attemptedSource);
		}
	}

	private void AttackControl(){


        if ((!_isTalking || (_isTalking && _tempTauntAllow))&&!_isBlocking && !_isDashing && !_chargingAttack && !InAttack()){
			comboDuration -= Time.deltaTime*0.6f;
			if (comboDuration <= 0 && currentChain != -1){
				currentChain = -1;
				enemiesHitByLastAttack.Clear();
				_playerAug.ResetParanoidMult();
			}
		}

		if (_chargingAttack && (ShootInputPressed() || _chargeAttackTriggered)){
			if (!_isDashing){
				_chargeAttackTime+= Time.deltaTime;
				if (preAttackSlowdown && ((!speedUpChargeAttack && _chargeAttackTime >= _chargeAttackTrigger-preAttackSlowTime) ||
					(speedUpChargeAttack && _chargeAttackTime >= dashChargeAllowMult*_chargeAttackTrigger-preAttackSlowTime))){
					CameraShakeS.C.SloAndPunch(preAttackSlowTime, preAttackPunchMult, preAttackHangTime, true, preAttackExtraSlow);
					preAttackSlowdown = false;
					Debug.Log("Charge attack slowdown!");
				}
			}
			if (!_chargeAttackTriggered && ((!speedUpChargeAttack && _chargeAttackTime >= _chargeAttackTrigger) ||
			                                (speedUpChargeAttack && _chargeAttackTime >= dashChargeAllowMult*_chargeAttackTrigger))){
				_chargeAttackTriggered = true;
				//_chargeCollider.TriggerAttack(transform.position, ShootDirection());

				TurnOffProjectileAnimStop();
				GameObject newCharge = Instantiate(_chargePrefab, transform.position, Quaternion.identity)
					as GameObject;
				newCharge.GetComponent<ProjectileS>().Fire(false,
				                                           ShootDirection(), ShootDirection(), this);
				
				SpawnAttackPuff();
				canDoAdaptive = true;

				_myStats.ManaCheck(_chargeAttackCost*VirtueStaminaMult(), !_playerAug.fosAug);

				if (_chargeAttackUseAll){
					_myStats.ChargeCheck(9999f);
				}else{
				_myStats.ChargeCheck(_chargeAttackCost);
				}

				_playerSound.PlayChargeSound();

				//_specialFlash.Flash();
			}
			if (_chargeAttackTime >= _chargeAttackDuration){
				_chargingAttack = false;
				_canSwap = true;
				_chargeAttackTriggered = false;
				_myAnimator.SetBool("Charging", false);
			}

		}
		if (_chargingAttack && !ShootInputPressed() && !_chargeAttackTriggered){
			_chargingAttack = false;
			_canSwap = true;
			_myAnimator.SetBool("Charging", false);
			shootButtonUp = false;
			allowChargeAttack = true;
		}

		if (_attackBuffered && CanInputShoot() && !_isDashing && !counterQueued && !_delayWitchTime){

			_attackBuffered = false;
			// started buffered attack
			attackDelay = currentAttackS.delayShotTime*(1f-PlayerAugmentsS.addSpeedPerBios*activeBios)
				*TransformedAttackSpeedMult();

			if (_playerAug.animaAug){
				attackDelay*=_playerAug.AnimaAugAmt();
			}

			// add slow effect (melee attack)
			if (currentAttackS.slowTime > 0){
				preAttackSlowdown = true;
				preAttackSlowTime = currentAttackS.slowTime;
				preAttackPunchMult = currentAttackS.punchMult;
				preAttackHangTime = currentAttackS.hangTime;
				preAttackExtraSlow = currentAttackS.extraSlow;
			}else{
				preAttackSlowdown = false;
			}

			_attackingWeapon = equippedWeapon;
			attackingWeaponAug = EquippedWeaponAug();


				currentAttackS.StartKnockback(this, ShootDirection());
				_attackStartDirection = ShootDirection();
				if (_isTransformed){
					equippedWeapon.AttackFlash(transform.position, ShootDirection(), transform, attackDelay, transformedColor);
				}else{
					if (_doingHeavyAttack){
					attackingWeaponAug.AttackFlash(transform.position, ShootDirection(), transform, attackDelay, attackingWeaponAug.swapColor);
					}else{
						_attackingWeapon.AttackFlash(transform.position, ShootDirection(), transform, attackDelay, _attackingWeapon.swapColor);
					}
				}

			attackTriggered = true;
			TriggerFosAttack();
			canDoAdaptive = false;

			if (_isTransformed){
				myTracker.FireEffect(ShootDirection(), transformedColor, attackDelay, Vector3.zero);
			}else{
				if (_doingHeavyAttack){
					myTracker.FireEffect(ShootDirection(), _attackingWeapon.swapColor, attackDelay, Vector3.zero);
				}else{
					myTracker.FireEffect(ShootDirection(), _attackingWeapon.swapColor, attackDelay, Vector3.zero);
				}
			}
			//weaponTriggered = equippedWeapon;
			_isShooting = true;
			if (currentAttackS.chargeAttackTime <=  0){
				allowChargeAttack = true;
			}
			else{
				allowChargeAttack = false;
			}

			AttackAnimationTrigger(_doingHeavyAttack);

			if (tutorialRef != null){
				if (_doingHeavyAttack){
					tutorialRef.AddHeavyAttack();
				}else{
					tutorialRef.AddLightAttack();
				}
			}
		}

        if (!hitStopped || _tempTauntAllow){
		attackDelay -= Time.deltaTime;
		}
		if (preAttackSlowdown && attackDelay <= preAttackSlowTime){
			CameraShakeS.C.SloAndPunch(preAttackSlowTime, preAttackPunchMult, preAttackHangTime, true, preAttackExtraSlow);
			preAttackSlowdown = false;
		}
		allowParryCountdown -= Time.deltaTime;
		// check if parry conditions are met before attack fires off
		/*if (superCloseEnemyDetect.EnemyToParry() != null && !_allowCounterAttack && equippedUpgrades.Contains(5)
			&& !_doingCounterAttack && attackDelay > 0f && allowParryCountdown > 0f && controller.HeavyButton()){
			List<EnemyS> enemiesToParry = superCloseEnemyDetect.EnemyToParry();
			for (int i = 0; i < enemiesToParry.Count; i++){
				enemiesToParry[i].AutoCrit(enemiesToParry[i].myRigidbody.velocity.normalized*-2f, 3f);
			}
			CameraShakeS.C.SmallShake();
			//CameraShakeS.C.SmallSleep();
			DelayWitchTimeActivate(enemiesToParry[0]);
			shootButtonUp = false;
			CancelAttack();
			PrepParryAnimation();
		}**/

		if (attackDelay <= 0 && attackTriggered){

			GameObject newProjectile;

			if (queuedAttacks.Count > 0){
				if (queuedAttacks[0].GetComponent<ProjectileS>().momsEye){
					momsEyeMult*=-1f;
				}
				newAttack = false;
				if (_projectilePool.ContainsProjectileID(currentAttackS.projectileID)){
					newProjectile = _projectilePool.GetProjectile(currentAttackS.projectileID,
						transform.position, Quaternion.identity).gameObject;
				}else{
					newProjectile = Instantiate(queuedAttacks[0], transform.position, Quaternion.identity)
						as GameObject;
				}
				queuedAttacks.RemoveAt(0);
			}
			else{
				newAttack = true;
				_playerAug.AddToParanoidMult();
				momsEyeMult = 1f;
				if (_doingCounterAttack){
					if (_doingHeavyAttack){
						if (_projectilePool.ContainsProjectileID
							(currentAttackS.projectileID)){
							newProjectile = _projectilePool.GetProjectile(currentAttackS.projectileID,
								transform.position, Quaternion.identity).gameObject;
						}else{
							newProjectile = (GameObject)Instantiate(attackingWeaponAug.counterAttackHeavy, 
						                                        transform.position, 
						                                        Quaternion.identity);
						}
					}else{
						if (_projectilePool.ContainsProjectileID
							(currentAttackS.projectileID)){
							newProjectile = _projectilePool.GetProjectile(currentAttackS.projectileID,
								transform.position, Quaternion.identity).gameObject;
						}else{
							newProjectile = (GameObject)Instantiate(_attackingWeapon.counterAttack, 
					                                        transform.position, 
					                                        Quaternion.identity);
						}
					}
				}
			else if (_doingDashAttack){
					if (_projectilePool.ContainsProjectileID
						(currentAttackS.projectileID)){
						newProjectile = _projectilePool.GetProjectile(currentAttackS.projectileID,
							transform.position, Quaternion.identity).gameObject;
					}else{
						if (_doingHeavyAttack){
							newProjectile = (GameObject)Instantiate(attackingWeaponAug.dashAttack, 
								transform.position, 
								Quaternion.identity);
						}else{
							newProjectile = (GameObject)Instantiate(_attackingWeapon.dashAttack, 
				                                        transform.position, 
				                                        Quaternion.identity);
						}
					}
			}else{

					if (_doingHeavyAttack){
						if (allowChainHeavy){
							currentChain++;
						}else{
							currentChain = 0;
						}
						if (currentChain > attackingWeaponAug.heavyChain.Length-1){
							currentChain = 0;
						}

						// Opportunistic effect
						if (_playerAug.opportunisticAug && staggerBonusTime > 0){
							currentChain = attackingWeaponAug.heavyChain.Length-1;
						}

						if (_projectilePool.ContainsProjectileID
							(currentAttackS.projectileID)){
							newProjectile = _projectilePool.GetProjectile(currentAttackS.projectileID,
								transform.position,Quaternion.identity).gameObject;
						}else{
							newProjectile = (GameObject)Instantiate(attackingWeaponAug.heavyChain[currentChain], 
						                                        transform.position, 
						                                        Quaternion.identity);
						}
						prevChain = currentChain;
					}
					else{
						if (!currentAttackS.isTaunt){
						if (allowChainLight){
							currentChain++;
						}else{
							currentChain = 0;
						}
						if (currentChain > _attackingWeapon.attackChain.Length-1){
						currentChain = 0;
					}
						}

						// Opportunistic effect
						//if (_playerAug.opportunisticAug && staggerBonusTime > 0){
					//		currentChain = _attackingWeapon.attackChain.Length-1;
					//	}

						if (_projectilePool.ContainsProjectileID
							(currentAttackS.projectileID)){
							newProjectile = _projectilePool.GetProjectile(currentAttackS.projectileID,
								transform.position, Quaternion.identity).gameObject;
						}else{
							if (currentAttackS.isTaunt){
								newProjectile = (GameObject)Instantiate(tauntPrefab, 
									transform.position, 
									Quaternion.identity);
							}else{
							newProjectile = (GameObject)Instantiate(_attackingWeapon.attackChain[currentChain], 
								transform.position, 
								Quaternion.identity);
							}
						}
						if (!currentAttackS.isTaunt){
						prevChain = currentChain;
						}
					}
				
			}
			}


			comboDuration = currentAttackS.comboDuration;
			adaptComboCutoff = comboDuration-adaptPostRecover;

			currentAttackS = newProjectile.GetComponent<ProjectileS>();

			if (_playerAug.thanaAug){
				currentAttackS.dmg *= PlayerAugmentsS.thanaAugAmt;
				if (_playerAug.doubleMantra){
					currentAttackS.dmg *= 1.2f;
				}
			}

			if (newAttack && currentAttackS.numAttacks > 1){
				for (int i = 0; i < currentAttackS.numAttacks - 1; i++){
					if (_doingCounterAttack){
						if (_doingHeavyAttack){
							queuedAttacks.Add(attackingWeaponAug.counterAttackHeavy);
						}else{
							queuedAttacks.Add(_attackingWeapon.counterAttack);
						}
						if (_playerAug.animaAug){
							queuedAttackDelays.Add(currentAttackS.timeBetweenAttacks*_playerAug.AnimaAugAmt()
								*TransformedAttackSpeedMult()*(1f-PlayerAugmentsS.addSpeedPerBios*activeBios));
						}else{
							queuedAttackDelays.Add(currentAttackS.timeBetweenAttacks
								*TransformedAttackSpeedMult()*(1f-PlayerAugmentsS.addSpeedPerBios*activeBios));
						}
					}
					else if (_doingDashAttack){
						if (_doingHeavyAttack){
							queuedAttacks.Add(attackingWeaponAug.dashAttack);
					}else{
						queuedAttacks.Add(_attackingWeapon.dashAttack);
					}
						if (_playerAug.animaAug){
							queuedAttackDelays.Add(currentAttackS.timeBetweenAttacks*_playerAug.AnimaAugAmt()
								*TransformedAttackSpeedMult()*(1f-PlayerAugmentsS.addSpeedPerBios*activeBios));
						}else{
							queuedAttackDelays.Add(currentAttackS.timeBetweenAttacks
								*TransformedAttackSpeedMult()*(1f-PlayerAugmentsS.addSpeedPerBios*activeBios));
						}
					}else{
						if (_doingHeavyAttack){
							queuedAttacks.Add(attackingWeaponAug.heavyChain[prevChain]);
						}else{
							queuedAttacks.Add(_attackingWeapon.attackChain[currentChain]);
						}
						if (_playerAug.animaAug){
							queuedAttackDelays.Add(currentAttackS.timeBetweenAttacks
								*TransformedAttackSpeedMult()*_playerAug.AnimaAugAmt()*(1f-PlayerAugmentsS.addSpeedPerBios*activeBios));
						}else{
							queuedAttackDelays.Add(currentAttackS.timeBetweenAttacks
								*TransformedAttackSpeedMult()*(1f-PlayerAugmentsS.addSpeedPerBios*activeBios));
						}
					}
				}
			}

			if (_doingCounterAttack && _counterTarget != null){
				savedDir = (_counterTarget.transform.position-transform.position).normalized;
			}

			newProjectile.transform.position += savedDir.normalized*currentAttackS.spawnRange;

			TurnOffProjectileAnimStop();
			if (newAttack){
				allowChainHeavy = currentAttackS.allowChainHeavy;
				allowChainLight = currentAttackS.allowChainLight;
				canDoAdaptive = true;
				if (currentTargetEnemy){
					currentAttackS.Fire(Vector3.SqrMagnitude(currentTargetEnemy.transform.position-transform.position)
						<= ENEMY_TOO_CLOSE_DISTANCE, savedDir*momsEyeMult, savedDir*momsEyeMult, this, true, activeBios);
				}else{
					currentAttackS.Fire(false, savedDir*momsEyeMult, savedDir*momsEyeMult, this, true, activeBios);
				}
			}else{
				if (currentTargetEnemy){
					currentAttackS.Fire(Vector3.SqrMagnitude(currentTargetEnemy.transform.position-transform.position)
						<= ENEMY_TOO_CLOSE_DISTANCE, savedDir*momsEyeMult, savedDir*momsEyeMult, this, true, activeBios);
				}else{
					currentAttackS.Fire(false, savedDir*momsEyeMult, savedDir*momsEyeMult, this, true, activeBios);
				}
			}
			if (_doingHeavyAttack){
				_playerSound.PlayHeavyAttackSound();
			}else{
			_playerSound.PlayLightAttackSound();
			}
			SpawnAttackPuff();
			_isSprinting = false;
			_myRigidbody.drag = startDrag;

			// subtract mana cost
			_myStats.ManaCheck(currentAttackS.staminaCost*VirtueStaminaMult(), newAttack);


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

			//muzzleFlare.Fire(currentAttackS.knockbackTime, savedDir, newProjectile.transform.localScale.x, equippedWeapon.swapColor);

			if (queuedAttackDelays.Count > 0){
				attackDelay = queuedAttackDelays[0];
				queuedAttackDelays.RemoveAt(0);
			}else{
				attackTriggered = false;
				_attackingWeapon = equippedWeapon;
				attackingWeaponAug = EquippedWeaponAug();
			}
		}
		else{
			if (!hitStopped){
			attackDuration -= Time.deltaTime;
			}

		if (CanInputShoot()){
				if (
                    ((ShootInputPressed() && shootButtonUp && !_tempTauntAllow) || (tauntButtonUp && controller.GetCustomInput(8) &&
						equippedTech.Contains(9))) 
					&& !counterQueued && !_delayWitchTime
					&& (StaminaCheck(1f, false))
					|| ((counterQueued || heavyCounterQueued) && _dodgeEffectRef.AllowAttackTime())
				){

					// demo reset count
					resetCountdown = resetTimeMax;

					if (_allowCounterAttack && !_dodgeEffectRef.AllowAttackTime() && ShootInputPressed()){
						if (controller.GetCustomInput(1)){
							heavyCounterQueued = true;
						}
						counterQueued = true;
						_allowCounterAttack = false;
						shootButtonUp = false;
						_allowDashAttack = false;
					}
					else{

						if (ShootInputPressed()){
				shootButtonUp = false;
						}
					_doingDashAttack = false;
					_doingHeavyAttack = false;
					_doingCounterAttack = false;

					attackEffectRef.EndAttackEffect();

						if ((counterQueued || _allowCounterAttack) && ShootInputPressed()){

							if ((counterQueued && heavyCounterQueued) || (controller.GetCustomInput(1) && !counterQueued)){
							_doingHeavyAttack = true;
								currentAttackS = attackingWeaponAug.counterAttackHeavy.GetComponent<ProjectileS>();
						}else{
							currentAttackS = _attackingWeapon.counterAttack.GetComponent<ProjectileS>();
						}
						_doingCounterAttack = true;
						_allowCounterAttack = false;
						_allowDashAttack = false;

							_myRigidbody.drag = startDrag;
						heavyCounterQueued = false;
						counterQueued = false;

						counterAttackTime = 0f;
							if (_blockRef.doingParry){
								_blockRef.DoFlash();
							}
						CameraShakeS.C.CancelSloMo();

					}
						else if ((_isDashing || _isSprinting) && ShootInputPressed()){

							if (_allowDashAttack || _isSprinting){
							if (controller.GetCustomInput(1)){
								_doingHeavyAttack = true;
								currentAttackS = attackingWeaponAug.dashAttack.GetComponent<ProjectileS>();
							}else{
						currentAttackS = _attackingWeapon.dashAttack.GetComponent<ProjectileS>();
							}

						//_isSprinting = false;
						_doingDashAttack = true;

							
							_myAnimator.SetBool("Evading", false);
							_isDashing = false;
							dashCollider.enabled = false;
							_myRigidbody.drag = startDrag;

							allowParryCountdown = allowParryInput;
							}else if (!_attackBuffered){

								// input buffer
								_attackBuffered = true;

								int nextAttack = currentChain+1;
								if (myControl.GetCustomInput(1)){
									if (!allowChainHeavy){
										nextAttack = 0;
									}
									if (nextAttack > attackingWeaponAug.heavyChain.Length-1){
										nextAttack = 0;
									}
									// Opportunistic effect
									if (_playerAug.opportunisticAug && staggerBonusTime > 0){
										nextAttack = attackingWeaponAug.heavyChain.Length-1;
									}
									currentAttackS = attackingWeaponAug.heavyChain[nextAttack].GetComponent<ProjectileS>();
									_doingHeavyAttack = true;
								}else{
									if (!allowChainLight){
										nextAttack = 0;
									}
									if (nextAttack > _attackingWeapon.attackChain.Length-1){
										nextAttack = 0;
									}

									currentAttackS = _attackingWeapon.attackChain[nextAttack].GetComponent<ProjectileS>();
								}

								allowParryCountdown = allowParryInput;


							}


					}else{
							if (myControl.GetCustomInput(8)){
                                // trigger taunt!
								currentAttackS = tauntPrefab.GetComponent<ProjectileS>();
								tauntButtonUp = false;
							}else{
						int nextAttack = currentChain+1;
							if (myControl.GetCustomInput(1)){
								if (!allowChainHeavy){
									nextAttack = 0;
								}
								if (nextAttack > attackingWeaponAug.heavyChain.Length-1){
								nextAttack = 0;
							}
							// Opportunistic effect
							if (_playerAug.opportunisticAug && staggerBonusTime > 0){
									nextAttack = attackingWeaponAug.heavyChain.Length-1;
							}
								currentAttackS = attackingWeaponAug.heavyChain[nextAttack].GetComponent<ProjectileS>();
							_doingHeavyAttack = true;
						}else{
								if (!allowChainLight){
									nextAttack = 0;
								}
								if (nextAttack > _attackingWeapon.attackChain.Length-1){
								nextAttack = 0;
							}
							// Opportunistic effect
							if (_playerAug.opportunisticAug && staggerBonusTime > 0){
									nextAttack = _attackingWeapon.attackChain.Length-1;
							}
							currentAttackS = _attackingWeapon.attackChain[nextAttack].GetComponent<ProjectileS>();
						}
							
							allowParryCountdown = allowParryInput;
							}

					}
					
						if (!_attackBuffered){
						attackDelay = currentAttackS.delayShotTime*(1f-PlayerAugmentsS.addSpeedPerBios*activeBios)
							*TransformedAttackSpeedMult();

					if (_playerAug.animaAug){
							attackDelay*=_playerAug.AnimaAugAmt();
					}

						// add slow effect (melee attack)
						if (currentAttackS.slowTime > 0){
							preAttackSlowdown = true;
							preAttackSlowTime = currentAttackS.slowTime;
							preAttackPunchMult = currentAttackS.punchMult;
							preAttackHangTime = currentAttackS.hangTime;
							preAttackExtraSlow = currentAttackS.extraSlow;
						}else{
							preAttackSlowdown = false;
						}

						_attackingWeapon = equippedWeapon;
						attackingWeaponAug = EquippedWeaponAug();

					if (_doingCounterAttack && _counterTarget != null){
						Vector3 targetDir = (_counterTarget.transform.position-transform.position).normalized;
							_attackStartDirection = targetDir;
						currentAttackS.StartKnockback(this, targetDir);
							if (_isTransformed){
								equippedWeapon.AttackFlash(transform.position, targetDir, transform, attackDelay, transformedColor);
							}else{
								if (_doingHeavyAttack){
										attackingWeaponAug.AttackFlash(transform.position, targetDir, transform, attackDelay, attackingWeaponAug.swapColor);
								}else{
									_attackingWeapon.AttackFlash(transform.position, targetDir, transform, attackDelay, _attackingWeapon.swapColor);
								}
							}
					}else{
							currentAttackS.StartKnockback(this, ShootDirection());
							_attackStartDirection = ShootDirection();
							if (_isTransformed){
								equippedWeapon.AttackFlash(transform.position, ShootDirection(), transform, attackDelay, transformedColor);
							}else{
								if (_doingHeavyAttack){
									attackingWeaponAug.AttackFlash(transform.position, ShootDirection(), transform, attackDelay, attackingWeaponAug.swapColor);
								}else{
									_attackingWeapon.AttackFlash(transform.position, ShootDirection(), transform, attackDelay, _attackingWeapon.swapColor);
								}
							}
					}
						attackTriggered = true;
						TriggerFosAttack();
						canDoAdaptive = false;

							if (!myControl.GetCustomInput(8)){
						if (_isTransformed){
							myTracker.FireEffect(ShootDirection(), transformedColor, attackDelay, Vector3.zero);
						}else{
							if (_doingHeavyAttack){
								myTracker.FireEffect(ShootDirection(), _attackingWeapon.swapColor, attackDelay, Vector3.zero);
							}else{
							myTracker.FireEffect(ShootDirection(), _attackingWeapon.swapColor, attackDelay, Vector3.zero);
							}
						}
							}
						//weaponTriggered = equippedWeapon;


					_isShooting = true;
					if (currentAttackS.chargeAttackTime <=  0){
						allowChargeAttack = true;
					}
					else{
						allowChargeAttack = false;
					}

							AttackAnimationTrigger(_doingHeavyAttack, currentAttackS.isTaunt);

							if (tutorialRef != null && !myControl.GetCustomInput(8)){
							if (_doingHeavyAttack){
								tutorialRef.AddHeavyAttack();
							}else{
								tutorialRef.AddLightAttack();
							}
						}
						}
				}
			
				}else if (ShootInputPressed() && !shootButtonUp && allowChargeAttack){
					if (_myStats.ManaCheck(1, false) && _myStats.ChargeCheck(1, false) && equippedTech.Contains(6)){
					// charge attack

						if (prevChain < 0){
							prevChain = 0;
						}
						_attackingWeapon = EquippedWeapon();
						attackingWeaponAug = EquippedWeaponAug();

						ProjectileS chargeAttackRef;
						if (_doingHeavyAttack){
							if (prevChain > attackingWeaponAug.heavyChain.Length-1){
								prevChain =attackingWeaponAug.heavyChain.Length-1;
							}
							chargeAttackRef = 
								_attackingWeapon.heavyChain[prevChain].GetComponent<ProjectileS>()
									.chargeAttackPrefab.GetComponent<ProjectileS>();
						}else{
							if (prevChain > _attackingWeapon.attackChain.Length-1){
								prevChain = _attackingWeapon.attackChain.Length-1;
							}
							chargeAttackRef = 
								_attackingWeapon.attackChain[prevChain].GetComponent<ProjectileS>()
									.chargeAttackPrefab.GetComponent<ProjectileS>();
						}
						ChargeAttackSet(chargeAttackRef.gameObject, 
						                chargeAttackRef.chargeAttackTime, 
						                chargeAttackRef.staminaCost, 
						                (chargeAttackRef.chargeAttackTime+chargeAttackRef.knockbackTime),
						                chargeAttackRef.animationSpeedMult, chargeAttackRef.attackAnimationTrigger,
							chargeAttackRef.useAllCharge);

					_chargingAttack = true;
						_canSwap = false;
						_doingCounterAttack = false;
						//EquippedWeapon().AttackFlash(transform.position, ShootDirection(), transform, _chargeAttackTrigger,1);
					_chargeAttackTriggered = false;
					_chargeAttackTime = 0;

						_myAnimator.SetInteger("WeaponNumber",_attackingWeapon.weaponNum);
						ChargeAnimationTrigger();
					allowChargeAttack = false;
						// add slow effect (chargeattack
						if (chargeAttackRef.slowTime > 0){
							//Debug.Log("Charge slowdown set!");
							preAttackSlowdown = true;
							preAttackSlowTime = chargeAttackRef.slowTime;
							preAttackPunchMult = chargeAttackRef.punchMult;
							preAttackHangTime = chargeAttackRef.hangTime;
							preAttackExtraSlow = chargeAttackRef.extraSlow;
						}else{
							preAttackSlowdown = false;
						}

						_attackingWeapon = equippedWeapon;
						attackingWeaponAug = EquippedWeaponAug();
					}else{
						allowChargeAttack = false;
					}
				}
			else{if (attackDuration <= 0){
				_isShooting = false;

						if (_doingDashAttack){
						_doingDashAttack = false;
						}
						if (_doingCounterAttack){
							_doingCounterAttack = false;
						}
						TurnOffAttackAnimation();
						
					}

					if (_chargingAttack && (_chargeAttackTime < _chargeAttackTrigger 
					                        || _chargeAttackTime > _chargeAttackDuration)){
						_chargingAttack = false;
						_canSwap = true;
						_chargeAttackTime = 0;
						_myAnimator.SetBool("Charging", false);
					}
			}}
		}

	}

	private void SwapControl(){

		if (!myControl.GetCustomInput(5)){
			switchButtonUp = true;
		}

		if (!myStats.PlayerIsDead() && SubWeapon() != null && _canSwap && !isNatalie){
		
            if (switchButtonUp && (_myBuddy.canSwitch || killedFamiliar)){
				if (myControl.GetCustomInput(5)){

					resetCountdown = resetTimeMax;
					_currentParadigm++;
					if (_currentParadigm > equippedWeapons.Count-1){
						_currentParadigm = 0;
						_subParadigm = 1;
					}else{
						_subParadigm = 0;
					}
					SwitchParadigm(_currentParadigm);

					AdaptiveCheck();

                    if (!killedFamiliar)
                    {
                        BuddyS tempSwap = _myBuddy;
                        if (!altBuddyCreated)
                        {
                            altBuddyCreated = true;
                            if (equippedBuddies.Count < 2)
                            {
                                equippedBuddies.Add(equippedBuddies[0]);
                            }
                            GameObject newBuddy = Instantiate(equippedBuddies[_currentParadigm],
                                                              transform.position, Quaternion.identity)
                                as GameObject;
                            newBuddy.transform.parent = transform;
                            _altBuddy = newBuddy.GetComponent<BuddyS>();
                        }
                        _altBuddy.SetPositions(buddyPos, buddyPosLower);
                        _myBuddy = _altBuddy;
                        _myBuddy.transform.position = tempSwap.transform.position;
                        if (!InGameCinematicS.turnOffBuddies && !isNatalie)
                        {
                            _myBuddy.gameObject.SetActive(true);
                            Instantiate(_myBuddy.buddySound);
                        }
                        _altBuddy = tempSwap;
                        _altBuddy.gameObject.SetActive(false);
                        if (_isTransformed)
                        {
                            _myBuddy.gameObject.SetActive(false);
                        }
                        else
                        {

                            _buddyEffect.ChangeEffect(_myBuddy.shadowColor, _myBuddy.transform);
                        }
                    }

					_playerAug.RefreshAll();

					if (tutorialRef != null){
						tutorialRef.AddShift();
					}
					switchButtonUp = false;
					_myStats.SetMinChargeUse(_myBuddy.costPerUse, _myBuddy.useAllCharge);

					mantraText.ShowMantraText(EquippedWeapon().weaponName);

					if (!attackTriggered){
						_attackingWeapon = equippedWeapon;
						attackingWeaponAug = EquippedWeaponAug();
					}
	
				}
			}
		}

		if (myControl.GetCustomInput(5)){
			switchButtonUp = false;
		}

	}

	private void AdaptiveCheck(){
		if (_isDashing){
			if (_playerAug.adaptiveAug && canDoAdaptive && ((dashDurationTimeMax - dashDurationTime) <= ADAPTIVE_WINDOW
				|| (dashCooldown > 0 && !_isDashing))){
				_myStats.ResetStamina(true, true, 0.8f);
			}
		}
		else if (_playerAug.adaptiveAug && (_isShooting || comboDuration > adaptComboCutoff) 
			&& (!_chargingAttack || (_chargingAttack && _chargeAttackTriggered)) 
			&& canDoAdaptive && attackDuration <=  ADAPTIVE_WINDOW+currentAttackS.chainAllow){
				_myStats.ResetStamina(true, true, 0.8f);
			}
		
			canDoAdaptive = false;

	}

	private void LockOnControl(){


			if (_myLockOn.lockedOn){

			if (!_lockButtonDown && myControl.LockOnButton()){
				_lockButtonDown = true;
				_myLockOn.EndLockOn();
				lockInputReset = false;
				
			}
				if (myDetect.allEnemiesInRange.Count > 0){
					

					// allow change of target
					if (myDetect.allEnemiesInRange.Count > 1 && lockInputReset){
					if ((controller.ControllerAttached() && (myControl.RightHorizontal() > 0.1f || myControl.RightVertical() > 0.1f))
					    || (!controller.ControllerAttached() && myControl.ChangeLockTargetKeyRight())){
						int currentLockedEnemy = myDetect.allEnemiesInRange.IndexOf(_myLockOn.myEnemy);
						currentLockedEnemy++;
						if (currentLockedEnemy > myDetect.allEnemiesInRange.Count-1){
							currentLockedEnemy = 0;
						}
						_myLockOn.LockOn(myDetect.allEnemiesInRange[currentLockedEnemy]);
							lockInputReset = false;
						}
						if ((controller.ControllerAttached() && (myControl.RightHorizontal() < -0.1f || myControl.RightVertical() < -0.1f))
					    || (!controller.ControllerAttached() && myControl.ChangeLockTargetKeyLeft())){
							int currentLockedEnemy = myDetect.allEnemiesInRange.IndexOf(_myLockOn.myEnemy);
							currentLockedEnemy--;
							if (currentLockedEnemy < 0){
								currentLockedEnemy = myDetect.allEnemiesInRange.Count-1;
							}
							_myLockOn.LockOn(myDetect.allEnemiesInRange[currentLockedEnemy]);
							lockInputReset = false;
						}
					}

					

			}
				
			}else{
				if (!_lockButtonDown && myControl.LockOnButton()){
					_lockButtonDown = true;
					if (myDetect.allEnemiesInRange.Count > 0){
						_myLockOn.LockOn(myDetect.closestEnemy);
						_isSprinting = false;
					_myRigidbody.drag = startDrag;
					}
				}
			}

			if (!myControl.LockOnButton()){
				_lockButtonDown = false;
			}

		if (controller.ControllerAttached()){
			if (Mathf.Abs(myControl.RightVertical()) <= 0.1f && Mathf.Abs(myControl.RightHorizontal()) <= 0.1f){
				lockInputReset = true;
			}
		}else{
			if (!controller.ChangeLockTargetKeyLeft() && !controller.ChangeLockTargetKeyRight()){
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

	public void ChangeParryRange(float newRange = -1, bool newSlowing = false, float newDetectionMult = 1f){
		if (newRange > 0){
			currentParrySize.y = newRange;
			superCloseEnemyDetect.GetComponent<BoxCollider>().size = currentParrySize;
			dontSlowWhenClose = true;
		}else{

			currentParrySize = superCloseEnemyDetect.GetComponent<BoxCollider>().size = parrySizeStart;
			dontSlowWhenClose = false;
		}
		dontSlowWhenClose = newSlowing;

		if (newDetectionMult > 0){
			enemyDetectCollider.size = startDetectSize*newDetectionMult;
		}else{
			enemyDetectCollider.size = startDetectSize;
		}
	}

	private void ChargeAttackSet(GameObject chargePrefab, float chargeTime, float chargeCost, float cDuration,
		float animationSpeed, string animationTrigger, bool useAll = false){
		_chargePrefab = chargePrefab;
		_chargeAttackTrigger = chargeTime*(1f-PlayerAugmentsS.addSpeedPerBios*activeBios)*TransformedAttackSpeedMult();
		_chargeAttackCost = chargeCost;
		_chargeAttackDuration = cDuration*(1f-PlayerAugmentsS.addSpeedPerBios*activeBios)*TransformedAttackSpeedMult();
		_chargeAnimationSpeed = animationSpeed*(1f-PlayerAugmentsS.addSpeedPerBios*activeBios)*TransformedAttackSpeedMult();
		_chargeAttackUseAll = useAll;

		if (_playerAug.animaAug){
			_chargeAttackTrigger *= _playerAug.AnimaAugAmt();
			_chargeAttackDuration *= _playerAug.AnimaAugAmt();
			_chargeAnimationSpeed *= _playerAug.AnimaAugAmt();
		}
	}

	public void ParadigmCheck(){
		SwitchParadigm(_currentParadigm, true);
	}
	public void BuddyLoad(int buddyIndex, GameObject buddyPrefab){

		if (buddyIndex == _subParadigm){
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

	private void SwitchParadigm (int newPara, bool fromMenu = false){

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

		// OLD ADAPTIVE AUG USE
		/*if (_playerAug.adaptiveAug){
			_adaptiveCountdown = _adaptiveTimeMax;
		}*/
		
		// switchWeapon
		equippedWeapon = equippedWeapons[_currentParadigm];
		/*if (currentChain > equippedWeapon.attackChain.Length-1){
			currentChain = -1;
		}**/
		if (currentChain < EquippedWeaponAug().heavyChain.Length && currentChain > -1){
			allowChainHeavy = EquippedWeaponAug().heavyChain[currentChain].GetComponent<ProjectileS>().allowChainHeavy;
		}else{
			allowChainHeavy = false;
		}

		if (currentChain < equippedWeapon.attackChain.Length && currentChain > -1){
			allowChainHeavy = equippedWeapon.attackChain[currentChain].GetComponent<ProjectileS>().allowChainLight;
		}else{
			allowChainLight = false;
		}

		_myAnimator.SetInteger("WeaponNumber",equippedWeapon.weaponNum);
		_myLockOn.SetSprite();
		weaponSwitchIndicator.Flash(equippedWeapon);
		if (!_isTransformed){
		myRenderer.color = equippedWeapon.swapColor;
		}else{
			myRenderer.color = transformedColor;
		}

		if (_currentCombatManager && _inCombat){
			_currentCombatManager.ChangeFeatherCols(equippedWeapon.swapColor);
		}

		if (_blockRef){
			_blockRef.ChangeColors(equippedWeapon.swapColor);
		}

		if (fromMenu){
			_attackingWeapon = equippedWeapon;
			attackingWeaponAug = EquippedWeaponAug();
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
			tauntButtonUp = false;
			allowChargeAttack = false;
		}

		if (!controller.GetCustomInput(0) && !controller.GetCustomInput(1)){
			shootButtonUp = true;
			allowChargeAttack = false;
		}

		if (!controller.GetCustomInput(8)){
			tauntButtonUp = true;
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
			//doWakeUp = true;
			if (gameObject.layer != DODGE_PHYSICS_LAYER){
				gameObject.layer = DODGE_PHYSICS_LAYER;
			}
		}else{
			if (gameObject.layer != START_PHYSICS_LAYER){
				if (!_isDashing && dontGetStuckInEnemiesCheck.NoEnemies()){
					gameObject.layer = START_PHYSICS_LAYER;
				}
			}
		}

		if (wakingUp){
			wakeUpCountdown -= Time.deltaTime;
			if (wakeUpCountdown <= 0){
				if (!InGameCinematicS.inGameCinematic){
					wakingUp = false;
					_isTalking = false;
				}
			}
		}

		if (_isStunned){
			if (!hitStopped){
			stunTime -= Time.deltaTime;
			if (stunTime <= 0){
				_isStunned = false;
			}
			}
		}

		if (!hitStopped){
			ManageBios();
		}

		if (delayAttackAllow > 0){
			delayAttackAllow -= Time.deltaTime;
		}

		if (_adaptiveCountdown > 0){
			_adaptiveCountdown -= Time.deltaTime;
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

		if (embracing){
			embraceOutCount -= Time.deltaTime;
			if (embraceOutCount <= 0){
				embracing = false;
				SetTalking(false);
			}
		}

        if (attackTriggered && _isTalking && !_tempTauntAllow){
			CancelAttack();
		}


	}

	private void DelayWitchTimeActivate(EnemyS targetEnemy){
		_delayWitchTime = true;
		_counterTarget = targetEnemy;
		_counterNormal = (targetEnemy.transform.position-transform.position).normalized;
		parryDelayWitchCountdown = parryDelayWitchTime;
		_dodgeEffectRef.FireEffect(true);
		if (_blockRef){
			_blockRef.ChangeColors(equippedWeapon.swapColor);
		
			_blockRef.FireParryEffect(targetEnemy.transform.position);
		}
		FlashMana();
	}

	private void ManageCounterTimer(){
		if (_delayWitchTime){
			parryDelayWitchCountdown -= Time.deltaTime;
			if (parryDelayWitchCountdown <= 0){
				_delayWitchTime = false;
				WitchTime(_counterTarget, true);
			}
		}
		if (_allowCounterAttack){
			counterAttackTime -= Time.unscaledDeltaTime;
			if (counterAttackTime <= 0){
				_allowCounterAttack = false;
			}
		}
	}

	private void CurrentEnemyCheck(){
		if (currentTargetEnemy != null){
			if (currentTargetEnemy.isDead){
				currentTargetEnemy = null;
			}
		}
	}

	private void ManageFlash(){

		if (flashDamageFrames > 0){
			//if (myRenderer.material != damageFlashMat){
			//	myRenderer.material = damageFlashMat;
			//}
		}
		else if (flashHealFrames > 0){
			//if (myRenderer.material != healFlashMat){
			//	myRenderer.material = healFlashMat;
			//}
		}
		else if (flashManaFrames > 0){
			//if (myRenderer.material != manaFlashMat){
			//	myRenderer.material = manaFlashMat;
			//}

		}else if (flashChargeFrames > 0){
			//if (myRenderer.material != chargeFlashMat){
			//	myRenderer.material = chargeFlashMat;
			//}
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

	public void TriggerWakeUp(){
		_playerSound.SetWalking(false);
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

		if (tutorialRef != null){
			tutorialRef.AddReset();
		}
	}

	public void ResetCombat(){
		if (_currentCombatManager != null){
			_currentCombatManager.Initialize(true);
			//FlashMana();
			CameraPOIS.POI.JumpToPoint(transform.position);
			CameraEffectsS.E.ResetEffect();
			CameraShakeS.C.SmallShakeCustomDuration(0.6f);
			CameraShakeS.C.TimeSleep(0.08f);
			_myStats.ResetCombatStats();
			_isStunned = false;
			hitStopped = false;
			tauntEffect.gameObject.SetActive(false);
			tauntText.SetEffect(EquippedWeapon().swapColor, 1f, false);
		}
	}

	public void ItemHeal(){
				CameraPOIS.POI.JumpToPoint(transform.position);
				CameraEffectsS.E.HealEffect();
				CameraShakeS.C.SmallShakeCustomDuration(0.6f);
				CameraShakeS.C.TimeSleep(0.08f);
				_myStats.ResetCombatStats();

	}

	private void AttackAnimationTrigger(bool heavy = false, bool taunt = false){

		if (heavy){
			if (!_doingDashAttack){
			_myAnimator.SetBool("HeavyAttacking", true);
			}
			_myAnimator.SetInteger("WeaponNumber", attackingWeaponAug.weaponNum);
		}else if (taunt){
			_myAnimator.SetBool("HeavyAttacking", false);
			_myAnimator.SetInteger("WeaponNumber", 0);
		}else{
			_myAnimator.SetBool("HeavyAttacking", false);
			_myAnimator.SetInteger("WeaponNumber", _attackingWeapon.weaponNum);
		}
		_myAnimator.SetTrigger("AttackTrigger");

		if (_playerAug.animaAug){
			_myAnimator.SetFloat("AttackAnimationSpeed", currentAttackS.animationSpeedMult/_playerAug.AnimaAugAmt()/(1f-PlayerAugmentsS.addSpeedPerBios*activeBios)
				/TransformedAttackSpeedMult());
		}else{
			_myAnimator.SetFloat("AttackAnimationSpeed", currentAttackS.animationSpeedMult/(1f-PlayerAugmentsS.addSpeedPerBios*activeBios)
				/TransformedAttackSpeedMult());
		}
		_myAnimator.SetTrigger(currentAttackS.attackAnimationTrigger);
		if (heavy){
			// Debug.Log("Heavy Attack chain " + currentChain + " : " + currentAttackS.attackAnimationTrigger);
		}
		_myAnimator.SetBool("Attacking", true);
		

	}

	private void ChargeAnimationTrigger(bool inDash = false){

		
		_myAnimator.SetBool("Charging", true);
		_myAnimator.SetTrigger("Charge Attack");

		ProjectileS currentProj = _chargePrefab.GetComponent<ProjectileS>();

		_myAnimator.SetTrigger(currentProj.attackAnimationTrigger);

		if (_playerAug.animaAug){
		if (inDash){
			_myAnimator.SetFloat("AttackAnimationSpeed", currentProj.animationSpeedMult/dashChargeAllowMult
					/_playerAug.AnimaAugAmt()/(1f-PlayerAugmentsS.addSpeedPerBios*activeBios)
					/TransformedAttackSpeedMult());
		}else{
			_myAnimator.SetFloat("AttackAnimationSpeed", currentProj.animationSpeedMult
					/_playerAug.AnimaAugAmt()/(1f-PlayerAugmentsS.addSpeedPerBios*activeBios)
					/TransformedAttackSpeedMult());
		}
		}else{
			if (inDash){
				_myAnimator.SetFloat("AttackAnimationSpeed", currentProj.animationSpeedMult/dashChargeAllowMult
					/(1f-PlayerAugmentsS.addSpeedPerBios*activeBios)
					/TransformedAttackSpeedMult());
			}else{
				_myAnimator.SetFloat("AttackAnimationSpeed", currentProj.animationSpeedMult
					/(1f-PlayerAugmentsS.addSpeedPerBios*activeBios)
					/TransformedAttackSpeedMult());
			}
		}
		speedUpChargeAttack = inDash;

	}

	private void PrepBlockAnimation(){
		
		_myAnimator.SetBool("Attacking", false);
		_myAnimator.SetBool("Blocking", true);
		_myAnimator.SetLayerWeight(3, 1f);
		FaceLeftRight();
	}

	private void PrepParryAnimation(){
		_myAnimator.SetBool("Attacking", false);
		_myAnimator.SetTrigger("Parry");
		FaceLeftRight();
	}

	private void TurnOffProjectileAnimStop(){
		for (int i = 0; i < _activeProjectiles.Count; i++){
			_activeProjectiles[i].TurnOffAnimationStop();
		}
	}
	
	private void TurnOffBlockAnimation(){
		_myAnimator.SetLayerWeight(3, 0f);
		_myAnimator.SetBool("Blocking", false);
	}

	private void TurnOffAttackAnimation(){
		_myAnimator.SetBool("Charging", false);
		_myAnimator.SetBool("Attacking", false);
		_myAnimator.SetBool("HeavyAttacking", false);
		if (attackEffectRef){
			attackEffectRef.EndAttackEffect();
		}
		if (_myFace){
			_myFace.AllowFace();
		}
	}

	void TriggerFosAttack(){
		if (fosCharges.Count > 0){
			Vector3 targetDir = Vector3.zero;
			for (int i = 0; i < fosCharges.Count; i++){

				if (currentTargetEnemy){
					targetDir = currentTargetEnemy.transform.position-fosCharges[i].transform.position;
				}else if (_counterTarget){
					targetDir = _counterTarget.transform.position-fosCharges[i].transform.position;
				}else if (enemyDetect.closestEnemyTransform){
					targetDir = enemyDetect.closestEnemyTransform.position-fosCharges[i].transform.position;
				}else{
					targetDir = ShootDirectionAssisted();
				}
				targetDir.z = 0f;
				fosCharges[i].FosPause(targetDir);
			}
			StartCoroutine(FireFosCharges());
		}
	}

	IEnumerator FireFosCharges(){

		yield return new WaitForSeconds(0.2f);
		bool firstFired = true;
		while (fosCharges.Count > 0){
			fosCharges[0].FosDirectedFire(firstFired);
			fosCharges.RemoveAt(0);
			firstFired = false;
			yield return new WaitForSeconds(0.12f);
		}
	}

	public void AddFosCharge(ChargeAttackS newCharge){
		fosCharges.Add(newCharge);
	}
	public void RemoveFosCharge(ChargeAttackS usedCharge){
		if (fosCharges.Contains(usedCharge)){
			fosCharges.Remove(usedCharge);
		}
	}
	public void TriggerAllFos(ChargeAttackS removeCharge){
		fosCharges.Remove(removeCharge);
		for (int i=0; i < fosCharges.Count; i++){
			fosCharges[i].FosEndFire(true);
		}
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

	public void FaceDown(){
		if (!_myAnimator){
			_myAnimator = GetComponentInChildren<Animator>();
		}
		if (!_isBlocking){
		_myAnimator.SetLayerWeight(1, 1f);
		_myAnimator.SetLayerWeight(2, 0f);
		_facingDown = true;
		_facingUp = false;
		}

	}
	private void FaceUp(){
		if (!_myAnimator){
			_myAnimator = GetComponentInChildren<Animator>();
		}
		if (!_isBlocking){
		_myAnimator.SetLayerWeight(2, 1f);
		_facingUp = true;
		_facingDown = false;
		}
		
	}
	private void FaceLeftRight(){
		if (!_myAnimator){
			_myAnimator = GetComponentInChildren<Animator>();
		}
		_myAnimator.SetLayerWeight(1, 0f);
		_myAnimator.SetLayerWeight(2, 0f);
		_facingDown = false;
		_facingUp = false;
	}

	private bool CanInputMovement(){

		if (!_isDashing && !_isStunned && attacksRemaining <= 0 && !attackTriggered 
		    && !_doingCounterAttack && !counterQueued && !_delayWitchTime && !_allowCounterAttack
			&& !doingBlockTrigger && attackDuration <= 0 && !_chargeAttackTriggered 
			&& (!_isTalking || _allowWalk) && !_usingItem){
			return true;
		}
		else{
			return false;
		}

	}

	private bool CanInputDash(bool ignoreCooldown = false){

		bool dashAllow = false;

		/*if (blockPrepMax-blockPrepCountdown+timeInBlock < DASH_THRESHOLD && blockPrepCountdown > 0 &&
		    (controller.Horizontal() != 0 || controller.Vertical() != 0) && !_isDashing
		    && !_isStunned && _myStats.currentDefense > 0 && (!_examining || enemyDetect.closestEnemy)){**/
		
		if (!_isTalking && !_isStunned && !_delayWitchTime && !_usingItem  
			&& (dashCooldown <= CHAIN_DASH_THRESHOLD || ignoreCooldown)){
			// && attackDuration <= currentAttackS.chainAllow
			dashAllow = true;
		}

		return dashAllow;
	}

	private bool CanInputShoot(){

		if (!attackTriggered && !_isStunned // && (!_isDashing || (_isDashing && _allowDashAttack) || (_isDashing && _allowCounterAttack))
			&& !_triggerBlock && attackDuration <= currentAttackS.chainAllow && !_chargingAttack && !_usingItem && !hitStopped && delayAttackAllow <= 0f){
			return true;
		}
		else{
			/*Debug.Log("Shoot check failed!\n" + 
				!attackTriggered + " : " + !_isStunned + " : " + (!_isDashing || (_isDashing && _allowDashAttack) || (_isDashing && _allowCounterAttack))
				+ " : " + !_triggerBlock + " : " + (attackDuration <= currentAttackS.chainAllow) + " : " + !_chargingAttack 
				+ " : " + !_usingItem + " : " + !hitStopped);**/
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

	public void SetAllowItem(bool newI){
		allowItemUse = newI;
	}

	public bool CanUseItem(){
		if (CanInputShoot() && !_isDashing && !_isTalking && Time.timeScale > 0 && allowItemUse){
			return true;
		}else{
			return false;
		}
	}

	public bool CanBeCountered(float allowTime){
		if (attackTriggered && attackDelay <= allowTime){
			return true;
		}else{
			return false;
		}
	}

	private Vector3 ShootDirectionUnlocked(){

		Vector3 InputDirection = Vector3.zero;

			
        InputDirection.x = controller.Horizontal();
        InputDirection.y = controller.Vertical();

		
        return InputDirection.normalized;

	}

	private Vector3 GetMouseDirection(){

		Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = transform.position.z;

		return (mousePosition-transform.position).normalized;

	}

	private Vector3 ShootDirection(bool moveOverride = false){

		Vector3 InputDirection = Vector3.zero;

		// read left analogue input
		//if (Input.GetJoystickNames().Length > 0 || moveOverride){
		if (ControlManagerS.controlProfile == 0 || ControlManagerS.controlProfile == 2 || ControlManagerS.controlProfile == 3){
            InputDirection.x = controller.Horizontal();
            InputDirection.y = controller.Vertical();
		}else {
            InputDirection = GetMouseDirection();
		}

		// first, do lock on
		/*if (_myLockOn.lockedOn){
			inputDirection = (_myLockOn.myEnemy.transform.position-transform.position).normalized;
		}**/
		if (lockOnEnemyDetect.allEnemiesInRange.Count == 1){
			currentTargetEnemy = lockOnEnemyDetect.allEnemiesInRange[0];
            InputDirection = (currentTargetEnemy.transform.position-transform.position).normalized;
		}else if (lockOnEnemyDetect.allEnemiesInRange.Count > 1){
			currentTargetEnemy = lockOnEnemyDetect.ReturnClosestAngleEnemy(inputDirection);
            InputDirection = (currentTargetEnemy
				.transform.position-transform.position).normalized;
		}// if no enemies in lock on detect, attack closest enemy in general detect
		else if (enemyDetect.closestEnemy != null){
			currentTargetEnemy = enemyDetect.closestEnemy;
            InputDirection = (currentTargetEnemy.transform.position-transform.position).normalized;
		}
		// now check 4/8 directional, if applicable
		else if (Mathf.Abs(inputDirection.x) <= 0.04f && Mathf.Abs(inputDirection.y) <= 0.04f){
			if (currentTargetEnemy != null){
				if (!currentTargetEnemy.isDead){
                    InputDirection = (currentTargetEnemy
					                  .transform.position-transform.position).normalized;
				}else{
                    InputDirection = savedDir;
				}
			}else{
                InputDirection = savedDir;
			}
		}
		else if (_shoot8Dir){

			//Debug.Log("8 dir!");

			float directionZ = FindDirectionOfVector(inputDirection.normalized);

			if (directionZ > 348.75f || directionZ <= 11.25f){
                InputDirection.x = 1;
                InputDirection.y = 0;
				FaceLeftRight();
				//Debug.Log("Look 1!");
			}
			else if (directionZ > 11.25f && directionZ <= 33.75f){
                InputDirection.x = 1f;
                InputDirection.y = 0.5f;
				FaceLeftRight();
				//Debug.Log("Look 2!");
			}
			else if (directionZ > 33.75f && directionZ <= 56.25f){
                InputDirection.x = 1;
                InputDirection.y = 1;
				FaceLeftRight();
				//Debug.Log("Look 3!");
			}
			else if (directionZ > 56.25f && directionZ <= 78.75f){
                InputDirection.x = 0.5f;
                InputDirection.y = 1;
				FaceLeftRight();
				//FaceUp();
				//Debug.Log("Look 4!");
			}
			else if (directionZ > 78.75f && directionZ <= 101.25f) {
                InputDirection.x = 0;
                InputDirection.y = 1;
				FaceLeftRight();
				//FaceUp();
				//Debug.Log("Look 5!");
			}
			else if (directionZ > 101.25f && directionZ <= 123.75f) {
                InputDirection.x = -0.5f;
                InputDirection.y = 1;
				FaceLeftRight();
				//Debug.Log("Look 6!");
			}
			else if (directionZ > 123.75f && directionZ <= 146.25f) {
                InputDirection.x = -1;
                InputDirection.y = 1;
				FaceLeftRight();
				//Debug.Log("Look 7!");
			}
			else if (directionZ > 146.25f && directionZ <= 168.75f) {
                InputDirection.x = -1;
                InputDirection.y = 0.5f;
				FaceLeftRight();
				//Debug.Log("Look 8!");
			}
			else if (directionZ > 168.75f && directionZ <= 191.25f) {
                InputDirection.x = -1;
                InputDirection.y = 0;
				FaceLeftRight();
				//Debug.Log("Look 9!");
			}
			else if (directionZ > 191.25f && directionZ <= 213.75f) {
                InputDirection.x = -1;
                InputDirection.y = -0.5f;
				FaceLeftRight();
				//Debug.Log("Look 10!");
			}
			else if (directionZ > 213.75f && directionZ <= 236.25f) {
                InputDirection.x = -1;
                InputDirection.y = -1;
				FaceLeftRight();
				//Debug.Log("Look 11!");
			}
			else if (directionZ > 236.25f && directionZ <= 258.75f){
                InputDirection.x = -0.5f;
                InputDirection.y = -1;
				FaceLeftRight();
				//FaceDown();
				//Debug.Log("Look 12!");
			}
			else if (directionZ > 258.75f && directionZ <= 281.25f)  {
                InputDirection.x = 0;
                InputDirection.y = -1;
				FaceLeftRight();
				//FaceDown();
				//Debug.Log("Look 13!");
			}
			else if (directionZ > 281.25f && directionZ <= 303.75f) {
                InputDirection.x = 0.5f;
                InputDirection.y = -1;
				FaceLeftRight();
				//Debug.Log("Look 14!");
			}
			else if (directionZ > 303.75f && directionZ <= 326.25f) {
                InputDirection.x = 1;
                InputDirection.y = -1;
				FaceLeftRight();
				//Debug.Log("Look 15!");
			}
			else{
                InputDirection.x = 1;
                InputDirection.y = -0.5f;
				FaceLeftRight();
				//Debug.Log("Look 16!");
			}

		}

        savedDir = InputDirection.normalized;
		FaceAttackDirection(savedDir);
        return InputDirection.normalized;
		

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
			//FaceUp();
			//Debug.Log("Look 4!");
		}
		else if (directionZ > 78.75f && directionZ <= 101.25f) {
			//FaceUp();
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
			//FaceDown();
			//Debug.Log("Look 12!");
		}
		else if (directionZ > 258.75f && directionZ <= 281.25f)  {
			//FaceDown();
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
			return (enemyDetect.closestEnemyTransform.position - transform.position).normalized;
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
		return (controller.GetCustomInput(0) || controller.GetCustomInput(1));
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

	public void SetAttackEffectRef(PlayerDashEffectS dRef){
		attackEffectRef = dRef;
	}

	public void SetLockOnIndicator(LockOnS newLock){
		_myLockOn = newLock;
		//_myLockOn.SetSprite();
	}

	public bool InAttack(){
		if (attackTriggered || attackDelay > 0 || _chargingAttack || attackDuration > 0){
			return true;
		}
		else{
			return false;
		}
	}
	public bool InSpecialAttack(){
		if ((attackTriggered || attackDuration > 0) && (_doingHeavyAttack || _doingDashAttack)){
			return true;
		}
		else{
			return false;
		}
	}

	public bool IsSliding(){
		return (_isDashing && dashDurationTime >= dashDurationTimeMax-triggerDashSlideTime);
	}

	public Vector3 ShootPosition(){
		return (ShootDirectionUnlocked());
	}

	public bool IsRunning(){
		return (_myAnimator.GetFloat("Speed") > 0.8f);
	}

	public void SetTutorial(InstructionTrigger newT){
		tutorialRef = newT;
	}

	public void SetCombat(bool combat){
		_inCombat = combat;
		if (combat = false){
			_currentCombatManager = null;
			CancelAttack();
		}
	}

	public void SetCombatManager(CombatManagerS m){
		_currentCombatManager = m;
		_myStats.SaveStats();
	}

	public void SetExamining(bool nEx, Vector3 newExaminePos, string newExString = ""){

		_overrideExamineString = newExString;
		if (newExaminePos != Vector3.zero){
		_examineStringPos = newExaminePos;
		}
		
		_examining = nEx;
	}

	public void SetTalking(bool nEx, bool allowWalk = false, bool allowTransform = false, bool tauntTutorial = false){

		if (_isTalking && !nEx){
			delayAttackAllow = 0.2f;
		}
		_isTalking = nEx;
		_tempTransformAllow = allowTransform;
        _tempTauntAllow = tauntTutorial;
        if (_tempTauntAllow){
            PlayerInventoryS.I.AddEarnedTech(9);
        }
		_allowWalk = allowWalk;
		if (_isTalking){
			DeactivateTransform();
			if (!_myRigidbody){
				_myRigidbody = GetComponent<Rigidbody>();
			}
			_myRigidbody.velocity = Vector3.zero;
			if (!_myAnimator){
				_myAnimator = GetComponentInChildren<Animator>();
			}
			_myAnimator.SetFloat("Speed", 0f);
			_myAnimator.SetBool("Attacking", false);
			if (!_playerSound){
				_playerSound = GetComponent<PlayerSoundS>();
			}
			_myAnimator.SetBool("Evading", false);
			_isDashing = false;
			_myRigidbody.drag = startDrag;

			if (dontGetStuckInEnemiesCheck.NoEnemies() && !PlayerSlowTimeS.witchTimeActive){
				gameObject.layer = START_PHYSICS_LAYER;
			}

			if (!myRenderer.enabled){
				myRenderer.enabled = true;
			}
			dashDurationTime = 0f;
			_playerSound.SetWalking(false);
            if (!tauntTutorial)
            {
                CancelAttack();
            }
		}
	}

	public void SetBuddy(bool onOff){
		if (!onOff){
			if (_myBuddy != null){
				_myBuddy.gameObject.SetActive(false);
			}
		}else{
			if (_myBuddy != null && !InGameCinematicS.turnOffBuddies && !isNatalie){
				_myBuddy.gameObject.SetActive(true);
				_buddyEffect.ChangeEffect(_myBuddy.shadowColor, _myBuddy.transform);
			}
		}
	}

	public void SetCanSwap (bool newCanSwap)
	{
		_canSwap = newCanSwap;
	}

	public void DelayAttackAllow(){
		delayAttackAllow = 0.2f;
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
		return equippedBuddies[_currentParadigm].GetComponent<BuddyS>();
	}
	public BuddyS ParadigmIBuddy(){
		return equippedBuddies[0].GetComponent<BuddyS>();
	}
	public BuddyS SubBuddy(){
		if (equippedBuddies.Count > 1){
			return (equippedBuddies[_subParadigm].GetComponent<BuddyS>());
		}
		else{
			return null;
		}
	}
	public BuddyS ParadigmIIBuddy(){
		if (equippedBuddies.Count > 1){

			return equippedBuddies[1].GetComponent<BuddyS>();

		}else{
			return null;
		}
	}

	public Vector3 SprintProjection(){
		return (transform.position+_myRigidbody.velocity.normalized*3f);
	}
		

	public bool AllowDodgeEffect(){
		if (_isDashing && dashDurationTime <= dashEffectThreshold && _playerAug.HasWitchAug()){
			return true;
		}else{
			return false;
		}
	}
	public bool InWitchAnimation(){
		if (parryDelayWitchCountdown > 0 || counterAttackTime > 0){
			return true;
		}else{
			return false;
		}
	}

	float GetIncensedSprintMult(){
		if (_playerAug.incensedAug){
			return _playerAug.incensedStaminaMult;
		}else{
			return 1f;
		}
	}

	bool SprintMoveCondition(){
		return inputDirection != Vector3.zero;
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

	public void AnimationStop(float stopTime){
		if (!_isDashing && !_isSprinting){
		StartCoroutine(HitStopRoutine(stopTime));
		}
	}

	public void ResetTimeMax(){
		resetCountdown = resetTimeMax;
	}

	public float VirtueStaminaMult(){
		float returnMult = 1f;
		if (_playerAug.pyraAug){
			if (_playerAug.doubleMantra){
				returnMult *= PlayerAugmentsS.pyraDoubleAugAmt;
			}else{
				returnMult *= PlayerAugmentsS.pyraAugAmt;
			}
		}
		if (_playerAug.empowered && _myStats.currentHealth >= _myStats.maxHealth){
			returnMult *= 0.5f;
		}
		return returnMult;
	}

	private float TransformedAttackSpeedMult(){
		float returnMult = 1f;
		if (_isTransformed){
			returnMult = transformedAttackSpeedMult;
		}
		return returnMult;
	}

	IEnumerator HitStopRoutine(float sTime){

		if (!hitStopped){
			_myAnimator.enabled = false;
			savedHitVelocity = _myRigidbody.velocity;
		}
		hitStopped = true;
		_myRigidbody.velocity = Vector3.zero;
		yield return new WaitForSeconds(sTime);
		if (hitStopped){
			_myAnimator.enabled = true;
			_myRigidbody.velocity = savedHitVelocity;
			hitStopped = false;
		}
		
	}

    public void DoLookAway(){
        SetTalking(true, false, false, false);
        StartCoroutine(LookAwayCoroutine());
    }
    IEnumerator LookAwayCoroutine(){
        SetExamining(false, Vector3.zero);
        myAnimator.SetTrigger("LookAway");
        yield return new WaitForSeconds(2.8f);
        SetTalking(false);
        DialogueManagerS.D.EndText(true,false);
    }

    public void SetScorned(bool scornOn){
        if (trackingScorn != scornOn)
        {
            EquipMenuS menuRef = GameObject.Find("Menus").GetComponent<InGameMenuManagerS>().EquipMenu;
            if (!scornOn)
            {
                int currentVirtue = _myStats.usedVirtue;
                for (int i = equippedVirtues.Count-1; i >= 0; i--)
                {
                    if (currentVirtue > (_myStats.virtueAmtNoScorn) && menuRef.allVirtueItems[equippedVirtues[i]].virtueCost > 0){
                        currentVirtue -= menuRef.allVirtueItems[equippedVirtues[i]].virtueCost;
                        equippedVirtues.RemoveAt(i);
                    }
            }
                _playerAug.RefreshAll();
                _myStats.ChangeVirtue(currentVirtue - _myStats.usedVirtue);
                menuRef.UpdateVirtues(true);
                menuRef.UpdateVirtueDisplay();
            }
            trackingScorn = scornOn;
        }
    }

    public static void AddMarked(){
        /*if (equippedVirtues == null)
        {
            equippedVirtues = new List<int>();
            equippedVirtues.Add(0);
        }
        if (!equippedVirtues.Contains(15)){
            equippedVirtues.Add(15);
            Debug.Log("Added marked!!");
        }**/
        PlayerInventoryS.I.AddEarnedVirtue(15);
    }

}
