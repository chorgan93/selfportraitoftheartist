using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CombatGiverUIS : MonoBehaviour {

	private CombatGiverS giverRef;

	public RectTransform selector;
	public RectTransform shopSelector;
	public GameObject selectMenu;
	public Text[] menuOptions;
	public RectTransform[] menuOptionPositions;
	public GameObject buyMenu;
	public List<RectTransform> merchantItemPositions;

	public Text rulesText;
	public Image rulesBG;
	public bool showCurrentRank = false;

	[Header("Main Arena Properties")]
	public int[] arenaCombatIDs;
	private int avgCombatRanking = -1;
	public int playerRank { get { return avgCombatRanking; } }

	private int currentPos;
	private bool inShopMenu = false;

    public GameObject giveAchievementObj;

    private void Start()
    {
        if (giveAchievementObj != null){
            int rankCheck = 0;
            string rankSave = "";
            if (PlayerInventoryS.I.dManager.combatClearedAtLeastOnce != null)
            {
                for (int i = 0; i < arenaCombatIDs.Length; i++)
                {
                    if (PlayerInventoryS.I.dManager.combatClearedAtLeastOnce.Contains(arenaCombatIDs[i]))
                    {
                        if (PlayerInventoryS.I.dManager.combatClearedRankGrades.Count >=
                            PlayerInventoryS.I.dManager.combatClearedAtLeastOnce.IndexOf(arenaCombatIDs[i]))
                        {
                            rankSave = PlayerInventoryS.I.dManager.combatClearedRankGrades[
                                PlayerInventoryS.I.dManager.combatClearedAtLeastOnce.IndexOf(arenaCombatIDs[i])];
                            if (rankSave == "S")
                            {
                                rankCheck += 4;
                            }
                            else if (rankSave == "A")
                            {
                                rankCheck += 3;
                            }
                            else if (rankSave == "B")
                            {
                                rankCheck += 2;
                            }
                            else if (rankSave == "C")
                            {
                                rankCheck += 1;
                            }
                            else
                            {
                                rankCheck += 0;
                            }
                        }
                    }
                }
            }
            if (rankCheck >= 16){
                giveAchievementObj.SetActive(true);
            }
        }
    }

    public void TurnOn(CombatGiverS newMerchant){
		giverRef = newMerchant;
		currentPos = 0;
		inShopMenu = false;
		SetSelector();
		buyMenu.SetActive(false);
		selectMenu.SetActive(true);
		rulesText.text = "";
		rulesBG.enabled = false;

	}

	public void SetRulesText(string newText){
		if (!showCurrentRank){
		if (newText == ""){

			rulesText.text = "";
			rulesBG.enabled = false;
		}else{

			rulesText.text = newText;
				rulesText.enabled = true;
			rulesBG.enabled = true;
		}
		}else{
			if (newText == ""){

				rulesText.text = "";
				rulesBG.enabled = false;
			}else{
				if (avgCombatRanking < 0){
			avgCombatRanking = 0;
			string rankSave = "";
			if (PlayerInventoryS.I.dManager.combatClearedAtLeastOnce != null){
				for (int i = 0; i < arenaCombatIDs.Length; i++){
					if (PlayerInventoryS.I.dManager.combatClearedAtLeastOnce.Contains(arenaCombatIDs[i])){
						if (PlayerInventoryS.I.dManager.combatClearedRankGrades.Count >= 
							PlayerInventoryS.I.dManager.combatClearedAtLeastOnce.IndexOf(arenaCombatIDs[i])){
							rankSave = PlayerInventoryS.I.dManager.combatClearedRankGrades[
								PlayerInventoryS.I.dManager.combatClearedAtLeastOnce.IndexOf(arenaCombatIDs[i])];
							if (rankSave == "S"){
								avgCombatRanking += 4;
							}else if (rankSave == "A"){
								avgCombatRanking += 3;
							}else if (rankSave == "B"){
								avgCombatRanking += 2;
							}else if (rankSave == "C"){
								avgCombatRanking += 1;
							}else{
								avgCombatRanking += 0;
							}
						}
					}
				}
			}
			avgCombatRanking = Mathf.RoundToInt(avgCombatRanking/arenaCombatIDs.Length);
				}
				rulesText.text = "Current Rank: ";
				switch (avgCombatRanking){
				default:
					rulesText.text += " [C]";
					break;
				case (2):
					rulesText.text += " [B]";
					break;
				case (3):
					rulesText.text += " [A]";
					break;
				case (4):
					rulesText.text += " [S]";
					break;
				}
				if (newText == "A" && avgCombatRanking < 3){
					rulesText.text += " : Required Rank: <color=red>[" + newText + "]</color>";
				}else{
					rulesText.text += " : Required Rank: [" + newText + "]";
				}
			rulesText.enabled = true;
			rulesBG.enabled = true;
			}
		}
	}

	public void MoveSelector(int dir){

		if (dir > 0){
			if (inShopMenu){
				if (currentPos < giverRef.possChoices.Length-1){
					currentPos++;
				}else{
					currentPos = 0;
				}
			}else{
				// options outside of buy menu: fight, talk, exit (0,1,2)
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
			//DialogueManagerS.D.SetDisplayText(GetCurrentDescription(), false, false, true);
			//DialogueManagerS.D.CompleteText();
			SetRulesText(giverRef.possChoices[currentPos].rulesString);
		}
	}

	public void SelectOption(){
		if (inShopMenu){
			giverRef.SelectOption(currentPos);
		}else{
			if (currentPos == 0){
				OpenShopMenu();
			}else if (currentPos == 1){
				giverRef.StartTalk();
				HideMenus();
			}else{
				giverRef.StartExit();
				TurnOff();
			}
		}
	}

	public void ExitOption(){
		if (!inShopMenu){
			TurnOff();
			giverRef.StartExit();
		}else{
			CloseShopMenu();
		}
	}

	void OpenShopMenu(){
		if (giverRef.possChoices[0].mainArenaCombat){
			SetRulesText("C");
		}else{
			SetRulesText("");
		}
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
		SetRulesText("");
		inShopMenu = false;
		currentPos = 0;
		selectMenu.SetActive(true);
		buyMenu.SetActive(false);
		SetSelector();
		giverRef.ResetMessage();
	}

	public void TurnOff(){

		SetRulesText("");
		buyMenu.SetActive(false);
		selectMenu.SetActive(false);
		currentPos = 0;
		inShopMenu = false;
	}

	public void DisplayItems(){

		SetItems();
	}

	void SetItems(){
		for (int i = 0; i < giverRef.possChoices.Length; i++){
			if (i < giverRef.possChoices.Length){
				giverRef.possChoices[i].SetItem(this);
			}else{
				giverRef.possChoices[i].TurnOffItem();
			}
		}
	}
		
}
