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
				enemiesInRange.RemoveAt(i);
			}else{
				if (enemiesInRange[i].isDead || !enemiesInRange[i].gameObject.activeSelf){
					enemiesInRange.RemoveAt(i);
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

			EnemyS otherEnemy = other.gameObject.GetComponent<EnemyS>();

			if (!otherEnemy.isDead && !otherEnemy.isFriendly){
				enemiesInRange.Add(otherEnemy);
			}

		}

	}

	void OnTriggerExit(Collider other){
		
		if (other.gameObject.tag == "Enemy" && initialized){
			
			EnemyS otherEnemy = other.gameObject.GetComponent<EnemyS>();
			
			if (!otherEnemy.isDead && enemiesInRange.Count > 0){
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
}
