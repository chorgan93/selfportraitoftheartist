﻿using UnityEngine;
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
	public float maxHealth;
	public int sinAmt;
	public Color bloodColor = Color.red;
	public float lockOnSize = 1f;
	public float knockbackTime;
	private float criticalRecoverTime = 0.5f;
	private float _currentHealth;
	private bool _isDead;
	public Material flashMaterial;
	private Material startMaterial;

	private float startDrag;

	//____________________________________INSTANCE PROPERTIES

	private Rigidbody _myRigidbody;
	public SpriteRenderer myRenderer;
	public SpriteRenderer myShadow;
	private Animator _myAnimator;
	private Collider _myCollider;

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

	private float vulnerableDelay;
	private float vulnerableCountdown;
	private float _flashAmt;
	private Color _flashCol;
	public GameObject critObjRef;
	public GameObject deathObjRef;
	private bool spawnedDeathObj = false;
	private int deathFrameDelay = 1;

	
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

	//______________________________________ACTION HOLDERS

	private void AliveUpdate(){

		if (!_isDead){
			ManageFacing();
			ManageZ();
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

		_currentHealth = maxHealth;
		_isDead = false;
		_isActive = false;

		startSize = transform.localScale;

		spawnedDeathObj = false;
		deathFrameDelay = 3;

		_myRigidbody = GetComponent<Rigidbody>();
		_myCollider = GetComponent<Collider>();
		_myAnimator = myRenderer.GetComponent<Animator>();
		startMaterial = myRenderer.material;

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
			if (activationDetect.PlayerInRange()){
				_isActive = true;
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
		deathObj.GetComponent<EnemyDeathShadowS>().StartFade(myRenderer.sprite, myRenderer.transform.localScale);

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
		_currentState.EndBehavior();
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
		if (_facePlayer && GetPlayerReference() != null){
			float playerX = GetPlayerReference().transform.position.x;
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

	private void ManageZ(){
		if (GetPlayerReference() != null){
			Vector3 fixPos = transform.position;
			Vector3 playerPos = GetPlayerReference().transform.position;
			if (playerPos.y < myShadow.transform.position.y){
				fixPos.z = playerPos.z + Z_POS_BEHIND_PLAYER;
			}else{
				fixPos.z = playerPos.z + Z_POS_FRONT_PLAYER;
			}
			transform.position = fixPos;
		}
	}


	//_______________________________________________PUBLIC METHODS
	public void CheckBehaviorStateSwitch(bool dont){

		CheckStates(dont);

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

	public void TakeDamage(Vector3 knockbackForce, float dmg, float stunMult, float critDmg, float sTime = 0f){

		_currentHealth -= dmg;
		_breakAmt += dmg*stunMult;

		if (_breakAmt >= _breakThreshold){
			_behaviorBroken = true;
		}

		if (_isCritical){
			_currentHealth -= critDmg;
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
					CameraShakeS.C.TimeSleep(0.08f, true);
					_isCritical = true;
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
			_isDead = true;
			Stun (0);
			EndAllBehaviors();
			GetPlayerReference().myStats.uiReference.cDisplay.AddCurrency(sinAmt);
			_myAnimator.SetLayerWeight(1, 0f);
			_myAnimator.SetBool("Death", true);
			//_myCollider.enabled = false;
			gameObject.layer = LayerMask.NameToLayer(DEAD_LAYER);
			_myRigidbody.velocity = Vector3.zero;
			_myRigidbody.AddForce(knockbackForce*1.5f, ForceMode.VelocityChange);
			transform.position = new Vector3(transform.position.x, transform.position.y, 
			                                 GetPlayerReference().transform.position.z + ENEMY_DEATH_Z);

			ResetMaterial();

			GetComponent<BleedingS>().StartDeath();
			
			CameraShakeS.C.LargeShake();
			CameraShakeS.C.BigSleep(true);
			
			currentKnockbackCooldown = knockbackTime;
		}



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
