using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Loadout Master Content", menuName = "melessthanthree/Create Loadout Master Content", order = 1)]
public class LoadoutMasterScriptableObject : ScriptableObject {

	public List<PlayerWeaponS> masterWeaponList;
	public List<BuddyS> masterBuddyList;
	public List<LevelUpS> levelUpList;
}
