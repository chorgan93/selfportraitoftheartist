using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyS : MonoBehaviour {

	private const string DEAD_LAYER = "EnemyColliderDead";
	private const int FLASH_FRAME_COUNT = 3;

	private const float Z_POS_BEHIND_PLAYER = 2f;
	private const float ENEMY_DEATH_Z = 3f;
	private const float Z_POS_FRONT_PLAYER = -1f;

	private const float VULN_EFFECT_RATE = 0.083f;
	private const float VULN_EFFECT_AMT = 0.9f;
	
	//____________________________________ENEMY PROPERTIES

	[Header("Enemy Properties")]
	public string enemyName = "sickman";
	public bool isFriendly = false;
	public float maxHealth;
	public bool showHealth;
	public int sinAmt;
	public Color bloodColor = Color.red;
	public float knockbackTime;
	private float criticalRecoverTime = 0.5f;
	private float _currentHealth;
	private bool _isDead;
	public Material flashMaterial;
	private Material startMaterial;

	private float startDrag;

	//____________________________________INSTANCE PROPERTIES

	private int ALIVE_LAYER;

	private Rigidbody _myRigidbody;
	public SpriteRenderer myRenderer;
	public SpriteRenderer myShadow;
	private Animator _myAnimator;
	private Collider _myCollider;

	private EnemyHealthUIS healthUIReference;

	private Vector3 startSize;

	private int flashFrames;

	private float currentKnockbackCooldown;
	private bool _canBeStunned;
	private bool _hitStunned;
	private bool _facePlayer;

	private bool _isVulnerable = false;
	private bool _isCritical = false;
	private bool vulnerableEffectEnded = false;
	private float vulnEffectCountdown;

	[HideInInspector]
	public bool canBeParried = false;

	private float vulnerableDelay;
	private float vulnerableCountdown;
	private float _flashAmt;
	private Color _flashCol;
	public GameObject critObjRef;
	public GameObject deathObjRef;
	private bool spawnedDeathObj = false;
	private int deathFrameDelay = 1;

	private FlashEffectS _critScreen;
	private FlashEffectS _killScreen;

	private Color fadeInColor;
	private float fadeRate = 1f;
	private bool fadedIn = false;

	
	[Header("Sound Properties")]
	public GameObject hitSound;
	public GameObject deathSound;


	//____________________________________ENEMY STATES

	private bool _initialized;

	private bool _isActive;
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

	private Vector3 _currentTarget;
	public Vector3 currentTarget { get { return _currentTarget; } }

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

	//______________________________________PUBLIC METHODS

	public void SetBehavior(EnemyBehaviorS newBehavior){
		_currentBehavior = newBehavior;
	}

	public void ForceBehaviorState(EnemyBehaviorStateS newState){

		_currentBehavior.EndAction(false);
		_currentState = newState;

	}

	public void SetActing(bool newActingState){
		_isActing = newActingState;
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

	//______________________________________ACTION HOLDERS

	private void AliveUpdate(){

		if (!_isDead){
			ManageFacing();
		//	ManageZ();
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
			CheckStatus();
		}

	}

	private void DeadLateUpdate(){
		
		if (_isDead){

		}
		
	}


	//______________________________________PRIVATE METHODS

	private void Initialize(){

		if (!_isDead){
			_currentHealth = maxHealth;
			_isActive = false;
		}

		startSize = transform.localScale;

		spawnedDeathObj = false;
		deathFrameDelay = 3;

		ALIVE_LAYER = gameObject.layer;

		_critScreen = CameraEffectsS.E.critFlash;
		_killScreen = CameraEffectsS.E.killFlash;

		_myRigidbody = GetComponent<Rigidbody>();
		_myCollider = GetComponent<Collider>();
		_myAnimator = myRenderer.GetComponent<Animator>();
		startMaterial = myRenderer.material;

		if (myRenderer.color.a < 1f){
			fadedIn = false;
			fadeInColor = myRenderer.color;
		}else{
			fadedIn = true;
		}

		startDrag = _myRigidbody.drag;

		if (!_initialized){
			_initialized = true;
			_behaviorStates = GetBehaviorStates();

			foreach (EnemyBehaviorStateS bState in _behaviorStates){
				bState.SetEnemy(this);
			}

			activationDetect = transform.FindChild("PlayerDetect").GetComponent<PlayerDetectS>();
		}
		
		CheckStates(false);
		if (showHealth){
			if (healthUIReference == null){
				healthUIReference = GameObject.Find("Enemy Health").GetComponent<EnemyHealthUIS>();
			}
			healthUIReference.NewTarget(this);
		}

	}

	public void Reinitialize(){

		_isDead = false;
		myShadow.GetComponent<EnemyShadowS>().Reinitialize();

		EndAllBehaviors();

		_isActive = false;
		
		_myAnimator.SetBool("Death", false);
			_currentHealth = maxHealth;
			_isActive = false;

		_myCollider.enabled = true;
		gameObject.layer = ALIVE_LAYER;
		
		spawnedDeathObj = false;
		deathFrameDelay = 3;
		
		startDrag = _myRigidbody.drag;

		_behaviorStates = GetBehaviorStates();
			
		foreach (EnemyBehaviorStateS bState in _behaviorStates){
			bState.SetEnemy(this);
		}
			
		
		CheckStates(false);
		if (showHealth){
			healthUIReference.NewTarget(this);
		}

	}

	private void CheckStatus(){

		// check vulnerable/critical
		if (_isVulnerable){
			if (_isCritical){
				vulnerableCountdown -= Time.deltaTime;
				if (vulnerableCountdown <= 0){
					_isCritical = false;
					_isVulnerable = false;

					_myAnimator.SetBool("Crit", false);

					if (!_hitStunned){
						_myAnimator.SetLayerWeight(1, 0f);
					}

					// reset whichever state should be active
					_currentBehavior.EndAction(false);
					_currentState.StartActions();
				}
			}else{
				VulnerableEffect();
				// countdown to end vuln state
				vulnerableCountdown -= Time.deltaTime;
				if (vulnerableCountdown <= 0){
					_isVulnerable = false;
				}
			}


		}else{
			EndVulnerableEffect();

			vulnerableDelay -= Time.deltaTime;
			if (vulnerableDelay <= 0 && vulnerableCountdown > 0){
				_isVulnerable = true;
				flashFrames = FLASH_FRAME_COUNT;
				myRenderer.material = flashMaterial;
				myRenderer.material.SetColor("_FlashColor", Color.red);
			}
		}
		// first, check active state
		if (!_isActive){
			if (!isFriendly){
				if (activationDetect.PlayerInRange()){
					_isActive = true;
				}
			}else{
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
				_myAnimator.SetLayerWeight(1, 0f);
			_hitStunned = false;
		}
		}

		if (_currentState == null){
			CheckStates(false);
		}

	}

	private void VulnerableEffect(){
		vulnerableEffectEnded = false;
		vulnEffectCountdown -= Time.deltaTime;
		if (vulnEffectCountdown <= 0){
			//myRenderer.material.SetFloat("_FlashAmount", VULN_EFFECT_AMT);
			/*myRenderer.material.SetColor("_FlashColor", new Color(Random.Range(0.72f,1f),
			                                                      Random.Range(0.72f,1f),
			                                                      Random.Range(0.72f,1f)));*/
			_flashAmt = VULN_EFFECT_AMT;
			_flashCol = new Color(Random.Range(0.72f,1f),
			                                                      Random.Range(0.72f,1f),
			                                                      Random.Range(0.72f,1f));
			vulnEffectCountdown = VULN_EFFECT_RATE;
		}
	}

	private void EndVulnerableEffect(){
		if (!vulnerableEffectEnded && myRenderer.material == startMaterial){
			/*myRenderer.material.SetFloat("_FlashAmount", 0f);
			myRenderer.material.SetColor("_FlashColor", Color.white);*/
			_flashAmt = 0f;
			_flashCol = Color.white;
			vulnerableEffectEnded = true;
			vulnEffectCountdown = VULN_EFFECT_RATE;
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

	private void ChangeBehaviorState(EnemyBehaviorStateS newState){

		_currentState = newState;
		_currentState.StartActions();
		
			
	}

	private void CheckStates(bool allowStateChange){

		if (!_isDead){

			behaviorSet = false;

			bool dontChange = allowStateChange;

			if (!dontChange){
	
			foreach (EnemyBehaviorStateS bState in _behaviorStates){
				if (bState.isActive() && !behaviorSet && !bState.doNotActAgain){
					stateToChangeTo = bState;
					behaviorSet = true;
				}
			}

		if (stateToChangeTo != null){
			if (_currentState == null){
				ChangeBehaviorState(stateToChangeTo);
			}
			else{
				if (_currentState != stateToChangeTo){
					ChangeBehaviorState(stateToChangeTo);
				}
				else{
					_currentState.NextBehavior();
				}
			}
				}}
			else{
				_currentState.NextBehavior();
			}
		}


	}

	private void SpawnDeathObj(){

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

		spawnedDeathObj = true;
			}
		}
	}

	private void ResetMaterial(){
		//myRenderer.material = startMaterial;
		//myRenderer.material.SetFloat("_FlashAmount", 0f);
		//myRenderer.material.SetColor("_FlashColor", Color.white);
		_flashAmt = 0f;
		_flashCol = Color.white;
	}

	private void EndAllBehaviors(){
		if (_currentState != null){
		_currentState.EndBehavior();
		}
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
		Vector3 newSize = startSize;
		if (!_hitStunned || !_isCritical){
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

	private void FadeIn(){
		fadeInColor = myRenderer.color;
		fadeInColor.a += fadeRate * Time.deltaTime;
		if (fadeInColor.a >= 1f){
			fadeInColor.a = 1f;
			fadedIn = true;
		}
		myRenderer.color = fadeInColor;
	}


	//_______________________________________________PUBLIC METHODS
	public void CheckBehaviorStateSwitch(bool dont){

		CheckStates(dont);

	}

	public Transform GetTargetReference(){

		if (activationDetect.currentTarget != null){
			return activationDetect.currentTarget;
		}else{
			return transform;
		}

	}

	public PlayerController GetPlayerReference(){

		return activationDetect.player;

	}

	public void SetFaceStatus(bool setFace){
		_facePlayer = setFace;
	}

	public void SetStunStatus(bool setStun){
		_canBeStunned = setStun;
		_hitStunned = false;
		currentKnockbackCooldown = 0f;
		_myAnimator.SetLayerWeight(1,0f);
	}

	public void SetVulnerableTiming(float duration, float delay){

		vulnerableCountdown = duration;
		vulnerableDelay = delay;

	}

	public void Stun(float sTime, bool overrideStun = false){
		if ((_canBeStunned||overrideStun||_behaviorBroken) && sTime > 0){
			_hitStunned = true;
			currentKnockbackCooldown = sTime;
			_myAnimator.SetTrigger("Hit");
			_myAnimator.SetLayerWeight(1,1f);
		}
		myRenderer.material = flashMaterial;
		myRenderer.material.SetColor("_FlashColor", Color.white);
		flashFrames = FLASH_FRAME_COUNT;
	}

	public void AutoCrit(Vector3 knockback, float critTime){
		_isVulnerable = true;
		_breakAmt = _breakThreshold+1f;
		TakeDamage(knockback, 0f, 0f, 0f, critTime);
		canBeParried = false;
	}

	public void TakeDamage(Vector3 knockbackForce, float dmg, float stunMult, float critDmg, float sTime = 0f){

		float damageTaken = 0;
		_breakAmt += dmg*stunMult;


		if (_breakAmt >= _breakThreshold){
			_behaviorBroken = true;
		}

		if (_isCritical){
			_currentHealth -= dmg*critDmg;
			damageTaken+=dmg*critDmg;
		}else{
			_currentHealth -= dmg;
			damageTaken += dmg;
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

			if (hitSound){
				Instantiate(hitSound);
			}
			_myRigidbody.AddForce(knockbackForce, ForceMode.VelocityChange);
			
			CameraShakeS.C.SmallShake();
			CameraShakeS.C.SmallSleep();


			if (sTime != 0f){
				currentKnockbackCooldown = sTime;
				Stun(sTime);
			}else{
				currentKnockbackCooldown = knockbackTime;
				Stun(knockbackTime);
			}

			if (_isVulnerable || _behaviorBroken){
				if (!_isCritical){
					_myAnimator.SetBool("Hit", true);
					CameraShakeS.C.TimeSleep(0.2f);
					CameraShakeS.C.SloAndPunch(0.1f, 0.7f, 0.1f);
					_isCritical = true;
					_critScreen.Flash();
					GetPlayerReference().SendCritMessage();
				}
				GameObject critBreak = Instantiate(critObjRef, transform.position, Quaternion.identity)
					as GameObject;
				EnemyBreakS breakRef = critBreak.GetComponent<EnemyBreakS>();
				breakRef.transformRef = transform;
				breakRef.pieceColor = bloodColor;
				breakRef.ChangeScale(Mathf.Abs(transform.localScale.x*3f/4f));
				vulnerableCountdown = criticalRecoverTime;

				Stun(criticalRecoverTime,true);
			}
		}
		else{
			if (deathSound){
				Instantiate(deathSound);
			}
			_killScreen.Flash();
			_isDead = true;
			Stun (0);
			EndAllBehaviors();
			GetPlayerReference().myStats.uiReference.cDisplay.AddCurrency(sinAmt);
			_myAnimator.SetLayerWeight(1, 0f);
			_myAnimator.SetBool("Death", true);
			_myAnimator.SetFloat("DeathSpeed", 1f);
			//_myCollider.enabled = false;
			gameObject.layer = LayerMask.NameToLayer(DEAD_LAYER);
			_myRigidbody.velocity = Vector3.zero;
			_myRigidbody.AddForce(knockbackForce*1.5f, ForceMode.VelocityChange);
			transform.position = new Vector3(transform.position.x, transform.position.y, 
			                                 GetPlayerReference().transform.position.z + ENEMY_DEATH_Z);

			ResetMaterial();

			GetComponent<BleedingS>().StartDeath();
			
			CameraShakeS.C.LargeShake();
			CameraShakeS.C.BigSleep();
			CameraShakeS.C.SloAndPunch(0.3f, 0.7f, 0.2f);
			
			currentKnockbackCooldown = knockbackTime;
		}



	}

	public void KillWithoutXP(){

		if (!_initialized){
			Initialize();
		}

		_currentHealth = 0;
		_isDead = true;
		Stun (0);
		EndAllBehaviors();
		_myAnimator.SetLayerWeight(1, 0f);
		_myAnimator.SetBool("Death", true);
		_myAnimator.SetFloat("DeathSpeed", 10f);
		currentKnockbackCooldown = knockbackTime;
		gameObject.layer = LayerMask.NameToLayer(DEAD_LAYER);
		_myRigidbody.velocity = Vector3.zero;
		transform.position = new Vector3(transform.position.x, transform.position.y, ENEMY_DEATH_Z);
		myRenderer.material = startMaterial;

	}

	public void AttackKnockback(Vector3 knockbackForce){
		
		_myRigidbody.AddForce(knockbackForce, ForceMode.Impulse);
		
	}

	public void Deflect(){
		Vector3 currentVelocity = _myRigidbody.velocity;
		_myRigidbody.velocity = currentVelocity.magnitude*currentVelocity.normalized*-0.8f;
	}

	public void SetTargetReference(Vector3 setMe){
		_currentTarget = setMe;
	}

}
