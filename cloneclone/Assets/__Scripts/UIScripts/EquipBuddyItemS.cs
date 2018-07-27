using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipBuddyItemS : MonoBehaviour {
	
	private PlayerInventoryS inventoryRef;
	private BuddyS buddyRef;

	public GameObject buddyInstance;
	public Image buddyImage;
	public int buddyNum;

	public string buddyDescription;
	private bool _unlocked = false;
	public bool unlocked { get { return _unlocked; } }

	public void Initialize(PlayerInventoryS i){

		inventoryRef = i;

		bool turnOn = false;
		foreach (BuddyS w in i.unlockedBuddies){
			if (w.buddyNum == buddyNum){
				turnOn = true;
				buddyRef = w;
			}
		}

		if (!turnOn){
			buddyImage.enabled = false;
			//weaponName.enabled = false;
			_unlocked = false;
		}else{
			buddyImage.sprite = buddyRef.buddyMenuSprite;
            if (PlayerController.killedFamiliar)
            {
                buddyImage.color = Color.black;
            }
            else
            {
                buddyImage.color = buddyRef.shadowColor;
            }
			buddyImage.enabled = true;

			//weaponName.color = weaponRef.swapColor;
			//weaponName.enabled = true;
			_unlocked = true;
		}

	}

}
