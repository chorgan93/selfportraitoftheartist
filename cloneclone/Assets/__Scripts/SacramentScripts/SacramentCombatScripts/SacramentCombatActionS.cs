using UnityEngine;
using System.Collections;

public class SacramentCombatActionS : MonoBehaviour {

	public enum SacramentActionType {Flavor, Attack, Overwatch, FirstAid};
	public SacramentActionType actionType;

	private SacramentCombatantS _myActor;
	public SacramentCombatantS myActor {get {return _myActor;}}
	public bool targetsEnemy = false;
	public bool targetsAlly = false;

	[Header("Action Properties")]
	public int actionCooldown = 4;
	public string[] actionLines;
	public string[] reactionLines;
	public string[] missLines;
	public string[] chooseTargetLines;
	private SacramentCombatantS _currentTarget;
	public SacramentCombatantS currentTarget { get { return _currentTarget; } }
	public SacramentCombatActionS actionToQueue;

	[Header("Numbers and Such")]
	public float successRate = 1f;
	public bool revealsActor = true;
	public bool hidesActor = false;
	public float attackPower;

	[Header("Stat Buffs")]
	public int[] statBuffs;
	public float[] buffDurations;
	public float[] buffAmounts;

	[Header("Enemy Debuffs")]
	public int[] enemyDebuffs;
	public float[] debuffDurations;
	public float[] debuffAmounts;

	[Header("AI Properties")]
	public bool randomTargeting;
	public bool viciousTargeting;
	public SacramentCombatantS setTarget;
	private bool actionHit = false;

	private bool _initialized = false;
	public bool initialized { get { return _initialized; } }


	// Use this for initialization
	void Start () {
	
		_myActor = GetComponentInParent<SacramentCombatantS>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void StartAction(SacramentCombatantS myC, bool wasChosen = false, bool isInterrupt = false, bool chooseOverride = false){
		if (!_initialized){
			Initialize(myC);
		}
		if (setTarget){
			_currentTarget = setTarget;
		}else if (!_currentTarget || (!targetsEnemy && !targetsAlly)){
			if (myC.savedTarget != null){
				_currentTarget = myC.savedTarget;
			}else{
			_currentTarget = myC;
			}
		}

		if (myC.isEnemy && targetsEnemy){
			if (randomTargeting){
				_currentTarget = myC.myManager.playerParty[Mathf.FloorToInt(Random.Range(0,myC.myManager.playerParty.Length))];
			}
			if (viciousTargeting){
				_currentTarget = WeakestTarget(myC.myManager.playerParty);
			}
		}
		if (!myC.isEnemy && targetsEnemy){
			_currentTarget = myC.myManager.targetEnemies[0];
		}
		if (!myC.isEnemy && targetsAlly&& !chooseOverride){
			myC.myManager.StartAllyChoose(myC, this);
			myActor.myManager.combatText.AddToString(GetChooseLine(), this, true, true, true);
		}else{


			if (!isInterrupt){
				myC.DecayBuffs();
			}
		actionHit = true;

		if (_currentTarget != myC){
				float accuracyTarget = 1f;
				if (actionType == SacramentActionType.FirstAid || (actionType == SacramentActionType.Overwatch && targetsAlly)){
					accuracyTarget *= (myC.currentAccuracy*myC.CriticalMult())*successRate;
				}else{
					accuracyTarget =( (1f/_currentTarget.currentEvasion)*_currentTarget.CriticalMult())
				*(myC.currentAccuracy*myC.CriticalMult())*successRate;
				}
			if (_currentTarget.isHiding){
				accuracyTarget*=_currentTarget.hiddenEvadeMult;
			}
			float hitNum = Random.Range(0f,1f);
				if (hitNum >= accuracyTarget){
					actionHit = false;
				}
		}

			myC.SetActionQueue(actionToQueue);


		
		if (reactionLines.Length > 0 || !actionHit){
			myActor.myManager.combatText.AddToString(GetActionLine(), this, false, wasChosen);
		}else{
			myActor.myManager.combatText.AddToString(GetActionLine(), null, false, wasChosen);
		}
		_myActor.SetPriority(actionCooldown);
			_myActor.SaveTarget(_currentTarget);
		}
	}

	SacramentCombatantS WeakestTarget(SacramentCombatantS[] party){
		SacramentCombatantS returnTarget = null;
		float tieBreaker = 0;
		for (int i = 0; i < party.Length; i++){
			if (!returnTarget){
				returnTarget = party[i];
			}else{
				if (party[i].returnHealth < returnTarget.returnHealth){
					returnTarget = party[i];
				}else if (party[i].returnHealth == returnTarget.returnHealth){
					tieBreaker = Random.Range(0f,1f);
					if (tieBreaker < 0.5f){
						returnTarget = party[i];
					}
				}
			}
		}
		return returnTarget;
	}

	public virtual void AdvanceAction(bool interrupted = false){
		if (!interrupted){
		if (actionHit){
		myActor.myManager.combatText.AddToString(GetReactionLine(), null);
		}else{
			myActor.myManager.combatText.AddToString(GetMissLine(), null);
		}


		if (hidesActor){
			_myActor.SetHiding(true);
		}

		if (revealsActor){
			_myActor.SetHiding(false);
		}
			AddBuffs();
		if (attackPower > 0){
			if (targetsEnemy){
				if (actionHit){
					_myActor.myManager.hurtEffect.StartFlashing(false, 3);
					_currentTarget.TakeDamage(attackPower*_myActor.currentStrength*_myActor.CriticalMult());
					AddDebuffs(_currentTarget);
				}else{
					_myActor.myManager.hurtEffect.StartFlashing(true, 3);
				}
			}else if (targetsAlly){
				_currentTarget.TakeDamage(-attackPower*_myActor.CriticalMult());
			}else{
				_myActor.TakeDamage(-attackPower*_myActor.CriticalMult());
			}
		}
		}else{

			_myActor.SetHiding(false);
		}

		_myActor.RemoveBuffs(0.5f);
	}

	public virtual void Initialize(SacramentCombatantS myCombatant){
		_myActor = myCombatant;
		_initialized = true;
	}
	public void SetActor(SacramentCombatantS myC){
		_myActor = myC;
	}

	string GetActionLine(){
		string actionLine = actionLines[Mathf.FloorToInt(Random.Range(0, actionLines.Length))];
		actionLine = actionLine.Replace("TARGET", _currentTarget.combatantName);
		return actionLine;
	}
	string GetReactionLine(){
		string reactionLine = reactionLines[Mathf.FloorToInt(Random.Range(0, reactionLines.Length))];
		reactionLine = reactionLine.Replace("TARGET", _currentTarget.combatantName);
		return reactionLine;
	}
	string GetMissLine(){
		string missLine = missLines[Mathf.FloorToInt(Random.Range(0, missLines.Length))];
		missLine = missLine.Replace("TARGET", _currentTarget.combatantName);
		return missLine;
	}
	string GetChooseLine(){
		string missLine = chooseTargetLines[Mathf.FloorToInt(Random.Range(0, chooseTargetLines.Length))];
		missLine = missLine.Replace("TARGET", _currentTarget.combatantName);
		return missLine;
	}

	public bool ValidAction(){
		bool canAct= true;
		if (hidesActor && _myActor.isHiding){
			canAct = false;
		}
		return canAct;
	}

	void AddBuffs(){
		if (statBuffs.Length > 0){
			_myActor.AddBuffs(statBuffs, buffDurations, buffAmounts);
		}
	}

	void AddDebuffs(SacramentCombatantS buffTarget){
		if (enemyDebuffs.Length > 0){
			buffTarget.AddBuffs(enemyDebuffs, debuffDurations, debuffAmounts);
		}
	}

	public void SetActionTargetExt(SacramentCombatantS newTarget){
		_currentTarget = newTarget;
		StartAction(_myActor, true, false, true);
		if (actionType == SacramentActionType.Overwatch && targetsAlly){
			_myActor.SetOverwatchTarget(_currentTarget);
		}
	}
}
