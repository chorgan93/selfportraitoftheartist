using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryUIS : MonoBehaviour {

	public Image[] itemOutlines;
	//public LayoutElement[] itemOutlineElements;
	public Image[] itemImages;
	public Text[] itemNumbers;

	private Vector2 startSizeDeltaOutline;
	private Vector2 startSizeDeltaImage;
	private float sizeMult = 1.5f;

	private InventoryManagerS managerRef;

	// Use this for initialization
	void Start () {
	
		//startSizeDeltaOutline = itemOutlines[0].rectTransform.sizeDelta;
		startSizeDeltaImage = itemImages[0].rectTransform.sizeDelta;
		managerRef = GameObject.Find("InventoryManager").GetComponent<InventoryManagerS>();
		UpdateUI();

	}
	
	// Update is called once per frame
	void Update () {

		if (managerRef.updateUICall){
			UpdateUI();
		}
	
	}

	private void UpdateUI(){

		int currentSelection = managerRef.currentSelection;

		int nextSelection = currentSelection+1;
		if (nextSelection >= managerRef.equippedInventory.Count){
			nextSelection = 0;
		}

		int lastSelection = currentSelection+2;
		if (lastSelection > managerRef.equippedInventory.Count-1){
			lastSelection = lastSelection-managerRef.equippedInventory.Count;
		}

		int prevSelection = currentSelection-1;
		if (prevSelection < 0){
			prevSelection = managerRef.equippedInventory.Count-1;
		}

		// set image 0
		if (managerRef.equippedInventory[currentSelection] < 0){
			itemImages[0].enabled = false;
			itemNumbers[0].enabled = false;
		}else{
			itemImages[0].enabled = true;
			itemNumbers[0].enabled = true;
			itemImages[0].rectTransform.sizeDelta = startSizeDeltaImage*sizeMult;
			itemImages[0].sprite = managerRef.itemSprites[managerRef.equippedInventory[currentSelection]];
			itemNumbers[0].text = PlayerInventoryS.I.GetItemCount(managerRef.equippedInventory[currentSelection]).ToString();
		}
		// set image 1
		if (managerRef.equippedInventory[nextSelection] < 0){
			itemImages[1].enabled = false;
			itemNumbers[1].enabled = false;
		}else{
			itemImages[1].enabled = true;
			itemNumbers[1].enabled = true;
			itemImages[1].rectTransform.sizeDelta = startSizeDeltaImage;
			itemImages[1].sprite = managerRef.itemSprites[managerRef.equippedInventory[nextSelection]];
			itemNumbers[1].text = PlayerInventoryS.I.GetItemCount(managerRef.equippedInventory[nextSelection]).ToString();
		}
		// set image 2
		if (managerRef.equippedInventory[prevSelection] < 0){
			itemImages[2].enabled = false;
			itemNumbers[2].enabled = false;
		}else{
			itemImages[2].enabled = true;
			itemNumbers[2].enabled = true;
			itemImages[2].rectTransform.sizeDelta = startSizeDeltaImage;
			itemImages[2].sprite = managerRef.itemSprites[managerRef.equippedInventory[prevSelection]];
			itemNumbers[2].text = PlayerInventoryS.I.GetItemCount(managerRef.equippedInventory[prevSelection]).ToString();
		}

		// set image 2
		if (managerRef.equippedInventory[lastSelection] < 0){
			itemImages[3].enabled = false;
			itemNumbers[3].enabled = false;
		}else{
			itemImages[3].enabled = true;
			itemNumbers[3].enabled = true;
			itemImages[3].rectTransform.sizeDelta = startSizeDeltaImage;
			itemImages[3].sprite = managerRef.itemSprites[managerRef.equippedInventory[lastSelection]];
			itemNumbers[3].text = PlayerInventoryS.I.GetItemCount(managerRef.equippedInventory[lastSelection]).ToString();
		}


		managerRef.UIUpdated();
	}
}
