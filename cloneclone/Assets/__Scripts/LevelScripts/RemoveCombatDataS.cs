using UnityEngine;
using System.Collections;

public class RemoveCombatDataS : MonoBehaviour {

	public int[] combatIDsToClear;
	public int[] enemyIDsToClear;

	// Use this for initialization
	void Start () {

		if (PlayerInventoryS.I.dManager.clearedCombatTriggers != null){
			for (int i = 0; i < combatIDsToClear.Length; i++){
				PlayerInventoryS.I.dManager.clearedCombatTriggers.Remove(combatIDsToClear[i]);
			}
		}

		if (PlayerInventoryS.I.dManager.enemiesDefeated != null){
			for (int i = 0; i < enemyIDsToClear.Length; i++){
				PlayerInventoryS.I.dManager.enemiesDefeated.Remove(enemyIDsToClear[i]);
			}
		}
	
	}
}
