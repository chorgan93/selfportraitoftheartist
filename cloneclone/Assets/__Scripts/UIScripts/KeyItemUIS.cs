using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class KeyItemUIS : MonoBehaviour {

	public bool doNotShowInScene = false;

	[Header("Instance Properties")]
	public Image[] keyItemSlots;
	public Image[] keyItemBGs;


	[Header("Item Sprite Settings")]
	public Sprite[] keyItemSprites;
	public int[] keyItemInts;
	public int[] keyItemAreas;
	private List<int> setKeyItems;

	private int mapScene = -1;

	int currentSlot = 0;

	private bool _initialized = false;
	public bool initialized { get { return _initialized; } }

	public static KeyItemUIS K;


	void Awake(){
		K = this;
	}
	// Use this for initialization
	void Start () {

		Initialize();
	
	}


	void Initialize(){

		if (!_initialized){
			TurnOffItemSlots();
		}
		mapScene = 
			EquipMenuS.mapToUse;
		EvaluateItems();
		
	}

	void TurnOffItemSlots(){
		for (int i = 0; i < keyItemSlots.Length; i++){
			keyItemSlots[i].enabled = false;
			keyItemBGs[i].enabled = false;
		}
		if (setKeyItems != null){
			setKeyItems.Clear();
		}else{
			setKeyItems = new List<int>();
		}
		currentSlot = 0;
	}

	public void EvaluateItems(bool reset = false){
		if (!doNotShowInScene && !PlayerStatDisplayS.RECORD_MODE){
		if (reset){
			TurnOffItemSlots();
		}
		for (int i = 0; i < keyItemInts.Length; i++){
			if (currentSlot < keyItemSlots.Length){
			if (PlayerInventoryS.I.collectedItems.Contains(keyItemInts[i])
				&& !setKeyItems.Contains(keyItemInts[i]) && !PlayerInventoryS.I.clearedWalls.Contains(keyItemInts[i])
						&& (keyItemAreas[i] <= -1 || (keyItemAreas[i] > -1 && mapScene == keyItemAreas[i]))){
					keyItemSlots[currentSlot].sprite = keyItemSprites[i];
					keyItemSlots[currentSlot].enabled = keyItemBGs[currentSlot].enabled = true;
					currentSlot++;
					setKeyItems.Add(keyItemInts[i]);
			}
			}
		}
		}
	}
}
