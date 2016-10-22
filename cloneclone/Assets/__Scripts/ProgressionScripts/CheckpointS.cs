using UnityEngine;
using System.Collections;

public class CheckpointS : MonoBehaviour {

	public bool fullCheckpoint = true;
	private PlayerDetectS _playerDetect;
	private bool _examining = false;

	private InGameMenuManagerS _menuManager;

	private bool _exitButtonUp = false;
	private bool _talkButtonUp = false;

	private InstructionTextS instructionText;

	private string healMessage = "Health restored. Progress saved.";

	// Use this for initialization
	void Start () {

		_playerDetect = GetComponentInChildren<PlayerDetectS>();

		if (!fullCheckpoint){
			instructionText = GameObject.Find("InstructionText").GetComponent<InstructionTextS>();
		}else{	
			_menuManager = GameObject.Find("Menus").GetComponent<InGameMenuManagerS>();
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_playerDetect.PlayerInRange() && !_examining){
			if (_playerDetect.player.myControl.TalkButton() && _talkButtonUp){
				
				if (fullCheckpoint){
					_examining = true;
						_menuManager.TurnOnLevelUpMenu();
					_playerDetect.player.SetTalking(true);

				}
			else{
				instructionText.SetTimedMessage(healMessage, 2f);
					//Debug.Log("YEAH");
			}
			// heal player
			_playerDetect.player.myStats.FullRecover();
			_talkButtonUp = false;
			}
		}

		if (_examining){
			if ((_playerDetect.player.myControl.ExitButton() && _exitButtonUp && _menuManager.levelMenu.canBeExited)
			    || _menuManager.levelMenu.sendExitMessage){
				// exit menu
				_examining = false;
				_menuManager.TurnOffLevelUpMenu();
				_playerDetect.player.SetTalking(false);
				_exitButtonUp = false;
				_menuManager.levelMenu.sendExitMessage = false;
			}
		}

		if (_playerDetect.player != null){
		if (_playerDetect.player.myControl.ExitButtonUp()){
			_exitButtonUp = true;
		}else{
			_exitButtonUp = false;
		}
		
		if (!_playerDetect.player.myControl.TalkButton()){
			_talkButtonUp = true;
		}else{
			_talkButtonUp = false;
		}
		}else{
			_talkButtonUp = false;
			_exitButtonUp = false;
		}

	
	}
}
