using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleEnemyDetectS : MonoBehaviour {

	private EnemyS _closestEnemy;
	public EnemyS closestEnemy { get { return _closestEnemy; } }

	private List<EnemyS> enemiesInRange = new List<EnemyS>();
	public List<EnemyS> EnemiesInRange { get { return enemiesInRange; } }
	private Vector3 _enemyCenterpoint;
	public Vector3 enemyCenterpoint { get { return _enemyCenterpoint; } }

	private EnemyS targetEnemy;

	private float lowestX;
	private float lowestY;
	private float largestX;
	private float largestY;

	public bool debugTrigger = false;

	private bool initialized = false;

	void Start(){


		enemiesInRange = new List<EnemyS>();
		initialized = true;

	}


	void LateUpdate(){

		CleanEnemyList();
		FindClosestEnemy();

	}
	

	private void CleanEnemyList(){

		for (int i = 0; i < enemiesInRange.Count; i++){

			if (enemiesInRange[i] == null){
				if (debugTrigger){
					Debug.Log("Removing enemy at slot " + i + " because enemy is now null.", gameObject);
				}
				enemiesInRange.RemoveAt(i);
			}else{
				if (enemiesInRange[i].isDead || !enemiesInRange[i].gameObject.activeSelf){
					if (debugTrigger){
						Debug.Log("Removing " + enemiesInRange[i].gameObject.name + " because enemy is dead or inactive.", gameObject);
					}
					enemiesInRange.RemoveAt(i);
				}
			}

		}
		// check for duplicates
		for (int i = enemiesInRange.Count-1; i >= 0; i--){
			for (int j = 0; j < enemiesInRange.Count; j++){
				if (enemiesInRange[i].mySpawner == enemiesInRange[j].mySpawner && i != j){
					if (debugTrigger){
						Debug.Log("Removing " + enemiesInRange[i].gameObject.name + " because enemy is duplicate.", gameObject);
					}
					enemiesInRange.RemoveAt(i);
					j--;
				}
			}
		}

	}

	private void FindClosestEnemy(){

		_closestEnemy = null;

		if (enemiesInRange.Count > 0){
			Vector2 closestEnemyPosition = Vector2.zero;
			Vector2 otherEnemyPosition = Vector2.zero;

			float closestDistance = 0;

			foreach (EnemyS enemy in enemiesInRange){
				if (_closestEnemy == null){

					_closestEnemy = enemy;
					closestEnemyPosition.x = _closestEnemy.transform.position.x;
					closestEnemyPosition.y = _closestEnemy.transform.position.y;

					closestDistance = Vector2.Distance(ownPosition2D(), closestEnemyPosition);
				}
				else{

					otherEnemyPosition.x = enemy.transform.position.x;
					otherEnemyPosition.y = enemy.transform.position.y;
					float newDistance = Vector2.Distance(ownPosition2D(), otherEnemyPosition);

					if (newDistance < closestDistance){
						_closestEnemy = enemy;
						closestEnemyPosition.x = _closestEnemy.transform.position.x;
						closestEnemyPosition.y = _closestEnemy.transform.position.y;
						
						closestDistance = Vector2.Distance(ownPosition2D(), closestEnemyPosition);
					}

				}
			}
		}

	}

	private Vector2 ownPosition2D(){

		return new Vector2(transform.position.x, transform.position.y);

	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Enemy" && initialized){

			if (debugTrigger){
				Debug.Log("Attempting to add enemy " + other.gameObject.name, gameObject);
			}

			EnemyS otherEnemy = other.gameObject.GetComponent<EnemyS>();
			if (!otherEnemy){
				otherEnemy = other.transform.GetComponentInParent<EnemyS>();
			}

			if (!otherEnemy.isDead && !otherEnemy.isFriendly && !hasEnemy(otherEnemy.mySpawner)){
				if (debugTrigger){
					Debug.Log("Enemy added! " + other.gameObject.name, gameObject); 
				}
				enemiesInRange.Add(otherEnemy);
			}

		}

	}

	void OnTriggerExit(Collider other){
		
		if (other.gameObject.tag == "Enemy" && initialized){
			
			if (debugTrigger){
				Debug.Log("Attempting to remove enemy " + other.gameObject.name, gameObject);
			}
			
			EnemyS otherEnemy = other.gameObject.GetComponent<EnemyS>();
			if (!otherEnemy){
				otherEnemy = other.transform.GetComponentInParent<EnemyS>();
			}
			
			if (!otherEnemy.isDead && enemiesInRange.Count > 0 && hasEnemy(otherEnemy.mySpawner)){
				if (debugTrigger){
					Debug.Log("Enemy removed! " + other.gameObject.name, gameObject); 
				}
				enemiesInRange.Remove(otherEnemy);
			}
			
		}
		
	}


	public void SetTargetEnemy(EnemyS nTargetEnemy){
		targetEnemy = nTargetEnemy;
	}

	public bool TargetEnemyInRange(){
		if (enemiesInRange.Count > 0 && targetEnemy != null){
			return (enemiesInRange.Contains(targetEnemy));
		}else{
			return false;
		}
	}

	private bool hasEnemy(EnemySpawnerS enemyCheck){
		bool hasEnemy = false;
		if (enemyCheck != null){
		for (int i = 0; i < enemiesInRange.Count; i++){
			if (enemiesInRange[i].mySpawner == enemyCheck){
				hasEnemy = true;
			}
		}
		}
		return hasEnemy;
	}

	public bool enemiesToCorrupt(EnemyS ignoreEnemy){
		bool enemyToEnrage = false;
		for (int i = 0; i < enemiesInRange.Count; i++){
			if (enemiesInRange[i] != ignoreEnemy && !enemiesInRange[i].isCorrupted){
				enemyToEnrage = true;
			}
		}
		return enemyToEnrage;
	}
}
