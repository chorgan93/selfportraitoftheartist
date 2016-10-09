using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipMenuS : MonoBehaviour {

	public Image playerImage;
	private PlayerController pRef;

	private bool controlStickMoved = false;
	private bool selectButtonDown = false;
	private bool exitButtonDown = false;

	public Image mantraMain;
	public Image mantraSub;

	public GameObject mantraWhole;
	public GameObject mantraSubScreen;

	public Image buddyMain;
	public Image buddyMainOutline;
	public Image buddySub;
	public Image buddySubOutline;
	
	public GameObject buddyWhole;
	public GameObject buddySubScreen;

	
	public GameObject virtueWhole;
	public GameObject inventoryWhole;

	public RectTransform selector;
	public RectTransform[] selectorPositions;
	public Image[] selectorElements;
	private int currentPos = 0;

	private float startElementAlpha;

	private bool onMainScreen = true;

	private bool _canBeQuit = false;
	public bool canBeQuit { get { return _canBeQuit; } }

	private bool inWeaponMenu;
	private bool changingWeapon;
	private int currentWeaponSelected = 0;
	public EquipWeaponItemS[] allMantraItems;
	public RectTransform[] selectorPositionsWeapon;
	public Image[] selectorElementsWeapon;

	private bool _initialized = false;

	private PlayerInventoryS inventoryRef;


	public void TurnOn(){

		if (!_initialized){
			playerImage.enabled = false;
			mantraMain.enabled = false;
			mantraSub.enabled = false;
			buddyMain.enabled = false;
			buddySub.enabled = false;
	
			inventoryRef = PlayerInventoryS.I;
			
			startElementAlpha = selectorElements[0].color.a;
			SetSelector(currentPos);
			_initialized = true;

			if (!pRef){
				pRef = GetComponentInParent<InGameMenuManagerS>().pRef;
				
				
				playerImage.color = pRef.myRenderer.color;
				playerImage.sprite = pRef.myRenderer.sprite;
				playerImage.enabled = true;

			}
		}
		
		SetSelector(0);
		UpdateMantraDisplay();
		UpdateBuddyDisplay();
		UpdateMantras();
		gameObject.SetActive(true);
	}
	
	

	// Update is called once per frame
	void Update () {
		
		playerImage.color = pRef.myRenderer.color;
		playerImage.sprite = pRef.myRenderer.sprite;

		// MAIN MENU SECTION

		if (onMainScreen){

			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					controlStickMoved = true;
					int targetPos = currentPos+1;
					if (targetPos > selectorPositions.Length-1){
						targetPos = 0;
					}
					SetSelector(targetPos);
					controlStickMoved = true;
				}
				if (pRef.myControl.Horizontal() <= -0.1f || pRef.myControl.Vertical() >= 0.1f){
					controlStickMoved = true;
					int targetPos = currentPos-1;
					if (targetPos < 0){
						targetPos = selectorPositions.Length-1;
					}
					SetSelector(targetPos);
					controlStickMoved = true;
				}
			}

			if (pRef.myControl.MenuSelectButton() && !selectButtonDown){
				if (currentPos == 0){
					currentPos = 0;
					selectButtonDown = true;
					inWeaponMenu = true;
					onMainScreen = false;
					buddyWhole.gameObject.SetActive(false);
					mantraSubScreen.gameObject.SetActive(true);
					inventoryWhole.gameObject.SetActive(false);
					virtueWhole.gameObject.SetActive(false);
					SetSelectorWeapon(0);
				}
			}
		
			_canBeQuit = true;

		}

		// EQUIP MANTRA SECTION

		if (inWeaponMenu){
			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					controlStickMoved = true;
					int targetPos = currentPos+1;
					if (!changingWeapon){
						if (targetPos > 1){
							targetPos = 0;
						}
						SetSelectorWeapon(targetPos);
					}else{
						if (targetPos > selectorPositionsWeapon.Length-1){
							targetPos = 2;
						}
						SetSelectorWeapon(currentPos, 1);
					}
					controlStickMoved = true;
				}
				if (pRef.myControl.Horizontal() <= -0.1f || pRef.myControl.Vertical() >= 0.1f){
					controlStickMoved = true;
					int targetPos = currentPos-1;
					if (!changingWeapon){
						if (targetPos < 0){
							targetPos = 1;
						}
						SetSelectorWeapon(targetPos);
					}else{
						if (targetPos < 1){
							targetPos = selectorPositionsWeapon.Length-1;
						}
						SetSelectorWeapon(currentPos, -1);
					}
					controlStickMoved = true;
				}
			}

			// changing mantra function
			if (!changingWeapon){
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingWeapon = true;
					selectButtonDown = true;
					if (currentPos == 0){
						currentWeaponSelected = pRef.currentParadigm;
					}else{
						currentWeaponSelected = pRef.subParadigm;
					}
					SetSelectorWeapon(FindMantraPosition(pRef.EquippedWeapon().weaponNum)); // replace with mantra's position
				}
			}
			else{
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingWeapon = false;
					selectButtonDown = true;
					// swap actual mantra equip & update display
					pRef.equippedWeapons[currentWeaponSelected] = allMantraItems[currentPos-2].WeaponRefForSwitch();
					if (currentWeaponSelected == pRef.currentParadigm){
						pRef.ParadigmCheck();
					}
					UpdateMantraDisplay();
					// switch weapon positions (equip mantra)
					SetSelectorWeapon(currentWeaponSelected); // replace with swapped mantra's position

				}
				if (!exitButtonDown && pRef.myControl.ExitButton()){
					// exit out of mantra swap
					changingWeapon = false;
					exitButtonDown = true;
					SetSelectorWeapon(currentWeaponSelected); // replace with selected mantra's position
				}
			}

			if (!exitButtonDown && pRef.myControl.ExitButton()){
				SetSelector(0);
				buddyWhole.gameObject.SetActive(true);
				mantraSubScreen.gameObject.SetActive(false);
				inventoryWhole.gameObject.SetActive(true);
				virtueWhole.gameObject.SetActive(true);
				inWeaponMenu = false;
				onMainScreen = true;
				exitButtonDown = true;
			}

			_canBeQuit = false;
		}

		if (pRef.myControl.MenuSelectUp()){
			selectButtonDown = false;
		}

		if (pRef.myControl.ExitButtonUp()){
			exitButtonDown = false;
			if (exitButtonDown){
				Debug.Log("Exit reset!");
			}
			if (onMainScreen){
				_canBeQuit = true;
			}else{
				_canBeQuit = false;
			}
		}

		if (Mathf.Abs(pRef.myControl.Horizontal()) < 0.1f && Mathf.Abs(pRef.myControl.Vertical()) < 0.1f){
			controlStickMoved = false;
		}

	}

	public void SetSelector(int newPos){

		Color changeCols = selectorElements[currentPos].color;
		changeCols.a = startElementAlpha;
		selectorElements[currentPos].color = changeCols;

		changeCols.a = 1f;
		selectorElements[newPos].color = changeCols;
		
		currentPos = newPos;
		selector.anchoredPosition = selectorPositions[currentPos].anchoredPosition;
	}

	public void SetSelectorWeapon(int newPos, int dir = 0){

		Color changeCols = selectorElementsWeapon[currentPos].color;
		changeCols.a = startElementAlpha;
		selectorElementsWeapon[currentPos].color = changeCols;

		int nextAvailable = newPos;

		if (dir != 0 && newPos > 1){
			nextAvailable = FindNextAvailableMantra(newPos, dir);
		}
		
		changeCols.a = 1f;
		selectorElementsWeapon[nextAvailable].color = changeCols;
		
		currentPos = nextAvailable;
		selector.anchoredPosition = selectorPositionsWeapon[currentPos].anchoredPosition;

	}

	public void TurnOff(){
		buddyWhole.gameObject.SetActive(true);
		mantraSubScreen.gameObject.SetActive(false);
		inventoryWhole.gameObject.SetActive(true);
		virtueWhole.gameObject.SetActive(true);
		inWeaponMenu = false;
		exitButtonDown = true;
		currentPos = 0;
		onMainScreen = true;
		gameObject.SetActive(false);
	}

	private void UpdateMantras(){
		foreach (EquipWeaponItemS w in allMantraItems){
			w.Initialize(inventoryRef);
		}
	}

	private int FindNextAvailableMantra(int startPt, int dir = 1){

		int nextAvail = startPt-2;

		if (dir > 0){
			if (startPt < allMantraItems.Length-3){
				for (int i = startPt-2; i < allMantraItems.Length; i++){
					if (allMantraItems[i].unlocked && nextAvail == startPt-2){
						nextAvail = i;
					}
				}
			}
			if (nextAvail == startPt){
				for (int j = 0; j < startPt-2; j++){
					if (allMantraItems[j].unlocked && nextAvail == startPt-2){
						nextAvail = j;
					}
				}
			}
		}else{
			if (startPt > 2){
				for (int j = startPt-2; j > -1; j--){
					if (allMantraItems[j].unlocked && nextAvail == startPt-2){
						nextAvail = j;
					}
				}
			}
			if (nextAvail == startPt){
				for (int i = allMantraItems.Length-1; i > startPt-2; i--){
					if (allMantraItems[i].unlocked && nextAvail == startPt-2){
						nextAvail = i;
					}
				}
			}
		}

		Debug.Log(nextAvail);

		return nextAvail+2;

	}

	private int FindMantraPosition(int wepNum){
		int returnNum = 0;
		int listCount = 0;
		foreach (EquipWeaponItemS w in allMantraItems){
			if (w.weaponNum == wepNum){
				returnNum =  listCount;
			}
			listCount++;
		}
		return returnNum+2;
	}

	private void UpdateMantraDisplay(){
		mantraMain.color = pRef.EquippedWeapon().swapColor;
		mantraMain.sprite = pRef.EquippedWeapon().swapSprite;
		mantraMain.enabled = true;
		
		mantraSub.color = pRef.SubWeapon().swapColor;
		mantraSub.sprite = pRef.SubWeapon().swapSprite;
		mantraSub.enabled = true;
	}

	private void UpdateBuddyDisplay(){
		buddyMain.color = buddyMainOutline.color = pRef.EquippedBuddy().shadowColor;
		buddyMain.sprite = pRef.EquippedBuddy().buddyMenuSprite;
		buddyMain.enabled = true;
		
		buddySub.color = buddySubOutline.color = pRef.SubBuddy().shadowColor;
		buddySub.sprite = pRef.SubBuddy().buddyMenuSprite;
		buddySub.enabled = true;
	}


}
