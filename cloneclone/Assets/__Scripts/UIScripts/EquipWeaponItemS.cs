using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipWeaponItemS : MonoBehaviour {
	
	private PlayerInventoryS inventoryRef;
	private PlayerWeaponS weaponRef;

	public Image weaponImage;
	public int weaponNum;

	public string weaponDescriptionMain;
	public string weaponDescriptionSub;
	private bool _unlocked = false;
	public bool unlocked { get { return _unlocked; } }

	public void Initialize(PlayerInventoryS i){

		inventoryRef = i;

		bool turnOn = false;
		foreach (PlayerWeaponS w in i.unlockedWeapons){
			if (w.weaponNum == weaponNum){
				turnOn = true;
				weaponRef = w;
			}
		}

		if (!turnOn){
			weaponImage.enabled = false;
			//weaponName.enabled = false;
			_unlocked = false;
		}else{
			weaponImage.sprite = weaponRef.swapSprite;
			weaponImage.color = weaponRef.swapColor;
			weaponImage.enabled = true;

			//weaponName.color = weaponRef.swapColor;
			//weaponName.enabled = true;
			_unlocked = true;
		}

	}

	public PlayerWeaponS WeaponRefForSwitch(){
		return weaponRef;
	}
}
