using UnityEngine;
using System.Collections;

public class PlayerAugmentsS : MonoBehaviour {

	// script handles all upgrades from sub-weapons and virtues
	private PlayerController _playerReference;

	// weapon augmentations
	private bool _lunaAug = false;
	public bool lunaAug { get { return _lunaAug; } }
	public const float lunaAugAmt = 1.5f;

	private bool _animaAug = false;
	public bool animaAug { get { return _animaAug; } }
	public const float animaAugAmt = 0.9f;

	private bool _solAug = false;
	public bool solAug { get { return _solAug; } }
	public const float solAugAmt = 1.5f;

	private bool _thanaAug = false;
	public bool thanaAug { get { return _thanaAug; } }
	public const float thanaAugAmt = 1.4f;

	private bool _gaeaAug = false;
	public bool gaeaAug { get { return _gaeaAug; } }
	public const float gaeaAugAmt = 0.75f;

	private bool _aeroAug = false;
	public bool aeroAug { get { return _aeroAug; } }

	// virtue augmentations
	private bool _secondChanceAug = false;
	public bool secondChanceAug { get { return _secondChanceAug; } }
	
	private bool _opportunisticAug = false;
	public bool opportunisticAug { get { return _opportunisticAug; } }
	
	private bool _dashAug = false;
	public bool dashAug { get { return _dashAug; } }
	
	private bool _enragedAug = false;
	public bool enragedAug { get { return _enragedAug; } }

	private bool _initialized;

	// Update is called once per frame
	void Update () {

		if (_initialized){

		}
	
	}

	private void Initialize(){

		if (!_initialized){
			_initialized = true;
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
		_gaeaAug = false;
		_aeroAug = false;
		_thanaAug = false;

		// turn off all virtue augs
		_opportunisticAug = false;
		_secondChanceAug = false;
		_dashAug = false;
		_enragedAug = false;


	}

	public void RefreshAll(){
		TurnOffAll();

		// turn on weapon augs
		if (_playerReference.EquippedWeaponAug() != null){
			TurnOnWeaponAugs();
		}

		// turn on virtues
		if (_playerReference.equippedVirtues.Count > 0){
			TurnOnVirtueAugs();
		}
	}

	private void TurnOnWeaponAugs(){
		if (_playerReference.EquippedWeaponAug().weaponNum == 0){
			_lunaAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 1){
			_thanaAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 2){
			_aeroAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 3){
			_gaeaAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 4){
			_animaAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 5){
			_solAug = true;
		}
	}

	private void TurnOnVirtueAugs(){

		if (_playerReference.equippedVirtues.Contains(0)){
			_secondChanceAug = true;
		}
		if (_playerReference.equippedVirtues.Contains(1)){
			_opportunisticAug = true;
		}
		if (_playerReference.equippedVirtues.Contains(2)){
			_dashAug = true;
		}
		if (_playerReference.equippedVirtues.Contains(3)){
			_enragedAug = true;
		}

	}
}
