using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipMenuS : MonoBehaviour {

	[Header("Demo Properties")]
	private bool disableTech = false;
	[Header("Instance Properties")]
	public Image playerImage;
	public Text playerLevel;
	public Text playerName;
	public Text descriptionText;
	private PlayerController pRef;
	public GameObject hideOnMap;
	[Header("Text Properties")]
	public string paradigmIString;
	public string paradigmIIString;
	public string virtueString;
	public string inventoryString;

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
	public GameObject virtueSubscreen;
	public GameObject inventoryWhole;
	public GameObject inventorySubscreen;

	public RectTransform selector;
	public RectTransform[] selectorPositions;
	public Image[] selectorElements;
	private int currentPos = 0;

	[Header("Sound Properties")]
	public GameObject openSound;
	public GameObject moveSound;
	public GameObject selectSound;
	public GameObject cancelSound;
	public GameObject closeSound;

	private float startElementAlpha;

	private bool onMainScreen = true;

	private bool _canBeQuit = false;
	public bool canBeQuit { get { return _canBeQuit; } }

	// PARADIGM ELEMENTS
	private bool inParadigmIMenu;
	private bool inParadigmIIMenu;
	private bool changingWeapon;
	private int currentWeaponSelected = 0;
	[Header("Paradigm Edit Components")]
	public EquipWeaponItemS[] allMantraItems;
	public EquipBuddyItemS[] allBuddyItems;
	public RectTransform[] selectorPositionsParadigmI;
	public Image[] selectorElementsParadigmI;
	public RectTransform[] selectorPositionsParadigmII;
	public Image[] selectorElementsParadigmII;
	public RectTransform[] selectorPositionsBuddy;
	public Image[] selectorElementsBuddy;

	// VIRTUE ELEMENTS
	private bool inVirtueMenu;
	private bool changingVirtue;
	private int currentVirtueSelected = 0;
	public Text virtueAmtDisplay;
	public EquipVirtueItemS[] allVirtueItems;
	public Image[] selectorElementsVirtues;
	public RectTransform[] selectorPositionsVirtues;
	private int virtueCapacity = 4;
	public Image virtueBarUsed;
	public Image virtueBarFull;
	public Text virtueBarAmtText;
	private Vector2 virtueBarSizeDelta;
	private float virtueBarMaxX;

	// MAP ELEMENTS
	public MapScreenS mapScreen;
	public static int mapToUse = -1;
	private bool onMapScreen = false;
	public bool inMap { get { return onMapScreen; } }
	private bool mapButtonDown = false;
	public Text toggleTextLeft;
	public Text toggleTextRight;
	private string toMapString = "MAP";
	private string toMenuString = "MENU";
	public Sprite[] toggleButtonSpritesLeft;
	public Sprite[] toggleButtonSpritesRight;
	public Image toggleButtonSpriteLeft;
    public Image toggleButtonSpriteRight;
    public Image toggleButtonBGLeft;
    public Image toggleButtonBGRight;

	// INVENTORY ELEMENTS
	private bool inInventoryMenu;
	public EquipTechItemS[] allInventoryItems;
	public Image[] selectorElementsInventory;
	public RectTransform[] selectorPositionsInventory;

	private bool _initialized = false;

	private PlayerInventoryS inventoryRef;


	public void TurnOn(bool goToMap = false){

		if (!_initialized){
			mapScreen.gameObject.SetActive(false);
			playerImage.enabled = false;
			mantraMainParadigmI.enabled = false;
			mantraSubParadigmI.enabled = false;
			buddyParadigmI.enabled = false;
			buddyParadigmII.enabled = false;

			playerName.text = TextInputUIS.playerName;
	
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

			for (int i = 0; i < allVirtueItems.Length; i++){
				if (PlayerController.equippedVirtues.Contains(allVirtueItems[i].virtueNum)){
					pRef.myStats.ChangeVirtue(allVirtueItems[i].virtueCost);
				}
			}
		}

		if (pRef.myStats.currentLevel > LevelUpItemS.MAX_LEVEL_UP){
			playerLevel.text = "LV. MAX";
		} 
		else if (pRef.myStats.currentLevel < 10){
			playerLevel.text = "LV. 0" + pRef.myStats.currentLevel;
		}else{
			playerLevel.text = "LV. " + pRef.myStats.currentLevel;
		}
		pRef.FaceDown();
		virtueBarMaxX = virtueBarFull.rectTransform.sizeDelta.x-2;
		virtueAmtDisplay.text = "VP: " + pRef.myStats.usedVirtue + " / " + pRef.myStats.virtueAmt;

		toggleTextLeft.text = toggleTextRight.text = toMapString;
		//TurnOnToggleButton();

		hideOnMap.SetActive(true);
		InGameMenuManagerS.menuInUse = true;
		descriptionText.text = "";
		SetSelector(0);
		UpdateMantraDisplay();
		UpdateBuddyDisplay();
		UpdateVirtueDisplay();
		UpdateInventoryDisplay();
		UpdateMantras();
		UpdateBuddies();
		UpdateVirtues();
		gameObject.SetActive(true);
		onMapScreen = false;
		onMainScreen = true;
		mapScreen.gameObject.SetActive(false);

        if (DarknessPercentUIS.DPERCENT.UseDescent){
            mapToUse = -1;
        }

		if (openSound){
			Instantiate(openSound);
		}

		if (goToMap){
			TurnOnMapScreen();
        }else{

                toggleTextLeft.enabled = toggleTextRight.enabled = true;
            toggleButtonSpriteRight.gameObject.SetActive(true);
            toggleButtonSpriteLeft.gameObject.SetActive(true);
            if (toggleButtonBGLeft){
                toggleButtonBGLeft.enabled = true;
            }
            if (toggleButtonBGRight)
            {
                toggleButtonBGRight.enabled = true;
            }
        }
	}
	
	

	// Update is called once per frame
	void Update () {
		
		playerImage.color = pRef.myRenderer.color;
		playerImage.sprite = pRef.myRenderer.sprite;



		// MAIN MENU SECTION
		if (onMapScreen){
			if (ToggleMapButton()){
				pRef.ResetTimeMax();
				if (!mapButtonDown){
					TurnOn();
				}
				mapButtonDown = true;
			}else{
				mapButtonDown = false;
			}
		}
		else if (onMainScreen){

			if (ToggleMapButton()){
				pRef.ResetTimeMax();
				if (!mapButtonDown){
					TurnOnMapScreen();
				}
					mapButtonDown = true;
			}else{
				mapButtonDown = false;
			}


			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					pRef.ResetTimeMax();
					controlStickMoved = true;
					int targetPos = currentPos+1;
					if (targetPos > selectorPositions.Length-1){
						targetPos = 0;
					}
					SetSelector(targetPos);
					controlStickMoved = true;
				}
				if (pRef.myControl.Horizontal() <= -0.1f || pRef.myControl.Vertical() >= 0.1f){
					pRef.ResetTimeMax();
					controlStickMoved = true;
					int targetPos = currentPos-1;
					if (targetPos < 0){
						targetPos = selectorPositions.Length-1;
					}
					SetSelector(targetPos);
					controlStickMoved = true;
				}
			}

			if (pRef.myControl.GetCustomInput(3) && !selectButtonDown){
				pRef.ResetTimeMax();
				if (openSound){
					Instantiate(openSound);
				}
				if (currentPos == 0){
					GoToParadigmISetUp();
				}
				if (currentPos == 1 && pRef.SubWeapon() != null){
					GoToParadigmIISetUp();
				}
				if (currentPos == 2 && inventoryRef.earnedVirtues.Count > 0){
					GoToVirtueSetUp();
				}
				if (currentPos == 3){
					if (!disableTech){
						GoToInventorySetUp();
					}else{
						selectButtonDown = true;
					}
				}
			}
		
			_canBeQuit = true;

		}


		//______________________________________________________START LOADOUT SECTION

		// EQUIP MANTRA PARADIGM I SECTION

		if (inParadigmIMenu){
			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					pRef.ResetTimeMax();
					controlStickMoved = true;
					int targetPos = currentPos+1;
					if (!changingWeapon){
						if (targetPos > 2){
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
					pRef.ResetTimeMax();
					controlStickMoved = true;
					int targetPos = currentPos-1;
					if (!changingWeapon){
						if (targetPos < 0){
							targetPos = 2;
						}
						SetSelectorParadigmI(targetPos, 0);
					}else{
						if (targetPos < 2){
							targetPos = selectorPositionsParadigmI.Length-1;
						}
						SetSelectorParadigmI(currentPos, 0, -1);
					}
					controlStickMoved = true;
				}
			}

			// changing mantra function
			if (!changingWeapon){
				if (!selectButtonDown && pRef.myControl.GetCustomInput(3)){

                    selectButtonDown = true;
                    if (currentPos != 2 || (currentPos == 2 && !PlayerController.killedFamiliar))
                    {
                        if (openSound)
                        {
                            Instantiate(openSound);
                        }
                        pRef.ResetTimeMax();
                        changingWeapon = true;
                        currentWeaponSelected = currentPos;
                        if (currentPos == 0)
                        {
                            SetSelectorParadigmI(FindMantraPosition(pRef.EquippedWeapon().weaponNum), 0); // replace with mantra's pos
                        }
                        else if (currentPos == 1)
                        {
                            // sub wep of main paradigm
                            SetSelectorParadigmI(FindMantraPosition(pRef.EquippedWeaponAug().weaponNum), 0); // replace with mantra's pos
                        }
                        else
                        {
                            SetSelectorParadigmI(FindBuddyPosition(pRef.EquippedBuddy().buddyNum), 0);
                        }
                    }else{
                        if (cancelSound){
                            Instantiate(cancelSound);
                        }
                    }
				}
			}
			else{
				if (!selectButtonDown && pRef.myControl.GetCustomInput(3)){
					pRef.ResetTimeMax();
					changingWeapon = false;
					selectButtonDown = true;
					if (selectSound){
						Instantiate(selectSound);
					}

					// swap actual mantra equip & update display
					if (currentWeaponSelected == 0){
						pRef.equippedWeapons[pRef.currentParadigm] = allMantraItems[currentPos-3].WeaponRefForSwitch();
					}else if (currentWeaponSelected == 1){
						pRef.subWeapons[pRef.currentParadigm] = allMantraItems[currentPos-3].WeaponRefForSwitch();
					}else{
						pRef.BuddyLoad(pRef.currentParadigm, allBuddyItems[currentPos-3].buddyInstance);
						UpdateBuddyDisplay();
					}

					pRef.ParadigmCheck();

					UpdateMantraDisplay();
					// switch weapon positions (equip mantra)
					SetSelectorParadigmI(currentWeaponSelected, 0); // replace with swapped mantra's position

				}
				if (!exitButtonDown && pRef.myControl.GetCustomInput(1)){
					if (cancelSound){
						Instantiate(cancelSound);
					}
					pRef.ResetTimeMax();
					// exit out of mantra swap
					changingWeapon = false;
					exitButtonDown = true;
					SetSelectorParadigmI(currentWeaponSelected, 0); // replace with selected mantra's position
				}
			}

			if (!exitButtonDown && pRef.myControl.GetCustomInput(1)){
				pRef.ResetTimeMax();
				SetSelector(0);
				paradigmMantraSubscreen.gameObject.SetActive(false);
				paradigmBuddySubscreen.gameObject.SetActive(false);
				inventoryWhole.gameObject.SetActive(true);
				virtueWhole.gameObject.SetActive(true);
				inParadigmIMenu = false;
				onMainScreen = true;
				exitButtonDown = true;
				if (cancelSound){
					Instantiate(cancelSound);
				}

                toggleTextLeft.enabled = toggleTextRight.enabled = true;
                toggleButtonSpriteRight.gameObject.SetActive(true);
                toggleButtonSpriteLeft.gameObject.SetActive(true);
                if (toggleButtonBGLeft)
                {
                    toggleButtonBGLeft.enabled = true;
                }
                if (toggleButtonBGRight)
                {
                    toggleButtonBGRight.enabled = true;
                }
			}
			
			_canBeQuit = false;

		}

		// EQUIP MANTRA PARADIGM II SECTION
			
		if (inParadigmIIMenu){
			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					pRef.ResetTimeMax();
					controlStickMoved = true;
					int targetPos = currentPos+1;
					if (!changingWeapon){
						if (targetPos > 2){
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
					pRef.ResetTimeMax();
					controlStickMoved = true;
					int targetPos = currentPos-1;
					if (!changingWeapon){
						if (targetPos < 0){
							targetPos = 2;
						}
						SetSelectorParadigmII(targetPos);
					}else{
						if (targetPos < 2){
							targetPos = selectorPositionsParadigmII.Length-1;
						}
						SetSelectorParadigmII(currentPos, -1);
					}
					controlStickMoved = true;
				}
			}

			// changing mantra function
			if (!changingWeapon){
                if (!selectButtonDown && pRef.myControl.GetCustomInput(3))
                {
                    selectButtonDown = true;
                    if (currentPos != 2 || (currentPos == 2 && !PlayerController.killedFamiliar))
                    {
                        if (selectSound)
                        {
                            Instantiate(selectSound);
                        }
                        pRef.ResetTimeMax();
                        changingWeapon = true;
                        currentWeaponSelected = currentPos;
                        if (currentPos == 0)
                        {
                            SetSelectorParadigmII(FindMantraPosition(pRef.SubWeapon().weaponNum)); // replace with mantra's pos
                        }
                        else if (currentPos == 1)
                        {
                            // sub wep of main paradigm
                            SetSelectorParadigmII(FindMantraPosition(pRef.SubWeaponAug().weaponNum)); // replace with mantra's pos
                        }
                        else
                        {
                            SetSelectorParadigmII(FindBuddyPosition(pRef.SubBuddy().buddyNum));
                        }
                    }
                    else
                    {
                        if (cancelSound) {
                            Instantiate(cancelSound);
                    }
                }
				}
			}
			else{
				if (!selectButtonDown && pRef.myControl.GetCustomInput(3)){
					if (selectSound){
						Instantiate(selectSound);
					}
					pRef.ResetTimeMax();
					changingWeapon = false;
					selectButtonDown = true;
					// swap actual mantra equip & update display
					if (currentWeaponSelected == 0){
						pRef.equippedWeapons[pRef.subParadigm] = allMantraItems[currentPos-3].WeaponRefForSwitch();
					}else if (currentWeaponSelected == 1){
						pRef.subWeapons[pRef.subParadigm] = allMantraItems[currentPos-3].WeaponRefForSwitch();
					}
					else{
						pRef.BuddyLoad(pRef.subParadigm, allBuddyItems[currentPos-3].buddyInstance);
						UpdateBuddyDisplay();
					}


					UpdateMantraDisplay();
					// switch weapon positions (equip mantra)
					SetSelectorParadigmII(currentWeaponSelected); // replace with swapped mantra's position

				}
				if (!exitButtonDown && pRef.myControl.GetCustomInput(1)){
					pRef.ResetTimeMax();
					// exit out of mantra swap
					changingWeapon = false;
					exitButtonDown = true;
					if (cancelSound){
						Instantiate(cancelSound);
					}
					SetSelectorParadigmII(currentWeaponSelected); // replace with selected mantra's position
				}
			}
				
			if (!exitButtonDown && pRef.myControl.GetCustomInput(1)){
				pRef.ResetTimeMax();
				SetSelector(1);
				paradigmMantraSubscreen.gameObject.SetActive(false);
				paradigmBuddySubscreen.gameObject.SetActive(false);
				inventoryWhole.gameObject.SetActive(true);
				virtueWhole.gameObject.SetActive(true);
				inParadigmIIMenu = false;
				onMainScreen = true;
				exitButtonDown = true;
				if (cancelSound){
					Instantiate(cancelSound);
				}

                toggleTextLeft.enabled = toggleTextRight.enabled = true;
                toggleButtonSpriteRight.gameObject.SetActive(true);
                toggleButtonSpriteLeft.gameObject.SetActive(true);
                if (toggleButtonBGLeft)
                {
                    toggleButtonBGLeft.enabled = true;
                }
                if (toggleButtonBGRight)
                {
                    toggleButtonBGRight.enabled = true;
                }
			}

			_canBeQuit = false;
		}

		//_______________________________________________END LOADOUT SECTION

		//_______________________________________________START VIRTUE SECTION
		if (inVirtueMenu){
			changingVirtue = true;
			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					pRef.ResetTimeMax();
					controlStickMoved = true;
					int targetPos = currentPos+1;
					SetSelectorVirtue(targetPos, 1);
					controlStickMoved = true;
				}
				if (pRef.myControl.Horizontal() <= -0.1f || pRef.myControl.Vertical() >= 0.1f){
					pRef.ResetTimeMax();
					controlStickMoved = true;
					int targetPos = currentPos-1;
					SetSelectorVirtue(targetPos, -1);
					controlStickMoved = true;
				}
			}
			// changing virtue function
			if (!selectButtonDown && pRef.myControl.GetCustomInput(3)){
				pRef.ResetTimeMax();
					//changingVirtue = false;
					selectButtonDown = true;
					// swap actual virtue equip & update display
					if (PlayerController.equippedVirtues.Contains(allVirtueItems[currentPos].virtueNum) &&
                        allVirtueItems[currentPos].virtueNum != 15){
							PlayerController.equippedVirtues.Remove(allVirtueItems[currentPos].virtueNum);
						pRef.myStats.ChangeVirtue(-allVirtueItems[currentPos].virtueCost);
					allVirtueItems[currentPos].Unequip();
					if (cancelSound){
						Instantiate(cancelSound);
					}
					}else{
						if (pRef.myStats.usedVirtue + allVirtueItems[currentPos].virtueCost <= pRef.myStats.virtueAmt){
							PlayerController.equippedVirtues.Add (allVirtueItems[currentPos].virtueNum);
							pRef.myStats.ChangeVirtue(allVirtueItems[currentPos].virtueCost);
						allVirtueItems[currentPos].Equip();
						if (selectSound &&
                        allVirtueItems[currentPos].virtueNum != 15)
                        {
							Instantiate(selectSound);
						}
						}
					}

				pRef.playerAug.RefreshAll();
					
					UpdateVirtueDisplay();

				if (!exitButtonDown && pRef.myControl.GetCustomInput(1)){
					pRef.ResetTimeMax();
					// exit out of mantra swap
					changingVirtue = false;
					exitButtonDown = true;
					if (cancelSound){
						Instantiate(cancelSound);
					}
					SetSelectorVirtue(currentVirtueSelected); // replace with selected mantra's position
				}
			}
			if (!exitButtonDown && pRef.myControl.GetCustomInput(1)){
				pRef.ResetTimeMax();
				virtueAmtDisplay.text = "VP: " + pRef.myStats.usedVirtue + " / " + pRef.myStats.virtueAmt;
				currentPos = 0;
				SetSelector(2);
				virtueSubscreen.gameObject.SetActive(false);
				inventoryWhole.gameObject.SetActive(true);
				virtueWhole.gameObject.SetActive(true);
				paradigmIMantraWhole.SetActive(true);
				paradigmIIMantraWhole.SetActive(true);
				inVirtueMenu = false;
				onMainScreen = true;
				exitButtonDown = true;
				if (cancelSound){
					Instantiate(cancelSound);
				}

                toggleTextLeft.enabled = toggleTextRight.enabled = true;
                toggleButtonSpriteRight.gameObject.SetActive(true);
                toggleButtonSpriteLeft.gameObject.SetActive(true);
                if (toggleButtonBGLeft)
                {
                    toggleButtonBGLeft.enabled = true;
                }
                if (toggleButtonBGRight)
                {
                    toggleButtonBGRight.enabled = true;
                }
			}
			
			_canBeQuit = false;
		}
		//_______________________________________________END VIRTUE SECTION

		//_______________________________________________START INVENTORY SECTION
		if (inInventoryMenu){
			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					pRef.ResetTimeMax();
					controlStickMoved = true;
					int targetPos = currentPos+1;
					SetSelectorInventory(targetPos, 1);
					controlStickMoved = true;
				}
				if (pRef.myControl.Horizontal() <= -0.1f || pRef.myControl.Vertical() >= 0.1f){
					pRef.ResetTimeMax();
					controlStickMoved = true;
					int targetPos = currentPos-1;
					SetSelectorInventory(targetPos, -1);
					controlStickMoved = true;
				}
			}

			if (!selectButtonDown && pRef.myControl.GetCustomInput(3)){
				pRef.ResetTimeMax();
				selectButtonDown = true;
					
				// toggle unlocked tech
				if (allInventoryItems[currentPos].unlocked){
					allInventoryItems[currentPos].ToggleOnOff();
					if (allInventoryItems[currentPos].techEquipped){
						if (selectSound){
							Instantiate(selectSound);
						}
					}else{
						if (cancelSound){
							Instantiate(cancelSound);
						}
					}
				}
					
					
					
					UpdateInventoryDisplay();
					
			}

			if (!exitButtonDown && pRef.myControl.GetCustomInput(1)){
				pRef.ResetTimeMax();
				currentPos = 0;
				SetSelector(3);
				inventorySubscreen.gameObject.SetActive(false);
				inventoryWhole.gameObject.SetActive(true);
				virtueWhole.gameObject.SetActive(true);
				virtueWhole.gameObject.SetActive(true);
				paradigmIMantraWhole.SetActive(true);
				paradigmIIMantraWhole.SetActive(true);
				inVirtueMenu = false;
				onMainScreen = true;
				exitButtonDown = true;
				if (cancelSound){
					Instantiate(cancelSound);
				}

                toggleTextLeft.enabled = toggleTextRight.enabled = true;
                toggleButtonSpriteRight.gameObject.SetActive(true);
                toggleButtonSpriteLeft.gameObject.SetActive(true);
                if (toggleButtonBGLeft)
                {
                    toggleButtonBGLeft.enabled = true;
                }
                if (toggleButtonBGRight)
                {
                    toggleButtonBGRight.enabled = true;
                }
			}
			
			_canBeQuit = false;
		}
		//_______________________________________________END INVENTORY SECTION

        if (!pRef.myControl.GetCustomInput(3)){
			selectButtonDown = false;
		}

        if (!pRef.myControl.GetCustomInput(1)){
			exitButtonDown = false;
            if (onMainScreen || onMapScreen){
				_canBeQuit = true;
			}else{
				_canBeQuit = false;
			}
		}

		if (Mathf.Abs(pRef.myControl.Horizontal()) < 0.1f && Mathf.Abs(pRef.myControl.Vertical()) < 0.1f){
			controlStickMoved = false;
		}

	}


	void TurnOnToggleButton(){
		int buttonToTurnOn = ControlManagerS.controlProfile;
		toggleButtonSpriteLeft.sprite = toggleButtonSpritesLeft[buttonToTurnOn];
		toggleButtonSpriteRight.sprite = toggleButtonSpritesRight[buttonToTurnOn];
	}

	public void SetSelector(int newPos){

		Color changeCols = selectorElements[currentPos].color;
		changeCols.a = startElementAlpha;
		selectorElements[currentPos].color = changeCols;

		changeCols.a = 1f;
		selectorElements[newPos].color = changeCols;

		if (newPos == 0){
			descriptionText.text = paradigmIString;
		}
		if (newPos == 1){
			descriptionText.text = paradigmIIString;
		}
		
		if (newPos == 2){
			descriptionText.text = virtueString;
		}
		
		if (newPos == 3){
			descriptionText.text = inventoryString;
		}

		if (moveSound){
			Instantiate(moveSound);
		}
		currentPos = newPos;
		selector.anchoredPosition = selectorPositions[currentPos].anchoredPosition;
	}

	public void SetSelectorVirtue(int newPos, int dir = 0){

		Color changeCols = selectorElementsVirtues[currentPos].color;
		changeCols.a = startElementAlpha;
		selectorElementsVirtues[currentPos].color = changeCols;

		int nextAvailable = newPos;
		//if (changingVirtue){

			nextAvailable = FindNextAvailableVirtue(currentPos, dir);

		/*}else{
			if (nextAvailable >= virtueCapacity){
				nextAvailable = 0;
			}
			if (nextAvailable < 0){
				nextAvailable = virtueCapacity-1;
			}
		}**/

		currentPos = nextAvailable;
		changeCols.a = 1f;
		selectorElementsVirtues[nextAvailable].color = changeCols;
		selector.position = selectorPositionsVirtues[currentPos].position;
		if (allVirtueItems[currentPos].virtueNum > -1){
			/*if (!changingVirtue){
				descriptionText.text = "Virtue Slot 0" + (currentPos+1) + ": " +
					allVirtueItems[currentPos].virtueDescription;
			}else{**/
				descriptionText.text = allVirtueItems[currentPos].virtueDescription;
			//}
		}else{
			descriptionText.text = "";
		}
		if (moveSound){
			Instantiate(moveSound);
		}

	}

	public void SetSelectorInventory(int newPos, int dir = 0){
		
		Color changeCols = selectorElementsInventory[currentPos].color;
		changeCols.a = startElementAlpha;
		selectorElementsInventory[currentPos].color = changeCols;
		
		int nextAvailable = newPos;

		if (nextAvailable >= selectorElementsInventory.Length-1){
			nextAvailable = 0;
		}
		if (nextAvailable < 0){
			nextAvailable = selectorElementsInventory.Length-1;
		}

		nextAvailable = FindNextAvailableTech(currentPos, dir);

		
		currentPos = nextAvailable;
		changeCols.a = 1f;
		selectorElementsInventory[nextAvailable].color = changeCols;
		selector.anchoredPosition = selectorPositionsInventory[currentPos].anchoredPosition;

		descriptionText.text = allInventoryItems[currentPos].techDescription;
			
		if (moveSound){
			Instantiate(moveSound);
		}
	}

	public void SetSelectorParadigmI(int newPos, int paradigmNum, int dir = 0){

		Color changeCols = selectorElementsParadigmI[currentPos].color;
		changeCols.a = startElementAlpha;

		if (!paradigmBuddySubscreen.gameObject.activeSelf || currentPos <= 2){
			if (paradigmNum == 0){
				changeCols.a = startElementAlpha;
				selectorElementsParadigmI[currentPos].color = changeCols;
			}else{
				changeCols = selectorElementsParadigmII[currentPos].color;
				changeCols.a = startElementAlpha;
				selectorElementsParadigmII[currentPos].color = changeCols;
			}
		}
		else{
			changeCols.a = startElementAlpha;
			selectorElementsBuddy[currentPos-3].color = changeCols;
		}

		int nextAvailable = newPos;
		
		if (dir == 0 && newPos <= 2){
			if (newPos == 2){
				paradigmBuddySubscreen.gameObject.SetActive(true);
				paradigmMantraSubscreen.gameObject.SetActive(false);
                if (PlayerController.killedFamiliar)
                {
                    descriptionText.text = "";
                }
                else if (paradigmNum == 0){
                    
					descriptionText.text = allBuddyItems[pRef.EquippedBuddy().buddyNum].buddyDescription;
				}else{
					descriptionText.text = allBuddyItems[pRef.SubBuddy().buddyNum].buddyDescription;
				}
			}else{
				paradigmBuddySubscreen.gameObject.SetActive(false);
				paradigmMantraSubscreen.gameObject.SetActive(true);
				if (paradigmNum == 0){
					if (newPos == 0){
					descriptionText.text = allMantraItems[pRef.EquippedWeapon().displayNum].weaponDescriptionMain 
						+ "\n" + allMantraItems[pRef.EquippedWeapon().displayNum].weaponDescriptionSub;
					}else if (newPos == 1){
						descriptionText.text = allMantraItems[pRef.EquippedWeaponAug().displayNum].weaponDescriptionMain 
							+ "\n" + allMantraItems[pRef.EquippedWeaponAug().displayNum].weaponDescriptionSub;
					}
					
				}else{
					if (newPos == 0){
					descriptionText.text = allMantraItems[pRef.SubWeapon().displayNum].weaponDescriptionMain 
						+ "\n" + allMantraItems[pRef.SubWeapon().displayNum].weaponDescriptionSub;
					}else if (newPos == 1){
						descriptionText.text = allMantraItems[pRef.SubWeaponAug().displayNum].weaponDescriptionMain 
							+ "\n" + allMantraItems[pRef.SubWeaponAug().displayNum].weaponDescriptionSub;
					}
				}
			}
		}
		else{
			if (newPos > 2 && dir != 0){
				if (!paradigmBuddySubscreen.gameObject.activeSelf){
					nextAvailable = FindNextAvailableMantra(newPos, dir);
				}else{
					nextAvailable = FindNextAvailableBuddy(newPos, dir);
				}
			}
		}

		currentPos = nextAvailable;
		changeCols.a = 1f;
		if (moveSound){
			Instantiate(moveSound);
		}
		
		if (!paradigmBuddySubscreen.gameObject.activeSelf || currentPos <= 2){
			if (paradigmNum == 0){
				selectorElementsParadigmI[nextAvailable].color = changeCols;
				selector.anchoredPosition = selectorPositionsParadigmI[currentPos].anchoredPosition;
				if(currentPos > 2){

						descriptionText.text = allMantraItems[nextAvailable-3].weaponDescriptionMain + "\n"
						+ allMantraItems[nextAvailable-3].weaponDescriptionSub;


				}
			}else{
				selectorElementsParadigmII[nextAvailable].color = changeCols;
				selector.anchoredPosition = selectorPositionsParadigmII[currentPos].anchoredPosition;
				if(currentPos > 2){

					descriptionText.text = allMantraItems[nextAvailable-3].weaponDescriptionMain + "\n"
						+ allMantraItems[nextAvailable-3].weaponDescriptionSub;;
					

				}
			}
		}else{
			selectorElementsBuddy[nextAvailable-3].color = changeCols;
			selector.anchoredPosition = selectorPositionsBuddy[currentPos-3].anchoredPosition;
            if (PlayerController.killedFamiliar)
            {
                descriptionText.text = "";
            }
            else
            {
                descriptionText.text = allBuddyItems[nextAvailable - 3].buddyDescription;
            }
		}

	}
	public void SetSelectorParadigmII(int newPos, int dir = 0){
		
		SetSelectorParadigmI(newPos, 1, dir);
		
	}

	public void SetMapScene(int newScene){
		mapToUse = newScene;
	}

	public void TurnOff(){
		paradigmMantraSubscreen.gameObject.SetActive(false);
		paradigmBuddySubscreen.gameObject.SetActive(false);
		virtueSubscreen.gameObject.SetActive(false);
		inventorySubscreen.gameObject.SetActive(false);
		inventoryWhole.gameObject.SetActive(true);
		virtueWhole.gameObject.SetActive(true);
		paradigmIMantraWhole.gameObject.SetActive(true);
		paradigmIIMantraWhole.gameObject.SetActive(true);
		inParadigmIMenu = false;
		inParadigmIIMenu = false;
		inInventoryMenu = false;
		inVirtueMenu = false;
		exitButtonDown = true;
		currentPos = 0;
		onMainScreen = true;
		gameObject.SetActive(false);
		InGameMenuManagerS.menuInUse = false;
		onMapScreen = false;

		if (closeSound){
			Instantiate(closeSound);
		}
	}

	void TurnOnMapScreen(){
		hideOnMap.SetActive(false);
		paradigmMantraSubscreen.gameObject.SetActive(false);
		paradigmBuddySubscreen.gameObject.SetActive(false);
		virtueSubscreen.gameObject.SetActive(false);
		inventorySubscreen.gameObject.SetActive(false);
		inventoryWhole.gameObject.SetActive(true);
		virtueWhole.gameObject.SetActive(true);
		paradigmIMantraWhole.gameObject.SetActive(true);
		paradigmIIMantraWhole.gameObject.SetActive(true);
		inParadigmIMenu = false;
		inParadigmIIMenu = false;
		inInventoryMenu = false;
		inVirtueMenu = false;
		exitButtonDown = true;
		currentPos = 0;
		onMainScreen = false;

		toggleTextLeft.text = toggleTextRight.text = toMenuString;
		mapScreen.Activate(mapToUse, Application.loadedLevel);
		onMapScreen = true;
		if (moveSound){
			Instantiate(moveSound);
		}
	}

	private void UpdateMantras(){
		foreach (EquipWeaponItemS w in allMantraItems){
			w.Initialize(inventoryRef);
		}
	}
	private void UpdateBuddies(){
		foreach (EquipBuddyItemS b in allBuddyItems){
			b.Initialize(inventoryRef);
		}
	}
	public void UpdateVirtues(bool fromScorned = false){
		foreach (EquipVirtueItemS v in allVirtueItems){
            v.Initialize(inventoryRef, pRef, fromScorned);
		}
	}
	private void UpdateInventory(){
		foreach (EquipTechItemS i in allInventoryItems){
			i.Initialize(inventoryRef);
		}
	}

	// TODO adjust so that we can get rid of the 3, just look at mantra/mantra/buddy of paradigm
	private int FindNextAvailableBuddy(int startPt, int dir = 1){

		int nextAvail = startPt-3;

		if (dir > 0){
			if (startPt-3 < allBuddyItems.Length){
				for (int i = startPt-3; i < allBuddyItems.Length; i++){
					if (allBuddyItems[i].unlocked && nextAvail == startPt-3){
						nextAvail = i;
					}
				}
			}
			if (nextAvail == startPt-3){
				for (int j = 0; j < startPt-3; j++){
					if (allBuddyItems[j].unlocked && nextAvail == startPt-3){
						nextAvail = j;
					}
				}
			}
		}else{
			if (startPt-3 > 0){
				for (int j = startPt-3; j >= 0; j--){
					if (allBuddyItems[j].unlocked && nextAvail == startPt-3){
						nextAvail = j;
					}
				}
			}
			if (nextAvail == startPt-3){
				for (int i = allBuddyItems.Length-1; i > startPt-3; i--){
					if (allBuddyItems[i].unlocked && nextAvail == startPt-3){
						nextAvail = i;
					}
				}
			}
		}

		return nextAvail+3;

	}

	private int FindNextAvailableMantra(int startPt, int dir = 1){
		
		int nextAvail = startPt-3;
		
		if (dir > 0){
			if (startPt-3 <= allMantraItems.Length){
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
			if (startPt-3 >= 0){
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

	private int FindNextAvailableVirtue(int startPt, int dir){
		int nextAvail = startPt;
		if (dir > 0){
			if (startPt < allVirtueItems.Length){
				for (int i = startPt; i < allVirtueItems.Length; i++){
					if (allVirtueItems[i].unlocked  && nextAvail == startPt){
						nextAvail = i;
					}
				}
			}
			if (nextAvail == startPt){
				for (int j = 0; j < startPt; j++){
					if (allVirtueItems[j].unlocked && nextAvail == startPt){
						nextAvail = j;
					}
				}
			}
		}else if (dir == 0){
			nextAvail = -1;
			for (int i = 0; i < allVirtueItems.Length; i++){
				if (allVirtueItems[i].unlocked  && nextAvail <0){
					nextAvail = i;
				}
			}
		}else{
			if (startPt > 0){
				for (int j = startPt; j >= 0; j--){
					if (allVirtueItems[j].unlocked && nextAvail == startPt){
						nextAvail = j;
					}
				}
			}
			if (nextAvail == startPt){
				for (int i = allVirtueItems.Length-1; i > startPt; i--){
					if (allVirtueItems[i].unlocked && nextAvail == startPt){
						nextAvail = i;
					}
				}
			}
		}
		return nextAvail;
	}

	private int FindNextAvailableTech(int startPt, int dir){
		int nextAvail = startPt;
		if (dir > 0){
			if (startPt < allInventoryItems.Length){
				for (int i = startPt; i < allInventoryItems.Length; i++){
					if (allInventoryItems[i].unlocked  && nextAvail == startPt){
						nextAvail = i;
					}
				}
			}
			if (nextAvail == startPt){
				for (int j = 0; j < startPt; j++){
					if (allInventoryItems[j].unlocked && nextAvail == startPt){
						nextAvail = j;
					}
				}
			}
		}else if (dir == 0){
			nextAvail = -1;
			for (int i = 0; i < allInventoryItems.Length; i++){
				if (allInventoryItems[i].unlocked  && nextAvail <0){
					nextAvail = i;
				}
			}
		}else{
			if (startPt > 0){
				for (int j = startPt; j >= 0; j--){
					if (allInventoryItems[j].unlocked && nextAvail == startPt){
						nextAvail = j;
					}
				}
			}
			if (nextAvail == startPt){
				for (int i = allInventoryItems.Length-1; i > startPt; i--){
					if (allInventoryItems[i].unlocked && nextAvail == startPt){
						nextAvail = i;
					}
				}
			}
		}
		
		return nextAvail;
	}


	// FIND POSITION Functions
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

	private int FindBuddyPosition(int budNum){
		int returnNum = 0;
		int listCount = 0;
		foreach (EquipBuddyItemS b in allBuddyItems){
			if (b.buddyNum == budNum){
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

		if (pRef.EquippedWeaponAug() != null){
			mantraSubParadigmI.color = pRef.EquippedWeaponAug().swapColor;
			mantraSubParadigmI.sprite = pRef.EquippedWeaponAug().swapSprite;
			mantraSubParadigmI.enabled = true;
		}else{
			mantraSubParadigmI.enabled = false;
		}

		if (pRef.SubWeapon() != null){
		mantraMainParadigmII.color = pRef.SubWeapon().swapColor;
		mantraMainParadigmII.sprite = pRef.SubWeapon().swapSprite;
		mantraMainParadigmII.enabled = true;
		}else{	
			mantraMainParadigmII.enabled = false;
		}

		if (pRef.SubWeaponAug() != null){
			mantraSubParadigmII.color = pRef.SubWeaponAug().swapColor;
			mantraSubParadigmII.sprite = pRef.SubWeaponAug().swapSprite;
			mantraSubParadigmII.enabled = true;
		}
		else{
			mantraSubParadigmII.enabled = false;
		}

		pRef.playerAug.RefreshAll();
	}

	private void UpdateBuddyDisplay(){

        if (PlayerController.killedFamiliar)
        {
            buddyParadigmI.color = buddyParadigmIOutline.color = Color.grey;
            buddyParadigmI.sprite = pRef.EquippedBuddy().buddyMenuSprite;
            buddyParadigmI.color = Color.black;
            buddyParadigmI.enabled = true;

            if (pRef.SubBuddy() != null)
            {
                buddyParadigmII.color = buddyParadigmIIOutline.color =  Color.grey;
                buddyParadigmII.sprite = pRef.SubBuddy().buddyMenuSprite;
                buddyParadigmII.color = Color.black;
                buddyParadigmII.enabled = buddyParadigmIIOutline.enabled = true;
            }
            else
            {
                buddyParadigmII.enabled = buddyParadigmIIOutline.enabled = false;
            }
        }
        else
        {
            buddyParadigmI.color = buddyParadigmIOutline.color = pRef.EquippedBuddy().shadowColor;
            buddyParadigmI.sprite = pRef.EquippedBuddy().buddyMenuSprite;
            buddyParadigmI.enabled = true;

            if (pRef.SubBuddy() != null)
            {
                buddyParadigmII.color = buddyParadigmIIOutline.color = pRef.SubBuddy().shadowColor;
                buddyParadigmII.sprite = pRef.SubBuddy().buddyMenuSprite;
                buddyParadigmII.enabled = buddyParadigmIIOutline.enabled = true;
            }
            else
            {
                buddyParadigmII.enabled = buddyParadigmIIOutline.enabled = false;
            }
        }
	}

	public void UpdateVirtueDisplay(){

		
		virtueBarAmtText.text = "VP: " + pRef.myStats.usedVirtue + " / " + pRef.myStats.virtueAmt;
		virtueBarSizeDelta = virtueBarUsed.rectTransform.sizeDelta;
		virtueBarSizeDelta.x = virtueBarMaxX*pRef.myStats.usedVirtuePercent;
		virtueBarUsed.rectTransform.sizeDelta = virtueBarSizeDelta;
	}

	private void UpdateInventoryDisplay(){
		
		UpdateInventory();
	}

	private void GoToParadigmISetUp(){
		currentPos = 0;
		selectButtonDown = true;
		inParadigmIMenu = true;
		onMainScreen = false;
		paradigmBuddySubscreen.gameObject.SetActive(false);
		paradigmMantraSubscreen.gameObject.SetActive(true);
		inventoryWhole.gameObject.SetActive(false);
		virtueWhole.gameObject.SetActive(false);
		SetSelectorParadigmI(0, 0);

        toggleTextLeft.enabled = toggleTextRight.enabled = false;
        toggleButtonSpriteRight.gameObject.SetActive(false);
        toggleButtonSpriteLeft.gameObject.SetActive(false);
        if (toggleButtonBGLeft)
        {
            toggleButtonBGLeft.enabled = false;
        }
        if (toggleButtonBGRight)
        {
            toggleButtonBGRight.enabled = false;
        }
	}

	private void GoToParadigmIISetUp(){
		currentPos = 0;
		selectButtonDown = true;
		inParadigmIIMenu = true;
		onMainScreen = false;
		paradigmBuddySubscreen.gameObject.SetActive(false);
		paradigmMantraSubscreen.gameObject.SetActive(true);
		inventoryWhole.gameObject.SetActive(false);
		virtueWhole.gameObject.SetActive(false);
		SetSelectorParadigmII(0);

        toggleTextLeft.enabled = toggleTextRight.enabled = false;
        toggleButtonSpriteRight.gameObject.SetActive(false);
        toggleButtonSpriteLeft.gameObject.SetActive(false);
        if (toggleButtonBGLeft)
        {
            toggleButtonBGLeft.enabled = false;
        }
        if (toggleButtonBGRight)
        {
            toggleButtonBGRight.enabled = false;
        }
	}

	private void GoToVirtueSetUp(){
		currentPos = 0;
		selectButtonDown = true;
		inVirtueMenu = true;
		onMainScreen = false;
		virtueWhole.gameObject.SetActive(false);
		paradigmBuddySubscreen.gameObject.SetActive(false);
		paradigmIMantraWhole.gameObject.SetActive(false);
		paradigmIIMantraWhole.gameObject.SetActive(false);
		paradigmMantraSubscreen.gameObject.SetActive(false);
		inventoryWhole.gameObject.SetActive(false);
		inventorySubscreen.gameObject.SetActive(false);
		virtueSubscreen.gameObject.SetActive(true);
		currentVirtueSelected = 0;
		changingVirtue = false;
		SetSelectorVirtue(0);

        toggleTextLeft.enabled = toggleTextRight.enabled = false;
        toggleButtonSpriteRight.gameObject.SetActive(false);
        toggleButtonSpriteLeft.gameObject.SetActive(false);
        if (toggleButtonBGLeft)
        {
            toggleButtonBGLeft.enabled = false;
        }
        if (toggleButtonBGRight)
        {
            toggleButtonBGRight.enabled = false;
        }
	}
	private void GoToInventorySetUp(){
		currentPos = 0;
		selectButtonDown = true;
		inInventoryMenu = true;
		onMainScreen = false;
		paradigmBuddySubscreen.gameObject.SetActive(false);
		paradigmIMantraWhole.gameObject.SetActive(false);
		paradigmIIMantraWhole.gameObject.SetActive(false);
		paradigmMantraSubscreen.gameObject.SetActive(false);
		inventoryWhole.gameObject.SetActive(false);
		inventorySubscreen.gameObject.SetActive(true);
		virtueWhole.gameObject.SetActive(false);
		virtueSubscreen.gameObject.SetActive(false);
		SetSelectorInventory(0);

        toggleTextLeft.enabled = toggleTextRight.enabled = false;
        toggleButtonSpriteRight.gameObject.SetActive(false);
        toggleButtonSpriteLeft.gameObject.SetActive(false);
        if (toggleButtonBGLeft)
        {
            toggleButtonBGLeft.enabled = false;
        }
        if (toggleButtonBGRight)
        {
            toggleButtonBGRight.enabled = false;
        }
	}

    bool ToggleMapButton(){
        bool buttonDown = false;
        if (
            pRef.myControl.GetCustomInput(7) || pRef.myControl.GetCustomInput(6)){
            buttonDown = true;
        }
        return buttonDown;
    }
}
