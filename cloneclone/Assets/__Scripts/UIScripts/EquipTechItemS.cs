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
	public bool techEquipped {get { return _techEquipped; } }

	public PlayerStatDisplayS statUIRef;
	public EnemyHealthUIS bossUIRef;
	public PlayerCurrencyDisplayS currencyUIRef;
	public ResetUIS resetUIRef;
	public VerseDisplayS verseUIRef;
    public KeyItemUIS keyItemUIRef;

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
			if (PlayerController.equippedTech.Contains(techNum)){
				techText.color = textOnColor;
				_techEquipped = true;
			}else{
				techText.color = textOffColor;
				_techEquipped = false;
			}
			techText.text = techName;

			_unlocked = true;
		}

	}

	public void ToggleOnOff(){
		_techEquipped = !_techEquipped;
		if (_techEquipped){
			if (!PlayerController.equippedTech.Contains(techNum)){
				PlayerController.equippedTech.Add(techNum);
				techText.color = textOnColor;

				if (techNum == 0){
					statUIRef.EnableUI();
					currencyUIRef.Show();
				}
				if (techNum == 1){
					bossUIRef.Show();
				}
				if (techNum == 2){
                    resetUIRef.Show();
                    keyItemUIRef.Show();
				}
				if (techNum == 3){
					verseUIRef.Show();
				}
			}
		}else{
			if (PlayerController.equippedTech.Contains(techNum)){
				PlayerController.equippedTech.Remove(techNum);
				techText.color = textOffColor;
				if (techNum == 0){
					statUIRef.DisableUI();
					currencyUIRef.Hide();
				}
				if (techNum == 1){
					bossUIRef.Hide();
				}
				if (techNum == 2){
                    resetUIRef.Hide();
                    keyItemUIRef.Hide();
				}
				if (techNum == 3){
					verseUIRef.Hide();
				}
			}
		}
	}

}
