using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDetectS : MonoBehaviour {

	public List<GameObject> playerList = new List<GameObject>();
	private List<EnemyS> enemyList = new List<EnemyS>();
	private List<EnemyS> friendlyEnemyList = new List<EnemyS>();
	
	private Transform _currentTarget;
	public Transform currentTarget { get { return _currentTarget; } }
	
	private PlayerController playerReference;
	public PlayerController player { get { return playerReference; } }

	public string examineString;
	public string examineStringNoController = "";
	public Vector3 examinePos = new Vector3(0, 1f, 0);
	private Vector3 defaultPos = new Vector3(0, 1f, 0);

	float whichTargetChance = 0.5f;
	float targetFlip;

	private float lazyUpdateTimeMin = 3f;
	private float lazyUpdateTimeMax = 7f;
	private float lazyUpdateCountdown;

	private bool keepTrackOfEnemies = false;

	void Start(){

		if (transform.parent != null){
			if (transform.parent.GetComponent<EnemyS>() != null){
				if (transform.parent.GetComponent<EnemyS>().isFriendly){
					keepTrackOfEnemies = true;
				}
			}
		}
		FindTarget();
		lazyUpdateCountdown = Random.Range(lazyUpdateTimeMin,lazyUpdateTimeMax);

	}

	void Update(){
		LazyUpdate();
	}

	void LazyUpdate(){
		lazyUpdateCountdown -= Time.deltaTime;
		if (lazyUpdateCountdown <= 0){
			lazyUpdateCountdown = Random.Range(lazyUpdateTimeMin,lazyUpdateTimeMax);
			FindTarget();
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

		}else{
			if (friendlyEnemyList.Count > 0){
				for (int i = 0; i < friendlyEnemyList.Count; i++){
					if (friendlyEnemyList[i].isDead){
						friendlyEnemyList.RemoveAt(i);
					}
				}
			}
		}

	}

	public void FindTarget(){

		if (!keepTrackOfEnemies){
			if (playerList.Count > 0 && friendlyEnemyList.Count > 0){
				targetFlip = Random.Range(0f,1f);
				if (targetFlip <= whichTargetChance){
					if (playerReference == null){
						playerReference = playerList[0].GetComponent<PlayerController>();
					}
					_currentTarget = playerReference.transform;
				}else{
					if (friendlyEnemyList.Count > 1){
						_currentTarget = friendlyEnemyList[Mathf.RoundToInt(Random.Range(0, friendlyEnemyList.Count-1))].transform;
					}else{
						_currentTarget = friendlyEnemyList[0].transform;
					}
				}
			}
			else if (playerReference != null){
				if (playerList.Count > 0){
				_currentTarget = playerReference.transform;
				}
				else{
					_currentTarget = null;
				}
			}else if (friendlyEnemyList.Count > 0){
				_currentTarget = friendlyEnemyList[0].transform;
			}else{
				_currentTarget = null;
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
			FindTarget();

			if (playerReference == null && other.gameObject.GetComponent<PlayerController>() != null){
				playerReference = other.gameObject.GetComponent<PlayerController>();
			}

			if (playerReference != null && examineString != ""){
				if (playerReference.myControl.ControllerAttached() || examineStringNoController == ""){
					if (examinePos != defaultPos){
						playerReference.SetExamining(true, examinePos, examineString);
					}else{
					playerReference.SetExamining(true, Vector3.zero, examineString);
					}
				}else{
					if (examinePos != defaultPos){
						playerReference.SetExamining(true, examinePos, examineStringNoController);
					}else{
						playerReference.SetExamining(true, Vector3.zero, examineStringNoController);
					}
				}
			}
		}

		if (other.gameObject.tag == "Enemy" && other.gameObject.GetComponent<EnemyS>()!=null){
			EnemyS enemyRef = other.gameObject.GetComponent<EnemyS>();
			if (!enemyRef.isDead){
			if (keepTrackOfEnemies){
				enemyList.Add(enemyRef);
					FindTarget();

			}else{
				if (enemyRef.isFriendly){
						friendlyEnemyList.Add(enemyRef);
						FindTarget();
				}
			}
			}
		}

	}

	void OnTriggerExit(Collider other){
		
		if (other.gameObject.tag == "Player"){
			playerList.Remove(other.gameObject);
			FindTarget();

			if (playerReference != null && examineString != ""){
				playerReference.SetExamining(false, Vector3.zero, "");
			}
		}

		if (other.gameObject.tag == "Enemy" && other.gameObject.GetComponent<EnemyS>()!=null){
			EnemyS enemyRef = other.gameObject.GetComponent<EnemyS>();
			if (keepTrackOfEnemies){
				enemyList.Remove(enemyRef);
				FindTarget();

			}else{
				if (enemyRef.isFriendly){
					friendlyEnemyList.Remove(enemyRef);
					FindTarget();
				}
			}
		}
		
	}

	public bool PlayerInRange(){

		if (keepTrackOfEnemies){
			return (enemyList.Count > 0);
		}else{

		return (playerList.Count > 0);
		}

	}

	void OnDisable(){
		playerList.Clear();
	}
}
