using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDetectS : MonoBehaviour {
	
	private PlayerController playerReference;
	
	public float inputRange;
	private EnemyS _closestEnemy;
	public EnemyS closestEnemy { get { return _closestEnemy; } }
	private Transform _closestEnemyTransform;
	public Transform closestEnemyTransform { get { return _closestEnemyTransform; } }
	
	private List<EnemyS> enemiesInRange;
	private List<Transform> enemyTransforms;
	private List<EnemyS> friendliesInRange;
	public List<EnemyS> allEnemiesInRange { get { return enemiesInRange; } }
	private Vector3 _enemyCenterpoint;
	public Vector3 enemyCenterpoint { get { return _enemyCenterpoint; } }
	
	private float lowestX;
	private float lowestY;
	private float largestX;
	private float largestY;

	private int critEnemies = 0;
	private float critEnemyWeight = 0.6f;
	private Vector3 critCenterpoint = Vector3.zero;

	private float lowestCritX;
	private float lowestCritY;
	private float largestCritX;
	private float largestCritY;
	
	private List<EnemyS> parryEnemies = new List<EnemyS>();
	
	public bool faceVector = false;
	private Vector3 currentRotation = Vector3.zero;

	[Header("Testing Properties")]
	public bool debugCollider = false;
	
	void Start(){
		
		
		playerReference = GetComponentInParent<PlayerController>();
		playerReference.SetDetect(this);
		
		enemiesInRange = new List<EnemyS>();
		friendliesInRange = new List<EnemyS>();
		enemyTransforms = new List<Transform>();
		
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
		
		if (playerReference.ShootPosition().x != 0 && playerReference.ShootPosition().y != 0){
			Vector3 updatePos = playerReference.ShootPosition().normalized;
			updatePos*=inputRange;
			updatePos.z = transform.localPosition.z;
			
			transform.localPosition = updatePos;
			
			if (faceVector){
				RotateToDirection(transform.localPosition.normalized);
			}
		}
		
	}
	
	private void UpdateEnemyPosition(){
		
		_enemyCenterpoint = playerReference.transform.position;
		critEnemies = 0;

		if (enemiesInRange.Count > 0){
			lowestX = lowestY = largestX = largestY = 0f;
			lowestCritX = lowestCritY = largestCritX = largestCritY = 0f;
			for (int i = 0; i < enemyTransforms.Count; i++){
				if (enemyTransforms[i].position.x < lowestX || lowestX == 0f){
					lowestX = enemyTransforms[i].position.x;
				}
				if (enemyTransforms[i].position.x > largestX || largestX == 0f){
					largestX = enemyTransforms[i].position.x;
				}
				if (enemyTransforms[i].position.y < lowestY || lowestY == 0f){
					lowestY = enemyTransforms[i].position.y;
				}
				if (enemyTransforms[i].position.y > largestY || largestY == 0f){
					largestY = enemyTransforms[i].position.y;
				}

				if (enemiesInRange[i].isCritical){
					critEnemies++;
					if (enemyTransforms[i].position.x < lowestCritX || lowestCritX == 0f){
						lowestCritX = enemyTransforms[i].position.x;
					}
					if (enemyTransforms[i].position.x > largestCritX || largestCritX == 0f){
						largestCritX = enemyTransforms[i].position.x;
					}
					if (enemyTransforms[i].position.y < lowestCritY || lowestCritY == 0f){
						lowestCritY = enemyTransforms[i].position.y;
					}
					if (enemyTransforms[i].position.y > largestCritY || largestCritY == 0f){
						largestCritY = enemyTransforms[i].position.y;
					}
				}
			}
			_enemyCenterpoint.x = (lowestX+largestX)/2f;
			_enemyCenterpoint.y = (lowestY+largestY)/2f;

			// add crit'd enemies
			if (critEnemies > 0){
				critCenterpoint.x = (lowestCritX+largestCritX)/2f;
				critCenterpoint.y = (lowestCritY+largestCritY)/2f;
				_enemyCenterpoint = (_enemyCenterpoint+critCenterpoint*critEnemyWeight)/(1f + critEnemyWeight);
			}

		}
		
	}
	
	private void CleanEnemyList(){
		
		for (int i = 0; i < enemiesInRange.Count; i++){
			
			if (enemiesInRange[i] == null){
				enemiesInRange.RemoveAt(i);
				enemyTransforms.RemoveAt(i);
			}else{
				if (enemiesInRange[i].isDead || !enemiesInRange[i].gameObject.activeSelf){
					enemiesInRange.RemoveAt(i);
					enemyTransforms.RemoveAt(i);
				}
			}
			
		}

		// check for duplicates
		/*for (int i = enemiesInRange.Count-1; i >= 0; i--){
			for (int j = 0; j < enemiesInRange.Count; j++){
				if (enemiesInRange[i].mySpawner == enemiesInRange[j].mySpawner && i != j){
						enemiesInRange.RemoveAt(i);
					j--;
				}
			}
		}**/


		for (int i = 0; i < friendliesInRange.Count; i++){

			if (friendliesInRange[i] == null){
				friendliesInRange.RemoveAt(i);
			}else{
				if (friendliesInRange[i].isDead || !friendliesInRange[i].gameObject.activeSelf){
					friendliesInRange.RemoveAt(i);
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
			
			for (int i = 0; i < enemiesInRange.Count; i++){
				if (_closestEnemy == null){
					
					_closestEnemy = enemiesInRange[i];
					_closestEnemyTransform = enemyTransforms[i];
					closestEnemyPosition.x = enemyTransforms[i].position.x;
					closestEnemyPosition.y = enemyTransforms[i].position.y;
					
					closestDistance = Vector2.SqrMagnitude(closestEnemyPosition-playerPosition2D());
				}
				else{
					
					otherEnemyPosition.x = enemyTransforms[i].position.x;
					otherEnemyPosition.y = enemyTransforms[i].position.y;
					float newDistance = Vector2.SqrMagnitude(otherEnemyPosition-playerPosition2D());
					
					if (newDistance < closestDistance){
						_closestEnemy = enemiesInRange[i];
						_closestEnemyTransform = enemyTransforms[i];
						closestEnemyPosition.x = enemyTransforms[i].position.x;
						closestEnemyPosition.y = enemyTransforms[i].position.y;
						
						closestDistance = Vector2.SqrMagnitude(closestEnemyPosition-playerPosition2D());
					}
					
				}
			}
		}
		
	}
	
	private Vector2 playerPosition2D(){
		
		return new Vector2(playerReference.transform.position.x, playerReference.transform.position.y);
		
	}
	
	void OnTriggerEnter(Collider other){
		
		if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Destructible"){
			
			EnemyS otherEnemy = other.gameObject.GetComponent<EnemyS>();
			if (!otherEnemy){
				otherEnemy = other.gameObject.GetComponentInParent<EnemyS>();
			}
			
			if (!otherEnemy.isDead && !otherEnemy.isFriendly){
				enemiesInRange.Add(otherEnemy);
				enemyTransforms.Add(other.transform);
				if (debugCollider){
					Debug.Log("added " + otherEnemy.enemyName + " to enemy list! : " + enemiesInRange.Count);
				}
			}
			if (!otherEnemy.isDead && otherEnemy.isFriendly && !hasFriendly(otherEnemy.mySpawner)){
				friendliesInRange.Add(otherEnemy);
			}
			
		}
		
	}
	
	void OnTriggerExit(Collider other){
		
		if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Destructible"){
			
			EnemyS otherEnemy = other.gameObject.GetComponent<EnemyS>();
			if (!otherEnemy){
				otherEnemy = other.gameObject.GetComponentInParent<EnemyS>();
			}

			if (!otherEnemy.isDead && !otherEnemy.isFriendly){
				enemyTransforms.Remove(other.transform);
				enemiesInRange.Remove(otherEnemy);
			}
			if (!otherEnemy.isDead && otherEnemy.isFriendly && hasFriendly(otherEnemy.mySpawner)){
				friendliesInRange.Remove(otherEnemy);
			}
			
		}
		
	}
	
	void RotateToDirection(Vector3 aimDir){
		float rotateZ = 0;
		
		Vector3 targetDir = aimDir.normalized;
		
		if(targetDir.x == 0){
			if (targetDir.y > 0){
				rotateZ = 90;
			}
			else{
				rotateZ = -90;
			}
		}
		else{
			rotateZ = Mathf.Rad2Deg*Mathf.Atan((targetDir.y/targetDir.x));
		}	
		
		
		if (targetDir.x < 0){
			rotateZ += 180;
		}
		currentRotation = transform.rotation.eulerAngles;
		currentRotation.z = rotateZ+90f;
		transform.rotation = Quaternion.Euler(currentRotation);
	}
	
	public bool NoEnemies(){
		return (enemiesInRange.Count <= 0 && friendliesInRange.Count <= 0);
	}
	
	bool parryEnemyInRange(){
		bool parryInRange = false;
		parryEnemies.Clear();
		for (int i = 0; i < enemiesInRange.Count; i++){
			if (enemiesInRange[i].canBeParried && !enemiesInRange[i].isCritical){
				parryInRange = true;
				parryEnemies.Add(enemiesInRange[i]);
			}
		}
		return parryInRange;
	}
	
	public List<EnemyS> EnemyToParry(){
		if (!NoEnemies()){
			if (parryEnemyInRange()){
				return parryEnemies;
			}else{
				return null;
			}
		}else{
			return null;
		}
	}
	
	public EnemyS ReturnClosestAngleEnemy(Vector3 refVector){
		EnemyS closestEnemyToAngle = null;
		float lowestAngle = -9999;
		float currentAngle;
		for (int i = 0; i < allEnemiesInRange.Count; i++){
			currentAngle = Mathf.Abs(Vector3.Angle(refVector, allEnemiesInRange[i].transform.position));
			if (lowestAngle == -9999 || currentAngle <= lowestAngle){
				closestEnemyToAngle = allEnemiesInRange[i];
			}
		}
		return closestEnemyToAngle;
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

	private bool hasFriendly(EnemySpawnerS enemyCheck){
		bool hasEnemy = false;
		if (enemyCheck != null){
		for (int i = 0; i < friendliesInRange.Count; i++){
			if (friendliesInRange[i].mySpawner == enemyCheck){
				hasEnemy = true;
			}
		}
		}
		return hasEnemy;
	}

}
