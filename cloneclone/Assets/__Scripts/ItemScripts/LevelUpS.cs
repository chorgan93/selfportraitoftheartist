using UnityEngine;
using System.Collections;

public class LevelUpS : MonoBehaviour {

	public int upgradeID = -1; // determines type of upgrade (health, stamina, charge, etc)
	public string upgradeName;
	public string upgradeDescription;
	public int upgradeBaseCost;
	public int upgradeCostPerLv;
	public int numAllowedPer5Lvs = 5;
	public float expCostPerUpgradeOwned;
	public Sprite upgradeImg;
	public Sprite upgradeImgLocked;
	public LevelUpS[] addUpgrades;

	public bool LockedByCount(int playerLv, int numTaken){
		bool locked = false;
		if (numAllowedPer5Lvs > 0){
		int numAllowed = (Mathf.FloorToInt(playerLv*1f/5f)+1)*numAllowedPer5Lvs;
			if (numAllowed <= numTaken){
				locked = true;
			}
		}
		return locked;
	}
	
}
