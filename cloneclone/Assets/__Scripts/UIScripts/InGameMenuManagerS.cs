using UnityEngine;
using System.Collections;

public class InGameMenuManagerS : MonoBehaviour {

	private GameMenuS gameMenu;
	private EquipMenuS equipMenu;

	private bool gameMenuActive = false;
	private bool equipMenuActive = false;

	private bool gameMenuButtonDown = false;
	private bool equipMenuButtonDown = false;
	private bool exitButtonDown = false;

	private PlayerController _pRef;
	public PlayerController pRef { get { return _pRef; } }

	// Use this for initialization
	void Start () {

		gameMenu = GetComponentInChildren<GameMenuS>();
		equipMenu = GetComponentInChildren<EquipMenuS>();

		_pRef = GameObject.Find("Player").GetComponent<PlayerController>();

		gameMenu.gameObject.SetActive(false);
		equipMenu.gameObject.SetActive(false);
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!gameMenuActive && !equipMenuActive && !_pRef.InAttack() && !_pRef.isBlocking && !_pRef.isDashing
		    && !_pRef.talking){
			if (_pRef.myControl.StartButton() && !equipMenuButtonDown){
				equipMenuActive = true;
				equipMenu.SetSelector(0);
				equipMenu.gameObject.SetActive(true);
				_pRef.SetTalking(true);
				equipMenuButtonDown = true;
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
			if ((_pRef.myControl.StartButton() && !equipMenuButtonDown) || 
			    (!exitButtonDown && equipMenu.canBeQuit && _pRef.myControl.ExitButton())){
				equipMenuActive = false;
				equipMenu.gameObject.SetActive(false);
				_pRef.SetTalking(false);
				if (_pRef.myControl.ExitButton()){
					exitButtonDown = true;
					_pRef.SetCanSwap(false);
				}else{
					equipMenuButtonDown = true;
				}
			}
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
}
