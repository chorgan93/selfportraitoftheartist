using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipInventoryItemS : MonoBehaviour {
	
	private PlayerInventoryS inventoryRef;

	public bool equippedItem = false;
	public Image itemImage;
	public Text itemCount;
	public int itemIndex = 0;
	public int itemNum = -1;

	private bool _unlocked = false;
	public bool unlocked { get { return _unlocked; } }

	public void Initialize(PlayerInventoryS i){

		inventoryRef = i;

		bool turnOn = false;
		if (!equippedItem){
			if (itemIndex < i.collectedItems.Count){
				turnOn = true;
			}
	
			if (!turnOn){
				itemImage.enabled = false;
				itemCount.text = "";
				_unlocked = false;
			}else{
				itemNum = i.collectedItems[itemIndex];
				itemImage.sprite = i.iManager.itemSprites[itemNum];
				itemCount.text = i.collectedItemCount[itemIndex].ToString();
				itemImage.enabled = true;
	
				_unlocked = true;
			}
		}
		else{
			if (i.iManager.equippedInventory[itemIndex] != -1){
				turnOn = true;
			}
			if (!turnOn){
				itemImage.enabled = false;
				itemCount.text = "";
				_unlocked = false;
			}else{
				itemNum = i.iManager.equippedInventory[itemIndex];
				itemImage.sprite = i.iManager.itemSprites[itemNum];
				itemCount.text = i.collectedItemCount[itemIndex].ToString();
				itemImage.enabled = true;
				
				_unlocked = true;
			}
		}

	}

	public void Show(){
		itemImage.enabled = true;
		itemCount.enabled = false;
	}

	public void Hide(){
		itemImage.enabled = false;
		itemCount.enabled = false;
	}

	public string GetItemDescription(){
		string itemDesc = "";
		switch(itemNum){
		default:
			itemDesc = "";
			break;
		case(0):
			itemDesc = "HEALTH ESSENCE (Restores Health. Replenishes at Checkpoints)";
			break;
		case(1):
			itemDesc = "ENERGY ESSENCE (Restores Stamina. Replenishes at Checkpoints)";
			break;
		case(2):
			itemDesc = "CHARGE ESSENCE (Restores Charge. Replenishes at Checkpoints)";
			break;
		case(3):
			itemDesc = "MANA ESSENCE (Unleases hidden Power. Replenishes at Checkpoints)";
			break;
		case(4):
			itemDesc = "CRESCENT KEY (Key found in classroom)";
			break;
		case(5):
			itemDesc = "MOTHER'S SWORD (Rusted sword found in basement. I couldn't use this as a weapon)";
			break;
		}
		return itemDesc;
	}

}
