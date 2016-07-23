using UnityEngine;
using System.Collections;

public class PlayerStatsS : MonoBehaviour {

	private const float NO_MANA_STOP_TIME = 0.1f;
	private const float NEAR_DEATH_STOP_TIME = 0.1f;
	private const float DEATH_KNOCKBACK_MULT = 2f;
	private const float BIG_KNOCKBACK_TIME = 0.4f;
	private const float DEATH_DRAG = 3.4f;

	private PlayerController myPlayerController;

	public bool godMode = true;

	//________________________________HEALTH
	private float _baseHealth = 3;
	private float _addedHealth = 0; // (upgradeable)
	private float _currentHealth;

	public float currentHealth { get { return _currentHealth; } }
	public float maxHealth { get { return (_baseHealth+_addedHealth);}}
	
	//________________________________MANA
	private float _baseMana = 4;
	private float _addedMana = 0; // (upgradeable)
	private float _currentMana;
	private RefreshDisplayS myRefresh;
	
	public float maxMana { get { return (_baseMana+_addedMana);}}
	public float currentMana { get { return (_currentMana);}}

	//________________________________ATTACK
	private float _baseStrength = 1;
	private float _addedStrength = 4; // (upgradeable)
	public float strengthAmt { get { return (_baseStrength+_addedStrength);}}

	
	private float _baseCrit = 0;
	private float _addedCrit = 0; // (upgradeable)
	public float critAmt { get { return (_baseCrit+_addedCrit);}}

	//________________________________DEFENSE
	private float _baseDefense = 3f;
	private float _addedDefense = 0;
	private float _currentDefense;

	public float maxDefense { get { return (_baseDefense+_addedDefense);}}
	public float currentDefense { get { return _currentDefense; } }

	//_______________________________SPEED
	private float _baseSpeed = 1f;
	private float _addedSpeed = 4f;
	public float speedAmt { get { return (_baseSpeed+_addedSpeed);}}


	//________________________________RECOVERY
	private float _recoveryCooldownBase = 0.3f;
	private float _recoveryCooldownMultiplier = 1f; // higher = slower cooldown (upgradeable)
	public float recoveryCooldownMax { get { return (_recoveryCooldownBase*_recoveryCooldownMultiplier);}}
	private float _currentCooldownTimer;
	
	private float blockRecoverMult = 0.5f;

	private float _recoverRateMin = 1f;
	private float _recoverRateMultiplier = 1f; // higher = faster recovery (upgradeable)

	public float recoverRate { get { return (_recoverRateMin*_recoverRateMultiplier);}}
	
	private float recoverBurdenMin = 0.08f;
	private float recoverBurdenMax = 0.3f;
	private float currentRegenCountdown;
	public float currentRegenCount { get { return currentRegenCountdown; } }

	private float _currentManaUsed = 0;

	//________________________________________DEFENSE

	private float _defenseKnockbackMult = 0.5f;
	private float _extraKnockbackMult = 2f;
	private BlockDisplay3DS myBlocker;

	//_____________________________________UNITY FUNCTIONS

	// Use this for initialization
	void Start () {
		Initialize();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		ManaRecovery();
	}

	//________________________________________PUBLIC FUNCTIONS

	public bool ManaCheck(float useAmount, bool reduce = true){

		if (godMode){
			return true;
		}
		else{
		if (_currentMana > 0){
			if (reduce){
			if (_currentMana >= useAmount){
				_currentMana -= useAmount;
				
				_currentManaUsed += useAmount;

			}else{
				_currentManaUsed += _currentMana;

				_currentMana = 0;

			}

			_currentCooldownTimer = recoveryCooldownMax;

			currentRegenCountdown = GetRegenTime();
			}

			return true;
		}
		else{
			return false;
		}
		}
	}

	public float GetRegenTime(){
		return recoverBurdenMin+_currentManaUsed/(_baseMana+_addedMana)*recoverBurdenMax;
	}

	//________________________________________PRIVATE FUNCTIONS

	private void ManaRecovery(){

		if (RecoveryCheck()){

			// first burn down cooldown, then recover
			if (_currentCooldownTimer > 0){
				_currentCooldownTimer -= Time.deltaTime;
			}
			else{
				if (myPlayerController.isBlocking){
				currentRegenCountdown -= recoverRate*blockRecoverMult*Time.deltaTime;
				}
				else{
					currentRegenCountdown -= recoverRate*Time.deltaTime;
				}
				if (currentRegenCountdown <= 0){
					_currentMana++;
					_currentManaUsed--;
					//myRefresh.DoFlash();
					if (_currentMana < maxMana){
						currentRegenCountdown = GetRegenTime();
					}
				}
			}

		}

	}

	private bool RecoveryCheck(){

		bool canRecover = true;

		if (_currentMana < maxMana && !PlayerIsDead()){
			if (myPlayerController != null){
				if (myPlayerController.isDashing || myPlayerController.isStunned || myPlayerController.InAttack()
				    || myPlayerController.chargingAttack){
					canRecover = false;
				}
			}
			else{
				canRecover = false;
			}
		}
		else{
			canRecover = false;
		}

		return canRecover;

	}

	private void Initialize(){

		myPlayerController = GetComponent<PlayerController>();
		myPlayerController.SetStatReference(this);

		_currentMana = maxMana;
		_currentHealth = maxHealth;
		_currentDefense = maxDefense;

	}

	public void TakeDamage(EnemyS damageSource, float dmg, Vector3 knockbackForce, float knockbackTime){

		if (!PlayerIsDead() && !myPlayerController.isDashing){
			if (myPlayerController.isBlocking && _currentDefense > 0){
				if (!godMode){
				_currentDefense-=dmg;
				}
				if (_currentDefense <= 0){
					_currentDefense = 0;
					CameraShakeS.C.TimeSleep(NO_MANA_STOP_TIME);
					CameraShakeS.C.SmallShake();
					myPlayerController.myRigidbody.AddForce(knockbackForce*_extraKnockbackMult, ForceMode.Impulse);
					myPlayerController.Stun(BIG_KNOCKBACK_TIME);
					myPlayerController.myAnimator.SetTrigger("Hurt");
					myPlayerController.FlashMana();

				}
				else{
					myBlocker.DoFlash();
					myPlayerController.myRigidbody.AddForce(knockbackForce*_defenseKnockbackMult, ForceMode.Impulse);
					CameraShakeS.C.MicroShake();
				}
				if (damageSource != null){
					damageSource.Deflect();
				}
			}else{
				
				myPlayerController.Stun(knockbackTime);
				myPlayerController.myAnimator.SetTrigger("Hurt");
				myPlayerController.FlashDamage();
					
				if(!godMode){
					_currentHealth -= dmg;
				}
					if (_currentHealth <= 0){
						_currentHealth = 0;
						myPlayerController.myRigidbody.drag = DEATH_DRAG;
						myPlayerController.myRigidbody.AddForce(knockbackForce*DEATH_KNOCKBACK_MULT, ForceMode.Impulse);
						myPlayerController.myAnimator.SetTrigger("Dead");
						myPlayerController.myAnimator.SetBool("IsDead", true);
					}
					else{
					myPlayerController.myRigidbody.AddForce(knockbackForce, ForceMode.Impulse);
					}
				//}

				if (_currentHealth <= 0){
					CameraShakeS.C.LargeShake();
					CameraShakeS.C.TimeSleepBigPunch(0.2f);
					CameraShakeS.C.DeathTimeEffect();
				}
				else if (_currentHealth <= 1){
					CameraShakeS.C.LargeShake();
					CameraShakeS.C.TimeSleep(0.12f, true);
				}else{
					CameraShakeS.C.SmallShake();
					CameraShakeS.C.TimeSleep(0.08f);
				}
	
			}
		}

	}

	public void ActivateDefense(){
		_currentDefense = maxDefense;
	}

	public bool PlayerIsDead(){
		return (_currentHealth <= 0);
	}

	public void AddBlocker(BlockDisplay3DS newBlock){
		myBlocker = newBlock;
	}
	public void AddRefresh(RefreshDisplayS newBlock){
		myRefresh = newBlock;
	}
}
