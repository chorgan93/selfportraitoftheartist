using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipMenuS : MonoBehaviour {

	public Image playerImage;
	private PlayerController pRef;

	private bool controlStickMoved = false;
	private bool selectButtonDown = false;
	private bool exitButtonDown = false;


	// loadout one
	public Image mantraMainParadigmI;
	public Image mantraSubParadigmI;
	public GameObject paradigmIMantraWhole;
	public Image buddyParadigmI;
	public Image buddyParadigmIOutline;

	// loadout subscreens
	public GameObject paradigmMantraSubscreen;
	public GameObject paradigmBuddySubscreen;

	// loadout two
	public Image mantraMainParadigmII;
	public Image mantraSubParadigmII;
	public GameObject paradigmIIMantraWhole;
	public Image buddyParadigmII;
	public Image buddyParadigmIIOutline;

	
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

	private bool inParadigmIMenu;
	private bool inParadigmIIMenu;
	private bool changingWeapon;
	private int currentWeaponSelected = 0;
	public EquipWeaponItemS[] allMantraItems;
	public RectTransform[] selectorPositionsParadigmI;
	public Image[] selectorElementsParadigmI;
	public RectTransform[] selectorPositionsParadigmII;
	public Image[] selectorElementsParadigmII;

	private bool _initialized = false;

	private PlayerInventoryS inventoryRef;


	public void TurnOn(){

		if (!_initialized){
			playerImage.enabled = false;
			mantraMainParadigmI.enabled = false;
			mantraSubParadigmI.enabled = false;
			buddyParadigmI.enabled = false;
			buddyParadigmII.enabled = false;
	
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
					GoToParadigmISetUp();
				}
				if (currentPos == 1){
					GoToParadigmIISetUp();
				}
			}
		
			_canBeQuit = true;

		}

		//______________________________________________________START LOADOUT SECTION

		// EQUIP MANTRA PARADIGM I SECTION

		if (inParadigmIMenu){
			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					controlStickMoved = true;
					int targetPos = currentPos+1;
					if (!changingWeapon){
						if (targetPos > 1){
							targetPos = 0;
						}
						SetSelectorParadigmI(targetPos, 0);
					}else{
						if (targetPos > selectorPositionsParadigmI.Length-1){
							targetPos = 2;
						}
						SetSelectorParadigmI(currentPos, 0, 1);
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
						SetSelectorParadigmI(targetPos, 0);
					}else{
						if (targetPos < 1){
							targetPos = selectorPositionsParadigmI.Length-1;
						}
						SetSelectorParadigmI(currentPos, 0, -1);
					}
					controlStickMoved = true;
				}
			}

			// changing mantra function
			if (!changingWeapon){
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingWeapon = true;
					selectButtonDown = true;
					currentWeaponSelected = currentPos;
					if (currentPos == 0){
						SetSelectorParadigmI(FindMantraPosition(pRef.EquippedWeapon().weaponNum), 0); // replace with mantra's pos
					}else{
						// sub wep of main paradigm
						SetSelectorParadigmI(FindMantraPosition(pRef.EquippedWeaponAug().weaponNum), 0); // replace with mantra's pos
					}
				}
			}
			else{
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingWeapon = false;
					selectButtonDown = true;
					// swap actual mantra equip & update display
					if (currentWeaponSelected == 0){
						pRef.equippedWeapons[0] = allMantraItems[currentPos-3].WeaponRefForSwitch();
					}else{
						pRef.subWeapons[0] = allMantraItems[currentPos-3].WeaponRefForSwitch();
					}
					if (currentWeaponSelected == pRef.currentParadigm){
						pRef.ParadigmCheck();
					}
					UpdateMantraDisplay();
					// switch weapon positions (equip mantra)
					SetSelectorParadigmI(currentWeaponSelected, 0); // replace with swapped mantra's position

				}
				if (!exitButtonDown && pRef.myControl.ExitButton()){
					// exit out of mantra swap
					changingWeapon = false;
					exitButtonDown = true;
					SetSelectorParadigmI(currentWeaponSelected, 0); // replace with selected mantra's position
				}
			}

			if (!exitButtonDown && pRef.myControl.ExitButton()){
				SetSelector(0);
				paradigmMantraSubscreen.gameObject.SetActive(false);
				inventoryWhole.gameObject.SetActive(true);
				virtueWhole.gameObject.SetActive(true);
				inParadigmIMenu = false;
				onMainScreen = true;
				exitButtonDown = true;
			}
			
			_canBeQuit = false;

		}

		// EQUIP MANTRA PARADIGM II SECTION
			
		if (inParadigmIIMenu){
			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					controlStickMoved = true;
					int targetPos = currentPos+1;
					if (!changingWeapon){
						if (targetPos > 1){
							targetPos = 0;
						}
						SetSelectorParadigmII(targetPos);
					}else{
						if (targetPos > selectorPositionsParadigmII.Length-1){
							targetPos = 2;
						}
						SetSelectorParadigmII(currentPos, 1);
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
						SetSelectorParadigmII(targetPos);
					}else{
						if (targetPos < 1){
							targetPos = selectorPositionsParadigmII.Length-1;
						}
						SetSelectorParadigmII(currentPos, -1);
					}
					controlStickMoved = true;
				}
			}

			// changing mantra function
			if (!changingWeapon){
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingWeapon = true;
					selectButtonDown = true;
					currentWeaponSelected = currentPos;
					if (currentPos == 0){
						SetSelectorParadigmII(FindMantraPosition(pRef.SubWeapon().weaponNum)); // replace with mantra's pos
					}else{
						// sub wep of main paradigm
						SetSelectorParadigmII(FindMantraPosition(pRef.SubWeaponAug().weaponNum)); // replace with mantra's pos
					}
				}
			}
			else{
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingWeapon = false;
					selectButtonDown = true;
					// swap actual mantra equip & update display
					if (currentWeaponSelected == 0){
						pRef.equippedWeapons[1] = allMantraItems[currentPos-3].WeaponRefForSwitch();
					}else{
						pRef.subWeapons[1] = allMantraItems[currentPos-3].WeaponRefForSwitch();
					}
					if (currentWeaponSelected == pRef.currentParadigm){
						pRef.ParadigmCheck();
					}
					UpdateMantraDisplay();
					// switch weapon positions (equip mantra)
					SetSelectorParadigmII(currentWeaponSelected); // replace with swapped mantra's position

				}
				if (!exitButtonDown && pRef.myControl.ExitButton()){
					// exit out of mantra swap
					changingWeapon = false;
					exitButtonDown = true;
					SetSelectorParadigmII(currentWeaponSelected); // replace with selected mantra's position
				}
			}
				
			if (!exitButtonDown && pRef.myControl.ExitButton()){
				SetSelector(1);
				paradigmMantraSubscreen.gameObject.SetActive(false);
				inventoryWhole.gameObject.SetActive(true);
				virtueWhole.gameObject.SetActive(true);
				inParadigmIIMenu = false;
				onMainScreen = true;
				exitButtonDown = true;
			}

			_canBeQuit = false;
		}

		//_______________________________________________END LOADOUT SECTION



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

	public void SetSelectorParadigmI(int newPos, int paradigmNum, int dir = 0){

		Color changeCols = selectorElementsParadigmI[currentPos].color;
		changeCols.a = startElementAlpha;
		if (paradigmNum == 0){
			selectorElementsParadigmI[currentPos].color = changeCols;
		}else{
			selectorElementsParadigmII[currentPos].color = changeCols;
		}

		int nextAvailable = newPos;

		if (dir != 0 && newPos > 1){
			nextAvailable = FindNextAvailableMantra(newPos, dir);
		}
		
		changeCols.a = 1f;
		if (paradigmNum == 0){
			selectorElementsParadigmI[nextAvailable].color = changeCols;
		}else{
			selectorElementsParadigmII[nextAvailable].color = changeCols;
		}
		
		currentPos = nextAvailable;
		if (paradigmNum == 0){
			selector.anchoredPosition = selectorPositionsParadigmI[currentPos].anchoredPosition;
		}else{
			selector.anchoredPosition = selectorPositionsParadigmII[currentPos].anchoredPosition;
		}

	}
	public void SetSelectorParadigmII(int newPos, int dir = 0){
		
		SetSelectorParadigmI(newPos, 1, dir);
		
	}

	public void TurnOff(){
		paradigmMantraSubscreen.gameObject.SetActive(false);
		inventoryWhole.gameObject.SetActive(true);
		virtueWhole.gameObject.SetActive(true);
		inParadigmIMenu = false;
		inParadigmIIMenu = false;
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

		int nextAvail = startPt-3;

		if (dir > 0){
			if (startPt+3 < allMantraItems.Length){
				for (int i = startPt-3; i < allMantraItems.Length; i++){
					if (allMantraItems[i].unlocked && nextAvail == startPt-3){
						nextAvail = i;
					}
				}
			}
			if (nextAvail == startPt-3){
				for (int j = 0; j < startPt-3; j++){
					if (allMantraItems[j].unlocked && nextAvail == startPt-3){
						nextAvail = j;
					}
				}
			}
		}else{
			if (startPt-3 > 0){
				for (int j = startPt-3; j >= 0; j--){
					if (allMantraItems[j].unlocked && nextAvail == startPt-3){
						nextAvail = j;
					}
				}
			}
			if (nextAvail == startPt-3){
				for (int i = allMantraItems.Length-1; i > startPt-3; i--){
					if (allMantraItems[i].unlocked && nextAvail == startPt-3){
						nextAvail = i;
					}
				}
			}
		}

		return nextAvail+3;

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
		return returnNum+3;
	}

	private void UpdateMantraDisplay(){
		mantraMainParadigmI.color = pRef.EquippedWeapon().swapColor;
		mantraMainParadigmI.sprite = pRef.EquippedWeapon().swapSprite;
		mantraMainParadigmI.enabled = true;
		
		mantraSubParadigmI.color = pRef.EquippedWeaponAug().swapColor;
		mantraSubParadigmI.sprite = pRef.EquippedWeaponAug().swapSprite;
		mantraSubParadigmI.enabled = true;

		mantraMainParadigmII.color = pRef.SubWeapon().swapColor;
		mantraMainParadigmII.sprite = pRef.SubWeapon().swapSprite;
		mantraMainParadigmII.enabled = true;
		
		mantraSubParadigmII.color = pRef.SubWeaponAug().swapColor;
		mantraSubParadigmII.sprite = pRef.SubWeaponAug().swapSprite;
		mantraSubParadigmII.enabled = true;
	}

	private void UpdateBuddyDisplay(){
		buddyParadigmI.color = buddyParadigmIOutline.color = pRef.EquippedBuddy().shadowColor;
		buddyParadigmI.sprite = pRef.EquippedBuddy().buddyMenuSprite;
		buddyParadigmI.enabled = true;
		
		buddyParadigmII.color = buddyParadigmIIOutline.color = pRef.SubBuddy().shadowColor;
		buddyParadigmII.sprite = pRef.SubBuddy().buddyMenuSprite;
		buddyParadigmII.enabled = true;
	}

	private void GoToParadigmISetUp(){
		currentPos = 0;
		selectButtonDown = true;
		inParadigmIMenu = true;
		onMainScreen = false;
		paradigmMantraSubscreen.gameObject.SetActive(true);
		inventoryWhole.gameObject.SetActive(false);
		virtueWhole.gameObject.SetActive(false);
		SetSelectorParadigmI(0, 0);
	}

	private void GoToParadigmIISetUp(){
		currentPos = 0;
		selectButtonDown = true;
		inParadigmIIMenu = true;
		onMainScreen = false;
		paradigmMantraSubscreen.gameObject.SetActive(true);
		inventoryWhole.gameObject.SetActive(false);
		virtueWhole.gameObject.SetActive(false);
		SetSelectorParadigmII(0);
	}


}
