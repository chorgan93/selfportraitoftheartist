using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDestructionS : MonoBehaviour {

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

	
	private List<int> _clearedCombatTriggers;
	public List<int> clearedCombatTriggers { get { return _clearedCombatTriggers; } }

	public Sprite[] bloodSprites;
	public GameObject oldBlood;

	// Use this for initialization
	void Awake () {
	
		_enemiesDefeated = new List<int>();
		_bloodIds = new List<int>();
		_bloodSpriteNums = new List<int>();
		_enemiesDefeatedPos = new List<Vector3>();
		_bloodPos = new List<Vector3>();
		_clearedCombatTriggers = new List<int>();
		_combatClearedAtLeastOnce = new List<int>();

	}

	public void SpawnBlood(){
		if (_bloodIds.Count > 0){
			int index = 0;
			GameObject newBlood;
			foreach (int b in _bloodIds){
				if (Application.loadedLevel == b){
					newBlood = Instantiate(oldBlood, _bloodPos[index], Quaternion.identity)
						as GameObject;
					newBlood.GetComponent<SpriteRenderer>().sprite = bloodSprites[_bloodSpriteNums[index]];
				}
				index++;
			}
		}
	}

	public void AddEnemyDefeated(int i, Vector3 deathPos){
		_enemiesDefeated.Add(i);
		_enemiesDefeatedPos.Add(deathPos);
	}
	public void AddBlood(int scene, Vector3 pos, int spriteNum){
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
	}

	public void ClearAllSaved(){
		ClearAll();
		_combatClearedAtLeastOnce.Clear();
	}

	public void AddClearedCombat(int newI){
		_clearedCombatTriggers.Add(newI);
		if (!_combatClearedAtLeastOnce.Contains(newI)){
			_combatClearedAtLeastOnce.Add(newI);
		}
	}
	public void ClearCompletedCombat(){
		_clearedCombatTriggers.Clear();
	}
}
