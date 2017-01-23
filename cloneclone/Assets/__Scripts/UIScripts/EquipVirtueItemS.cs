using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipVirtueItemS : MonoBehaviour {
	
	private PlayerInventoryS inventoryRef;

	public Image virtueImage;
	public Image virtueEquip;
	public int virtueNum;
	public float virtueCost = 2f;

	public string virtueDescription;
	private bool _unlocked = false;
	public bool unlocked { get { return _unlocked; } }

	public void Initialize(PlayerInventoryS i, PlayerController pRef){

		inventoryRef = i;

		bool turnOn = false;
		foreach (int v in i.earnedVirtues){
			if (v == virtueNum){
				turnOn = true;
			}
		}

		if (!turnOn){
			virtueImage.enabled = false;
			virtueEquip.enabled = false;
			//weaponName.enabled = false;
			_unlocked = false;
		}else{
			virtueImage.enabled = true;

			if (PlayerController.equippedVirtues.Contains(virtueNum)){
				Equip();
			}else{
				Unequip();
			}

			//weaponName.color = weaponRef.swapColor;
			//weaponName.enabled = true;
			_unlocked = true;
		}

	}

	public void Show(){
		virtueImage.enabled = true;
	}

	public void Hide(){
		virtueImage.enabled = false;
	}

	public void Equip(){
		virtueEquip.enabled = true;
	}

	public void Unequip(){
		virtueEquip.enabled = false;
	}


}
