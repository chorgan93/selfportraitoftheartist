using UnityEngine;
using System.Collections;

public class RemoveCombatClearDataS : MonoBehaviour {

	public int combatToRemove = -1;

	// Use this for initialization
	void Start () {
	
		if (combatToRemove >= 0){
			if (PlayerInventoryS.I.dManager.combatClearedAtLeastOnce != null){
					
				PlayerInventoryS.I.dManager.RemoveCombatData(combatToRemove);

			}
		}
	}

}
