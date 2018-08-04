using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateOnTouchS : MonoBehaviour {

	public List<GameObject> turnOnObjects;
	public List<GameObject> turnOffObjects;

	public int[] onlyActivateIfEnemyDefeated;
	private bool didCombatCheck = false;
	private bool doNotTrigger = false;

	public bool allowMultipleUse = false;
    public bool doNotTurnOff = false;
	private bool turnedOn = false;


	void OnTriggerEnter(Collider other){

		if (!didCombatCheck && onlyActivateIfEnemyDefeated != null){
			CombatCheck();
		}
		
		if (other.gameObject.tag == "Player" && !turnedOn && !doNotTrigger){

			foreach (GameObject eh in turnOnObjects){
				eh.SetActive(true);
			}
			foreach (GameObject bleh in turnOffObjects){
				bleh.SetActive(false);
			}

			if (!allowMultipleUse){
			turnedOn = true;
                if (!doNotTurnOff)
                {
                    GetComponent<Collider>().enabled = false;
                }
			}
		}
	}

	void CombatCheck(){
		doNotTrigger = true;
		if (onlyActivateIfEnemyDefeated.Length > 0){
			for (int i = 0; i < onlyActivateIfEnemyDefeated.Length; i++){
				if (PlayerInventoryS.I.dManager.enemiesDefeated.Contains(onlyActivateIfEnemyDefeated[i])){
					doNotTrigger = false;
				}
			}
		}else{
			doNotTrigger = false;
		}
		didCombatCheck = true;
        if (doNotTrigger){
            if (!doNotTurnOff)
            {
                GetComponent<Collider>().enabled = false;
            }
        }
	}
}
