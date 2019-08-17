using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyS : MonoBehaviour {

	public const float DAMAGE_VARIANCE = 0.15f;
	private const float BOUNCE_MULT = 1.5f;
	private const float WALL_STICK_TIME = 0.08f;

	public const float FIX_DRAG_MULT = 1.5f;

	private const string DEAD_LAYER = "EnemyColliderDead";
	private const int FLASH_FRAME_COUNT = 10;
	private const int HURT_FRAME_COUNT = 6;

	private const float Z_POS_BEHIND_PLAYER = 2f;
	private const float ENEMY_DEATH_Z = 3f;
	private const float Z_POS_FRONT_PLAYER = -1f;

	private const float VULN_EFFECT_RATE = 0.083f;
	private const float VULN_EFFECT_AMT = 0.9f;

	private const float CRIT_PENALTY = 3f;

	private const float corruptedSpeedMult = 1.25f;
	private const float corruptedPowerMult = 1.25f;
	private const float corruptedDefenseMult = 0.5f;
	private const float enragedSpeedMult = 1.1f;
	private const float enragedPowerMult = 2.5f;
	private const float enragedDefenseMult = 2f;
    private const float tauntBreakMult = 1.75f; 

    private ZControlTargetS zController;
	
	//____________________________________ENEMY PROPERTIES

	[Header("Enemy Properties")]
	public string enemyName = "sickman";
	public bool isFriendly = false;
	public bool debugMark = false;
	public bool isGold = false;
	public bool isCorrupted = false;
	private bool _isEnraged = false;
	public bool isEnraged { get { return _isEnraged; } }
	private float tempCorruptionTimeMax = 10f;
	private float tempCorruptionTime;
	private float tempEnragedTimeMax = 8f;
	private float tempEnragedTime;
	private bool _startCorruption = false;
	[Header ("Health Properties")]
	public float maxHealth;
	public float maxCritDamage = 9999f;
	public float maxCritTime = 3f;
    public float rewindSickness = 1f;
	private float currentCritTime = 0f;
	private float currentCritDamage;
	public float cinematicKillAt = 0f;
    public bool ignoreDeathZ = false;
	[Header("Stunlock Properties")]
	public float stunLockHealthMult = 0.4f;
	public float stunlockTimeMult = 0.6f;
	private float stunLockTarget = 4f;
	private float currentStunLock = 0f;
	public float preventStunLockMax = 1.5f;
	private float preventStunLock = 0f;
	public float stunRecoverDelay = 2f;
	private float currentStunRecoverDelay = 0f;
	public float stunRecoverRate = 1f;
	[HideInInspector]
	public float actingMaxHealth;
	public bool showHealth;
	public bool hitWorkaround = false;
	public bool cantDie = false;
	public float damageMultiplier = 1f;
	public Vector3 healthBarOffset = new Vector3(0f,1.5f,1f);
	public float healthBarXSize = 2f;
	[Header("Special Animation Properties")]
	public bool dontRepeatHitInCrit = false;
	private bool didFirstCritHit = false;
	[Header ("Death Properties")]
	public int sinAmt;
	public Color bloodColor = Color.red;
	public Color deadColor = new Color(0.3f, 0.3f, 0.3f);
	private Color startColor;
	public float knockbackTime;
	private float criticalRecoverTime = 0.5f;
	private float _currentHealth;
	private bool _isDead;
	public Material flashMaterial;
	private Material startMaterial;

	private bool _doingDeathFade = false;
	public bool doingDeathFade { get { return _doingDeathFade; } }

	private bool _preventKnockback = false;
	private bool _preventFace = false;
	public bool preventFace { get { return _preventFace; } }


    [Header("NG+ Properties")]
    public float newGamePlusHealthMult = 1.5f;
    public float newGamePlusSpeedMult = 1.025f;
    public bool requireTransformForNGMult = false;

	private float currentDifficultyMult;

	[HideInInspector]
	public float currentDifficultyAnimationFloat = 1f;

	private float startDrag;

	[HideInInspector]
	public bool ignorePush = false;

    bool fakeMarked = false;

	//____________________________________INSTANCE PROPERTIES

	private int ALIVE_LAYER;

	private Rigidbody _myRigidbody;
	public SpriteRenderer myRenderer;
	public SpriteRenderer myShadow;
    private EnemyResetMaskS _mySpriteMask;
	private Animator _myAnimator;
	private Collider _myCollider;

	private float _enemyActiveTime = 0f;
	public float enemyActiveTime { get { return _enemyActiveTime; } }

	private EnemyHealthUIS healthUIReference;
	private EnemyHealthFeathersS healthFeatherReference;
	//private EnemyHealthBarS healthBarReference;

	private TrackingEffectS _myTracker;
	public TrackingEffectS myTracker { get { return _myTracker; } }

	private EnragedEffectS myEnraged;

	private Vector3 startSize;

	private int flashFrames;
	public int flashReference { get { return flashFrames; } }

	private float currentKnockbackCooldown;
	private bool _canBeStunned;
	private bool _hitStunned;
	private bool _facePlayer;
	private bool _faceLocked = false;

	private float currentDefenseMult = 1f;
	private float currentStunResistMult = 1f;

	private bool _invulnerable = false;
	public bool invulnerable { get { return _invulnerable; } }

	private bool _isVulnerable = false;
	private bool _isCritical = false;
	private bool vulnerableEffectEnded = false;
	private float vulnEffectCountdown;

	[HideInInspector]
	public bool canBeParried = false;

	[HideInInspector]
	public bool isInvincible = false;

	private float vulnerableDelay;
	private float vulnerableCountdown;
	private float _flashAmt;
	private Color _flashCol;
	public GameObject critObjRef;
	private bool wasParried = false;
	public GameObject deathObjRef;
	private bool spawnedDeathObj = false;
	private int deathFrameDelay = 1;

	private FlashEffectS _critScreen;
	private FlashEffectS _killScreen;

	private Color fadeInColor;
	private float fadeRate = 1f;
	private bool fadedIn = false;

	private float knockbackDelay = 0.1f;

	[Header("Interaction Properties")]
	public EnemyStatusReferencesS myStatusMessenger;
	public CorruptedEffectS corruptionManager;
	public InstantiateOnEnemyHealthS healthSpawn;

	private TransformStartEffectS corruptStartEffect;

	
	[Header("Sound Properties")]
	public GameObject hitSound;
	public GameObject breakSound;
	public GameObject deathSound;
	public GameObject detectSound;

	[HideInInspector]
	public EnemySpawnerS mySpawner;

	[HideInInspector]
	public bool OverrideSpacingRequirement = false;

	private Vector3 hitVelocity;
	private bool touchingWall = false;
	public bool hitWall { get { return touchingWall; } }
	private Vector3 currentWallNormal;

	private float killAtLessThan = 0f;
	public float killAtLessThanRef { get { return killAtLessThan; } }


	//____________________________________ENEMY STATES

	private bool _initialized;

	private bool _isActive;
	private bool hasBeenActive = false;
	private bool allowActivation = true;
	private bool behaviorSet;
	private PlayerDetectS activationDetect;

	private float _breakAmt = 0f;
	private float _breakThreshold = 9999f;
	private bool _behaviorBroken = false;

	private bool _isActing; // currently doing an action
	private EnemyBehaviorS _currentBehavior; // action that is currently acting
	private EnemyBehaviorStateS stateToChangeTo;

	public List<GameObject> behaviorStates;
	private EnemyBehaviorStateS[] _behaviorStates;
	private EnemyBehaviorStateS _currentState;
	public EnemyBehaviorStateS currentState { get { return _currentState; } }

	private Vector3 _currentTarget;
	public Vector3 currentTarget { get { return _currentTarget; } }

	private int _numAttacksTakenInBehavior = 0;
	public int numAttacksTaken { get { return _numAttacksTakenInBehavior; } }

	//_____________________________________________GETTERS AND SETTERS

	public Rigidbody myRigidbody { get { return _myRigidbody;} }
	public Collider myCollider { get { return _myCollider;} }
	public Animator myAnimator { get { return _myAnimator;} }

	public float currentHealth { get { return _currentHealth; } }

	public bool initialized { get {return _initialized; } }
	public bool isActive { get {return _isActive;}}
	public bool isActing { get {return _isActing;}}
	public bool hitStunned { get {return _hitStunned;}}
	public bool isDead { get { return _isDead;}}

	public bool isVulnerable { get { return _isVulnerable;}}
	public bool isCritical { get { return _isCritical;}}

	public float flashAmt { get { return _flashAmt; } }
	public Color flashCol { get { return _flashCol; } }

	public bool behaviorBroken { get { return _behaviorBroken; } }

	private bool inWitchTime = false;
	private bool hitInWitchTime = false;
	private bool inWitchKnockback = false;
	float witchVelT = 0f;
	private float witchVelTimeMax = 0.5f;
	private float currentWitchVelTime = 0f;
	private Vector3 witchTargetVel;
	private Vector3 witchCapturedVel;

	private int doubleCriticalTime = 0;

	//_____________________________________UNITY METHODS
	// Use this for initialization
	void Start () {

		Initialize();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!fadedIn){
			FadeIn();
		}

		AliveUpdate();
		DeadUpdate();
	
	}

	void FixedUpdate () {
		
		AliveFixedUpdate();
		DeadFixedUpdate();
		
	}

	void LateUpdate () {

		AliveLateUpdate();
		DeadLateUpdate();

	}

	void OnEnable(){
		if (_isDead){
			_myAnimator.SetLayerWeight(2,1f);
			myShadow.enabled = false;
		}
	}

	void OnCollisionEnter(Collision other){
		if (other.gameObject.tag == "Wall"){
			touchingWall = true;
			if (currentKnockbackCooldown > 0 && other.contacts.Length > 0){
				WallBounce(other.contacts[0].normal);
			}
		}
	}

	void OnCollisionStay(Collision other){
		if (other.gameObject.tag == "Wall"){
			if (other.contacts.Length > 0){
				currentWallNormal = other.contacts[0].normal;
			}
		}
	}

	void OnCollisionExit(Collision other){
		if (other.gameObject.tag == "Wall"){
			touchingWall = false;
		}
	}

	//______________________________________PUBLIC METHODS

	public void SetBehavior(EnemyBehaviorS newBehavior){
		_currentBehavior = newBehavior;
		behaviorSet = true;
	}

    public void SetMask(EnemyResetMaskS myMask){
        _mySpriteMask = myMask;
    }

	public void ForceBehaviorState(EnemyBehaviorStateS newState){

		_currentBehavior.CancelAction();
		_currentState = newState;

	}

	public void SetActiveState(bool newActive){
		_isActive = allowActivation = newActive;
		if (!_isActive && _currentBehavior != null){
			_currentBehavior.EndAction(false);
		}
		CheckBehaviorStateSwitch(false);
	}

	public void SetActing(bool newActingState){
		_isActing = newActingState;
	}

	public void SetFaceForAttack(Vector3 targetLook){
		_faceLocked = true;
		Vector3 newSize = transform.localScale;
		if (targetLook.x < transform.position.x){
			newSize.x *= -1f;
			transform.localScale = newSize;
		}
		if (targetLook.x > transform.position.x){
			transform.localScale = newSize;
		}
	}
	public void ResetFaceLock(){
		_faceLocked = false;
	}

	public void SetTransformStartEffect(TransformStartEffectS newEffect){
		corruptStartEffect = newEffect;
	}

	public void SetBreakState(float newBreakAmt, float newRecover){
		_breakThreshold = newBreakAmt;
		_breakAmt = 0f;
		_behaviorBroken = false;
		criticalRecoverTime = newRecover;
	}

	public void SetUIReference(EnemyHealthUIS newRef){

		//healthUIReference = newRef;

	}

	public void RemoveUIReference(){

		healthUIReference = null;

	}

	public void DestructibleDead(){
		_currentHealth = 0f;
		_isDead = true;
	}

	//______________________________________ACTION HOLDERS

	private void AliveUpdate(){

		if (!_isDead){
			ManageFacing();
		//	ManageZ();

			if (isCorrupted && !_startCorruption){
				tempCorruptionTime += Time.deltaTime/currentDifficultyMult;
				if (tempCorruptionTime >= tempCorruptionTimeMax || _isDead){
					SetCorruption(false);
				}
			}

			if (_isEnraged && !_hitStunned){
				tempEnragedTime += Time.deltaTime;
				if (tempEnragedTime >= tempEnragedTimeMax){
					SetEnraged(false);
				}
			}

			#if UNITY_EDITOR
			/*if (Input.GetKeyDown(KeyCode.I)){
				Debug.Log("Corrupt debug!");
				SetCorruption(!isCorrupted);
			}**/
			if (debugMark){
				if (Input.GetKeyDown(KeyCode.Alpha9)){
					Debug.Log(enemyName + " : " + GetNumberOfActiveBehaviors(), gameObject);
				} 
			}
			#endif
		}
		FlashFrameManager();

	}

	private void DeadUpdate(){

        if (_isDead){
			SpawnDeathObj();
		}
		
	}

	private void AliveFixedUpdate(){

		if (!_isDead){

			CheckStatus();


		}

	}

	private void DeadFixedUpdate(){

		if (_isDead){
			
		}
		
	}

	private void AliveLateUpdate(){

		if (!_isDead){
			//CheckStatus();
		}

	}

	private void DeadLateUpdate(){
		
		if (_isDead){

		}
		
	}


	//______________________________________PRIVATE METHODS

	private void Initialize(){

		//enemyName = enemyName.Replace("PLAYERNAME", TextInputUIS.playerName);

        if (DarknessPercentUIS.DPERCENT != null){
            fakeMarked = DarknessPercentUIS.DPERCENT.UseDescent;
        }

		currentDifficultyMult = DifficultyS.GetSinMult(isGold);
        if (PlayerAugmentsS.MARKED_AUG || fakeMarked)
        {
            currentDifficultyMult *= newGamePlusSpeedMult;
            if (!requireTransformForNGMult || (requireTransformForNGMult && PlayerInventoryS.I.earnedTech.Contains(8))) {
            actingMaxHealth = maxHealth * newGamePlusHealthMult * currentDifficultyMult;
            maxCritDamage *= newGamePlusHealthMult;
            }else{
                Debug.Log("Weaker form for transform unlock!");
                actingMaxHealth = maxHealth * currentDifficultyMult;
            }
            sinAmt *= 3;
        }else{
            actingMaxHealth = maxHealth * currentDifficultyMult;
        }
        if (PlayerController.equippedTech.Contains(14) && !isFriendly){
            actingMaxHealth *= 0.5f;
        }
		stunLockTarget = actingMaxHealth*stunLockHealthMult;
		maxCritDamage *= DifficultyS.GetSinMult(isGold);
		maxCritTime *= DifficultyS.GetSinMult(isGold);

		_startCorruption = isCorrupted;

		_myTracker = GetComponentInChildren<TrackingEffectS>();

		ResetFaceLock();

		touchingWall = false;

		_invulnerable = false;
		doubleCriticalTime = 0;

        zController = GetComponent<ZControlTargetS>();


		if (!_isDead){
			_currentHealth = actingMaxHealth;
			_isActive = false;
			//EffectSpawnManagerS.E.SpawnEnemyHealthBar(this);
		}

		startSize = transform.localScale;

		spawnedDeathObj = false;
		deathFrameDelay = 3;

		ALIVE_LAYER = gameObject.layer;

		_critScreen = CameraEffectsS.E.critFlash;
		_killScreen = CameraEffectsS.E.killFlash;

		_myRigidbody = GetComponent<Rigidbody>();
		if (_myRigidbody.mass >= 999){
			_preventKnockback = true;
			_preventFace = true;
		}
		_myCollider = GetComponent<Collider>();
		_myAnimator = myRenderer.GetComponent<Animator>();
		startMaterial = myRenderer.material;
		startColor = myRenderer.color;
		startColor.a = 1f;

		_myAnimator.SetFloat("WitchSpeed", 1f);

		killAtLessThan = 0f;

		if (myRenderer.color.a < 1f){
			fadedIn = false;
			fadeInColor = myRenderer.color;
		}else{
			fadedIn = true;
		}

		startDrag = _myRigidbody.drag*FIX_DRAG_MULT;

		if (corruptionManager){
			corruptionManager.Initialize(this);
		}

		if (!_initialized){
			_initialized = true;
			_behaviorStates = GetBehaviorStates();

			foreach (EnemyBehaviorStateS bState in _behaviorStates){
				bState.SetEnemy(this);
			}

			activationDetect = transform.Find("PlayerDetect").GetComponent<PlayerDetectS>();
		}
		
		CheckStates(false);
		if (showHealth){
			if (healthUIReference == null){
				healthUIReference = GameObject.Find("Enemy Health").GetComponent<EnemyHealthUIS>();
			}
			healthUIReference.NewTarget(this);
		}

		healthFeatherReference = GetComponentInChildren<EnemyHealthFeathersS>();
		if (healthFeatherReference){
			healthFeatherReference.SetUpEnemy(this);
			if (mySpawner){
				healthFeatherReference.ChangeFeatherColor(mySpawner.myManager.pRef.EquippedWeapon().swapColor);
				if (myStatusMessenger){
					myStatusMessenger.FeatherMessage(mySpawner.myManager.pRef.EquippedWeapon().swapColor);
				}
			}
			if (!PlayerController.equippedVirtues.Contains(5)){
				healthFeatherReference.Hide();
			}
		}



	}

	public void Reinitialize(bool fromRewind = true){

		_isDead = false;
		myShadow.GetComponent<EnemyShadowS>().Reinitialize();
		CameraFollowS.F.RemoveStunnedEnemy(this);

		didFirstCritHit = false;

        if (zController) { zController.enabled = true; }

		ResetFaceLock();
		if (healthSpawn){
			healthSpawn.ResetSpawn();
		}

		_invulnerable = false;

		touchingWall = false;
		doubleCriticalTime = 0;

		currentCritDamage = 0;
		currentCritTime = 0f;
		killAtLessThan = 0f;
		wasParried = false;

		isCorrupted = _startCorruption;
		if (corruptionManager){
			corruptionManager.Initialize(this);
		}

		ResetEnraged();

		//EndAllBehaviors();

		//CheckBehaviorStateSwitch(false);

		_isActive = false;

		_myAnimator.SetLayerWeight(2, 0f);
		_myAnimator.SetBool("Death", false);
		_myAnimator.SetBool("Crit", false);
		ResetAnimatorTriggers();
		_currentHealth = actingMaxHealth;
			_isActive = false;

		_myCollider.enabled = true;
		gameObject.layer = ALIVE_LAYER;

        if (_mySpriteMask){
            _mySpriteMask.gameObject.SetActive(true);
        }

		//_myAnimator.Rebind();
		//_myAnimator.Update(0f);
		
		spawnedDeathObj = false;
		deathFrameDelay = 3;
		
		startDrag = _myRigidbody.drag*FIX_DRAG_MULT;

		_behaviorStates = GetBehaviorStates();
			
		foreach (EnemyBehaviorStateS bState in _behaviorStates){
			bState.SetEnemy(this, true);
		}
		if (myStatusMessenger){
			myStatusMessenger.ResetMessage();
		}
		CancelBehaviors();
		//EndAllBehaviors();
		CheckStates(false, true);
		if (showHealth){
			healthUIReference.NewTarget(this);
		}

		if (healthFeatherReference){
			healthFeatherReference.SetUpEnemy(this);
			if (!GetPlayerReference()){
				// TODO make sure this isn't the issue
				//healthFeatherReference.Hide ();
			}else{
				if (GetPlayerReference().playerAug.perceptiveAug){
					healthFeatherReference.Show();
				}
			}
		}

		_myRigidbody.velocity = Vector3.zero;

		myRenderer.color = startColor;

        if (fromRewind){
            RewindSickness();
        }

		/*if (healthBarReference != null){
			EffectSpawnManagerS.E.SpawnEnemyHealthBar(this);
		}**/

	}

	private void CheckStatus(){

        // always count up crit time
        /*if (currentCritTime < maxCritTime)
        {
            if (doubleCriticalTime > 0)
            {
                if (doubleCriticalTime > 1)
                {
                    currentCritTime += Time.deltaTime / (PlayerAugmentsS.aquaAugMult + 0.5f);
                }
                else
                {
                    currentCritTime += Time.deltaTime / PlayerAugmentsS.aquaAugMult;
                }
            }
            else
            {
                currentCritTime += Time.deltaTime;
            }
            if (_isCritical){
                _isVulnerable = true; // try to hard force no weirdness
            }
        }**/
        if (_isCritical)
        {
            _isVulnerable = true; // try to hard force no weirdness
        }

		// check vulnerable/critical
		if (_isVulnerable){
			if (_isCritical){
				if (!inWitchTime){
					vulnerableCountdown -= Time.deltaTime;
					
					if (vulnerableCountdown <= 0 || currentCritTime >= maxCritTime){
					_isCritical = false;
						didFirstCritHit = false;
						preventStunLock = preventStunLockMax;
						_breakAmt = 0f;
						_isVulnerable = false;
						_behaviorBroken = false;
						vulnerableCountdown = 0;
						doubleCriticalTime = 0;
					CameraFollowS.F.RemoveStunnedEnemy(this);
					_isVulnerable = false;
						currentCritTime = 0f;
					_myAnimator.SetBool("Crit", false);

					if (!_hitStunned && !inWitchTime){
						_myAnimator.SetLayerWeight(1, 0f);
					}

					// reset whichever state should be active
					_currentState.CancelAllActions();
					_currentState.StartActions(true);
				}
				}
			}else{
				VulnerableEffect();
				// countdown to end vuln state
				if (!inWitchTime){
					vulnerableCountdown -= Time.deltaTime;
				
				if (vulnerableCountdown <= 0){
					_isVulnerable = false;
				}
				}
			}


		}else{
			if (currentStunLock > 0 && !_isCritical){
			if (currentStunRecoverDelay > 0){
					currentStunRecoverDelay -= Time.deltaTime*currentDifficultyMult;
				}else{
					currentStunLock -= Time.deltaTime*stunRecoverRate;
				}
			}
			EndVulnerableEffect();
			/*if (_isCritical){
				_isCritical = false;
				didFirstCritHit = false;

				_breakAmt = 0f;
				_isVulnerable = false;
				_behaviorBroken = false;
				vulnerableCountdown = 0;
				CameraFollowS.F.RemoveStunnedEnemy(this);
				_isVulnerable = false;
				currentCritTime = 0f;
				_myAnimator.SetBool("Crit", false);

				if (!_hitStunned && !inWitchTime){
					_myAnimator.SetLayerWeight(1, 0f);
				}

				// reset whichever state should be active
				_currentState.CancelAllActions();
				_currentState.StartActions(true);
			}**/

			vulnerableDelay -= Time.deltaTime;
			if (vulnerableDelay <= 0 && vulnerableCountdown > 0){
				_isVulnerable = true;
				flashFrames = HURT_FRAME_COUNT;
				myRenderer.material = flashMaterial;
				myRenderer.material.SetColor("_FlashColor", Color.red);
			}
		}
		// first, check active state
		if (!_isActive && allowActivation){
			if (!isFriendly){
				if (activationDetect.PlayerInRange()){
					if (!hasBeenActive && detectSound){
						Instantiate(detectSound);
					}
					_isActive = hasBeenActive = true;
					CheckBehaviorStateSwitch(false);
					if (healthFeatherReference && GetPlayerReference() != null){
						if (GetPlayerReference().playerAug.perceptiveAug){
							healthFeatherReference.Show ();
						}
					}
				}
			}else{
				activationDetect.FindTarget();
				if (activationDetect.currentTarget != null){
					_isActive = true;
				}
			}
		}

		if (_hitStunned){
		if (currentKnockbackCooldown > 0){
			currentKnockbackCooldown -= Time.deltaTime;
		}
		else{
				if (preventStunLock > 0){
				preventStunLock -= Time.deltaTime;
				}
				_myAnimator.SetLayerWeight(1, 0f);
			_hitStunned = false;
            }
		}

		if (_currentState == null){
			CheckStates(false);
		}

	}

	public void AttackFlashEffect(){

		myRenderer.material = flashMaterial;
		myRenderer.material.SetColor("_FlashColor", bloodColor);
		myRenderer.material.SetFloat("_FlashAmount", 1f);
		flashFrames = FLASH_FRAME_COUNT;
	}

	public void SetCorruption(bool newC){
		isCorrupted = newC;
		if (corruptionManager){
			corruptionManager.Initialize(this);
		}
		if (corruptStartEffect){
			if (isCorrupted){
				tempCorruptionTime = 0f;
				corruptStartEffect.ActivateEffect();
			}else{
				corruptStartEffect.DeactivateEffect();
			}
		}
	}
	public void SetEnragedEffect(EnragedEffectS myEffect){
		myEnraged = myEffect;
	}
	public void SetEnraged(bool newE){
		_isEnraged = newE;
		if (corruptStartEffect){
			if (_isEnraged){
				tempEnragedTime = 0f;
				SpawnDeathObj(false);
				corruptStartEffect.ActivateEffect(false);
				myEnraged.ActivateEffect();
			}else{
				corruptStartEffect.DeactivateEffect();
				myEnraged.DeactivateEffect();
			}
		}
	}
	public void ResetCorruption(){
		isCorrupted = _startCorruption;
		if (!isCorrupted){
			tempCorruptionTime = 0f;
		}
		if (corruptionManager){
			corruptionManager.Initialize(this);
		}
	}

	public void ResetEnraged(){
		_isEnraged = false;
		tempEnragedTime = 0f;
        if (myEnraged != null)
        {
            myEnraged.DeactivateEffect();
        }
	}

	private void VulnerableEffect(){
		vulnerableEffectEnded = false;
		vulnEffectCountdown -= Time.deltaTime;
		if (vulnEffectCountdown <= 0){
			myRenderer.material.SetFloat("_FlashAmount", VULN_EFFECT_AMT);
			myRenderer.material.SetColor("_FlashColor", new Color(Random.Range(0.72f,1f),
			                                                      Random.Range(0.72f,1f),
			                                                      Random.Range(0.72f,1f)));
			_flashAmt = VULN_EFFECT_AMT;
			_flashCol = new Color(Random.Range(0.72f,1f),
			                                                      Random.Range(0.72f,1f),
			                                                      Random.Range(0.72f,1f));
			vulnEffectCountdown = VULN_EFFECT_RATE;
		}
	}

	private void EndVulnerableEffect(){
		if (!vulnerableEffectEnded && myRenderer.material == startMaterial){
			myRenderer.material.SetFloat("_FlashAmount", 0f);
			myRenderer.material.SetColor("_FlashColor", Color.white);
			_flashAmt = 0f;
			_flashCol = Color.white;
			vulnerableEffectEnded = true;
			vulnEffectCountdown = VULN_EFFECT_RATE;
		}
	}

	public void ResetAnimatorTriggers(){
		AnimatorControllerParameter[] parameters = _myAnimator.parameters;
		for (int i = 0; i < parameters.Length; i++){
			if (parameters[i].type == AnimatorControllerParameterType.Trigger){
				myAnimator.ResetTrigger(parameters[i].name);
			}
		}
	}

	private EnemyBehaviorStateS[] GetBehaviorStates(){

		EnemyBehaviorStateS[] states = new EnemyBehaviorStateS[behaviorStates.Count];

		int i = 0;

		foreach(GameObject bStateHolder in behaviorStates){
			states[i] = bStateHolder.GetComponent<EnemyBehaviorStateS>();
			i++;
		}

		return states;

	}

	private void ChangeBehaviorState(EnemyBehaviorStateS newState, bool fromReset = false){

		_currentState = newState;
		_currentState.StartActions(false, fromReset);
		
			
	}

	private void CheckStates(bool dontAllowStateChange, bool fromReset = false){

		if (!_isDead){

			behaviorSet = false;

			bool dontChange = dontAllowStateChange;

			if (!dontChange){
	
				for (int i = 0; i < _behaviorStates.Length; i++){
					if (_behaviorStates[i].isActive() && !behaviorSet && !_behaviorStates[i].doNotActAgain){
						stateToChangeTo = _behaviorStates[i];
					behaviorSet = true;
				}
			}

		if (stateToChangeTo != null){
			if (_currentState == null){
						ChangeBehaviorState(stateToChangeTo, fromReset);
			}
			else{
				if (_currentState != stateToChangeTo){
							_currentState.EndBehavior(false);
					ChangeBehaviorState(stateToChangeTo, fromReset);
				}
				else{
							if (fromReset){
								_currentState.StartActions(false,true);
							}else{
					_currentState.NextBehavior();
							}
				}
			}
				}}
			else{
				if (fromReset){
					_currentState.StartActions(false,true);
				}else{
					_currentState.NextBehavior();
				}
			}


		}


	}

	private void SpawnDeathObj(bool enragedEffect = false){

        if (!spawnedDeathObj){
			deathFrameDelay--;
			if (deathFrameDelay <= 0){
		GameObject deathObj = Instantiate(deathObjRef, transform.position, Quaternion.identity)
			as GameObject;
				if (transform.localScale.x < 0){
					Vector3 flipScale = deathObj.transform.localScale;
					flipScale.x *= -1f;
					deathObj.transform.localScale = flipScale;
				}
		//deathObj.GetComponent<EnemyDeathShadowS>().StartFade(myRenderer.sprite, myRenderer.transform.localScale);
				if (!enragedEffect){
		spawnedDeathObj = true;
				StartCoroutine(DeathFade());
				}
			}
		}
	}

	IEnumerator DeathFade(){
		float deathColorTime = 1f, deathColorCount = 1f, deathT = 1f;
		_doingDeathFade = true;
        //int endDeathAnim = 5;
		while (deathColorCount > 0f && _isDead){
			deathColorCount -= Time.deltaTime;
			if (deathColorCount < 0f){
				deathColorCount = 0f;
			}
			deathT = deathColorCount/deathColorTime;
            myRenderer.color = Color.Lerp(deadColor, startColor, deathT);

            /*if (endDeathAnim == 0)
            {
                _myAnimator.SetBool("Death", false);
                endDeathAnim--;
            }else{
                endDeathAnim--;
            }**/

			yield return null;
		}
        _doingDeathFade = false;
	}

	private void ResetMaterial(){
		myRenderer.material = startMaterial;
		myRenderer.material.SetFloat("_FlashAmount", 0f);
		myRenderer.material.SetColor("_FlashColor", Color.white);
		_flashAmt = 0f;
		_flashCol = Color.white;
	}

	private void CancelBehaviors(){

		_currentState = null;
		_currentBehavior = null;
		for (int i = 0; i < behaviorStates.Count; i++){
			for (int j = 0; j < _behaviorStates[i].behaviorSet.Length; j++){
				
				_behaviorStates[i].behaviorSet[j].CancelAction();

			}
		}
		behaviorSet = false;

	}

	public void EndAllBehaviors(){
		
		if (_currentState != null){
		_currentState.EndBehavior();
		}
	}

	public int GetNumberOfActiveBehaviors(){
		int numToReturn = 0;
		for (int i = 0; i < behaviorStates.Count; i++){
			for (int j = 0; j < _behaviorStates[i].behaviorSet.Length; j++){
				if (_behaviorStates[i].behaviorSet[j].BehaviorActing()){
					numToReturn++;
				}
			}
		}
		return numToReturn;
	}

	public string GetNamesOfActiveBehaviors(){
		string stringToReturn = "";
		for (int i = 0; i < behaviorStates.Count; i++){
			for (int j = 0; j < _behaviorStates[i].behaviorSet.Length; j++){
				if (_behaviorStates[i].behaviorSet[j].BehaviorActing()){
					stringToReturn += _behaviorStates[i].behaviorSet[j].behaviorName + " (" + _behaviorStates[i].stateName + ")\n";
				}
			}
		}
		return stringToReturn;
	}

	private void FlashFrameManager(){
		if (myRenderer.material != startMaterial){
		flashFrames--;
		if (flashFrames <= 0){
				myRenderer.material = startMaterial;
		}
		}
	}

	private void ManageFacing(){
		if (!inWitchTime){
		Vector3 newSize = startSize;
			if (!_faceLocked && !_preventFace){
		if (!_hitStunned && !_isCritical){
		if (_facePlayer && GetTargetReference() != null){
			float playerX = GetTargetReference().transform.position.x;
			if (playerX < transform.position.x){
				newSize.x *= -1f;
				transform.localScale = newSize;
			}
			if (playerX > transform.position.x){
				transform.localScale = newSize;
			}
		}else{
			if (_myRigidbody.velocity.x < 0){
				newSize.x *= -1f;
				transform.localScale = newSize;
			}
			
			if (_myRigidbody.velocity.x > 0){
				transform.localScale = newSize;
			}
		}
		}else{
			if (_myRigidbody.velocity.x > 0){
				newSize.x *= -1f;
				transform.localScale = newSize;
			}
			
			if (_myRigidbody.velocity.x < 0){
				transform.localScale = newSize;
			}
		}
			}
		}
	}

	private void FadeIn(){
		fadeInColor = myRenderer.color;
		fadeInColor.a += fadeRate * Time.deltaTime;
		if (fadeInColor.a >= 1f){
			fadeInColor.a = 1f;
			fadedIn = true;
		}
		myRenderer.color = fadeInColor;
	}

	IEnumerator WallBounce(Vector3 contactNormal, bool fromKnockback = false){

		if (!fromKnockback){
			_myRigidbody.velocity = Vector3.zero;
			yield return new WaitForSeconds(WALL_STICK_TIME);
		}

		Vector3 bounceVelocity = Vector3.Reflect(hitVelocity, contactNormal);
		if (!_isDead){
			_myRigidbody.AddForce(bounceVelocity*BOUNCE_MULT, ForceMode.Impulse);
		}else{
			_myRigidbody.AddForce(bounceVelocity*BOUNCE_MULT/4f, ForceMode.Impulse);
		}
		
	}


	//_______________________________________________PUBLIC METHODS
	public void StartWitchTime(){
		if (_currentBehavior){
			_currentBehavior.SetBehaviorActing(false);
		}
		_myAnimator.SetFloat("DifficultySpeed", 0f);
		_myAnimator.SetFloat("WitchSpeed", 0.1f);
		_myAnimator.SetFloat("DeathSpeed", 0.1f);
		_myRigidbody.velocity *= 0.1f;
		inWitchTime = true;
	}
	public void EndWitchTime(){
		if (hitInWitchTime){
			hitInWitchTime = false;
			if (_currentState){
				_currentBehavior.CancelAction();
				_currentState.StartActions();
			}
		}else{
			if (_currentBehavior){
				_currentBehavior.SetBehaviorActing(true);
			}
		}
		_myAnimator.SetFloat("DifficultySpeed", currentDifficultyAnimationFloat);
		_myAnimator.SetFloat("WitchSpeed", 1f);
		_myAnimator.SetFloat("DeathSpeed", 1f);
		if (!inWitchKnockback){
		_myRigidbody.velocity /= 0.1f;
		}
		inWitchKnockback = inWitchTime = false;
	}
	public void CheckBehaviorStateSwitch(bool dont){

		CheckStates(dont);

	}

	public Transform GetTargetReference(){

        // find activation detect, if it doesn't exist (edge case)
        if (!activationDetect){
            activationDetect = transform.Find("PlayerDetect").GetComponent<PlayerDetectS>();
        }

		if (activationDetect.currentTarget != null){
			return activationDetect.currentTarget;
		}else{
			return transform;
		}

	}

	public void RefreshTarget(){
		activationDetect.FindTarget();
	}

	public PlayerController GetPlayerReference(){

        // find activation detect, if it doesn't exist (edge case)
        if (!activationDetect){
            activationDetect = transform.Find("PlayerDetect").GetComponent<PlayerDetectS>();
        }

        if (activationDetect.player != null)
        {
            return activationDetect.player;
        }else{
            return null;
        }

	}

	public void SetFaceStatus(bool setFace){
		_facePlayer = setFace;
	}

	public void SetStunStatus(bool setStun){
		_canBeStunned = setStun;
        if (!setStun)
        {
            _hitStunned = false;
            currentKnockbackCooldown = 0f;
            _myAnimator.SetLayerWeight(1, 0f);
        }
	}

	public void SetVulnerableTiming(float duration, float delay){

		vulnerableCountdown = duration;
		vulnerableDelay = delay;

	}

	public void Stun(float sTime, bool overrideStun = false){
		if ((_canBeStunned||overrideStun||_behaviorBroken||inWitchTime) && sTime > 0){
			_hitStunned = true;
			currentKnockbackCooldown = sTime;
			if (_isCritical && currentStunLock >= stunLockTarget){
				currentStunLock = 0f;
				currentStunRecoverDelay = 0f;
				currentKnockbackCooldown *= stunlockTimeMult;
			}
			if (!dontRepeatHitInCrit || (dontRepeatHitInCrit && !didFirstCritHit)){
			_myAnimator.SetTrigger("Hit");
				didFirstCritHit = true;
			}
			_myAnimator.SetLayerWeight(1,1f);
			if (inWitchTime){
				hitInWitchTime = true;
			}
		}
		myRenderer.material = flashMaterial;
		myRenderer.material.SetColor("_FlashColor", Color.white);
		flashFrames = FLASH_FRAME_COUNT;
	}

    void RewindSickness(){
        Stun(rewindSickness, true);
        Debug.Log("Did rewind sickness!");
    }

	public void AutoCrit(Vector3 knockback, float critTime, int doubleTime = 0){
		_isVulnerable = true;
		_breakAmt = _breakThreshold+1f;
		_myRigidbody.velocity = Vector3.zero;
		wasParried = true;
		TakeDamage(transform, knockback, 0f, 0f, 0f, 0, 0.12f, critTime, false,doubleTime, 0f, true);
		canBeParried = false;
	}

	public float TakeDamage(Transform hitTransform, Vector3 knockbackForce, float dmg, float stunMult, float critDmg, int ignoreDefense = 0, 
		float hitStopAmt = 0.1f, float sTime = 0f, bool fromFriendly = false, int doubleStun = 0, 
		float killAtLess = 0f, bool fromParry = false){

		if (GetPlayerReference()){
			if (GetPlayerReference().playerAug.hatedAug){
				dmg*=PlayerAugmentsS.HATED_MULT;
			}
		}
        if (ignoreDefense > 0){
            stunMult = 0f;
        }

		float damageTaken = 0;
		float actingDamageMultiplier = damageMultiplier*CorruptedDefenseMult();
		if (ignoreDefense > 0){
			if (ignoreDefense > 1){
				actingDamageMultiplier = 1.3f;
			}else{
				actingDamageMultiplier = 1f;
			}
			_breakAmt += dmg*stunMult*currentStunResistMult;
		}else{
			_breakAmt += dmg*stunMult*currentStunResistMult*currentDefenseMult;
            if (_isEnraged){
                //enemy is more likely to break when taunted
                _breakAmt *= tauntBreakMult;
            }
		}
		if (!_isCritical && preventStunLock <= 0f){
			if (ignoreDefense > 0){
				if (ignoreDefense > 1){
					currentStunLock += dmg*stunMult*currentStunResistMult*1.3f;
				}else{
					currentStunLock += dmg*stunMult*currentStunResistMult;
				}
			}else{
                if (_isEnraged)
                {
                    //enemy is more likely to break when taunted
                    currentStunLock += dmg * stunMult * currentStunResistMult * currentDefenseMult * tauntBreakMult;
                }
                else
                {
                    currentStunLock += dmg * stunMult * currentStunResistMult * currentDefenseMult;
                }
			}
			currentStunRecoverDelay = stunRecoverDelay;
		}

		knockbackDelay = hitStopAmt;

		killAtLessThan = killAtLess+cinematicKillAt;



		if (_breakAmt >= _breakThreshold || currentStunLock >= stunLockTarget){
			_behaviorBroken = true;
#if UNITY_SWITCH
            if (GetPlayerReference() != null)
            {
                GetPlayerReference().myControl.ShakeController(1);
            }
#endif
            if (dmg > 0){
				wasParried = false;
			}
		}

		if (_isCritical){
			_currentHealth -= (dmg*CRIT_PENALTY*critDmg+0.5f*killAtLess)*actingDamageMultiplier;
			damageTaken+=(dmg*CRIT_PENALTY*critDmg+0.5f*killAtLess)*actingDamageMultiplier;
			currentCritDamage += (dmg*CRIT_PENALTY*critDmg+0.5f*killAtLess)*actingDamageMultiplier;
			if ((currentCritDamage > maxCritDamage && doubleCriticalTime <= 0f) 
				|| (currentCritDamage > maxCritDamage*PlayerAugmentsS.aquaAugMult && doubleCriticalTime > 0)){
				vulnerableCountdown = 0;
				_isCritical = false;
				doubleCriticalTime = 0;
				preventStunLock = preventStunLockMax;
				_breakAmt = 0f;
				_isVulnerable = false;
				_behaviorBroken = false;
				didFirstCritHit = false;
					CameraFollowS.F.RemoveStunnedEnemy(this);
					_isVulnerable = false;

					_myAnimator.SetBool("Crit", false);

				if (!_hitStunned && !inWitchTime){
						_myAnimator.SetLayerWeight(1, 0f);
					}
					// reset whichever state should be active
				if (_currentBehavior != null){
					_currentBehavior.CancelAction();
				}
				if(_currentState != null){ 
					_currentState.StartActions();
				}

			}
		}else{
			if (ignoreDefense > 0){
				if (ignoreDefense == 1){
				_currentHealth -= dmg*actingDamageMultiplier;
				damageTaken += dmg*actingDamageMultiplier;
				}else{
					_currentHealth -= dmg*actingDamageMultiplier*1.3f;
					damageTaken += dmg*actingDamageMultiplier*1.3f;
				}
			}else{
			_currentHealth -= dmg*actingDamageMultiplier*currentDefenseMult;
			damageTaken += dmg*actingDamageMultiplier*currentDefenseMult;
			}
			if (_behaviorBroken && _currentBehavior != null){
				_currentBehavior.CancelAction();
			}
		}

		if (damageTaken > 0){
			_numAttacksTakenInBehavior++;
		}

		if (!cantDie && _currentHealth <= killAtLessThan){
			_currentHealth -= killAtLessThan;
			damageTaken += killAtLessThan;
		}

		if (healthFeatherReference && gameObject.activeSelf){
				healthFeatherReference.EnemyHit(damageTaken);
		}


		if (cantDie && _currentHealth < 1f){
			_currentHealth = 1f;
		}



		if (healthUIReference != null && showHealth){
			healthUIReference.ResizeForDamage(_currentHealth <= 0 || _behaviorBroken || _isCritical);
		}else{
			/*if (GetPlayerReference()){
			if (!GetPlayerReference().myLockOn.lockedOn && _currentHealth > 0){
				GetPlayerReference().myLockOn.enemyHealthUI.NewTarget(this, damageTaken);
			}
			}**/
		}

		if (_currentHealth > 0){

			if (!fromParry && hitSound){
					
				Instantiate(hitSound);

			}
			//_myRigidbody.AddForce(knockbackForce, ForceMode.VelocityChange);

			StartCoroutine(KnockbackRoutine(knockbackForce, ForceMode.VelocityChange));
			
			CameraShakeS.C.SmallShake();


			/*if (!fromFriendly){
				CameraShakeS.C.SmallSleep();
			}**/


			if (sTime != 0f){
				currentKnockbackCooldown = sTime+knockbackDelay;
				Stun(sTime+knockbackDelay);
			}else{
				currentKnockbackCooldown = knockbackTime+knockbackDelay;
				Stun(knockbackTime+knockbackDelay);
			}
			currentKnockbackCooldown*=1.2f/currentDifficultyMult;

			if (_isVulnerable || _behaviorBroken){
				if (!_isCritical){
					_myAnimator.SetBool("Crit", true);
					_myAnimator.SetBool("Hit", true);
					//CameraShakeS.C.TimeSleep(0.2f);
					CameraShakeS.C.TimeSleep(0.1f);
					CameraShakeS.C.SloAndPunch(0.1f, 0.85f, 0.1f);
					_isCritical = true;
					doubleCriticalTime = doubleStun;
				//	Debug.Log("Enemy is critical! " + _isVulnerable + " : " + _behaviorBroken + " : " + _breakAmt + "/" + _breakThreshold + " : " + _currentBehavior.behaviorName);
					ResetFaceLock();
					if (GetPlayerReference()){
					GetPlayerReference().myStats.DrivenCheck();
                        GetPlayerReference().SendCritMessage();
                    }
                    if (breakSound){
						Instantiate(breakSound);
					}
					_isVulnerable = false;
					_breakAmt = 0f;
					_behaviorBroken = false;
					vulnerableCountdown = 0f;
					currentCritDamage = 0;
					currentCritTime = 0f;
					CameraFollowS.F.AddStunnedEnemy(this);
					_critScreen.Flash();
				
					// spawn break object on parry
					//if (dmg <= 0){
				GameObject critBreak = Instantiate(critObjRef, transform.position, Quaternion.identity)
					as GameObject;
				EnemyBreakS breakRef = critBreak.GetComponent<EnemyBreakS>();
				breakRef.transformRef = transform;
				breakRef.pieceColor = bloodColor;
						breakRef.ChangeScale(Mathf.Abs(transform.localScale.x*3f/4f));
						vulnerableCountdown = criticalRecoverTime*2f;
					breakRef.Activate(wasParried);
					wasParried = false;
					//}
				}
				if (vulnerableCountdown < criticalRecoverTime){
					vulnerableCountdown = criticalRecoverTime;
				}

				Stun(criticalRecoverTime,true);
			}
		}
		else{
			if (deathSound){
				Instantiate(deathSound);
			}
#if UNITY_SWITCH
            if (GetPlayerReference() != null)
            {
                GetPlayerReference().myControl.ShakeController(2);
            }
#endif
            _currentHealth = 0f;
			_killScreen.Flash();
			_isDead = true;
			ResetEnraged();
			if (corruptionManager){
				corruptionManager.SendDeathMessage();
			}
			ResetFaceLock();
			if (myStatusMessenger){
				myStatusMessenger.KillMessage();
			}

			CameraFollowS.F.RemoveStunnedEnemy(this);
			_canBeStunned = true;
			Stun (0);
			CancelBehaviors();
			GetPlayerReference().myStats.uiReference.cDisplay.AddCurrency(sinAmt);
			_myAnimator.SetLayerWeight(1, 0f);
        _myAnimator.SetBool("Crit", false);
			_myAnimator.SetBool("Death", true);
			_myAnimator.SetFloat("DeathSpeed", 1f);
			if (hitWorkaround){
				_myAnimator.SetTrigger("Hit");
			}
			//_myCollider.enabled = false;
			gameObject.layer = LayerMask.NameToLayer(DEAD_LAYER);
			_myRigidbody.velocity = Vector3.zero;
			StartCoroutine(KnockbackRoutine(knockbackForce*1.5f, ForceMode.VelocityChange));
            if (!ignoreDeathZ)
            {
                if (zController) { zController.enabled = false; }
                transform.position = new Vector3(transform.position.x, transform.position.y,
                                                 GetPlayerReference().transform.position.z + ENEMY_DEATH_Z);
            }

			ResetMaterial();

			GetComponent<BleedingS>().StartDeath();

			CameraPOIS.POI.JumpToMidpoint(transform.position, GetPlayerReference().transform.position);
			
			CameraShakeS.C.LargeShake();
			CameraShakeS.C.BigSleep();
			CameraShakeS.C.SloAndPunch(0.3f, 0.8f, 0.2f);
			
			currentKnockbackCooldown = knockbackTime;
            if (_mySpriteMask){
                _mySpriteMask.gameObject.SetActive(false);
            }
		}

		if (healthSpawn){
			healthSpawn.CheckSpawn(_currentHealth/actingMaxHealth);
		}

		EffectSpawnManagerS.E.SpawnDamangeNum(hitTransform.position, true, false, damageTaken, transform);

		//healthBarReference.ResizeForDamage();
		return damageTaken;

	}

	IEnumerator KnockbackRoutine(Vector3 forceAmt, ForceMode fMode){
		
		yield return new WaitForSeconds(knockbackDelay);

		if (!_preventKnockback){
		hitVelocity = forceAmt;
		if (!touchingWall){
			if (inWitchTime){
				StartCoroutine(WitchKnockbackRoutine(forceAmt, fMode));
			}else{
				_myRigidbody.AddForce(forceAmt, fMode);
			}
		}else{
			StartCoroutine(WallBounce(currentWallNormal, true));
		}
		}
		
	}

	IEnumerator WitchKnockbackRoutine(Vector3 forceAmt, ForceMode fMode){
						
		_myRigidbody.AddForce(forceAmt, fMode);
		inWitchKnockback = true;
		yield return new WaitForSeconds(0.2f);
		currentWitchVelTime = 0f;
		witchCapturedVel = _myRigidbody.velocity;
		witchTargetVel = witchCapturedVel*=0.1f;
		while (currentWitchVelTime < witchVelTimeMax){
			currentWitchVelTime += Time.deltaTime;
			witchVelT = currentWitchVelTime/witchVelTimeMax;
			witchVelT = Mathf.Sin(witchVelT * Mathf.PI * 0.5f);
			_myRigidbody.velocity = Vector3.Lerp(witchCapturedVel, witchTargetVel, witchVelT);
		}
		_myRigidbody.velocity = witchTargetVel;
		inWitchKnockback = false;

	}

	public void KillWithoutXP(bool fromMessenger = false){

		if (!_initialized){
			Initialize();
		}

		if (myStatusMessenger){
			myStatusMessenger.KillMessage();
		}
		if (healthFeatherReference && gameObject.activeSelf){
			healthFeatherReference.EnemyHit(_currentHealth);
			if (fromMessenger){
				healthFeatherReference.Hide();
			}
		}

		_currentHealth = 0;
		_isDead = true;
		ResetEnraged();
		if (corruptionManager){
			corruptionManager.SendDeathMessage();
		}
		ResetFaceLock();
		Stun (0);
		CancelBehaviors();
        _myAnimator.SetLayerWeight(1, 0f);
        _myAnimator.SetBool("Crit", false);
		_myAnimator.SetBool("Death", true);
		_myAnimator.SetFloat("DeathSpeed", 10f);
		currentKnockbackCooldown = knockbackTime;
		gameObject.layer = LayerMask.NameToLayer(DEAD_LAYER);
		_myRigidbody.velocity = Vector3.zero;
        if (_mySpriteMask){
            _mySpriteMask.gameObject.SetActive(false);
        }
        if (!ignoreDeathZ)
        {
            if (zController)
            {
                zController.enabled = false;
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, ENEMY_DEATH_Z);
        }
		myRenderer.material = startMaterial;

	}

	public void AttackKnockback(Vector3 knockbackForce){

		if (!_preventKnockback){
		_myRigidbody.AddForce(knockbackForce, ForceMode.Impulse);
		}
		
	}

	public void Deflect(){
		Vector3 currentVelocity = _myRigidbody.velocity;
		_myRigidbody.velocity = currentVelocity.magnitude*currentVelocity.normalized*-0.8f;
	}

	public void SetTargetReference(Vector3 setMe){
		_currentTarget = setMe;
	}

	public void SetHealthBar(EnemyHealthBarS newBar){
		//healthBarReference = newBar;
	}

	public void SetHealthDisplay(EnemyHealthFeathersS newFeather){
		healthFeatherReference = newFeather;
	}

	public void ChangeFeatherColor(Color newCol){
		if (healthFeatherReference)
			healthFeatherReference.ChangeFeatherColor(newCol);
		if (myStatusMessenger){
			myStatusMessenger.FeatherMessage(newCol);
		}
	}

	public void SetInvulnerable(bool newI){
		_invulnerable = newI;
	} 

	public void SetStateDefenses(float dMult, float sMult){
		currentDefenseMult = dMult;
		currentStunResistMult = sMult;
	}

	public void ResetAttackCount(){
		_numAttacksTakenInBehavior = 0;
	}

	public float CorruptedMult(){
		float returnMult = 1f;
		if (isCorrupted){
			returnMult = corruptedSpeedMult;
		}
		if (isEnraged){
			returnMult*=enragedSpeedMult;
		}
		return returnMult;
	}
	public float CorruptedPowerMult(){
		float powerMult = 1f;
		if (isCorrupted){
			powerMult = corruptedPowerMult;
		}
		if (isEnraged){
			powerMult*=enragedPowerMult;
		}
		return powerMult;
	}

	public float CorruptedDefenseMult(){
		float defenseMult = 1f;
		if (isCorrupted){
			defenseMult = corruptedDefenseMult;
		}
		if (isEnraged){
			defenseMult*=enragedDefenseMult;
		}
		return defenseMult;
	}

    //_____________________________________________________FOS CHARGE STUFF
    private List<EnemyChargeAttackS> fosCharges = new List<EnemyChargeAttackS>();

    public void TriggerFosAttack()
    {
        if (fosCharges.Count > 0)
        {
            Vector3 targetDir = Vector3.zero;
            for (int i = 0; i < fosCharges.Count; i++)
            {


                targetDir = GetTargetReference().transform.position - fosCharges[i].transform.position;
                

                targetDir.z = 0f;
                fosCharges[i].FosPause(targetDir);
            }
            StartCoroutine(FireFosCharges());
        }
    }

    IEnumerator FireFosCharges()
    {

        yield return new WaitForSeconds(0.2f);
        bool firstFired = true;
        while (fosCharges.Count > 0)
        {
            fosCharges[0].FosDirectedFire(firstFired);
            fosCharges.RemoveAt(0);
            firstFired = false;
            yield return new WaitForSeconds(0.12f);
        }
    }

    public void AddFosCharge(EnemyChargeAttackS newCharge)
    {
        fosCharges.Add(newCharge);
    }

    public void RemoveFosCharge(EnemyChargeAttackS usedCharge)
    {
        if (fosCharges.Contains(usedCharge))
        {
            fosCharges.Remove(usedCharge);
        }
    }
    public void TriggerAllFos(EnemyChargeAttackS removeCharge)
    {
        fosCharges.Remove(removeCharge);
        for (int i = 0; i < fosCharges.Count; i++)
        {
            fosCharges[i].FosEndFire(true);
        }
    }

}
