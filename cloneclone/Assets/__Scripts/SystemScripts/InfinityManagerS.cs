using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfinityManagerS : MonoBehaviour {

	public static int currentFight = 0;
	public GameObject currentGeometry;
	public SpawnPosManager spawnManager;

	public CombatManagerS[] combatPool;
	public static int lastCombat = -1;
	public int nextSectionNum = -1;
	public string nextSectionScene;
	public ChangeSceneTriggerS[] nextSceneSpawns;

	// Use this for initialization
	void Start () {

		if (currentFight == nextSectionNum-1){
			for (int i = 0; i < nextSceneSpawns.Length; i++){
				nextSceneSpawns[i].nextSceneString = nextSectionScene;
			}
		}
		ChooseCombat();
	
	}

	void ChooseCombat(){
		int newCombat = 0;
		List<CombatManagerS> possCombats = new List<CombatManagerS>();
		List<CombatManagerS> overrideCombats = new List<CombatManagerS>();
		for (int i = 0; i < combatPool.Length; i++){
			if (i!=lastCombat && combatPool[i].CheckDifficulty(currentFight)){
				if (combatPool[i].overrideOnDifficulty == currentFight){
					overrideCombats.Add(combatPool[i]);
				}else{
					possCombats.Add(combatPool[i]);
				}
			}
		}
		if (overrideCombats.Count > 0){
			newCombat = Mathf.FloorToInt(Random.Range(0, overrideCombats.Count-1));
			overrideCombats[newCombat].gameObject.SetActive(true);
		}else{
			newCombat = Mathf.FloorToInt(Random.Range(0, possCombats.Count-1));
			possCombats[newCombat].gameObject.SetActive(true);
		}
		lastCombat = newCombat;
	}

	public string CurrentVerseDisplay(){
		return ("ASCENSION " + (currentFight+1).ToString());
	}

	public void SetGeometrySize(float geometryMlt){
		Vector3 newSize = currentGeometry.transform.localScale;
		newSize.x *= geometryMlt;
		newSize.y *= geometryMlt;
		currentGeometry.transform.localScale = newSize;
		spawnManager.ResetPlayerPos();
	}

	public void AddCompletedFight(){
		currentFight++;
	}

	public void ResetCombatStats(){
		currentFight = 0;
		lastCombat = 0;
	}
}
