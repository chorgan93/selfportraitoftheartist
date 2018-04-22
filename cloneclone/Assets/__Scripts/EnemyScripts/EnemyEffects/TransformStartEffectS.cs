﻿using UnityEngine;
using System.Collections;

public class TransformStartEffectS : MonoBehaviour {

	public AnimObjS[] chargeAnims;
	public AnimObjS[] activateAnims;
	public AnimObjS[] deactivateAnims;

	private float timeBetweenCharges = 0f;
	private float activateTimeDelays = 0.08f;
	private int currentAnim = 0;

	private bool chargingUp = false;

	private PlayerController myControl;
	private EnemyS myEnemyControl;
	public GameObject enemyEcho;

	private float enemyTimeCharges = 0.08f;
	private float enemyActivateDelays = 0.1f;

	// Use this for initialization
	void Start () {
	
		TurnOffChargeAnims(true);
		TurnOffActivateAnims();
		TurnOffDeactivateAnims();
		myControl = GetComponentInParent<PlayerController>();
		if (!myControl){
			myEnemyControl = GetComponentInParent<EnemyS>();
			myEnemyControl.SetTransformStartEffect(this);
			timeBetweenCharges = enemyTimeCharges;
			activateTimeDelays = enemyActivateDelays;
		}else{
			myControl.SetTransformStartEffect(this);
			timeBetweenCharges = myControl.transformRequireHoldTime/(1f*chargeAnims.Length);
			activateTimeDelays = myControl.revertRequireHoldTime;
		}

	}
	
	public void StartCharge(){
		if (!chargingUp){
		StartCoroutine(ChargeUp());
		}
	}
	public void ActivateEffect(bool showCharge = true){
		StartCoroutine(ActivateTransform(showCharge));
	}
	public void DeactivateEffect(){
		StartCoroutine(DeactivateTransform());
	}

	private IEnumerator ChargeUp(){
		chargingUp = true;
		for (int i = 0; i < chargeAnims.Length; i++){
			if (chargingUp){
			chargeAnims[i].ResetAnimation();
			}
			yield return new WaitForSeconds(timeBetweenCharges);
		}
	}
	private IEnumerator ActivateTransform(bool fullEffect = true){
		TurnOffChargeAnims(true);
		TurnOffDeactivateAnims();
		if (enemyEcho){
			if (fullEffect){
			Instantiate(enemyEcho, transform.position,Quaternion.identity);
			}else{
				GameObject turnOffEffect = Instantiate(enemyEcho, transform.position,Quaternion.identity)
					as GameObject;
				turnOffEffect.transform.GetChild(0).gameObject.SetActive(false);
			}
		}
		for (int i = 0; i < activateAnims.Length; i++){
			activateAnims[i].ResetAnimation();
			yield return new WaitForSeconds(activateTimeDelays);
		}
	}
	private IEnumerator DeactivateTransform(){
		TurnOffChargeAnims(true);
		TurnOffActivateAnims();
		for (int i = 0; i < deactivateAnims.Length; i++){
			deactivateAnims[i].ResetAnimation();
			yield return new WaitForSeconds(activateTimeDelays);
		}
	}

	public void TurnOffChargeAnims(bool overrideCharge = false){
		if (chargingUp || overrideCharge){
		chargingUp  = false;
		for (int i = 0; i < chargeAnims.Length; i++){
			chargeAnims[i].gameObject.SetActive(false);
		}
		}

	}
	void TurnOffActivateAnims(){
		for (int i = 0; i < activateAnims.Length; i++){
			activateAnims[i].gameObject.SetActive(false);
		}
	}
	void TurnOffDeactivateAnims(){
		for (int i = 0; i < deactivateAnims.Length; i++){
			deactivateAnims[i].gameObject.SetActive(false);
		}
	}
}
