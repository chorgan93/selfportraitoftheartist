using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConditionUIS : MonoBehaviour {

	public Image[] ConditionBorders;
	public Text[] conditionTexts;
	public Image bgImage;
	public float[] fadeAlphaMax;
	public string noDamageString;
	public string timeLimitString;
	public string oneComboString;

	private float showTime = 2f;
	private float showTimeCount;
	private bool fadeOut = false;
	private float fadeOutTime = 1.4f;
	private float fadeOutCount;
	private Color fadeCol;

	// Use this for initialization
	void Start () {

		TurnOffAll();
	
	}

	void Update(){

		if (fadeOut){
			if (showTimeCount > 0){
				showTimeCount -= Time.deltaTime;
			}else{
				fadeOutCount += Time.deltaTime;
				int fadeInt = 0;
				for (int i = 0; i < ConditionBorders.Length; i++){
					fadeCol = ConditionBorders[i].color;
					fadeCol.a = (1f-fadeOutCount/fadeOutTime)*fadeAlphaMax[fadeInt];
					ConditionBorders[i].color = fadeCol;
					fadeInt++;
				}
				for (int i = 0; i < conditionTexts.Length; i++){
					fadeCol = conditionTexts[i].color;
					fadeCol.a = (1f-fadeOutCount/fadeOutTime)*fadeAlphaMax[fadeInt];
					conditionTexts[i].color = fadeCol;
					fadeInt++;
				}
				fadeCol = bgImage.color;
				fadeCol.a = (1f-fadeOutCount/fadeOutTime)*fadeAlphaMax[fadeInt];
				bgImage.color = fadeCol;
				if (fadeOutCount >= fadeOutTime){
					TurnOffAll();
				}
			}
		}
	}


	public void TurnOffAll(){
		for (int i = 0; i < ConditionBorders.Length; i++){
			ConditionBorders[i].enabled = false;
		}
		for (int i = 0; i < conditionTexts.Length; i++){
			conditionTexts[i].enabled = false;
		}
		bgImage.enabled = false;
        conditionTexts[0].text = LocalizationManager.instance.GetLocalizedValue("ui_condition_title");
		fadeOut = false;
		fadeOutCount = 0f;
	}

	public void FailCondition(){
		for (int i = 0; i < ConditionBorders.Length; i++){
			ConditionBorders[i].color = Color.red;
		}
		for (int i = 0; i < conditionTexts.Length; i++){
			conditionTexts[i].color = Color.red;
		}
		conditionTexts[0].text = LocalizationManager.instance.GetLocalizedValue("ui_condition_fail");
    }

	public void SuccessCondition(){
		for (int i = 0; i < ConditionBorders.Length; i++){
			ConditionBorders[i].color = Color.yellow;
		}
		for (int i = 0; i < conditionTexts.Length; i++){
			conditionTexts[i].color = Color.yellow;
		}
		conditionTexts[0].text = LocalizationManager.instance.GetLocalizedValue("ui_condition_pass");
        showTimeCount = showTime;
		fadeOut = true;
		fadeOutCount = 0f;
	} 

	public void TurnOnAll(CombatManagerS.CombatSpecialCondition conditionKind){
		for (int i = 0; i < ConditionBorders.Length; i++){
			ConditionBorders[i].color = Color.white;
			ConditionBorders[i].enabled = true;
		}
		for (int i = 0; i < conditionTexts.Length; i++){
			conditionTexts[i].color = Color.white;
			conditionTexts[0].text = LocalizationManager.instance.GetLocalizedValue("ui_condition_title");
            conditionTexts[i].enabled = true;
			if (i == 1){
				if (conditionKind == CombatManagerS.CombatSpecialCondition.NoDamage){
                    conditionTexts[1].text = LocalizationManager.instance.GetLocalizedValue(noDamageString);
				}
				if (conditionKind == CombatManagerS.CombatSpecialCondition.TimeLimit){
					ReplaceTimeString(RankManagerS.R.TimeLeftInSeconds().ToString());
				}
				if (conditionKind == CombatManagerS.CombatSpecialCondition.OneCombo){
                    conditionTexts[1].text = LocalizationManager.instance.GetLocalizedValue(oneComboString);
                }
			}
		}

		fadeCol = bgImage.color;
		fadeCol.a = 1f;
		bgImage.color = fadeCol;
		bgImage.enabled = true;
	}

	public void FadeOut(){
		fadeOutCount = 0f;
		fadeOut = true;
		showTimeCount = showTime;
	}

	public void ReplaceTimeString(string newTime){
        conditionTexts[1].text = LocalizationManager.instance.GetLocalizedValue(timeLimitString).Replace("{TIME}", newTime);
	}
}
