using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDetectS : MonoBehaviour {

	private PlayerController playerReference;

	public float inputRange;
	private EnemyS _closestEnemy;
	public EnemyS closestEnemy { get { return _closestEnemy; } }

	private List<EnemyS> enemiesInRange;
	public List<EnemyS> allEnemiesInRange { get { return enemiesInRange; } }
	private Vector3 _enemyCenterpoint;
	public Vector3 enemyCenterpoint { get { return _enemyCenterpoint; } }

	private float lowestX;
	private float lowestY;
	private float largestX;
	private float largestY;

	void Start(){


		playerReference = GetComponentInParent<PlayerController>();
		playerReference.SetDetect(this);

		enemiesInRange = new List<EnemyS>();

	}

	void FixedUpdate(){

		if (inputRange > 0){
			UpdatePosition();
		}
		UpdateEnemyPosition();

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

	private void UpdateEnemyPosition(){

		_enemyCenterpoint = Vector3.zero;

		if (enemiesInRange.Count > 0){
			lowestX = lowestY = largestX = largestY = 0f;
			foreach (EnemyS e in enemiesInRange){
				if (e.transform.position.x < lowestX || lowestX == 0f){
					lowestX = e.transform.position.x;
				}
				if (e.transform.position.x > largestX || largestX == 0f){
					largestX = e.transform.position.x;
				}
				if (e.transform.position.y < lowestY || lowestY == 0f){
					lowestY = e.transform.position.y;
				}
				if (e.transform.position.y > largestX || largestY == 0f){
					largestY = e.transform.position.y;
				}
			}
			_enemyCenterpoint.x = (lowestX+largestX)/2f;
			_enemyCenterpoint.y = (lowestY+largestY)/2f;
		}

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

			if (!otherEnemy.isDead && !otherEnemy.isFriendly){
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

	public bool NoEnemies(){
		return (enemiesInRange.Count <= 0);
	}
}
