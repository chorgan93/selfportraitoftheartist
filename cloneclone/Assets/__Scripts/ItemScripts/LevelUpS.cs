using UnityEngine;
using System.Collections;

public class LevelUpS : MonoBehaviour {

	public int upgradeID = -1; // determines type of upgrade (health, stamina, charge, etc)
	public string upgradeName;
	public string upgradeDescription;
	public int upgradeBaseCost;
	public int upgradeCostPerLv;
	public float expCostPerUpgradeOwned;
	public Sprite upgradeImg;
	public LevelUpS[] addUpgrades;
	
}
