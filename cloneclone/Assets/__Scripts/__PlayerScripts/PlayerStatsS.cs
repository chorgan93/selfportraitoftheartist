using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStatsS : MonoBehaviour {

	private const float NO_MANA_STOP_TIME = 0.1f;
	private const float NEAR_DEATH_STOP_TIME = 0.1f;
	private const float DEATH_KNOCKBACK_MULT = 2f;
	private const float BIG_KNOCKBACK_TIME = 0.4f;
	private const float DEATH_DRAG = 3.4f;
	private const float CAN_USE_MANA = 0.15f;
	private const float BREAK_STAMINA_PENALTY = 1.5f;

	private const float DESPERATE_MULT = 0.8f;
	private const float DESPERATE_HEAL_MULT = 0.6f;

	public const float STAMINA_ADD_PER_LVL = 0.3f;
	public const float HEALTH_ADD_AMT = 1f;

	private const float anxiousChargeRate = 0.025f;

	private const float DARKNESS_ADD_RATE = 0.001f;
	private const float DARKNESS_ADD_DEATH = 3f/4f;
	public const float DARKNESS_MAX = 100f;
	
	private const float VIRTUE_ADD_AMT = 5f;

	public static bool healOnStart = false;

	private PlayerController myPlayerController;
	public PlayerController pRef { get { return myPlayerController; } }

	public static bool godMode = false;
	public static bool dontDoCountUp = false;

	public static bool PlayerCantDie = false;

	//________________________________LEVEL
	private int _startLevel = 1;
	private int _addedLevel = 0;
	public int currentLevel { get { return _startLevel+_addedLevel; } }

	//________________________________HEALTH
	private float _baseHealth = 6;
	private float _addedHealth = 0; // max 8 (for 12 total)
	public float addedHealth { get { return _addedHealth; } }
	private static float _currentHealth;

	public float currentHealth { get { return _currentHealth; } }
	private float _canRecoverHealth = 0f;
	private float _canRecoverHealthStart;
	private float allowHealthRecoverMaxTime = 3f;
	private float allowRecoverAddTime = 0.3f;
	private float currentAllowRecoverTime;

	private float allowHealthEndTime = 0.8f;
	private float allowHealthEndCountdown;
	private float allowHealthT;

	public float canRecoverHealth { get { return _canRecoverHealth; } }
	public float maxHealth { get { return (_baseHealth+_addedHealth);}}
	private float _savedHealth = 8f;
	public float savedHealth { get { return _savedHealth; } }

	public static float _currentDarkness = 0f;
	public float currentDarkness {get { return _currentDarkness; } }
	
	//________________________________MANA
	private float _baseMana = 3;
	private float _addedMana = 0; // max 16 (for 20 total)
	public float addedMana { get { return _addedMana; } }
	private float _currentMana;
	private float _comboStartMana;
	private RefreshDisplayS myRefresh;

	private float _savedMana = 5;

	public float manaLevel { get { return (_baseMana+_addedMana);} }
	public float maxMana { get { return (_baseMana+_addedMana*STAMINA_ADD_PER_LVL);}}
	public float currentMana { get { return (_currentMana);}}

	private float _overchargeMana;
	public float overchargeMana { get { return _overchargeMana; } }
	private float overchargePenalty = 0.4f;

	//________________________________CHARGE
	private float _baseCharge = 5f;
	private float _addedCharge = 0;
	public float addedCharge { get { return _addedCharge; }}
	private static float _currentCharge = 0f;

	public float addedChargeLv {get { return Mathf.Round(_addedCharge/10f); } }
	
	public float maxCharge { get { return _baseCharge+_addedCharge;}}
	public float currentCharge { get { return _currentCharge;}}
	private float _savedCharge = 50f;

	private bool _exhausted = false;


	//________________________________CHARGE RECOVERY

	private int _currentChargeRecoverLv = 3;
	private float _baseChargeRecover = 1.2f;
	private float _addedChargeRecoverPerLevel = .2f;
	public float currentChargeRecover  { get { return 
			_baseChargeRecover+_addedChargeRecoverPerLevel*(_currentChargeRecoverLv*1f-1f); } }

	public float currentChargeRecoverLv { get { return _currentChargeRecoverLv*1f; } }


	//________________________________ATTACK
	private float _baseStrength = 0.7f;
	private float _addedStrength = 0; // (upgradeable)
	public float strengthLvl { get { return (_baseStrength*10f+_addedStrength-4f); } }
	public float addedStrength { get { return _addedStrength; } }

	
	private float _baseCrit = 0;
	private float _addedCrit = 0; // (upgradeable)
	public float critAmt { get { return (_baseCrit+_addedCrit);}}
	
	//________________________________VIRTUE
	private float _baseVirtue = 5f;
	private float _addedVirtue = 0; // (upgradeable)
	public float addedVirtue { get { return _addedVirtue; } }
	private float _usedVirtue = 0;
	public float usedVirtue {get {return _usedVirtue; } }
	public float usedVirtuePercent {get { return _usedVirtue/(_baseVirtue+_addedVirtue); } }
	public float virtueAmt { get { return (_baseVirtue+_addedVirtue);}}

	//________________________________DEFENSE
	private float _baseDefense = 9f;
	private float _addedDefense = 0;
	private float _currentDefense;

	public float addedDefense { get { return _addedDefense; } }

	public float maxDefense { get { return (_baseDefense+_addedDefense);}}
	public float currentDefense { get { return _currentDefense; } }

	//_______________________________SPEED
	private float _baseSpeed = 1f;
	private float _addedSpeed = 0f;
	public float speedAmt { get { return (_baseSpeed+_addedSpeed);}}


	//________________________________RECOVERY
	private float _baseRecovery = 0.75f;
	private float _addedRecovery = 0f;
	public float currentRecovery { get { return _baseRecovery+_addedRecovery; } }

	private float _recoveryCooldownBase = 0.4f;
	private float _recoveryCooldownMultiplier = 1f; // higher = slower cooldown (upgradeable)
	public float recoveryCooldownMax { get { return (_recoveryCooldownBase*(_recoveryCooldownMultiplier-
			                                                                        (0.5f*_recoveryCooldownMultiplier)));}}
	private float _currentCooldownTimer;
	public float currentCooldownTimer { get { return _currentCooldownTimer; } }
	
	private float blockRecoverMult = 0.5f;

	private float _recoverRateMin = 0.35f;
	private float _recoverRateMultiplier = 1f; // higher = faster recovery (upgradeable)
	private float recoverRateAddPerLevel = 0.02f;

	public float recoverRate { get { return (_recoverRateMin*(_recoverRateMultiplier+recoverRateAddPerLevel*(currentRecoverRateLv-1)));}}

	private float recoverRateIncrease;
	private float recoverRateAccelBase = .5f;
	private float recoverRateAccelAddPerLevel = 0.15f;
		private float recoverRateAccel { get { return recoverRateAccelBase+recoverRateAccelAddPerLevel*(currentRecoverRateLv-1); } }
	private int _recoverRateLv = 1;
	private int _addedRateLv = 0;
	public int currentRecoverRateLv { get { return _recoverRateLv+_addedRateLv; } }

	public int addedRateLv { get { return _addedRateLv; } }
	
	private float recoverBurdenMin = 0.1f;
	private float recoverBurdenMax = 0.25f;
	private float currentRegenCountdown;
	public float currentRegenCount { get { return currentRegenCountdown; } }

	private float _currentManaUsed = 0;

	//________________________________________DEFENSE

	private float _defenseKnockbackMult = 0.5f;
	private float _extraKnockbackMult = 2f;
	private BlockDisplay3DS myBlocker;

	private bool unstoppableActivatedOnHit = false;

	//________________________________________CONDEMNED

	private bool delayDeath = false;
	private float delayDeathCountdown = 0f;
	private float delayDeathCountdownMult = 0f;
	public float condemnedCurrentTime { get { return delayDeathCountdown; } }

	//________________________________________OTHER
	public GameObject hurtPrefab;

	private PlayerStatDisplayS _uiReference;
	public PlayerStatDisplayS uiReference { get { return _uiReference; } }
	private WarningManagerS warningReference;

	private ItemEffectS _itemEffect;
	public ItemEffectS itemEffect { get { return _itemEffect; } }

	private FlashEffectS _hurtFlash;
	private FlashEffectS _killFlash;

	private PlayerHealEffect rechargeEffectRef;

	[Header("Special Scene Properties")]
	public bool arcadeMode = false;

	//_____________________________________UNITY FUNCTIONS

	// Use this for initialization
	void Start () {
		Initialize();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		ManaRecovery();
		//FakeManaRecovery();
		ChargeRecovery();
		CondemnedHandler();
		DarknessAdd();
	}

	void Update(){
		HealthRecovery();
		#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
		if (Input.GetKeyDown(KeyCode.G)){
			godMode = !godMode;
		}
		#endif
	}

	//________________________________________PUBLIC FUNCTIONS

	public bool ManaCheck(float useAmount, bool reduce = true){

		if (godMode || arcadeMode){
			return true;
		}else if (!ManaUnlocked()){
			return false;
		}
		else{
		if (_currentMana > 0){
			if (reduce){
					if (_currentCooldownTimer <= 0){
						_comboStartMana = _currentMana;
					}
			if (_currentMana >= useAmount){
				_currentMana -= useAmount;
				
				_currentManaUsed += useAmount;

						if (_currentMana < maxMana*CAN_USE_MANA*0.5f){

							warningReference.NewMessage("— Stamina LOW —", Color.cyan, Color.grey, true, 1);
							//warningReference.NewMessage("— T H A N K _ Y O U ! ! —", Color.black, Color.cyan, true, 1);
						}

			}else{
				_currentManaUsed += _currentMana;

						_overchargeMana = useAmount-_currentMana;
						if (_overchargeMana > maxMana){
							_overchargeMana = maxMana;
						}
						// taking out overcharge for a bit
						_overchargeMana = 0;
				_currentMana = 0;
						_exhausted = true;

						warningReference.NewMessage("! ! STAMINA OUT ! !", Color.cyan, Color.red, true, 1);
						//warningReference.NewMessage("— T H A N K _ Y O U ! ! —", Color.black, Color.magenta, true, 1);

			}

			_currentCooldownTimer = recoveryCooldownMax;
					if (_currentMana <= 0){
						_currentCooldownTimer*=BREAK_STAMINA_PENALTY;
					}

			currentRegenCountdown = GetRegenTime();

			}

				_uiReference.UpdateFills();
			return true;
		}
		else{
			return false;
		}
		}
	}

	public float GetRegenTime(){
		float baseBurden = recoverBurdenMin+_currentManaUsed/(_baseMana+_addedMana)*recoverBurdenMax;

		return baseBurden - baseBurden*0.2f*(currentRecovery-1f);
	}

	public bool ChargeCheck(float reqCharge, bool useCharge = true){

		bool canUse =  (_currentCharge > 0);
		if (arcadeMode){
			return true;
		}else{

		if (useCharge && canUse){
			if (reqCharge > _currentCharge){
				reqCharge = _currentCharge;
			}
			_currentCharge -= reqCharge;
			_uiReference.ChargeUseEffect(reqCharge);
		}

		if (_currentCharge <= 0f){
			_currentCharge = 0f;

			if (canUse && useCharge){
				warningReference.NewMessage("! CHARGE OUT !", Color.cyan, Color.magenta, false, 1);
			}
		}else if (_currentCharge < maxCharge*0.2f){

			if (canUse && useCharge){
				warningReference.NewMessage("— Charge LOW —", Color.white, Color.magenta, false, 0);
			}
		}
		_uiReference.UpdateFills();
		return canUse;
		}
	}

	public void DrivenCheck(){
		if (myPlayerController.playerAug.drivenAug){
			RecoverCharge(100f);
		}
	}

	public void RecoverCharge(float addPercent, bool itemEffect = false, bool anxiousEffect = false){
		// add to charge (old)
		float amtAdded = addPercent*maxCharge*currentChargeRecover;
		if (_currentCharge + amtAdded > maxCharge){
			amtAdded = maxCharge-_currentCharge;
		}
		_uiReference.ChargeAddEffect(amtAdded);
		_currentCharge += amtAdded;
		warningReference.EndShow("! Charge OUT !");
		if (_currentCharge > maxCharge){
			_currentCharge = maxCharge;
		}
		if (_currentCharge >= maxCharge*0.2f){

			warningReference.EndShow("— Charge LOW —");
		}
		if (itemEffect){
			
			myPlayerController.FlashCharge();
			CameraShakeS.C.SmallShakeCustomDuration(0.6f);
			CameraShakeS.C.TimeSleep(0.08f);
			_itemEffect.Flash(myPlayerController.myRenderer.material.color);
		
		}else if (addPercent >= 100f){
			rechargeEffectRef.TriggerChargeEffect();
		}

		_uiReference.UpdateFills();

		// add to stamina (new)
		/*float amtAdded = addPercent*maxMana*currentChargeRecover;
		if (_overchargeMana > 0){
			if (_overchargeMana - amtAdded <= 0){
				amtAdded -= _overchargeMana;
				_overchargeMana = 0;
			}else{
				_overchargeMana -= amtAdded;
				amtAdded = 0;
			}
		}
		if (_currentMana + amtAdded > maxMana){
			amtAdded = maxMana-_currentMana;
		}
		//_uiReference.ChargeAddEffect(amtAdded);
		_currentMana += amtAdded;
		if (_currentMana > maxMana){
			_currentMana = maxMana;
		}
		if (itemEffect){

			myPlayerController.FlashCharge();
			CameraShakeS.C.SmallShakeCustomDuration(0.6f);
			CameraShakeS.C.TimeSleep(0.08f);
			_itemEffect.Flash(myPlayerController.myRenderer.material.color);

		}**/
	}

	public void AddUIReference(PlayerStatDisplayS sd){
		_uiReference = sd;
	}

	//________________________________________PRIVATE FUNCTIONS

	private void DarknessAdd(){
		if (!PlayerIsDead() && !myPlayerController.talking && !arcadeMode){
			_currentDarkness += Time.deltaTime*DARKNESS_ADD_RATE;
		}
	}

	private void ChargeRecovery(){
		if (myPlayerController.playerAug.anxiousAug && myPlayerController.inCombat){
			if (_currentCharge < maxCharge){
				RecoverCharge(anxiousChargeRate*Time.deltaTime, false, true);
			}
		}
	}

	private void FakeManaRecovery(){
		if (RecoveryCheck()){

			// first burn down cooldown, then recover
			if (_currentCooldownTimer > 0){
				_currentCooldownTimer -= Time.deltaTime;
				if (_currentCooldownTimer < 0){

					_currentCooldownTimer = 0;
				}
				recoverRateIncrease = 0f;
			}
		}
	}

	private void ManaRecovery(){

		if (RecoveryCheck()){

			// first burn down cooldown, then recover
			if (_currentCooldownTimer > 0){
				_currentCooldownTimer -= Time.deltaTime;
				if (_currentCooldownTimer < 0){
					
					_currentCooldownTimer = 0;
				}
				recoverRateIncrease = 0f;
			}
			else{

				float actingRecoverRate = recoverRate + recoverRateIncrease;

				// new way (simple)
				if (myPlayerController.isBlocking){
					actingRecoverRate*=blockRecoverMult;
				}else{
					recoverRateIncrease+=recoverRateAccel*Time.deltaTime;
				}
				if (_overchargeMana <= 0){
					_currentMana+=actingRecoverRate*Time.deltaTime*maxMana;
					_currentManaUsed-=actingRecoverRate*Time.deltaTime*maxMana;
				}else{
					_overchargeMana -= actingRecoverRate*maxMana*Time.deltaTime*overchargePenalty;
				}

				if (_exhausted){

					if (_currentMana >=  maxMana*CAN_USE_MANA/2f){
						warningReference.EndShow("! ! STAMINA OUT ! !");
					}
					if (_currentMana >= maxMana*CAN_USE_MANA){
						_exhausted = false;
					}
				}

				if (_currentMana >=  maxMana*CAN_USE_MANA*0.4f){
					warningReference.EndShow("— Stamina LOW —");
				}

				if (_currentMana > maxMana){
					_currentMana = maxMana;
				}

				_comboStartMana = _currentMana;
			}

			_uiReference.UpdateFills();

		}

	}

	private void HealthRecovery(){
		if (pRef.playerAug.desperateAug && _canRecoverHealth > 0){
			currentAllowRecoverTime -= Time.deltaTime;
			if (currentAllowRecoverTime <= 0){

				allowHealthEndCountdown += Time.deltaTime;
				allowHealthT = allowHealthEndCountdown/allowHealthEndTime;

				allowHealthT = Mathf.Sin(allowHealthT * Mathf.PI * 0.5f);
				_canRecoverHealth = Mathf.Lerp(_canRecoverHealthStart, 0f, allowHealthT);
				if (_canRecoverHealth <= 0 || allowHealthEndCountdown >= allowHealthEndTime){
					_canRecoverHealth = 0f;
				}
			}
			_uiReference.UpdateFills();
		}
	}

	private bool RecoveryCheck(){

		bool canRecover = true;

		if (_currentMana < maxMana && !PlayerIsDead()){
			if (myPlayerController != null){
				if (myPlayerController.isDashing || myPlayerController.isSprinting || myPlayerController.InWitchAnimation() 
					|| myPlayerController.InAttack()
				//if (myPlayerController.InSpecialAttack()
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

	private void CondemnedHandler(){
		if (delayDeath && !myPlayerController.usingitem){
			if (!myPlayerController.inCombat){
				Heal(1f,false);
				delayDeath = false;
			}else{
			delayDeathCountdown -= Time.deltaTime*delayDeathCountdownMult;
			if (delayDeathCountdown <= 0){
				TakeDamage(null, 1f, Vector3.zero, 0.2f, true, true);
				delayDeath = false;
			}
			}
			_uiReference.UpdateFills();
		}
	}

	public bool ManaUnlocked(){
		bool canUseMana = true;
		if (RecoveryCheck() && _exhausted && _currentMana < maxMana*CAN_USE_MANA){
			canUseMana = false;
		}
		return canUseMana;
	}

	private void Initialize(){

		myPlayerController = GetComponent<PlayerController>();
		myPlayerController.SetStatReference(this);

		rechargeEffectRef = GetComponentInChildren<PlayerHealEffect>();

		InitializeStats();

		_hurtFlash = CameraEffectsS.E.hurtFlash;
		_killFlash = CameraEffectsS.E.deathFlash;

		_itemEffect = GetComponentInChildren<ItemEffectS>();

		_currentMana = maxMana;
		// TODO find a way to remove doWakeUp from this without screwing everything up
		if ((PlayerController.doWakeUp && !PlayerController.dontHealWakeUp) || healOnStart){
			_currentHealth = maxHealth;
			_currentCharge = maxCharge;
			PlayerInventoryS.I.RefreshRechargeables();
			healOnStart = false;
		}
		PlayerController.dontHealWakeUp = false;
		_currentDefense = maxDefense;

		warningReference = GameObject.Find("WarningText").GetComponent<WarningManagerS>();
		delayDeathCountdownMult = 0f;

		if (arcadeMode){
			myPlayerController.SetAllowItem(false);
		}

	}

	private void InitializeStats(){
		if (PlayerInventoryS.I.earnedUpgrades.Count > 0){
			foreach(int i in PlayerInventoryS.I.earnedUpgrades){
				// count mana
				if (i == 0){
					_addedHealth+= HEALTH_ADD_AMT;
				}
				if (i == 1){
					_addedMana++;
				}
				if (i == 2){
					_addedCharge+=1f;
				}
				/*if (i == 3){
					//_addedStrength++;
					_addedVirtue += VIRTUE_ADD_AMT;
				}**/
				if (i == 4){
					_currentChargeRecoverLv++;
				}
				if (i == 5){
					_addedRateLv++;
				}
				if (i == 6){
					_addedStrength++;
				}
				_addedLevel++;
			}
		}
		_addedVirtue = VIRTUE_ADD_AMT*PlayerInventoryS.I.GetVPUpgradeCount();
	}

	public void AddStat(int i){
		if (i == 0){
			_addedHealth+= HEALTH_ADD_AMT;
			_currentHealth+= HEALTH_ADD_AMT;
		}
		if (i == 1){
			_addedMana++;
			_currentMana+=STAMINA_ADD_PER_LVL;
		}
		if (i == 2){
			_currentCharge+=1f;
			_addedCharge+=1f;
		}
		if (i == 3){
			//_addedStrength++;
			_addedVirtue += VIRTUE_ADD_AMT;
		}
		if (i == 4){
			_currentChargeRecoverLv++;
		}
		if (i == 5){
			_addedRateLv++;
		}
		if (i == 6){
			_addedStrength ++;
		}
		_addedLevel++;
		_uiReference.UpdateFills();
	}

	public void ResetStamina(bool fromVirtue = false, bool onlyCombo = false, float comboReduction = 1f){

		if (onlyCombo){
			if (_comboStartMana > _currentMana){
				_currentMana = _comboStartMana*comboReduction;
				_comboStartMana = _currentMana;

				_currentManaUsed = maxMana-_comboStartMana*comboReduction;
				_currentCooldownTimer = recoveryCooldownMax;
				currentRegenCountdown = GetRegenTime();
			}
		}else{
			_currentMana = maxMana;
			_currentManaUsed = 0f;
			_currentCooldownTimer = 0f;
		}
		_overchargeMana = 0f;
		myPlayerController.FlashMana(true);
		if (!fromVirtue){
		CameraShakeS.C.SmallShakeCustomDuration(0.6f);
		CameraShakeS.C.TimeSleep(0.08f);
		_itemEffect.Flash(myPlayerController.myRenderer.material.color);
		}else{
			rechargeEffectRef.TriggerStaminaEffect();
		}

		warningReference.EndShow("— Stamina LOW —");

		warningReference.EndShow("! ! STAMINA OUT ! !");
		_uiReference.UpdateFills();

	}

	public void Heal(float healAmt, bool doEffect = true){
		_currentHealth += healAmt;
		if (_currentHealth >= maxHealth){
			myPlayerController.playerAug.canUseUnstoppable = true;
			_currentHealth = maxHealth;
			delayDeathCountdownMult = 0f;
		}
		if (maxHealth - _currentHealth < _canRecoverHealth){
			_canRecoverHealth = maxHealth-_currentHealth;
		}
		if (doEffect){
			myPlayerController.FlashHeal();
			_itemEffect.Flash(myPlayerController.myRenderer.material.color);
			CameraShakeS.C.SmallShakeCustomDuration(0.6f);
			CameraShakeS.C.TimeSleep(0.08f);
		}else if (_canRecoverHealth > 0){
			_canRecoverHealth -= healAmt;
			_canRecoverHealthStart = _canRecoverHealth;
			if (_canRecoverHealth <= 0){
				_canRecoverHealth = 0f;
				_canRecoverHealthStart = 0f;
				allowHealthEndCountdown = allowHealthEndTime;
			}
		}
		warningReference.EndShow("! ! HEALTH LOW ! !");
		delayDeath = false;
		delayDeathCountdown = 0f;
		_uiReference.UpdateFills();
	}

	public void DamageEffect(float dmgAngle){
		Vector3 spawnPos = transform.position;
		spawnPos.z -= 2f;
		GameObject newDamageEffect = Instantiate(hurtPrefab, spawnPos, Quaternion.identity)
			as GameObject;
		newDamageEffect.transform.Rotate(new Vector3(0,0,dmgAngle+Random.insideUnitCircle.x*30f)); 
		if (Random.Range(0f,1f) < 0.5f){
			Vector3 scaleFlip = newDamageEffect.transform.localScale;
			scaleFlip.x *= -1f;
			newDamageEffect.transform.localScale = scaleFlip;
		}
	}

	public void TakeDamage(EnemyS damageSource, float dmg, Vector3 knockbackForce, float knockbackTime, bool dontTriggerWitch = false, bool overrideAll = false){

		unstoppableActivatedOnHit = false;
		dmg*=DifficultyS.GetPunishMult();
		float healthBeforeTakingDmg = _currentHealth;
		if (myPlayerController.playerAug.lovedAug){
			dmg*=0.75f;
		}
		if (myPlayerController.playerAug.hatedAug){
				dmg*=PlayerAugmentsS.HATED_MULT;
			}

		if (dmg > maxHealth){
			dmg = maxHealth;
		}

		if ((!PlayerIsDead() && !delayDeath && !myPlayerController.allowCounterAttack && !myPlayerController.doingCounterAttack && !myPlayerController.usingitem
		    && !myPlayerController.delayWitchTime && (!myPlayerController.isDashing || (myPlayerController.isDashing && myPlayerController.IsSliding())) 
			&& !myPlayerController.talking && !PlayerSlowTimeS.witchTimeActive) || overrideAll){
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
					CameraShakeS.C.SmallShake();
				}
				if (damageSource != null){
					damageSource.Deflect();
				}
			}else{
				
				myPlayerController.Stun(knockbackTime);
				myPlayerController.myAnimator.SetTrigger("Hurt");
				myPlayerController.FlashDamage();

				RankManagerS.R.TakeHit();

				EffectSpawnManagerS.E.SpawnDamangeNum(transform.position, false, true, dmg);
					
				if(!godMode){
					myPlayerController.currentCombatManager.SentHurtMessage(true);
					if (_currentHealth-dmg <= 0 && myPlayerController.playerAug.unstoppableAug && myPlayerController.playerAug.canUseUnstoppable 
						&&_currentHealth > maxHealth*0.01f){
						_currentHealth = maxHealth*0.01f;
						myPlayerController.playerAug.canUseUnstoppable = false;
						//unstoppableActivatedOnHit = true;
					}else{
						_currentHealth -= dmg;
					}
					if (PlayerCantDie && _currentHealth <= 0){
						_currentHealth = 0.1f;
					}
					CameraShakeS.C.CancelSloMo();
					//ChargeCheck(10f);

				}
				if (_currentHealth <= 0){
					_currentHealth = 0;
					if (!myPlayerController.playerAug.condemnedAug || (myPlayerController.playerAug.condemnedAug && delayDeath)){
					myPlayerController.playerSound.PlayDeathSound();
					myPlayerController.playerSound.SetWalking(false);
						myPlayerController.myRigidbody.drag = DEATH_DRAG;
						_canRecoverHealth = 0f;
						myPlayerController.myRigidbody.AddForce(knockbackForce*DEATH_KNOCKBACK_MULT, ForceMode.Impulse);
						myPlayerController.myAnimator.SetTrigger("Dead");
						myPlayerController.myAnimator.SetBool("IsDead", true);

					List<int> saveBuddyList = new List<int>();
					saveBuddyList.Add(myPlayerController.ParadigmIBuddy().buddyNum);
					if (myPlayerController.ParadigmIIBuddy() != null){
						saveBuddyList.Add(myPlayerController.ParadigmIIBuddy().buddyNum);
					}
					PlayerInventoryS.I.SaveLoadout(myPlayerController.equippedWeapons, myPlayerController.subWeapons,
					                               saveBuddyList);
						
					//_uiReference.cDisplay.DeathPenalty();

						RankManagerS.R.DieInCombat();
					if (dontDoCountUp){
						dontDoCountUp = false;
						}else if (!SceneManagerS.inInfiniteScene){
						_uiReference.transform.parent.GetComponentInChildren<DarknessPercentUIS>().ActivateDeathCountUp();
						_currentDarkness += DARKNESS_ADD_DEATH;
					}

					_killFlash.Flash();

					myPlayerController.myLockOn.enemyHealthUI.EndLockOn();

					GetComponent<BleedingS>().StartDeath();

					CameraFollowS.F.RemoveLimits();
					warningReference.EndAll();
						delayDeath = false;
						PlayerController.doWakeUp = true;
					}else{
						// start condemned process
						delayDeath = true;
						delayDeathCountdownMult += 1f;
						delayDeathCountdown = PlayerAugmentsS.CONDEMNED_TIME;
						if (pRef.playerAug.desperateAug && !godMode){
							//if (!unstoppableActivatedOnHit){
								currentAllowRecoverTime = allowHealthRecoverMaxTime;
								_canRecoverHealth = dmg*DESPERATE_MULT;
								allowHealthEndCountdown = 0f;
								if (_canRecoverHealth > healthBeforeTakingDmg){
									_canRecoverHealth = (healthBeforeTakingDmg-_currentHealth)*DESPERATE_MULT;
								}
								_canRecoverHealthStart = _canRecoverHealth;
							//}
						}
					}
					}
				else{
					_hurtFlash.Flash();
					myPlayerController.playerSound.PlayHurtSound();
					myPlayerController.myRigidbody.AddForce(knockbackForce, ForceMode.Impulse);
					if (pRef.playerAug.desperateAug){
						if (healthBeforeTakingDmg >= maxHealth*0.01f){
							currentAllowRecoverTime = allowHealthRecoverMaxTime;
							_canRecoverHealth = dmg*DESPERATE_MULT;
							allowHealthEndCountdown = 0f;
							if (_canRecoverHealth > healthBeforeTakingDmg){
								_canRecoverHealth = (healthBeforeTakingDmg-_currentHealth)*DESPERATE_MULT;
							}
							_canRecoverHealthStart = _canRecoverHealth;
						}
					}

					if(_currentHealth<maxHealth*0.33f){

						warningReference.NewMessage("! ! HEALTH LOW ! !", Color.white, Color.red, false, 2);
					}
					}
				//}

				if (knockbackTime > 0){
					if (damageSource != null){
						CameraPOIS.POI.JumpToMidpoint(transform.position, damageSource.transform.position);
					}else{
						CameraPOIS.POI.JumpToPoint(transform.position);
					}
				}
				if (_currentHealth <= 0 && !delayDeath){
					CameraShakeS.C.LargeShake();
					CameraShakeS.C.TimeSleepBigPunch(0.5f);
					CameraShakeS.C.DeathTimeEffect();
				}
				else if (_currentHealth <= 1){
					CameraShakeS.C.LargeShake();
					CameraShakeS.C.TimeSleep(0.28f, true);
				}else{
					CameraShakeS.C.SpecialAttackShake();

					if (knockbackTime > 0){
						CameraShakeS.C.TimeSleep(0.24f, true);
					}
				}
	
			}
		}else{
			if (myPlayerController.AllowDodgeEffect() && !dontTriggerWitch){
				myPlayerController.WitchTime(damageSource);
			}
		}

		_uiReference.UpdateFills();

	}

	public void GoToUnstoppableHealth(){
		if (_currentHealth > 0 && _currentHealth <= maxHealth*0.01f && myPlayerController.playerAug.unstoppableAug){
			_currentHealth = maxHealth*0.012f;
		}
	}

	public void DeathCountUp(bool isReduced = false){

		_uiReference.transform.parent.GetComponentInChildren<DarknessPercentUIS>().ActivateDeathCountUp();
		if (isReduced){
			_currentDarkness += DARKNESS_ADD_DEATH*0.5f;
		}else{
		_currentDarkness += DARKNESS_ADD_DEATH;
		}
	}

	public void DesperateRecover(float amtToRecover){
		if (pRef.playerAug.desperateAug && _canRecoverHealth > 0 && allowHealthEndCountdown <= 0){
			if (amtToRecover*DESPERATE_HEAL_MULT > _canRecoverHealth){
				Heal(_canRecoverHealth);
				_canRecoverHealth = 0;
			}else{
			Heal(amtToRecover*DESPERATE_HEAL_MULT, false);
			//_canRecoverHealth -= amtToRecover*DESPERATE_HEAL_MULT;
			//	_canRecoverHealthStart = _canRecoverHealth;
			}
			currentAllowRecoverTime+=allowRecoverAddTime;
			if (_canRecoverHealth < 0){
				_canRecoverHealth = 0f;
				_canRecoverHealthStart = 0f;
				allowHealthEndCountdown = allowHealthEndTime;
			}
		}
	}

	public void ActivateDefense(){
		_currentDefense = maxDefense;
	}

	public bool PlayerIsDead(){
		return (_currentHealth <= 0 && delayDeathCountdown <= 0f);
	}

	public void AddBlocker(BlockDisplay3DS newBlock){
		myBlocker = newBlock;
	}
	public void AddRefresh(RefreshDisplayS newBlock){
		myRefresh = newBlock;
	}

	public float OverCooldownMult(){
		return (_currentCooldownTimer/(recoveryCooldownMax*BREAK_STAMINA_PENALTY));
	}

	public float strengthAmt(){

		if (arcadeMode){
			return 2f;
		}else{
			return (_baseStrength+_addedStrength*0.075f);
		}
	}

	//__________________________________STAT UPGRADES
	public void AddStamina(float numToAdd = 1){
		_addedMana+=numToAdd;
		_currentMana=maxMana;
		_addedLevel++;
		_uiReference.UpdateFills();
	}

	public void AddHealth(float numToAdd = 1){
		_addedHealth += numToAdd;
		_currentHealth += numToAdd;
		_addedLevel++;
		_uiReference.UpdateFills();
	}
	public void AddCharge(float numToAdd = 10){
		_addedCharge += numToAdd;
		_currentCharge += numToAdd;
		_addedLevel++;
		_uiReference.UpdateFills();
	}

	public void ChangeVirtue(float numChange){
		_usedVirtue += numChange;
		if (_usedVirtue < 0){
			_usedVirtue = 0;
		}
	}

	public void FullRecover(){
		_currentHealth = maxHealth;
		_currentCharge = maxCharge;
		_overchargeMana = 0f;
		_currentMana = maxMana;
		warningReference.EndAll();
		_uiReference.UpdateFills();
	}

	public void SaveStats ()
	{
		_savedHealth = _currentHealth;
		_savedCharge = _currentCharge;
		myPlayerController.playerAug.canUseUnstoppable = true;
		//_savedMana = _currentMana;
	}
	public void ResetCombatStats(){
		_currentHealth = _savedHealth;
		delayDeath = false;
		delayDeathCountdown = 0f;
		_currentCharge = _savedCharge;
		myPlayerController.playerAug.canUseUnstoppable = true;
		_canRecoverHealth = 0;
		delayDeathCountdownMult = 0f;
		//_currentMana = _savedMana;
		warningReference.EndAll();
		if (PlayerInventoryS.I.GetItemCount(0) == 1){
			warningReference.NewMessage("— REWINDs LOW —",  warningReference.resetGreen, Color.white, false);
		}
		if (PlayerInventoryS.I.GetItemCount(0) == 0){
			warningReference.NewMessage("! REWINDS OUT !",  warningReference.resetGreen,Color.red, false, 1);
		}
		_uiReference.UpdateFills();
	}

	public bool InCondemnedState(){
		bool inCondemned = false;
		if (delayDeath && delayDeathCountdown > 0){
			inCondemned = true;
		}
		return inCondemned;
	}
}
