using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDetectS : MonoBehaviour {

	public List<GameObject> playerList = new List<GameObject>();
	private List<EnemyS> enemyList = new List<EnemyS>();
	
	private Transform _currentTarget;
	public Transform currentTarget { get { return _currentTarget; } }
	
	private PlayerController playerReference;
	public PlayerController player { get { return playerReference; } }

	private bool keepTrackOfEnemies = false;

	void Start(){

		if (transform.parent.GetComponent<EnemyS>() != null){
			if (transform.parent.GetComponent<EnemyS>().isFriendly){
				keepTrackOfEnemies = true;
			}
		}

	}

	void LateUpdate(){

		if (keepTrackOfEnemies){
			if (enemyList.Count > 0){
				for (int i = 0; i < enemyList.Count; i++){
					if (enemyList[i].isDead){
						enemyList.RemoveAt(i);
					}
				}
			}

		}

		FindTarget();

	}

	private void FindTarget(){

		if (!keepTrackOfEnemies){
			if (playerReference != null){
				if (playerList.Count > 0){
				_currentTarget = playerReference.transform;
				}
				else{
					_currentTarget = null;
				}
			}
		}else{
			if (enemyList.Count > 0){
			_currentTarget = enemyList[0].transform;
			}else{
				_currentTarget=null;
			}
		}

	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Player"){
			playerList.Add(other.gameObject);

			if (playerReference == null && other.gameObject.GetComponent<PlayerController>() != null){
				playerReference = other.gameObject.GetComponent<PlayerController>();
			}
		}

		if (other.gameObject.tag == "Enemy" && keepTrackOfEnemies){
			if (other.gameObject.GetComponent<EnemyS>() != null){
				enemyList.Add(other.gameObject.GetComponent<EnemyS>());
			}
		}

	}

	void OnTriggerExit(Collider other){
		
		if (other.gameObject.tag == "Player"){
			playerList.Remove(other.gameObject);
		}

		if (keepTrackOfEnemies){
			if (other.gameObject.tag == "Enemy" && other.gameObject.GetComponent<EnemyS>()!=null){
				enemyList.Remove(other.gameObject.GetComponent<EnemyS>());
			}
		}
		
	}

	public bool PlayerInRange(){


		return (playerList.Count > 0);

	}

	void OnDisable(){
		playerList.Clear();
	}
}
