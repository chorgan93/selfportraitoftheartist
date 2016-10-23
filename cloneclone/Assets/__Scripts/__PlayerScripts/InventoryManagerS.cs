using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManagerS : MonoBehaviour {

	public Sprite[] itemSprites;

	private List<int> _equippedInventory;
	public List<int> equippedInventory { get { return _equippedInventory; } }

	private int _currentSelection = 0;
	public int currentSelection { get { return _currentSelection; } }

	private PlayerController _pRef;
	private PlayerInventoryS _inventoryRef;
	private PlayerInteractCheckS _interactRef;

	private bool useItemButtonDown = false;
	private bool scrollItemButtonDown = false;

	private bool _updateUICall = false;
	public bool updateUICall { get {return _updateUICall; } }

	// Use this for initialization
	void Awake () {

		_inventoryRef = GetComponent<PlayerInventoryS>();

		InitializeInventory();
	}
	
	// Update is called once per frame
	void Update () {

		if (!_pRef){
			_pRef = GameObject.Find("Player").GetComponent<PlayerController>();
			_interactRef = _pRef.GetComponentInChildren<PlayerInteractCheckS>();
		}else{
			if (!_pRef.talking){
				SwitchControl();
				UseItemControl();
			}
		}
	
	}

	private void InitializeInventory(){

		// create inventory list
		_equippedInventory = new List<int>(4){-1,-1,-1,-1};
		// load from save data

	}

	private void SwitchControl(){
		if (!scrollItemButtonDown &&_pRef.myControl.ScrollItemRightButton()){
			_currentSelection++;
			if (_currentSelection > _equippedInventory.Count-1){
				_currentSelection = 0;
			}
			_updateUICall = true;
			scrollItemButtonDown = true;
		}else if (!scrollItemButtonDown &&_pRef.myControl.ScrollItemLeftButton()){
			_currentSelection--;
			if (_currentSelection < 0){
				_currentSelection = _equippedInventory.Count-1;
			}
			_updateUICall = true;
			scrollItemButtonDown = true;
		}else{
			if (!_pRef.myControl.ScrollItemLeftButton() && !_pRef.myControl.ScrollItemRightButton()){
				scrollItemButtonDown = false;
			}
		}
	}

	private void UseItemControl(){
		if (!useItemButtonDown && _pRef.myControl.UseItemButton()){
			UseItem(_equippedInventory[_currentSelection]);
			useItemButtonDown = true;
		}
		if (useItemButtonDown && !_pRef.myControl.UseItemButton()){
			useItemButtonDown = false;
		}
	}

	private void UseItem(int itemID){


		if (itemID >= 0){
			bool consumeItem = false;
		// do certain effect based on item id called
		switch(itemID){

			default:
				// check player item receivers
				if (_interactRef.CheckInteraction(itemID)){
					consumeItem = true;
				}
				break;

			// basic heal
			case 0:
				_pRef.myStats.Heal(20f);
				consumeItem = true;
				break;

			// stamina recharge
			case 1:
				_pRef.myStats.ResetStamina();
				consumeItem = true;
				break;

			// basic mana recarge
			case 2:
				_pRef.myStats.RecoverCharge(150f);
				consumeItem = true;
				break;
		}
			if (consumeItem){
			_inventoryRef.RemoveFromInventory(itemID);
			if (!_inventoryRef.CheckForItem(itemID)){
				RemoveItemAt(_currentSelection);
			}
			}
		_updateUICall = true;
		}

	}

	private void RemoveItemAt(int itemIndex){
		_equippedInventory[itemIndex] = -1;
		ShiftItems(_currentSelection);
		_currentSelection = NextAvailableItemSlot();
	}

	private void ShiftItems(int startPt){

		for (int i = startPt; i < _equippedInventory.Count; i++){
			if (_equippedInventory[i] < 0 && i+1 < _equippedInventory.Count){
				_equippedInventory[i] = _equippedInventory[i+1];
				_equippedInventory[i+1] = -1;
			}
		}


	}

	private int NextAvailableItemSlot(){
		int nextAvailable = 0;
		if (_currentSelection > 1){
			for (int i = _currentSelection-1; i > -1; i--){
				if (_equippedInventory[i] > -1 && nextAvailable == 0){
					nextAvailable = i;
				}
			}
		}
		return nextAvailable;
	}

	public void UIUpdated(){
		_updateUICall = false;
	}

	public void AddNextAvailable(int j){
		bool itemAdded = false;
		for (int i = 0; i < _equippedInventory.Count; i++){
			if (_equippedInventory[i] < 0 && !itemAdded){
				_equippedInventory[i] = j;
				itemAdded = true;
			}
		}
		_updateUICall = true;
	}
}
