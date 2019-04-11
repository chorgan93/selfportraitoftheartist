using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CombatGiverUIItemS : MonoBehaviour {

	public Text nameText;
	public Text completeText;
	public int combatID = -1;
	public bool mainArenaCombat = false;
	public string rulesString = "";
	public string completeString = "[COMPLETE]";
	public Color incompleteColor = Color.white;
	public Color completeColor = Color.cyan;
	public Color cannotSelectColor = Color.gray;
	public int minCombatRank = -1;

	private CombatGiverUIS uiRef;

	public void SetItem(CombatGiverUIS newUI){
		uiRef = newUI;
		if (!mainArenaCombat){
		if (combatID > -1 && PlayerInventoryS.I.dManager.specialConditionCombatCleared != null){
			if (PlayerInventoryS.I.dManager.specialConditionCombatCleared.Contains(combatID)){
				completeText.text = LocalizationManager.instance.GetLocalizedValue(completeString);
				nameText.color = completeText.color = completeColor;
			}else{
				completeText.text = "";
				nameText.color = completeText.color = incompleteColor;
			}

		}
		}else{
			if (!CanSelect()){
				nameText.color = completeText.color = cannotSelectColor;
			}
			else if (combatID > -1 && PlayerInventoryS.I.dManager.combatClearedAtLeastOnce != null){
				if (PlayerInventoryS.I.dManager.combatClearedAtLeastOnce.Contains(combatID)){
					completeText.text = "[" +
						PlayerInventoryS.I.dManager.combatClearedRankGrades[PlayerInventoryS.I.dManager.combatClearedAtLeastOnce.IndexOf(combatID)]
						+ "]";
					nameText.color = completeText.color = completeColor;
				}else{
					completeText.text = "";
					nameText.color = completeText.color = incompleteColor;
				}

			}
		}
		gameObject.SetActive(true);
	}

	public bool CanSelect(){
		bool canSelect= true;
		if (mainArenaCombat && minCombatRank > uiRef.playerRank){
			canSelect = false;
		}
		return canSelect;
	}

	public void TurnOffItem(){
		gameObject.SetActive(false);
	}
}
