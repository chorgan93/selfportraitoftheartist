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

	public GameObject useRewindSound;
	public GameObject useHealSound;

	private bool useItemButtonDown = false;
	private bool toggleItemButtonDown = false;
	private bool scrollItemButtonDown = false;

	private bool _updateUICall = false;
	public bool updateUICall { get {return _updateUICall; } }


	private float useItemTime = 0.3f;

	// Use this for initialization
	void Awake () {

		_inventoryRef = GetComponent<PlayerInventoryS>();

		InitializeInventory();
	}
	
	// Update is called once per frame
	void Update () {

		if (!CinematicHandlerS.inCutscene && !MainMenuNavigationS.inMain){
			if (!_pRef){
				_pRef = GameObject.Find("Player").GetComponent<PlayerController>();
				_interactRef = _pRef.GetComponentInChildren<PlayerInteractCheckS>();
			}else{
				if (!_pRef.talking && !_pRef.myStats.PlayerIsDead()){
					//SwitchControl();
					UseItemControl();
                }
			}
        }

		/*if (Input.GetKeyDown(KeyCode.R)){
			Debug.Log(
				(equippedInventory.Contains(0) && equippedInventory.Contains(1)) + " / In cutscene? "
				+ CinematicHandlerS.inCutscene + " / In main? " + MainMenuNavigationS.inMain);
		}**/
	
	}

	private void InitializeInventory(){

		// create inventory list
		_equippedInventory = new List<int>(1){0};
		_currentSelection = 0;
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
		if (equippedInventory.Contains(0) && equippedInventory.Contains(1)){
			if (!toggleItemButtonDown && _pRef.myControl.GetCustomInput(7)){
			_currentSelection++;
			if (_currentSelection > 1){
				_currentSelection = 0;
			}
			_updateUICall = true;
			toggleItemButtonDown = true;
				if (_pRef.tutorialReference != null){
					_pRef.tutorialReference.AddSwap();
				}
				_pRef.playerSound.PlayItemSound(_currentSelection);
		}
		}

		if (toggleItemButtonDown && !_pRef.myControl.GetCustomInput(7)){
			toggleItemButtonDown = false;
		}
		if (!useItemButtonDown && _pRef.myControl.GetCustomInput(6) && !_pRef.usingitem && _pRef.CanUseItem()){
            UseItem(_equippedInventory[_currentSelection]);
			useItemButtonDown = true;
		}
		if (useItemButtonDown && !_pRef.myControl.GetCustomInput(6)){
			useItemButtonDown = false;
		}
	}

	private IEnumerator ResetFunction(){
		if (useRewindSound){
			Instantiate(useRewindSound);
		}
		_pRef.TriggerItemAnimation();
		_pRef.myStats.itemEffect.Flash(Color.white);
		yield return new WaitForSeconds(0.1f);
		CameraShakeS.C.DodgeSloMo(0.22f, 0.12f, 0.7f, 0.2f);
		yield return new WaitForSeconds(useItemTime);
		_pRef.ResetCombat();
	}

	private IEnumerator HealFunction(){
        if (useRewindSound)
        {
            Instantiate(useHealSound);
        }
		_pRef.TriggerItemAnimation();
		_pRef.myStats.itemEffect.Flash(Color.white);
		yield return new WaitForSeconds(0.1f);
		CameraShakeS.C.DodgeSloMo(0.12f, 0.06f, 0.9f, 0.1f);
		yield return new WaitForSeconds(useItemTime);
		CameraEffectsS.E.HealEffect();
		_pRef.myStats.Heal(_pRef.myStats.maxHealth);

	}
	private IEnumerator RecoverStaminaFunction(){
		
		_pRef.TriggerItemAnimation();
		yield return new WaitForSeconds(useItemTime);
		_pRef.myStats.ResetStamina();
		
	}
	private IEnumerator RecoverChargeFunction(){
		
		_pRef.TriggerItemAnimation();
		yield return new WaitForSeconds(useItemTime);
		_pRef.myStats.RecoverCharge(150f, true);

	}

	private void UseItem(int itemID){

		if (itemID >= 0 && _inventoryRef.GetItemCount(itemID) > 0){
			bool consumeItem = false;
			bool rechargeable = false;
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
				//StartCoroutine(HealFunction());
				if (_pRef.inCombat){
					StartCoroutine(ResetFunction());
                        if (!PlayerController.equippedTech.Contains(13)){
						consumeItem = true;
					}
					rechargeable = true;
				}
				break;
			case 1:
                    StartCoroutine(HealFunction());
                    if (!PlayerController.equippedTech.Contains(13))
                    {
                        consumeItem = true;
                    }
					rechargeable = true;

				break;

			// stamina recharge
			/*case 3:
				StartCoroutine(RecoverStaminaFunction());
				consumeItem = true;
				rechargeable = true;
				break;

			// basic mana recarge
			case 2:
				StartCoroutine(RecoverChargeFunction());
				consumeItem = true;
				rechargeable = true;
				break;**/
		}
			if (consumeItem){
			_inventoryRef.RemoveFromInventory(itemID, rechargeable);
			if (!_inventoryRef.CheckForItem(itemID)){
				RemoveItemAt(_currentSelection);
			}
			}
		_updateUICall = true;
		}

	}

	private void RemoveItemAt(int itemIndex){
		_equippedInventory[itemIndex] = -1;
		//ShiftItems(_currentSelection);
		//_currentSelection = NextAvailableItemSlot();
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

	public void RefreshUI(){
		_updateUICall = true;
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
	public void AddAt(int index, int j){
		bool itemAdded = false;
		if (_equippedInventory.Count > index){
			equippedInventory.Insert(index, j);
		}else{
			equippedInventory.Add(j);
		}
		_updateUICall = true;
	}


	public void LoadInventory(List<int> newInventory, int currentSel = 0){
		_equippedInventory = newInventory;
		_currentSelection = currentSel;
	}

}
