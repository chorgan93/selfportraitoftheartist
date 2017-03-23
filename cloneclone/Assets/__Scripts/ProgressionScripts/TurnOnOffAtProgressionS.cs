using UnityEngine;
using System.Collections;

public class TurnOnOffAtProgressionS : MonoBehaviour {

	public int progressNum = -1;
	public GameObject[] onAtProgressObjects;
	public GameObject[] offAtProgressObjects;

	[Header("Alt progress checks")]
	public int turnOnOffAtItemInInventory = -1;
	public PlayerWeaponS turnOnOffAtMantraInInventory;
	public BuddyS turnOnOffAtBuddyInInventory;

	// Use this for initialization
	void Start () {

		if (progressNum > -1 && StoryProgressionS.storyProgress >= progressNum){
			TurnObjectsOnOff();
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
