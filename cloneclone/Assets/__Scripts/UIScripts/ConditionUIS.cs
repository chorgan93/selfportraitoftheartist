using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConditionUIS : MonoBehaviour {

	public Image[] ConditionBorders;
	public Text[] conditionTexts;
	public string noDamageString;
	public string timeLimitString;
	public string oneComboString;

	// Use this for initialization
	void Start () {

		TurnOffAll();
	
	}


	public void TurnOffAll(){
		for (int i = 0; i < ConditionBorders.Length; i++){
			ConditionBorders[i].enabled = false;
		}
		for (int i = 0; i < conditionTexts.Length; i++){
			conditionTexts[i].enabled = false;
		}
		conditionTexts[0].text = "CONDITION:";
	}

	public void FailCondition(){
		for (int i = 0; i < ConditionBorders.Length; i++){
			ConditionBorders[i].color = Color.red;
		}
		for (int i = 0; i < conditionTexts.Length; i++){
			conditionTexts[i].color = Color.red;
		}
		conditionTexts[0].text = "CONDITION [FAILED]";
	}

	public void TurnOnAll(CombatManagerS.CombatSpecialCondition conditionKind){
		for (int i = 0; i < ConditionBorders.Length; i++){
			ConditionBorders[i].color = Color.white;
			ConditionBorders[i].enabled = true;
		}
		for (int i = 0; i < conditionTexts.Length; i++){
			ConditionBorders[i].color = Color.white;
			conditionTexts[i].enabled = true;
			if (i == 1){
				if (conditionKind == CombatManagerS.CombatSpecialCondition.NoDamage){
					conditionTexts[1].text = noDamageString;
				}
				if (conditionKind == CombatManagerS.CombatSpecialCondition.TimeLimit){
					ReplaceTimeString(RankManagerS.R.TimeLeftInSeconds().ToString());
				}
				if (conditionKind == CombatManagerS.CombatSpecialCondition.OneCombo){
					conditionTexts[1].text = oneComboString;
				}
			}
		}
	}

	public void ReplaceTimeString(string newTime){
		conditionTexts[1].text = timeLimitString.Replace("{TIME}", newTime);
	}
}
