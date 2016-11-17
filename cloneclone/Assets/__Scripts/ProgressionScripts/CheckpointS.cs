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
	public int spawnNum = 0;

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

			
			_playerDetect.player.SetExamining(true, _playerDetect.examineString);

			if (_playerDetect.player.myControl.TalkButton() && _talkButtonUp
			    && !_playerDetect.player.talking){
				
				if (fullCheckpoint){
					_examining = true;
						_menuManager.TurnOnLevelUpMenu();
					_playerDetect.player.SetTalking(true);
					_playerDetect.player.SetExamining(true, "");

				}
				else{
					_playerDetect.player.TriggerResting(3f);
					instructionText.SetTimedMessage(healMessage, 1.4f);
					_playerDetect.player.SetExamining(true, "");
					//Debug.Log("YEAH");
			}
				// set revive pos
				GameOverS.reviveScene = Application.loadedLevelName;
				GameOverS.revivePosition = spawnNum;
			// heal player
			_playerDetect.player.myStats.FullRecover();
				PlayerInventoryS.I.dManager.ClearAll();
				PlayerInventoryS.I.RefreshRechargeables();
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
