using UnityEngine;
using System.Collections;

public class InfinitySpawnS : MonoBehaviour {

	public Transform playerSpawn;
	public int minDifficulty = 0;
	public int maxDifficulty = 9999;

	public EnemySpawnerS[] mySpawners;
	private int enemiesCleared = 0;

	private InfinityDemoS myInfinity;
	public Color flashColor = Color.white;

	private bool completed = false;


	public void SetInfinity(InfinityDemoS newI){

		myInfinity = newI;

	}

	public bool CheckDifficulty(int check){
		if (check >= minDifficulty && check <= maxDifficulty){
			return true;
		}else{
			return false;
		}
	}

	public void AddClear(){

		enemiesCleared++;

		if (mySpawners.Length <= enemiesCleared && !completed){
			//myInfinity.NextStage();
			completed = true;
		}

	}
}
