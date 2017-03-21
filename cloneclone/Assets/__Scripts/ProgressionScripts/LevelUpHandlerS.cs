using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelUpHandlerS : MonoBehaviour {

	// place this on collection handler and make sure to load along with collection

	public List<LevelUpS> nextLevelUps = new List<LevelUpS>();

	public List<LevelUpS> availableLevelUps = new List<LevelUpS>();

	public List<LockedLevelUpS> lockedLevelUps = new List<LockedLevelUpS>();

	private int currentPlayerLvl = 0;


	public void LoadLists(List<LevelUpS> nLU, List<LevelUpS> aLU, List<LockedLevelUpS> lLU){

		nextLevelUps = nLU;
		availableLevelUps = aLU;
		lockedLevelUps = lLU;
	
	}

	public void NewNextLevelUps(int newL){

		currentPlayerLvl = newL;


		// check for locked level ups
		for (int i = lockedLevelUps.Count-1; i >= 0; i--){
			if (lockedLevelUps[i].minLevelForUpgrade <= currentPlayerLvl){
				availableLevelUps.Add(lockedLevelUps[i].levelUpToAdd);
				lockedLevelUps.RemoveAt(i);
			}
		}

		foreach (LevelUpS l in nextLevelUps){
			availableLevelUps.Add(l);
		}
		nextLevelUps.Clear();

		int addLevelUpIndex = 0;

		while (nextLevelUps.Count < 4){
			addLevelUpIndex = Mathf.RoundToInt(Random.Range(0, availableLevelUps.Count-1));
			nextLevelUps.Add(availableLevelUps[addLevelUpIndex]);
			availableLevelUps.RemoveAt(addLevelUpIndex);
		}

	}

	public void AddAvailableUpgrade(LevelUpS nextLvU){
		availableLevelUps.Add(nextLvU);
	}

}
