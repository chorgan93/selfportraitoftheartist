using UnityEngine;
using System.Collections;

public class PlayerAugmentsS : MonoBehaviour {

	// script handles all upgrades from sub-weapons and virtues
	private PlayerController _playerReference;

	public const float ADAPTIVE_DAMAGE_BOOST = 1.5f;
	//public const float ENRAGED_DAMAGE_BOOST = 2f; // old
	public const float ENRAGED_DAMAGE_BOOST = 1.5f;
	public const float CAUTIOUS_MULT = 0.88f;
	public const float CONDEMNED_TIME = 3f;
	public const float HATED_MULT = 1.5f;
	public const float BIOS_MULT = 1.32f;

	//________________________________________________________weapon augmentations
	private bool _lunaAug = false;
	public bool lunaAug { get { return _lunaAug; } }
	public const float lunaAugAmt = 1.4f;

	private bool _animaAug = false;
	public bool animaAug { get { return _animaAug; } }
	public const float addSpeedPerBios = 0.01f;

	private bool _solAug = false;
	public bool solAug { get { return _solAug; } }

	private bool _thanaAug = false;
	public bool thanaAug { get { return _thanaAug; } }
	public const float thanaAugAmt = 1.3f;

	private bool _pyraAug = false;
	public bool pyraAug { get { return _pyraAug; } }
	public const float pyraAugAmt = 0.75f;
	public const float pyraDoubleAugAmt = 0.55f;

	private bool _realGaeaAug = false;
	public bool realGaeaAug { get { return _realGaeaAug; } }
	public const float realGaeaAugAmt = 1.5f;

	private bool _erebosAug = false;
	public bool erebosAug { get { return _erebosAug; } }
	public const float erebosAugAmt = 2f;

	private bool _aquaAug = false;
	public bool aquaAug { get { return _aquaAug; } }
	public const float aquaAugMult = 1.75f;

	private bool _aeroAug = false;
	public bool aeroAug { get { return _aeroAug; } }

	private bool _biosAug = false;
	public bool biosAug { get { return _biosAug; } }


	private bool _fosAug = false;
	public bool fosAug { get { return _fosAug; } }

	private bool _aetherAug = false;
	public bool aetherAug { get { return _aetherAug; } }

	private bool _doubleMantra = false;
	public bool doubleMantra { get { return _doubleMantra; } }

	//____________________________________________________________virtue augmentations
	// index 0
	private bool _unstoppableAug = false;
	public bool unstoppableAug { get { return _unstoppableAug; } }
	[HideInInspector]
	public bool canUseUnstoppable = true;

	// NOT USED
	private bool _opportunisticAug = false; // VIRTUE NOT USED
	public bool opportunisticAug { get { return _opportunisticAug; } }

	// NOT USED
	private bool _dashAug = false; // VIRTUE NOT USED
	public bool dashAug { get { return _dashAug; } }

	// index 1 (extra killAt dmg)
	private bool _determinedAug = false;
	public bool determinedAug { get { return _determinedAug; } }


	// index 2 (lower stamina at max health
	private bool _empoweredAug = false;
	public bool empowered { get { return _empoweredAug; } }

	// index 20 (more dmg at low health)
	private bool _enragedAug = false;
	public bool enragedAug { get { return _enragedAug; } }
	private bool _enragedActive = false;
	private float enragedTimeMax = 3f;
	private float enragedCountdown = 0f;

	// index 4
	private bool _adaptiveAug = false;
	public bool adaptiveAug { get { return _adaptiveAug; } }

	// index 5
	private bool _perceptiveAug = false;
	public bool perceptiveAug { get { return _perceptiveAug; } }
	
	// index 9 (stamina recharge on dodge)
	private bool _agileAug = false;
	public bool agileAug { get { return _agileAug; } }

	// index 7 (parry projectiles for massive damage)
	private bool _repellantAug = false;
	public bool repellantAug { get { return _repellantAug; } }
	
	// index 8 (stronger buddies)
	private bool _trustingAug = false;
	public bool trustingAug { get { return _trustingAug; } }

	// index 6 (charge refill on crit)
	private bool _drivenAug = false;
	public bool drivenAug { get { return _drivenAug; } }
	
	// index 10 (ambient charge refill during combat)
	private bool _anxiousAug = false;
	public bool anxiousAug { get { return _anxiousAug; } }

	// index 11 (stay alive for a bit after death)
	private bool _condemnedAug = false;
	public bool condemnedAug { get { return _condemnedAug; } }

	// index 13 (extra dmg everywhere)
	private bool _hatedAug = false;
	public bool hatedAug { get { return _hatedAug; } }

	// index 17 (witch time)
	private bool _untetheredAug = false;
	public bool untetheredAug { get { return _untetheredAug; } }

	// index 18 (teleport)
	private bool _disconnectedAug = false;
	public bool disconnectedAug { get { return _disconnectedAug; } }

	// index 12 (bloodborne rally)
	private bool _desperateAug = false;
	public bool desperateAug { get { return _desperateAug; } }

	// index 14 (extra defense)
	private bool _lovedAug = false;
	public bool lovedAug { get { return _lovedAug; } }

	// index 16 (slower enemies)
	private bool _cautiousAug = false;
	public bool cautiousAug { get { return _cautiousAug; } }

	// index 19 (super dodge-y)
	private bool _incensedAug = false;
	public bool incensedAug { get { return _incensedAug; } }
	private float _incensedStaminaMult = 0.4f;
	public float incensedStaminaMult { get { return _incensedStaminaMult; } }
	private float _incensedPowerMult = 1.5f;
	public float incensedPowerMult { get { return _incensedPowerMult; } }

	// index 3 (extra dmg per combo chain)
	private bool _paranoidAug = false;
	public bool paranoidAug { get { return _paranoidAug; } }
	private float _startParanoidMult = 0.875f;
	private float _currentParanoidMult = 0.875f;
	private float _addParanoidMult = 0.125f;

	// index 21 (decrease stats, increase VP)
	private bool _scornedAug = false;
	public bool scornedAug { get { return _scornedAug; } }

	[Header("Instance Properties")]
	public GameObject enragedShadow;

	private bool _initialized;

	// Update is called once per frame
	void Update () {

		if (_initialized){
			if (_enragedActive){
				enragedCountdown -= Time.deltaTime;
				if (enragedCountdown <= 0){
					enragedShadow.SetActive(false);
					_enragedActive = false;
				}
			}
		}
	
	}

	private void Initialize(){

		if (!_initialized){
			_initialized = true;
			enragedShadow.SetActive(false);
		}
		RefreshAll();

	}

	public void SetPlayerRef(PlayerController newRef){
		_playerReference = newRef;
		Initialize();
	}

	private void TurnOffAll(){

		// turn off weapon augs
		_lunaAug = false;
		_animaAug = false;
		_solAug = false;
		_pyraAug = false;
		_aeroAug = false;
		_thanaAug = false;
		_realGaeaAug = false;
		_erebosAug = false;
		_biosAug = false;
		_aquaAug = false;
		_fosAug = false;
		_aetherAug = false;

		_doubleMantra = false;

		// turn off all virtue augs
		_opportunisticAug = false;
		_unstoppableAug = false;
		_dashAug = false;
		_enragedAug = false;
		_determinedAug = false;
		_empoweredAug = false;
		_adaptiveAug = false;
		_perceptiveAug = false;
		_agileAug = false;
		_repellantAug = false;
		_trustingAug = false;
		_drivenAug = false;
		_anxiousAug = false;
		_untetheredAug = false;
		_desperateAug = false;
		_lovedAug = false;
		_hatedAug = false;
		_paranoidAug = false;
		_incensedAug = false;
		_cautiousAug = false;
		_disconnectedAug = false;


	}

	public void RefreshAll(){
		TurnOffAll();

		// turn on weapon augs
		if (_playerReference.EquippedWeaponAug() != null){
			TurnOnWeaponAugs();
		}

		// turn on virtues
		if (PlayerController.equippedVirtues.Count > 0 && !_playerReference.myStats.arcadeMode){
			TurnOnVirtueAugs();
		}
	}

	private void TurnOnWeaponAugs(){
		if (_playerReference.EquippedWeaponAug().weaponNum == 0 ||
			_playerReference.EquippedWeapon().weaponNum == 0){
			_lunaAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 1 ||
			_playerReference.EquippedWeapon().weaponNum == 1){
			_thanaAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 2 ||
			_playerReference.EquippedWeapon().weaponNum == 2){
			_aeroAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 3 ||
			_playerReference.EquippedWeapon().weaponNum == 3){
			_pyraAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 4 ||
			_playerReference.EquippedWeapon().weaponNum == 4){
			_animaAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 5 ||
			_playerReference.EquippedWeapon().weaponNum == 5){
			_solAug = true;
		}
		if (_playerReference.EquippedWeaponAug().weaponNum == 6 ||
			_playerReference.EquippedWeapon().weaponNum == 6){
			_realGaeaAug = true;
		}
		if (_playerReference.EquippedWeaponAug().weaponNum == 7 ||
			_playerReference.EquippedWeapon().weaponNum == 7){
			_erebosAug = true;
		}
		if (_playerReference.EquippedWeaponAug().weaponNum == 8 ||
			_playerReference.EquippedWeapon().weaponNum == 8){
			_biosAug = true;
		}

		if (_playerReference.EquippedWeaponAug().weaponNum == 9 ||
			_playerReference.EquippedWeapon().weaponNum == 9){
			_fosAug = true;
		}

		if (_playerReference.EquippedWeaponAug().weaponNum == 10 ||
			_playerReference.EquippedWeapon().weaponNum == 10){
			_aquaAug = true;
		}
		if (_playerReference.EquippedWeaponAug().weaponNum == 11 ||
			_playerReference.EquippedWeapon().weaponNum == 11){
			_aetherAug = true;
		}

		if (_playerReference.EquippedWeapon().weaponNum == _playerReference.EquippedWeaponAug().weaponNum){
			_doubleMantra = true;
		}
	}

	private void TurnOnVirtueAugs(){

        // as always, remove // before if statements when done testing
        if (_playerReference.isNatalie)
        {
            _unstoppableAug = true;
        }
        else
        {
            if (PlayerController.equippedVirtues.Contains(0))
            {
                _unstoppableAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(1))
            {
                _determinedAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(2))
            {
                _empoweredAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(20))
            {
                _enragedAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(4))
            {
                _adaptiveAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(5))
            {
                _perceptiveAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(9))
            {
                _agileAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(7))
            {
                _repellantAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(8))
            {
                _trustingAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(6))
            {
                _drivenAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(10))
            {
                _anxiousAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(11))
            {
                _condemnedAug = true;
            }

            if (PlayerController.equippedVirtues.Contains(13))
            {
                _hatedAug = true;
            }

            if (PlayerController.equippedVirtues.Contains(17))
            {
                _untetheredAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(18))
            {
                _disconnectedAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(14))
            {
                _lovedAug = true;
            }

            if (PlayerController.equippedVirtues.Contains(12))
            {
                _desperateAug = true;
            }

            if (PlayerController.equippedVirtues.Contains(3))
            {
                _paranoidAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(19))
            {
                _incensedAug = true;
            }
            if (PlayerController.equippedVirtues.Contains(16))
            {
                _cautiousAug = true;
            }
        }

	}

	public void EnragedTrigger(){
		if (_enragedAug){
			_enragedActive = true;
			enragedCountdown = enragedTimeMax;
			enragedShadow.SetActive(true);
		}
	}

	public float GetGaeaAug(){
		if (_realGaeaAug){
			if (_doubleMantra){
				return realGaeaAugAmt*1.3f;
			}else{
				return realGaeaAugAmt;
			}
		}else{
			return 1f;
		}
	}
	public float GetErebosAug(){
		if (_erebosAug){
			if (_doubleMantra){
				return erebosAugAmt * 1.25f;
			}else{
				return erebosAugAmt;
			}
		}else{
			return 1f;
		}
	}

	public bool HasWitchAug(){
		/*
		if (_untetheredAug || _agileAug){
			return true;
		}else{
			return false;
		}**/
		return true;
	}

	public float GetEnragedMult(){
		//return (Mathf.Lerp(ENRAGED_DAMAGE_BOOST, 1f,_playerReference.myStats.currentHealth/_playerReference.myStats.maxHealth));
		if (_enragedActive){
		return ENRAGED_DAMAGE_BOOST;
		}else{
			return 1f;
		}
	}

	public void AddToParanoidMult(){
		_currentParanoidMult += _addParanoidMult;
	}
	public void ResetParanoidMult(){
		_currentParanoidMult = _startParanoidMult;
	}
	public float GetParanoidMult(){
		if (_paranoidAug){
			return _currentParanoidMult;
		}else{
			return 1f;
		}
	}

	public float AnimaAugAmt(){
		if (_animaAug){
			if (_doubleMantra){
				return 0.92f;
			}else{
				return 0.95f;
			}
		}else{
			return 1f;
		}
	}

	public int SolAugAmt(){
		if (_solAug){
			if (_doubleMantra){
				return 2;
			}else{
				return 1;
			}
		}else{
			return 0;
		}
	}

	public int AquaAugAmt()
	{
		if (_aquaAug){
			if (_doubleMantra){
				return 2;
			}else{
				return 1;
			}
		}else{
			return 0;
		}
	}
}
