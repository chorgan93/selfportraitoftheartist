using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipTechItemS : MonoBehaviour {
	
	private PlayerInventoryS inventoryRef;

	public int techNum;
	public Text techText;
	public Image techBG;

	public Color textLockedColor;
	public Color textOnColor;
	public Color textOffColor;

	public string techDescription;
	private bool _unlocked = false;
	public bool unlocked { get { return _unlocked; } }

	private bool _techEquipped = false;

	public void Initialize(PlayerInventoryS i){

		inventoryRef = i;

		bool turnOn = false;
		foreach (int w in i.earnedTech){
			if (w == techNum){
				turnOn = true;
			}
		}

		if (!turnOn){
			_unlocked = false;
			techBG.enabled = true;
			techText.color = textLockedColor;
			techText.text = "— LOCKED —";
		}else{
			techBG.enabled = true;
			if (PlayerController.equippedUpgrades.Contains(techNum)){
				techText.color = textOnColor;
			}else{
				techText.color = textOffColor;
			}
			techText.text = techDescription;

			_unlocked = true;
		}

	}

	public void ToggleOnOff(){
		_techEquipped = !_techEquipped;
		if (_techEquipped){
			if (!PlayerController.equippedUpgrades.Contains(techNum)){
				PlayerController.equippedUpgrades.Add(techNum);
				techText.color = textOnColor;
			}
		}else{
			if (PlayerController.equippedUpgrades.Contains(techNum)){
				PlayerController.equippedUpgrades.Remove(techNum);
				techText.color = textOffColor;
			}
		}
	}

}
