using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelUpHandlerS : MonoBehaviour {

	// place this on collection handler and make sure to load along with collection

	public List<LevelUpS> nextLevelUps = new List<LevelUpS>();
	private List<LevelUpS> nextLevelUpsReset = new List<LevelUpS>();

	public List<LevelUpS> availableLevelUps = new List<LevelUpS>();
	private List<LevelUpS> availableLevelUpsReset = new List<LevelUpS>();

	public List<LockedLevelUpS> lockedLevelUps = new List<LockedLevelUpS>();
	private List<LockedLevelUpS> lockedLevelUpsReset = new List<LockedLevelUpS>();

	private int currentPlayerLvl = 0;


	private bool initialized = false;


	public void LoadLists(List<LevelUpS> nLU, List<LevelUpS> aLU, List<LockedLevelUpS> lLU){

		if (!initialized){
			SetDefaults();
			initialized = true;
		}
		nextLevelUps = nLU;
		availableLevelUps = aLU;
		lockedLevelUps = lLU;
	
	}

	void SetDefaults(){
		nextLevelUpsReset.Clear();
		availableLevelUpsReset.Clear();
		lockedLevelUpsReset.Clear();
		for (int i = 0; i < nextLevelUps.Count; i++){
			nextLevelUpsReset.Add(nextLevelUps[i]);
		}
		for (int i = 0; i < availableLevelUps.Count; i++){
			availableLevelUpsReset.Add(availableLevelUps[i]);
		}
		for (int i = 0; i < lockedLevelUps.Count; i++){
			lockedLevelUpsReset.Add(lockedLevelUps[i]);
		}
	}

	public void NewNextLevelUps(int newL){

		currentPlayerLvl = newL;


		// check for locked level ups
		for (int i = lockedLevelUps.Count-1; i >= 0; i--){
			if (lockedLevelUps[i].minLevelForUpgrade > 0){
				if (lockedLevelUps[i].minLevelForUpgrade <= currentPlayerLvl){
					availableLevelUps.Add(lockedLevelUps[i].levelUpToAdd);
					lockedLevelUps.RemoveAt(i);
				}
			}
			else if (lockedLevelUps[i].minHealthForUpgrade > 0){
				if (lockedLevelUps[i].minHealthForUpgrade <= PlayerInventoryS.I.GetUpgradeCount(0)){
					availableLevelUps.Add(lockedLevelUps[i].levelUpToAdd);
					lockedLevelUps.RemoveAt(i);
				}
			}
			else if (lockedLevelUps[i].minStaminaForUpgrade > 0){
				if (lockedLevelUps[i].minStaminaForUpgrade <= PlayerInventoryS.I.GetUpgradeCount(1)){
					availableLevelUps.Add(lockedLevelUps[i].levelUpToAdd);
					lockedLevelUps.RemoveAt(i);
				}
			}
			else if (lockedLevelUps[i].minChargeForUpgrade > 0){
				if (lockedLevelUps[i].minChargeForUpgrade <= PlayerInventoryS.I.GetUpgradeCount(2)){
					availableLevelUps.Add(lockedLevelUps[i].levelUpToAdd);
					lockedLevelUps.RemoveAt(i);
				}
			}
		
		}

		foreach (LevelUpS l in nextLevelUps){
			availableLevelUps.Add(l);
		}
		nextLevelUps.Clear();

		int addLevelUpIndex = 0;

		List<LevelUpS> levelUpPool = new List<LevelUpS>();
		for (int i = availableLevelUps.Count-1; i >= 0; i--){

			if (!availableLevelUps[i].LockedByCount(currentPlayerLvl, PlayerInventoryS.I.GetUpgradeCount(availableLevelUps[i].upgradeID))){
				levelUpPool.Add(availableLevelUps[i]);
			}
		}

		while (nextLevelUps.Count < 4 && levelUpPool.Count > 0){
			addLevelUpIndex = Mathf.RoundToInt(Random.Range(0, levelUpPool.Count-1));
			nextLevelUps.Add(levelUpPool[addLevelUpIndex]);
			availableLevelUps.Remove(levelUpPool[addLevelUpIndex]);
			levelUpPool.RemoveAt(addLevelUpIndex);
		}

	}

	public void AddAvailableUpgrade(LevelUpS nextLvU){
		availableLevelUps.Add(nextLvU);
	}

	public void ResetUpgrades(){
		
		nextLevelUps.Clear();
		availableLevelUps.Clear();
		lockedLevelUps.Clear();

		for (int i = 0; i < nextLevelUpsReset.Count; i++){
			nextLevelUps.Add(nextLevelUpsReset[i]);
		}
		for (int i = 0; i < availableLevelUpsReset.Count; i++){
			availableLevelUps.Add(availableLevelUpsReset[i]);
		}
		for (int i = 0; i < lockedLevelUpsReset.Count; i++){
			lockedLevelUps.Add(lockedLevelUpsReset[i]);
		}
	}

}
