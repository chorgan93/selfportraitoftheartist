using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelUpItemS : MonoBehaviour {

	private LevelUpS upgradeRef;

	public Image upgradeImage;
	public Text upgradeNameText;
	public Text upgradeDescriptionText;
	private int upgradeNum;
	private int upgradeCost;

	private string upgradeName;
	private string upgradeDescription;


	public void Initialize(LevelUpS l){

		upgradeRef = l;

		upgradeImage.sprite = upgradeRef.upgradeImg;
		upgradeDescription = upgradeRef.upgradeDescription;
		upgradeName = upgradeRef.upgradeName;
		upgradeCost = upgradeRef.upgradeCost;

		upgradeNameText.text = upgradeName + " (" + upgradeCost + + "/" + PlayerCollectionS.currencyCollected + ")";
		upgradeDescriptionText.text = upgradeDescription;
	

	}

	public bool CanBeUpgraded(){
		bool canBuy = false;
		if (PlayerCollectionS.currencyCollected >= upgradeCost){
			canBuy = true;
		}
		return canBuy;
	}

	public void BuyUpgrade(){
		PlayerCollectionS.currencyCollected -= upgradeCost;
	}
}
