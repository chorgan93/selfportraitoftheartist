using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipVirtueItemS : MonoBehaviour {
	
	private PlayerInventoryS inventoryRef;

	public Image virtueImage;
	public Image virtueEquip;
	public int virtueNum;
	public int virtueCost = 2;

	public string virtueDescription;
	public Sprite virtueEquippedSprite;
	public Sprite virtueUnequippedSprite;
	private bool _unlocked = false;
	public bool unlocked { get { return _unlocked; } }

    private const int scornedIndex = 21;
    public const int scornedAddVp = 20;

	public void Initialize(PlayerInventoryS i, PlayerController pRef, bool fromScorned = false){

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
                Unequip(fromScorned);
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
		virtueImage.sprite = virtueEquippedSprite;

        if (virtueNum == scornedIndex && GameObject.Find("Player") != null){
            GameObject.Find("Player").GetComponent<PlayerController>().SetScorned(true);
        }
	}

	public void Unequip(bool fromScorned = false){
		virtueEquip.enabled = false;
		virtueImage.sprite = virtueUnequippedSprite;
        if (virtueNum == scornedIndex && GameObject.Find("Player") != null && !fromScorned)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().SetScorned(false);
        }
	}


}
