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
	
	
	private List<int> _clearedCombatTriggers;
	public List<int> clearedCombatTriggers { get { return _clearedCombatTriggers; } }
	
	private List<GameObject> currentlySpawnedBlood;
	
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
		_combatClearedAtLeastOnce.Clear();
	}
	
	public void AddClearedCombat(int newI){
		_clearedCombatTriggers.Add(newI);
		if (_combatClearedAtLeastOnce == null){
			_combatClearedAtLeastOnce = new List<int>();
		}
		if (!_combatClearedAtLeastOnce.Contains(newI)){
			_combatClearedAtLeastOnce.Add(newI);
		}
	}
	public void ClearCompletedCombat(){
		_clearedCombatTriggers.Clear();
	}
	public void LoadCombatsCleared(List<int> newCombat){
		_combatClearedAtLeastOnce = newCombat;
	}
	
}
