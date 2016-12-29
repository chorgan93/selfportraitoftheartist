using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResetUIS : MonoBehaviour {

	public Image itemIcon;
	public Image itemHolder;
	public Text resetCount;
	public Text instruction;
	public Image countHolder;
	public Image instructHolder;

	private InventoryManagerS inventoryRef;

	// Use this for initialization
	void Start () {

		inventoryRef = PlayerInventoryS.I.iManager;
		UpdateUI ();

		if (GameObject.Find("Player").GetComponent<ControlManagerS>().ControllerAttached()){
			instruction.text = "LB";
		}else{
			instruction.text = "R";
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (inventoryRef.updateUICall){
			UpdateUI();
			inventoryRef.UIUpdated();
		}
	
	}

	public void UpdateUI(){
		if (PlayerInventoryS.I.CheckForItem(0)){
			itemIcon.enabled = true;
			itemHolder.enabled = true;
			countHolder.enabled = true;
			instructHolder.enabled = true;
			resetCount.enabled = true;
			instruction.enabled = true;
			if (InventoryManagerS.infiniteResets){
				resetCount.enabled = false;
				countHolder.enabled = false;
			}else{
				resetCount.text = PlayerInventoryS.I.GetItemCount(0).ToString();
			}
		}else{
			resetCount.enabled = false;
			itemIcon.enabled = false;
			itemHolder.enabled = false;
			countHolder.enabled = false;
			instruction.enabled = false;
			instructHolder.enabled = false;
		}
	}
}
