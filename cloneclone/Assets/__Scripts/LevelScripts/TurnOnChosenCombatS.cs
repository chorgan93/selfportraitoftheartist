using UnityEngine;
using System.Collections;

public class TurnOnChosenCombatS : MonoBehaviour {
	public CombatManagerS[] potentialCombats;

	// Use this for initialization
	void Start () {

		for (int i = 0; i < potentialCombats.Length; i++){
			if (potentialCombats[i].combatID == CombatGiverS.chosenSpecialCombat){
				potentialCombats[i].gameObject.SetActive(true);
			}else{
				potentialCombats[i].gameObject.SetActive(false);
			}
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
