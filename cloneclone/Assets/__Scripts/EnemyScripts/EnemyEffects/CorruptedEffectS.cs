using UnityEngine;
using System.Collections;

public class CorruptedEffectS : MonoBehaviour {

	public AnimObjS[] animObjs;
	public float[] activateRates;
	private float activateCountdown;
	private int currentObj = 0;

	public GameObject[] backingObjs;

	private EnemyS myEnemy;
	private bool _effectActive = false;
	private bool _initialized = false;


	// Use this for initialization
	public void Initialize (EnemyS enemyRef) {
	
		if (!_initialized){
			myEnemy = enemyRef;
		}
		if (myEnemy.isCorrupted){
			currentObj = 0;
			activateCountdown = activateRates[currentObj];
			TurnOnAll();
		}else{
			TurnOffAll();
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
	}
}
