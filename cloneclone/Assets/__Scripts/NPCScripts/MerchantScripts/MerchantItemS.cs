using UnityEngine;
using System.Collections;

public class MerchantItemS : MonoBehaviour {

	[Header("Sell Properties")]
	public string itemName = "";
	public int itemCost = 100;
	public string itemDescription;

	[Header("Item Properties")]
	public int giveVirtue = -1;
	public PlayerWeaponS giveWeapon;
	public BuddyS buddyToGive;
	public int giveItem = -1;
	public int giveRewind = -1;
	public int giveHeal = -1;
	public int giveVP = -1;
	public int giveCheckpt = -1;

	private PlayerStatsS statRef;

	public bool isAvailable(){
		bool available = true;

		if (giveVirtue > -1){
			if (PlayerInventoryS.I.earnedVirtues.Contains(giveVirtue)){
				available = false;
			}
		}

		if (giveItem > -1){
			if (PlayerInventoryS.I.collectedItems.Contains(giveItem)){
				available = false;
			}
		}

		if (giveRewind > -1){
			if (PlayerInventoryS.I.CheckHeal(giveRewind)){
				available = false;
			}
		}

		if (giveHeal > -1){
			if (PlayerInventoryS.I.CheckCharge(giveHeal)){
				available = false;
			}
		}

		if (giveVP > -1){
			if (PlayerInventoryS.I.CheckVP(giveVP)){
				available = false;
			}
		}

		if (giveCheckpt > -1){
			if (PlayerInventoryS.I.HasReachedScene(giveCheckpt)){
				available = false;
			}
		}

		if (giveWeapon){
			if (PlayerInventoryS.I.unlockedWeapons.Contains(giveWeapon)){
				available = false;
			}
		}
		if (buddyToGive){
			if (PlayerInventoryS.I.unlockedBuddies.Contains(buddyToGive)){
				available = false;
			}
		}

		return available;
	}

	public bool canBeBought(){
		if (itemCost <= PlayerCollectionS.currencyCollected && isAvailable()){
			if (!statRef){
				statRef = GameObject.Find("Player").GetComponent<PlayerStatsS>();
			}
			return true;
		}else{
			return false;
		}
	}

	public void Buy(){
		if (giveVirtue > -1){
			PlayerInventoryS.I.AddEarnedVirtue(giveVirtue);
		}

		if (giveItem > -1){
			PlayerInventoryS.I.AddToInventory(giveItem);
		}

		if (giveRewind > -1){
			PlayerInventoryS.I.AddHeal(giveRewind);
		}

		if (giveHeal > -1){
			PlayerInventoryS.I.AddCharge(giveHeal);
		}

		if (giveCheckpt > -1){
			PlayerInventoryS.I.AddCheckpoint(giveCheckpt, 0);
		}

		if (giveVP > -1){
			PlayerInventoryS.I.AddVP(giveVP);
			statRef.AddStat(3);
		}

		if (giveWeapon){
			PlayerInventoryS.I.unlockedWeapons.Add(giveWeapon);
		}
		if (buddyToGive){
			PlayerInventoryS.I.unlockedBuddies.Add(buddyToGive);
		}
	}
}
