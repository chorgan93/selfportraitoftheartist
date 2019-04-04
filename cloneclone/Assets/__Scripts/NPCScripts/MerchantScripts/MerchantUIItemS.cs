using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MerchantUIItemS : MonoBehaviour {

	public Text nameText;
	public Text costText;
	public string soldOutString = "[SOLD OUT]";
	public Color availableColor = Color.white;
	public Color unavailableColor = Color.grey;
	public Color cantAffordColor = Color.red;


	public void SetItem(MerchantItemS newItem){
        nameText.text = LocalizationManager.instance.GetLocalizedValue(newItem.itemName);
        costText.text = newItem.itemCost + " " + LocalizationManager.instance.GetLocalizedValue("game_la");
		if (newItem.isAvailable()){
			nameText.color = costText.color = availableColor;
			if (!newItem.canBeBought()){
				costText.color = cantAffordColor;
			}
		}else{
			nameText.color = costText.color = unavailableColor;
			costText.text = soldOutString;
		}
		gameObject.SetActive(true);
	}

	public void TurnOffItem(){
		gameObject.SetActive(false);
	}
}
