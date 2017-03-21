using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelUpItemS : MonoBehaviour {

	private LevelUpS upgradeRef;
	private int _upgradeID;
	public int upgradeID { get { return _upgradeID; } }

	public Image upgradeImage;
	public Text upgradeNameText;
	public Text upgradeDescriptionText;
	private int upgradeNum;
	private int upgradeCost;

	private string upgradeName;
	private string upgradeDescription;

	public PlayerLvDisplayS statDisplayRef;
	private PlayerCurrencyDisplayS cDisplay;

	private PlayerStatsS statRef;

	private Color lockedTextColor;

	void Start(){
		lockedTextColor = upgradeNameText.color;
		statRef = GameObject.Find("Player").GetComponent<PlayerStatsS>();
	}


	public void Initialize(LevelUpS l, PlayerCurrencyDisplayS cD){

		if (!statRef){
			statRef = GameObject.Find("Player").GetComponent<PlayerStatsS>();
		}
		if (!cDisplay){
			cDisplay = cD;
		}

		upgradeRef = l;

		_upgradeID = l.upgradeID;

		upgradeNum = upgradeRef.upgradeID;
		if (upgradeCost > PlayerCollectionS.currencyCollected){
			upgradeImage.sprite = upgradeRef.upgradeImgLocked;
		}else{
			upgradeImage.sprite = upgradeRef.upgradeImg;
		}
		upgradeDescription = upgradeRef.upgradeDescription;
		upgradeName = upgradeRef.upgradeName;
		upgradeCost = upgradeRef.upgradeBaseCost+upgradeRef.upgradeCostPerLv*statRef.currentLevel;



		// add cost per upgrade owned
		float numOwned = 0;
		if (_upgradeID == 0){
			numOwned = statRef.addedHealth;
		}
		if (_upgradeID == 1){
			numOwned = statRef.addedMana;
		}
		if (_upgradeID == 2){
			numOwned = statRef.addedChargeLv;
		}
		if (_upgradeID == 3){
			numOwned = statRef.addedVirtue;
		}
		if (_upgradeID == 4){
			numOwned = statRef.currentChargeRecoverLv-1f;
		}
		if (_upgradeID == 5){
			numOwned = statRef.addedRateLv*1f;
		}
		if (_upgradeID == 6){
			numOwned = statRef.addedStrength;
		}

		if (numOwned > 0){
			float newUpgradeAdd = 0f;
			newUpgradeAdd = Mathf.Pow(upgradeRef.expCostPerUpgradeOwned, numOwned);
			upgradeCost = Mathf.RoundToInt((upgradeCost+newUpgradeAdd)/10f);
			upgradeCost*=10;
		}

	}

	public void ShowText(){
		upgradeNameText.text = upgradeName + " (" + upgradeCost +  " la)";
		upgradeDescriptionText.text = upgradeDescription;
		
		if (upgradeCost > PlayerCollectionS.currencyCollected){
			SetTextColors(lockedTextColor);
			upgradeNameText.text = upgradeName + " <color=#ff0000ff>(" + upgradeCost +  " la)</color>";
		}else{
			SetTextColors(Color.white);
			upgradeNameText.text = upgradeName + " (" + upgradeCost +  " la)";
		}

		statDisplayRef.HighlightStat(upgradeNum);
	}

	private void SetTextColors(Color newCol){
		upgradeNameText.color = upgradeDescriptionText.color = newCol;
	}

	public bool CanBeUpgraded(){
		bool canBuy = false;
		if (PlayerCollectionS.currencyCollected >= upgradeCost){
			canBuy = true;
		}
		return canBuy;
	}

	public void BuyUpgrade(int index, LevelUpHandlerS lvH){
		cDisplay.AddCurrency(-upgradeCost);
		PlayerInventoryS.I.AddToUpgrades(upgradeNum);
		lvH.nextLevelUps.RemoveAt(index);
		if (upgradeRef.addUpgrades.Length > 0){
			foreach(LevelUpS u in upgradeRef.addUpgrades){
				lvH.AddAvailableUpgrade(u);
			}
		}
		lvH.NewNextLevelUps(statRef.currentLevel);
	}

	public void TurnOffVisual(){
		upgradeImage.enabled = false;
	}
	public void TurnOnVisual(){
		upgradeImage.enabled = true;
	}
}
