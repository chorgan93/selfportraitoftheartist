using UnityEngine;
using System.Collections;

public class TurnOnOffAtProgressionS : MonoBehaviour {

	public int progressNum = -1;
	public bool triggerIfContainsGreater = false;
	public GameObject[] onAtProgressObjects;
	public GameObject[] offAtProgressObjects;

	[Header("Alt progress checks")]
	public int turnOnOffAtItemInInventory = -1;
	public PlayerWeaponS turnOnOffAtMantraInInventory;
	public BuddyS turnOnOffAtBuddyInInventory;
	public int turnOnOffAtTechEarned = -1;

	// Use this for initialization
	void Start () {

		if (progressNum > -1){
			if (StoryProgressionS.storyProgress.Contains(progressNum) 
			    || (triggerIfContainsGreater && StoryProgressionS.ReturnHighestProgress() > progressNum)){
			TurnObjectsOnOff();
		}
		}
		else if (turnOnOffAtItemInInventory > -1){
			if (PlayerInventoryS.I.collectedItems.Contains(turnOnOffAtItemInInventory)){
				TurnObjectsOnOff();
			}	
		}
		

		else if (turnOnOffAtMantraInInventory != null){
		if (PlayerInventoryS.I.unlockedWeapons.Contains(turnOnOffAtMantraInInventory)){
			TurnObjectsOnOff();
			}
		}

		else if (turnOnOffAtBuddyInInventory != null){
			if (PlayerInventoryS.I.unlockedBuddies.Contains(turnOnOffAtBuddyInInventory)){
				TurnObjectsOnOff();
			}
		}
		else if (turnOnOffAtTechEarned > -1){
			if (PlayerInventoryS.I.earnedTech.Contains(turnOnOffAtTechEarned)){
				TurnObjectsOnOff();
			}	
		}
	
	}

	void TurnObjectsOnOff(){
		for (int i = 0; i < onAtProgressObjects.Length; i++){
			onAtProgressObjects[i].gameObject.SetActive(true);
		}
		for (int i = 0; i < offAtProgressObjects.Length; i++){
			offAtProgressObjects[i].gameObject.SetActive(false);
		}
	}
}
