using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDestructionS : MonoBehaviour {
	
	private int MAX_BLOOD_AMT = 80;
	
	// keep track of enemies defeated, blood, debris, etc
	private List<int> _enemiesDefeated;
	public List<int> enemiesDefeated { get { return _enemiesDefeated; } }
	private List<Vector3> _enemiesDefeatedPos;
	public List<Vector3> enemiesDefeatedPos { get { return _enemiesDefeatedPos; } }
	private List<int> _bloodIds;
	public List<int> bloodIds { get { return _bloodIds; } }
	private List<int> _bloodSpriteNums;
	public List<int> bloodSpriteNums { get { return _bloodSpriteNums; } }
	private List<Vector3> _bloodPos;
	public List<Vector3> bloodPos { get { return _bloodPos; } }
	
	private List<int> _combatClearedAtLeastOnce;
	public List<int> combatClearedAtLeastOnce { get { return _combatClearedAtLeastOnce; } }
	private List<int> _combatClearedRanks;
	public List<int> combatClearedRanks { get { return _combatClearedRanks; } }
	private List<string> _combatClearedRankGrades;
	public List<string> combatClearedRankGrades { get { return _combatClearedRankGrades; } }

	private List<int> _specialConditionCombatCleared;
	public List<int> specialConditionCombatCleared { get { return _specialConditionCombatCleared; } }
	
	
	private List<int> _clearedCombatTriggers;
	public List<int> clearedCombatTriggers { get { return _clearedCombatTriggers; } }
	
	private List<GameObject> currentlySpawnedBlood;
	
	public Sprite[] bloodSprites;
	public GameObject oldBlood;

	private int savedBloodCount;
	
	// Use this for initialization
	void Awake () {
		
		_enemiesDefeated = new List<int>();
		_bloodIds = new List<int>();
		_bloodSpriteNums = new List<int>();
		_enemiesDefeatedPos = new List<Vector3>();
		_bloodPos = new List<Vector3>();
		_clearedCombatTriggers = new List<int>();
		currentlySpawnedBlood = new List<GameObject>();
		
	}
	
	public void SpawnBlood(){
		currentlySpawnedBlood.Clear();
		if (_bloodIds.Count > 0){
			GameObject newBlood;
			for (int i = _bloodIds.Count-1; i >= 0; i--){
				if (Application.loadedLevel == _bloodIds[i] && currentlySpawnedBlood.Count < MAX_BLOOD_AMT){
					newBlood = Instantiate(oldBlood, _bloodPos[i], Quaternion.identity)
						as GameObject;
					currentlySpawnedBlood.Add(newBlood);
					newBlood.GetComponent<SpriteRenderer>().sprite = bloodSprites[_bloodSpriteNums[i]];
				}
			}
		}
	}
	
	public void AddEnemyDefeated(int i, Vector3 deathPos){
		_enemiesDefeated.Add(i);
		_enemiesDefeatedPos.Add(deathPos);
	}
	public void AddBlood(int scene, Vector3 pos, int spriteNum, GameObject objectRef){
		if (currentlySpawnedBlood.Count >= MAX_BLOOD_AMT){
			Destroy(currentlySpawnedBlood[0]);
			currentlySpawnedBlood.RemoveAt(0);
			currentlySpawnedBlood.Add(objectRef);
		}
		_bloodIds.Add(scene);
		_bloodPos.Add(pos);
		_bloodSpriteNums.Add(spriteNum);
	}
	public void ClearAll(){
		_enemiesDefeated.Clear();
		_enemiesDefeatedPos.Clear();
		_bloodIds.Clear();
		_bloodPos.Clear();
		_bloodSpriteNums.Clear();
		_clearedCombatTriggers.Clear();
	}
	
	public void ClearAllSaved(){
		ClearAll();
		if (_combatClearedAtLeastOnce != null){
			_combatClearedAtLeastOnce.Clear();
		}
	}
	
	public void AddClearedCombat(int newI, int newScore, string newScoreLetter){
		_clearedCombatTriggers.Add(newI);
		if (_combatClearedAtLeastOnce == null){
			_combatClearedAtLeastOnce = new List<int>();
		}
		if (_combatClearedRanks == null){
			_combatClearedRanks = new List<int>();
		}
		if (_combatClearedRankGrades == null){
			_combatClearedRankGrades = new List<string>();
		}
		if (!_combatClearedAtLeastOnce.Contains(newI)){
			_combatClearedAtLeastOnce.Add(newI);
			_combatClearedRanks.Add(newScore);
			_combatClearedRankGrades.Add(newScoreLetter);
		}else{
			if (_combatClearedRanks[_combatClearedAtLeastOnce.IndexOf(newI)] < newScore){
				_combatClearedRanks[_combatClearedAtLeastOnce.IndexOf(newI)] = newScore;
				_combatClearedRankGrades[_combatClearedAtLeastOnce.IndexOf(newI)] = newScoreLetter;
			}
		}
	}
	public void ClearCompletedCombat(){
		_clearedCombatTriggers.Clear();
	}
	public void AddSpecialConditionCompleteID(int newID){
		if (_specialConditionCombatCleared == null){
			_specialConditionCombatCleared = new List<int>();
		}
		if (!_specialConditionCombatCleared.Contains(newID)){
			_specialConditionCombatCleared.Add(newID);
		}
	}
	public void LoadCombatsCleared(List<int> newCombat, List<int> newCombatScores, List<string> newCombatGrades, List<int> specialCombats){
		_combatClearedAtLeastOnce = newCombat;
		if (newCombatScores != null){
		_combatClearedRanks = newCombatScores;
		}else{
			_combatClearedRanks = new List<int>();
		}
		if (newCombatGrades != null){
			_combatClearedRankGrades = newCombatGrades;
		}else{
			_combatClearedRankGrades = new List<string>();
		}

		if (specialCombats != null){
			_specialConditionCombatCleared = specialCombats;
		}else{
			_specialConditionCombatCleared = new List<int>();
		}
		if (_combatClearedRanks.Count < _combatClearedAtLeastOnce.Count){
			for (int i = _combatClearedRanks.Count; i < _combatClearedAtLeastOnce.Count; i++){
				_combatClearedRanks.Add(-1);
			}
		}
		if (_combatClearedRankGrades.Count < _combatClearedAtLeastOnce.Count){
			for (int i = _combatClearedRankGrades.Count; i < _combatClearedAtLeastOnce.Count; i++){
				_combatClearedRankGrades.Add("C");
			}
		}
	}

	public void RemoveCombatData(int cID){
		if (_combatClearedAtLeastOnce.Contains(cID)){
			int indexOfCombat = _combatClearedAtLeastOnce.IndexOf(cID);
			_combatClearedRanks.RemoveAt(indexOfCombat);
			_combatClearedRankGrades.RemoveAt(indexOfCombat);
			if (_specialConditionCombatCleared.Contains(cID)){
				_specialConditionCombatCleared.Remove(cID);
			}
			_combatClearedAtLeastOnce.Remove(cID);
		}
	}

	public void SetBattleBlood(){
		savedBloodCount = _bloodIds.Count;
	}

	public void ClearBattleBlood(){
		if (_bloodIds.Count > savedBloodCount){
			for (int i = _bloodIds.Count-1; i > savedBloodCount; i--){
				_bloodIds.RemoveAt(i);
				_bloodSpriteNums.RemoveAt(i);
			}
		}
		if (currentlySpawnedBlood.Count > 0){
			for (int i = 0; i < currentlySpawnedBlood.Count; i++){
				currentlySpawnedBlood[0].SetActive(false);
			}
			currentlySpawnedBlood.Clear();
		}
	}
}
