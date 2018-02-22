using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CorruptedEffectS : MonoBehaviour {

	public AnimObjS[] animObjs;
	public float[] activateRates;
	private List<Vector3> startPos;
	private Vector3 newPos;
	public float posMult = 0.3f;
	private float activateCountdown;
	private int currentObj = 0;

	[Header("Object Assignments")]
	public GameObject[] backingObjs;
	public bool ignoreEnemy = false;
	private EnemyS myEnemy;
	private bool _effectActive = false;
	private bool _initialized = false;

	void Start(){
		if (ignoreEnemy){
			Initialize(null);
		}
	}

	// Use this for initialization
	public void Initialize (EnemyS enemyRef) {
	
		if (!_initialized){
			if (!ignoreEnemy){
			myEnemy = enemyRef;
			}
			startPos = new List<Vector3>();
			for (int i = 0; i < animObjs.Length; i++){
				startPos.Add(animObjs[i].transform.localPosition);
			}
		}
		if (ignoreEnemy){

			currentObj = 0;
			activateCountdown = activateRates[currentObj];


			TurnOnAll();
		}else{
		if (myEnemy.isCorrupted){
			currentObj = 0;
			activateCountdown = activateRates[currentObj];


			TurnOnAll();
		}else{
			TurnOffAll();
		}
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (_effectActive){
			activateCountdown -= Time.deltaTime;
			if (activateCountdown <= 0){
				currentObj++;
				if (currentObj > animObjs.Length-1){
					currentObj = 0;
				}
				activateCountdown = activateRates[currentObj];
				ActivateObj(currentObj);
			}
		}
	}

	void ActivateObj(int objIndex){

		newPos = startPos[objIndex];
		newPos += Random.insideUnitSphere*posMult;
		newPos.z = startPos[objIndex].z;
		animObjs[objIndex].transform.localPosition = newPos;
		animObjs[objIndex].ResetAnimation();
		animObjs[objIndex].gameObject.SetActive(true);
	}

	void TurnOnAll(){
		for (int i = 0; i < animObjs.Length; i++){
			animObjs[i].ResetAnimation();
			animObjs[i].gameObject.SetActive(false);
		}
		for (int i = 0; i < backingObjs.Length; i++){
			backingObjs[i].gameObject.SetActive(true);
		}
		gameObject.SetActive(true);
		_effectActive = true;
		ActivateObj(currentObj);
	}

	void TurnOffAll(){
		for (int i = 0; i < backingObjs.Length; i++){
			backingObjs[i].gameObject.SetActive(false);
		}
		gameObject.SetActive(false);
	}

	public void SendDeathMessage(){
		_effectActive = false;
		TurnOffAll();
	}
}
