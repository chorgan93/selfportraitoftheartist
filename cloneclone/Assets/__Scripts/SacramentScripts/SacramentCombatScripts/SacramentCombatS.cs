using UnityEngine;
using System.Collections;

public class SacramentCombatS : MonoBehaviour {

	[Header("Combat Properties")]
	public SacramentCombatantS[] targetEnemies;
	public SacramentCombatantS[] playerParty;
	private SacramentStepS _myStep;
	public SacramentHurtEffectS hurtEffect;

	[Header("ProgressionProperties")]
	public SacramentStepS winStep;
	public SacramentStepS loseStep;
	private bool wonCombat = false;
	private bool combatActive = false;
	private bool _combatantActing = false;
	public bool combatantActing { get {return _combatantActing; } }

	private SacramentCombatantS currentTurn;

	[Header("Display Properties")]
	public SacramentCombatTextS combatText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (combatActive){

			if (!_combatantActing){
			GetNextCombatant();
			_combatantActing = true;
			}
		}
	
	}

	void GetNextCombatant(){
		if (currentTurn == null){
			currentTurn = targetEnemies[0];
		}
		for (int i = 0; i < targetEnemies.Length; i++){
			if (targetEnemies[i].currentPriority < currentTurn.currentPriority){
				currentTurn = targetEnemies[i];
			}
		}
		for (int i = 0; i < playerParty.Length; i++){
			if (playerParty[i].currentPriority < currentTurn.currentPriority){
				currentTurn = playerParty[i];
			}
		}
		// reduce all actor wait times once current turn is set
		for (int i = 0; i < targetEnemies.Length; i++){
			if (targetEnemies[i] != currentTurn){
			targetEnemies[i].SetPriority(targetEnemies[i].currentPriority-currentTurn.currentPriority);
			}
		}
		for (int i = 0; i < playerParty.Length; i++){
			if (playerParty[i] != currentTurn){
			playerParty[i].SetPriority(playerParty[i].currentPriority-currentTurn.currentPriority);
			}
		}
		currentTurn.StartActing(this);
	}

	public void StartCombat(SacramentStepS myStep){
		_myStep = myStep;
		combatActive = true;
		combatText.ActivateText(this);
	}

	public void AdvanceTurn(){
		_combatantActing = false;
	}

	public void EndCombat(){
		if (wonCombat){
			_myStep.EndCombat(winStep);
		}else{
			_myStep.EndCombat(loseStep);
		}
		combatActive = false;
	}

	public void ShowChoices(){
		currentTurn.ShowOptions();
	}

	public void HideChoices(){
		currentTurn.ShowOptions(false);
	}
}
