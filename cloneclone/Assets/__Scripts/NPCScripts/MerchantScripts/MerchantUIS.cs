using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MerchantUIS : MonoBehaviour {

	private MerchantS merchantRef;

	public RectTransform selector;
	public RectTransform shopSelector;
	public GameObject selectMenu;
	public Text[] menuOptions;
	public RectTransform[] menuOptionPositions;
	public GameObject buyMenu;
	public List<MerchantUIItemS> merchantItems;
	public List<RectTransform> merchantItemPositions;

	private int currentPos;
	private bool inShopMenu = false;

	public void TurnOn(MerchantS newMerchant){
		merchantRef = newMerchant;
		currentPos = 0;
		inShopMenu = false;
		SetSelector();
		buyMenu.SetActive(false);
		selectMenu.SetActive(true);

	}

	public void MoveSelector(int dir){

		Debug.Log("Moved selector!");
		if (dir > 0){
			if (inShopMenu){
				if (currentPos < merchantRef.itemsForSale.Length-1){
					currentPos++;
				}else{
					currentPos = 0;
				}
			}else{
				// options outside of buy menu: shop, talk, exit (0,1,2)
				if (currentPos < 2){
					currentPos++;
				}else{
					currentPos = 0;
				}
			}
		}else{
			if (currentPos > 0){
				currentPos--;
			}else if (!inShopMenu){
				currentPos = 2;
			}
		}
		SetSelector();
	}

	void SetSelector(){
		if (!inShopMenu){
			selector.anchoredPosition = menuOptionPositions[currentPos].anchoredPosition;
		}else{
			shopSelector.parent = merchantItemPositions[currentPos].parent;
			shopSelector.anchoredPosition = merchantItemPositions[currentPos].anchoredPosition;
			DialogueManagerS.D.SetDisplayText(GetCurrentDescription(), false, false, true);
			DialogueManagerS.D.CompleteText();
		}
	}

	public void SelectOption(){
		if (inShopMenu){
			merchantRef.AttemptBuy(currentPos);
		}else{
			if (currentPos == 0){
				OpenShopMenu();
			}else if (currentPos == 1){
				merchantRef.StartTalk();
				HideMenus();
			}else{
				merchantRef.StartExit();
				TurnOff();
			}
		}
	}

	public void ExitOption(){
		if (!inShopMenu){
			TurnOff();
			merchantRef.StartExit();
		}else{
			CloseShopMenu();
		}
	}

	void OpenShopMenu(){
		SetItems();
		//selectMenu.SetActive(false);
		currentPos = 0;
		buyMenu.SetActive(true);
		inShopMenu = true;
		SetSelector();
	}

	public void ShowMenus(){
		selectMenu.SetActive(true);
	}

	void HideMenus(){

		selectMenu.SetActive(false);
		buyMenu.SetActive(false);
	}

	void CloseShopMenu(){
		inShopMenu = false;
		currentPos = 0;
		selectMenu.SetActive(true);
		buyMenu.SetActive(false);
		SetSelector();
		merchantRef.ResetMessage();
	}

	public void TurnOff(){

		buyMenu.SetActive(false);
		selectMenu.SetActive(false);
		currentPos = 0;
		inShopMenu = false;
	}

	public void DisplayItems(){

		SetItems();
	}

	void SetItems(){
		for (int i = 0; i < merchantItems.Count; i++){
			if (i < merchantRef.itemsForSale.Length){
				merchantItems[i].SetItem(merchantRef.itemsForSale[i]);
			}else{
				merchantItems[i].TurnOffItem();
			}
		}
	}



	public string GetCurrentDescription(){
		return merchantRef.itemsForSale[currentPos].itemDescription;
	}
}
