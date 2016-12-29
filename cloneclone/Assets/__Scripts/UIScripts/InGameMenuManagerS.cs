﻿using UnityEngine;
using System.Collections;

public class InGameMenuManagerS : MonoBehaviour {

	private GameMenuS gameMenu;
	private EquipMenuS equipMenu;
	private LevelUpMenu levelUpMenu;

	public LevelUpMenu levelMenu { get { return levelUpMenu; } }

	private bool gameMenuActive = false;
	private bool equipMenuActive = false;
	private bool levelMenuActive = false;

	private bool gameMenuButtonDown = false;
	private bool equipMenuButtonDown = false;
	private bool exitButtonDown = false;

	private bool playerDead = false;

	public static bool allowMenuUse = false;
	public static bool hasUsedMenu = false;

	private PlayerController _pRef;
	public PlayerController pRef { get { return _pRef; } }

	// Use this for initialization
	void Start () {

		gameMenu = GetComponentInChildren<GameMenuS>();
		equipMenu = GetComponentInChildren<EquipMenuS>();
		levelUpMenu = GetComponentInChildren<LevelUpMenu>();

		_pRef = GameObject.Find("Player").GetComponent<PlayerController>();

		gameMenu.gameObject.SetActive(false);
		equipMenu.gameObject.SetActive(false);
		levelUpMenu.gameObject.SetActive(false);
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!_pRef.myStats.PlayerIsDead()){
		if (!levelMenuActive && !gameMenuActive && !equipMenuActive && !_pRef.InAttack() && !_pRef.isBlocking && !_pRef.isDashing
		    && !_pRef.talking){

				// TODO: turn back on once functional
			if (allowMenuUse && _pRef.myControl.StartButton() && !equipMenuButtonDown){
				equipMenuActive = true;
				equipMenu.TurnOn();
				_pRef.SetTalking(true);
				equipMenuButtonDown = true;
					hasUsedMenu = true;
			}
			if (_pRef.myControl.BackButton() && !gameMenuButtonDown){
				gameMenuActive = true;
				gameMenuButtonDown = true;
				gameMenu.gameObject.SetActive(true);
				_pRef.SetTalking(true);
			}
		}

		if (gameMenuActive){
			if (_pRef.myControl.BackButton() && !gameMenuButtonDown){
				gameMenuActive = false;
				gameMenuButtonDown = true;
				gameMenu.gameObject.SetActive(false);
				_pRef.SetTalking(false);
			}
		}

		if (equipMenuActive){

			if (!equipMenu.canBeQuit){
				if (_pRef.myControl.ExitButton()){
					exitButtonDown = true;
				}
			}

			if ((_pRef.myControl.StartButton() && !equipMenuButtonDown) || 
			    (equipMenu.canBeQuit && !exitButtonDown && _pRef.myControl.ExitButton())){
				equipMenuActive = false;
				//equipMenu.gameObject.SetActive(false);
				equipMenu.TurnOff();
				_pRef.SetTalking(false);
				if (_pRef.myControl.ExitButton()){
					exitButtonDown = true;
					_pRef.SetCanSwap(false);
				}else{
					equipMenuButtonDown = true;
				}
			}
		}
		}else{
			if (!playerDead){
				gameMenuActive = false;
				gameMenu.gameObject.SetActive(false);
				equipMenuActive = false;
				equipMenu.TurnOff();
				playerDead = true;
			}
		}

		if (levelMenuActive && _pRef.myControl.ExitButton()){
			exitButtonDown = true;
		}

		if (!_pRef.myControl.StartButton()){
			equipMenuButtonDown = false;
		}
		if (!_pRef.myControl.BackButton()){
			gameMenuButtonDown = false;
		}
	
		if (_pRef.myControl.ExitButtonUp()){
			exitButtonDown = false;
			_pRef.SetCanSwap(true);
		}

	}

	public void TurnOnLevelUpMenu(){
		levelUpMenu.TurnOn();
		levelMenuActive = true;
	}

	public void TurnOffLevelUpMenu(){
		levelUpMenu.gameObject.SetActive(false);
		levelMenuActive = false;
	}
}
