using UnityEngine;
using System.Collections;

public class SacramentCombatS : MonoBehaviour {

	[Header("Combat Properties")]
	public SacramentCombatantS[] targetEnemies;
	public SacramentCombatantS[] playerParty;
	private SacramentStepS _myStep;
	public SacramentHurtEffectS hurtEffect;
	public int endAtXTurns = -1;
	private bool timedBattle = false;

	[Header("ProgressionProperties")]
	public SacramentStepS winStep;
	public SacramentStepS loseStep;
	public string winLine = "";
	public string loseLine = "";
	private bool wonCombat = false;
	private bool endTextGiven = false;
	private bool combatActive = false;
	private bool _combatantActing = false;
	public bool combatantActing { get {return _combatantActing; } }

	private SacramentCombatantS currentTurn;

	[Header("Display Properties")]
	public SacramentCombatTextS combatText;
	public string startCombatString;
	public float delayStringStart = 1f;

	private SacramentCombatActionS choosingAction;
	private SacramentCombatActionS overwatchAction;

	// Use this for initialization
	void Start () {
	
		for (int i = 0; i < playerParty.Length; i++){
			playerParty[i].SetManager(this);
		}
		for (int i = 0; i < targetEnemies.Length; i++){
			targetEnemies[i].SetManager(this);
		}
		if (endAtXTurns > 0){
			timedBattle = true;
		}


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
		//combatActive = true;
		StartCoroutine(StartCombatEffects());
	}
	IEnumerator StartCombatEffects(){
		yield return new WaitForSeconds(delayStringStart);
		combatText.ActivateText(this, startCombatString);
	}
	public void Begin(){
		combatActive = true;
	}

	public void AdvanceTurn(){
		if(!CheckCombatEnd()){
		_combatantActing = false;
		}else{
			EndCombat();
		}
	}

	public void EndCombat(){
		if (!endTextGiven){
			endTextGiven = true;
			if (wonCombat){
				combatText.AddToString(winLine, null);
			}else{
				combatText.AddToString(loseLine, null);
			}
		}else{
		if (wonCombat){
			_myStep.EndCombat(winStep);
		}else{
			_myStep.EndCombat(loseStep);
		}
		combatActive = false;
		}
	}

	public void ShowChoices(){
		if (currentTurn){
		currentTurn.ShowOptions();
		}
	}

	public void HideChoices(){
		if (currentTurn){
		currentTurn.ShowOptions(false);
		}
	}

	public void StartAllyChoose(SacramentCombatantS choosingCombatant, SacramentCombatActionS chooseAction){
		choosingAction = chooseAction;
		/*if (chooseAction.actionType == SacramentCombatActionS.SacramentActionType.FirstAid){

			TurnOnAllyChoose(null);
		}else{
		TurnOnAllyChoose(choosingCombatant);
		}**/
		TurnOnAllyChoose(choosingCombatant);
	}

	void TurnOnAllyChoose(SacramentCombatantS chooser){
		for (int i = 0; i < playerParty.Length; i++){
			if (chooser != playerParty[i]){
				playerParty[i].canBeSelected = true;
			}
		}
		if(chooser){
		chooser.canBeSelected = false;
		}
	}

	public void ChooseActionTarget(SacramentCombatantS newTarget){
		choosingAction.SetActionTargetExt(newTarget);
		for (int i = 0; i < playerParty.Length; i++){
			playerParty[i].TurnOffChoosing();
		}
	}

	bool CheckCombatEnd(){
		bool combatOver = false;
		if (timedBattle){
			endAtXTurns--;
			if (endAtXTurns <= 0){
				combatOver = true;
			}
		}
		if (!combatOver){
		int koCount = 0;
		for (int i = 0; i < targetEnemies.Length; i++){
			if (targetEnemies[i].returnHealth <= 0){
				koCount++;
			}
		}
		if (koCount >= targetEnemies.Length){
			combatOver = true;
			wonCombat = true;
		}else{
			koCount = 0;
			for (int i = 0; i < playerParty.Length; i++){
				if (playerParty[i].returnHealth <= 0){
					koCount++;
				}
			}
			if (koCount >= playerParty.Length){
				combatOver = true;
				wonCombat = false;
		}
		}
		}
		return combatOver;
	}

	public bool CheckOverwatchAction(SacramentCombatActionS checkAction){
		bool actionInterrupted = false;

		if (checkAction.myActor.isEnemy && checkAction.targetsEnemy){
			for (int i = 0; i < playerParty.Length; i++){
				if (playerParty[i].overwatchTarget != null && playerParty[i].overwatchTarget == checkAction.currentTarget){
					overwatchAction = playerParty[i].queuedAction;
					actionInterrupted = true;
				}
			}
		}
		return actionInterrupted;
	}
	public void StartOverwatchAction(){
		overwatchAction.StartAction(overwatchAction.myActor);
	}
}
