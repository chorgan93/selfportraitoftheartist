using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateOnCombatClearedS : MonoBehaviour {
	
	public List<GameObject> turnOnObjects;
	public List<GameObject> turnOffObjects;

	public CombatManagerS turnOffReference;


	void Start(){
		if (PlayerInventoryS.I.dManager.clearedCombatTriggers != null){
			if (PlayerInventoryS.I.dManager.clearedCombatTriggers.Contains(turnOffReference.combatID)){
				TurnOn();
			}
		}
	}

	public void TurnOn(){

		for (int i = 0; i < turnOnObjects.Count; i++){ 
			if (turnOnObjects[i] != null){
				turnOnObjects[i].SetActive(true);
			}
		}
		for (int i = 0; i < turnOffObjects.Count; i++){ 
			if (turnOffObjects[i] != null){
				turnOffObjects[i].SetActive(false);
			}
		}


	}
}
