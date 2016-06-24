using UnityEngine;
using System.Collections;

public class PlayerStatsS : MonoBehaviour {

	private PlayerController myPlayerController;

	//________________________________HEALTH
	private float _baseHealth = 3;
	private float _addedHealth = 0; // (upgradeable)
	private float _currentHealth;

	public float currentHealth { get { return _currentHealth; } }
	public float maxHealth { get { return (_baseHealth+_addedHealth);}}
	
	//________________________________MANA
	private float _baseMana = 3;
	private float _addedMana = 0; // (upgradeable)
	private float _currentMana;
	
	public float maxMana { get { return (_baseMana+_addedMana);}}
	public float currentMana { get { return (_currentMana);}}

	//________________________________RECOVERY
	private float _recoveryCooldownBase = 0.3f;
	private float _recoveryCooldownMultiplier = 1f; // higher = slower cooldown (upgradeable)
	public float recoveryCooldownMax { get { return (_recoveryCooldownBase*_recoveryCooldownMultiplier);}}
	private float _currentCooldownTimer;

	private float _recoverRateMin = 0.6f;
	private float _recoverRateMultiplier = 1f; // higher = faster recovery (upgradeable)

	public float recoverRate { get { return (_recoverRateMin*_recoverRateMultiplier);}}

	private float _baseRecoverAdditive = 0.12f; // time added per mana used
	private float _additiveRateMult = 1f; // lower = faster recovery/mana used (upgradeable)
	
	public float recoverBurden { get { return (_baseRecoverAdditive*_additiveRateMult);}}
	private float currentRegenCountdown;
	public float currentRegenCount { get { return currentRegenCountdown; } }

	private float _currentManaUsed = 0;

	//________________________________________DEFENSE

	private float _defenseKnockbackMult = 0.5f;

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

	public bool ManaCheck(float useAmount){

		if (_currentMana > 0){
			if (_currentMana >= useAmount){
				_currentMana -= useAmount;
				_currentManaUsed += useAmount;
			}else{
				_currentManaUsed += _currentMana;
				_currentMana = 0;
			}
			_currentCooldownTimer = recoveryCooldownMax;

			currentRegenCountdown = GetRegenTime();

			return true;
		}
		else{
			return false;
		}
	}

	public float GetRegenTime(){
		return Mathf.Ceil(_currentManaUsed)*recoverBurden;
	}

	//________________________________________PRIVATE FUNCTIONS

	private void ManaRecovery(){

		if (RecoveryCheck()){

			// first burn down cooldown, then recover
			if (_currentCooldownTimer > 0){
				_currentCooldownTimer -= Time.deltaTime;
			}
			else{
				currentRegenCountdown -= recoverRate*Time.deltaTime;
				if (currentRegenCountdown <= 0){
					_currentMana++;
					_currentManaUsed--;
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
				if (myPlayerController.isDashing || myPlayerController.isStunned || myPlayerController.isShooting){
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

	}

	public void TakeDamage(float dmg, Vector3 knockbackForce, float knockbackTime){

		if (!PlayerIsDead()){
			if (myPlayerController.isBlocking && ManaCheck(dmg)){
				myPlayerController.myRigidbody.AddForce(knockbackForce*_defenseKnockbackMult, ForceMode.Impulse);
				CameraShakeS.C.MicroShake();
			}else{
				_currentHealth -= dmg;
				if (_currentHealth <= 0){
					_currentHealth = 0;
				}

				myPlayerController.Stun(knockbackTime);
				myPlayerController.myRigidbody.AddForce(knockbackForce, ForceMode.Impulse);

				if (_currentHealth <= 1){
					CameraShakeS.C.LargeShake();
					CameraShakeS.C.TimeSleep(0.12f);
				}else{
					CameraShakeS.C.SmallShake();
					CameraShakeS.C.TimeSleep(0.08f);
				}
	
			}
		}

	}

	public bool PlayerIsDead(){
		return (_currentHealth <= 0);
	}
}
