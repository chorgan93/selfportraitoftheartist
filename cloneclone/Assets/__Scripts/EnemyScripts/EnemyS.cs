using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyS : MonoBehaviour {

	private const string DEAD_LAYER = "EnemyColliderDead";
	private const int FLASH_FRAME_COUNT = 3;

	private const float Z_POS_BEHIND_PLAYER = 2f;
	private const float ENEMY_DEATH_Z = 2f;
	private const float Z_POS_FRONT_PLAYER = -1f;
	
	//____________________________________ENEMY PROPERTIES

	[Header("Enemy Properties")]
	public float maxHealth;
	public float knockbackTime;
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

	//____________________________________ENEMY STATES

	private bool _initialized;

	private bool _isActive;
	private bool behaviorSet;
	private PlayerDetectS activationDetect;

	private bool _isActing; // currently doing an action
	private EnemyBehaviorS _currentBehavior; // action that is currently acting
	private EnemyBehaviorStateS stateToChangeTo;

	public List<GameObject> behaviorStates;
	private EnemyBehaviorStateS[] _behaviorStates;
	private EnemyBehaviorStateS _currentState;

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

		_currentBehavior.EndAction();
		_currentState = newState;

	}

	public void SetActing(bool newActingState){
		_isActing = newActingState;
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
				if (bState.isActive() && !behaviorSet){
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
				Debug.Log("Skipped behavior");
			}
		}


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
		if (!_hitStunned){
		Vector3 newSize = startSize;
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
	}

	public void Stun(float sTime){
		if (_canBeStunned && sTime > 0){
			_hitStunned = true;
			currentKnockbackCooldown = sTime;
			_myAnimator.SetTrigger("Hit");
			_myAnimator.SetLayerWeight(1,1f);
		}
		myRenderer.material = flashMaterial;
		flashFrames = FLASH_FRAME_COUNT;
	}

	public void TakeDamage(Vector3 knockbackForce, float dmg, float sTime = 0f){

		_currentHealth -= dmg;
		if (_currentHealth > 0){
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
		}
		else{
			_isDead = true;
			Stun (0);
			EndAllBehaviors();
			_myAnimator.SetLayerWeight(1, 0f);
			_myAnimator.SetBool("Death", true);
			//_myCollider.enabled = false;
			gameObject.layer = LayerMask.NameToLayer(DEAD_LAYER);
			_myRigidbody.velocity = Vector3.zero;
			_myRigidbody.AddForce(knockbackForce*1.5f, ForceMode.VelocityChange);
			transform.position = new Vector3(transform.position.x, transform.position.y, 
			                                 GetPlayerReference().transform.position.z + ENEMY_DEATH_Z);
			
			CameraShakeS.C.LargeShake();
			CameraShakeS.C.BigSleep(true);
			
			currentKnockbackCooldown = knockbackTime;
		}



	}

	public void AttackKnockback(Vector3 knockbackForce){
		
		_myRigidbody.AddForce(knockbackForce, ForceMode.Impulse);
		
	}

}
