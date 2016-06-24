using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyS : MonoBehaviour {

	private const string DEAD_LAYER = "EnemyColliderDead";
	
	//____________________________________ENEMY PROPERTIES
	public bool debugState = false;

	[Header("Enemy Properties")]
	public float maxHealth;
	public float knockbackTime;
	private float _currentHealth;
	private bool _isDead;

	private float startDrag;

	//____________________________________INSTANCE PROPERTIES

	private Rigidbody _myRigidbody;
	private SpriteRenderer _myRenderer;
	private Collider _myCollider;

	private float currentKnockbackCooldown;
	private bool _hitStunned;

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
	public SpriteRenderer myRenderer { get { return _myRenderer;} }
	public Collider myCollider { get { return _myCollider;} }

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
			
		}

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

		_myRigidbody = GetComponent<Rigidbody>();
		_myCollider = GetComponent<Collider>();

		startDrag = _myRigidbody.drag;

		if (!_initialized){
			_initialized = true;
			_behaviorStates = GetBehaviorStates();

			foreach (EnemyBehaviorStateS bState in _behaviorStates){
				bState.SetEnemy(this);
			}

			activationDetect = transform.FindChild("PlayerDetect").GetComponent<PlayerDetectS>();
		}
		
		CheckStates();

	}

	private void CheckStatus(){
		
		// first, check active state
		if (!_isActive){
			if (activationDetect.PlayerInRange()){
				_isActive = true;
			}
		}

		if (currentKnockbackCooldown > 0){
			_hitStunned = true;
			currentKnockbackCooldown -= Time.deltaTime;
		}
		else{
			_hitStunned = false;
		}

		if (_currentState == null){
			CheckStates();
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

	private void CheckStates(){

		if (!_isDead){
			behaviorSet = false;
	
			foreach (EnemyBehaviorStateS bState in _behaviorStates){
				if (bState.isActive() && !behaviorSet){
					stateToChangeTo = bState;
					behaviorSet = true;

					if(debugState){
						Debug.Log("Switch to " + bState.stateName);
					}
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
		}
		}


	}

	private void EndAllBehaviors(){
		_currentState.EndBehavior();
	}


	//_______________________________________________PUBLIC METHODS
	public void CheckBehaviorStateSwitch(){

		CheckStates();

	}

	public PlayerController GetPlayerReference(){

		return activationDetect.player;

	}

	public void TakeDamage(Vector3 knockbackForce, float dmg){

		_currentHealth -= dmg;
		if (_currentHealth > 0){
			_myRigidbody.AddForce(knockbackForce, ForceMode.VelocityChange);
			
			CameraShakeS.C.SmallShake();
			CameraShakeS.C.SmallSleep();
			
			currentKnockbackCooldown = knockbackTime;
		}
		else{
			_isDead = true;
			EndAllBehaviors();
			//_myCollider.enabled = false;
			gameObject.layer = LayerMask.NameToLayer(DEAD_LAYER);
			_myRigidbody.velocity = Vector3.zero;
			_myRigidbody.AddForce(knockbackForce*1.5f, ForceMode.VelocityChange);
			
			CameraShakeS.C.LargeShake();
			CameraShakeS.C.BigSleep();
			
			currentKnockbackCooldown = knockbackTime;
		}



	}

	public void AttackKnockback(Vector3 knockbackForce){
		
		_myRigidbody.AddForce(knockbackForce, ForceMode.Impulse);
		
	}

}
