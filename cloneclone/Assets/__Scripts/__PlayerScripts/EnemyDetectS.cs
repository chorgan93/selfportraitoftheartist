using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDetectS : MonoBehaviour {

	private PlayerController playerReference;

	public float inputRange;
	private EnemyS _closestEnemy;
	public EnemyS closestEnemy { get { return _closestEnemy; } }

	private List<EnemyS> enemiesInRange;

	void Start(){

		playerReference = GetComponentInParent<PlayerController>();
		playerReference.SetDetect(this);

		enemiesInRange = new List<EnemyS>();

	}

	void FixedUpdate(){

		UpdatePosition();

	}

	void LateUpdate(){

		CleanEnemyList();
		FindClosestEnemy();

	}

	private void UpdatePosition(){

		Vector3 updatePos = playerReference.ShootPosition();
		updatePos*=inputRange;
		updatePos.z = transform.localPosition.z;

		transform.localPosition = updatePos;

	}

	private void CleanEnemyList(){

		for (int i = 0; i < enemiesInRange.Count; i++){

			if (enemiesInRange[i] == null){
				enemiesInRange.RemoveAt(i);
			}else{
				if (enemiesInRange[i].isDead){
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

					closestDistance = Vector2.Distance(playerPosition2D(), closestEnemyPosition);
				}
				else{

					otherEnemyPosition.x = enemy.transform.position.x;
					otherEnemyPosition.y = enemy.transform.position.y;
					float newDistance = Vector2.Distance(playerPosition2D(), otherEnemyPosition);

					if (newDistance < closestDistance){
						_closestEnemy = enemy;
						closestEnemyPosition.x = _closestEnemy.transform.position.x;
						closestEnemyPosition.y = _closestEnemy.transform.position.y;
						
						closestDistance = Vector2.Distance(playerPosition2D(), closestEnemyPosition);
					}

				}
			}
		}

	}

	private Vector2 playerPosition2D(){

		return new Vector2(playerReference.transform.position.x, playerReference.transform.position.y);

	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Enemy"){

			EnemyS otherEnemy = other.gameObject.GetComponent<EnemyS>();

			if (!otherEnemy.isDead){
				enemiesInRange.Add(otherEnemy);
			}

		}

	}

	void OnTriggerExit(Collider other){
		
		if (other.gameObject.tag == "Enemy"){
			
			EnemyS otherEnemy = other.gameObject.GetComponent<EnemyS>();
			
			if (!otherEnemy.isDead){
				enemiesInRange.Remove(otherEnemy);
			}
			
		}
		
	}
}
