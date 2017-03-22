using UnityEngine;
using System.Collections;

public class PlayerAugmentsS : MonoBehaviour {

	// script handles all upgrades from sub-weapons and virtues
	private PlayerController _playerReference;

	//________________________________________________________weapon augmentations
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

	//____________________________________________________________virtue augmentations
	// index 0
	private bool _unstoppableAug = false;
	public bool unstoppableAug { get { return _unstoppableAug; } }

	// NOT USED
	private bool _opportunisticAug = false; // VIRTUE NOT USED
	public bool opportunisticAug { get { return _opportunisticAug; } }

	// NOT USED
	private bool _dashAug = false; // VIRTUE NOT USED
	public bool dashAug { get { return _dashAug; } }

	// index 1
	private bool _determinedAug = false;
	public bool determinedAug { get { return _determinedAug; } }

	// index 2
	private bool _empoweredAug = false;
	public bool empowered { get { return _empoweredAug; } }

	// index 3
	private bool _enragedAug = false;
	public bool enragedAug { get { return _enragedAug; } }

	// index 4
	private bool _adaptiveAug = false;
	public bool adaptiveAug { get { return _adaptiveAug; } }

	// index 5
	private bool _perceptiveAug = false;
	public bool perceptiveAug { get { return _perceptiveAug; } }
	
	// index 6
	private bool _agileAug = false;
	public bool agileAug { get { return _agileAug; } }

	// index 7
	private bool _repellantAug = false;
	public bool repellantAug { get { return _repellantAug; } }
	
	// index 8
	private bool _trustingAug = false;
	public bool trustingAug { get { return _trustingAug; } }

	// index 9
	private bool _drivenAug = false;
	public bool drivenAug { get { return _drivenAug; } }
	
	// index 10
	private bool _anxiousAug = false;
	public bool anxiousAug { get { return _anxiousAug; } }

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


	}

	public void RefreshAll(){
		TurnOffAll();

		// turn on weapon augs
		if (_playerReference.EquippedWeaponAug() != null){
			TurnOnWeaponAugs();
		}

		// turn on virtues
		if (PlayerController.equippedVirtues.Count > 0){
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

		if (PlayerController.equippedVirtues.Contains(0)){
			_unstoppableAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(1)){
			//_opportunisticAug = true;
			_determinedAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(2)){
			//_dashAug = true;
			_empoweredAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(3)){
			_enragedAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(4)){
			_adaptiveAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(5)){
			_perceptiveAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(6)){
			_agileAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(7)){
			_repellantAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(8)){
			_trustingAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(9)){
			_drivenAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(10)){
			_anxiousAug = true;
		}

	}
}
