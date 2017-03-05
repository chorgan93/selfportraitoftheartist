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

	private string techName;
	public string techDescription;
	private bool _unlocked = false;
	public bool unlocked { get { return _unlocked; } }

	private bool _techEquipped = false;

	public PlayerStatDisplayS statUIRef;
	public EnemyHealthUIS bossUIRef;
	public ResetUIS resetUIRef;
	public VerseDisplayS verseUIRef;

	public void Initialize(PlayerInventoryS i){

		inventoryRef = i;

		bool turnOn = false;
		foreach (int w in i.earnedTech){
			if (w == techNum){
				turnOn = true;
			}
		}

		techName = techText.text;

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
			techText.text = techName;

			_unlocked = true;
		}

	}

	public void ToggleOnOff(){
		_techEquipped = !_techEquipped;
		if (_techEquipped){
			if (!PlayerController.equippedUpgrades.Contains(techNum)){
				PlayerController.equippedUpgrades.Add(techNum);
				techText.color = textOnColor;

				if (techNum == 0){
					statUIRef.EnableUI();
				}
				if (techNum == 1){
					
				}
				if (techNum == 2){
					
				}
				if (techNum == 3){
					verseUIRef.Show();
				}
			}
		}else{
			if (PlayerController.equippedUpgrades.Contains(techNum)){
				PlayerController.equippedUpgrades.Remove(techNum);
				techText.color = textOffColor;
				if (techNum == 0){
					statUIRef.DisableUI();
				}
				if (techNum == 1){

				}
				if (techNum == 2){
					
				}
				if (techNum == 3){
					verseUIRef.Hide();
				}
			}
		}
	}

}
