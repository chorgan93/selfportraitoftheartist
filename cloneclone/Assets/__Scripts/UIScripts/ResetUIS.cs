using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResetUIS : MonoBehaviour {

	public Image itemIcon;
	public Image itemHolder;
	public Text resetCount;
	public Text instruction;
	public Image countHolderLeft;
	public Image countHolderRight;
	public Image instructHolder;

	public Sprite healItemSprite;
	private Sprite rewindItemSprite;

	private InventoryManagerS inventoryRef;
	private bool isShowing = true;

	[Header("Special Scene Properties")]
	public bool arcadeMode = false;

	// Use this for initialization
	void Start () {

		inventoryRef = PlayerInventoryS.I.iManager;
		rewindItemSprite = itemIcon.sprite;
		UpdateUI ();


		if (GameObject.Find("Player").GetComponent<ControlManagerS>().ControllerAttached()){
			instruction.text = "RB";
		}else{
			instruction.text = "R";
		}
		if (PlayerController.equippedUpgrades.Contains(2) && !PlayerStatDisplayS.RECORD_MODE && !arcadeMode){
			Show ();
		}else{
			Hide ();
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
		if (PlayerInventoryS.I.CheckForItem(0) && isShowing){
			itemIcon.enabled = true;
			itemHolder.enabled = true;
			countHolderLeft.enabled = true;
			countHolderRight.enabled = true;
			instructHolder.enabled = true;
			resetCount.enabled = true;
			instruction.enabled = true;
			if (PlayerInventoryS.I.iManager.equippedInventory[PlayerInventoryS.I.iManager.currentSelection] == 0){
				itemIcon.sprite = rewindItemSprite;
				if (InventoryManagerS.infiniteResets){
					resetCount.enabled = false;
					countHolderLeft.enabled = false;
					countHolderRight.enabled = false;
				}else{
					resetCount.text = PlayerInventoryS.I.GetItemCount(0).ToString();
				}
			}else{
				itemIcon.sprite = healItemSprite;

				resetCount.text = PlayerInventoryS.I.GetItemCount(1).ToString();

			}
		}else{
			resetCount.enabled = false;
			itemIcon.enabled = false;
			itemHolder.enabled = false;
			countHolderLeft.enabled = false;
			countHolderRight.enabled = false;
			instruction.enabled = false;
			instructHolder.enabled = false;
		}
	}

	public void Show(){
		isShowing = true;
		UpdateUI();
	}

	public void Hide(){
		isShowing = false;
		UpdateUI();
	}
}
