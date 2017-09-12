using UnityEngine;
using System.Collections;

public class CombatTriggerS : MonoBehaviour {

	public CombatManagerS combatReference;
	private bool activated = false;
	public string verseTitle;
	public bool preventDeath = false;
	public InfinityManagerS myInfiniteManager;

	void Start(){
		if (combatReference.combatID > -1){
			if (PlayerInventoryS.I.dManager.clearedCombatTriggers.Contains(combatReference.combatID)){
				activated = true;
				combatReference.TurnOnOnceObjects();
				combatReference.TurnOffOnceObjects();
			}
		}
	}

	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" && !activated && combatReference.AllowCombat()){
			if (other.gameObject.GetComponent<PlayerController>() != null){
				combatReference.SetPlayerRef(other.gameObject.GetComponent<PlayerController>());
				other.gameObject.GetComponent<PlayerController>().SetCombatManager(combatReference);
				combatReference.Initialize();
				if (combatReference.darknessHolder){
					combatReference.darknessHolder.gameObject.SetActive(true);
				}
				if (myInfiniteManager != null){
					VerseDisplayS.V.NewVerse(myInfiniteManager.CurrentVerseDisplay());
				}else{
					VerseDisplayS.V.NewVerse(verseTitle);
				}
				activated = true;
				
				if (preventDeath){
					PlayerStatsS.PlayerCantDie = true;
				}
			}
		}
	}
}
