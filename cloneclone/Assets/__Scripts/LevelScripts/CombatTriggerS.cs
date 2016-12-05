using UnityEngine;
using System.Collections;

public class CombatTriggerS : MonoBehaviour {

	public CombatManagerS combatReference;
	private bool activated = false;
	public string verseTitle;

	void Start(){
		if (combatReference.combatID > -1){
			if (PlayerInventoryS.I.dManager.clearedCombatTriggers.Contains(combatReference.combatID)){
				activated = true;
			}
		}
	}

	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" && !activated){
			if (other.gameObject.GetComponent<PlayerController>() != null){
				combatReference.SetPlayerRef(other.gameObject.GetComponent<PlayerController>());
				other.gameObject.GetComponent<PlayerController>().SetCombatManager(combatReference);
				combatReference.Initialize();
				if (combatReference.darknessHolder){
					combatReference.darknessHolder.gameObject.SetActive(true);
				}
				VerseDisplayS.V.NewVerse(verseTitle);
				activated = true;
			}
		}
	}
}
