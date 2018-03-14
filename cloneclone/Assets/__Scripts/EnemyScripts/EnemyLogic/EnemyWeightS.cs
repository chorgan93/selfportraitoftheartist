using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWeightS : MonoBehaviour {

	private EnemyS myEnemy;
	private List<EnemyS> enemiesInRange = new List<EnemyS>();


	private PlayerController playerInRange;
	private bool pRange = false;

	public float forceAmt = 1000f;
	private Vector3 forceDir;

	// Use this for initialization
	void Start () {
	
		myEnemy = GetComponentInParent<EnemyS>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!myEnemy.isDead){
			for (int i = 0; i < enemiesInRange.Count; i++){
				if (!enemiesInRange[i].isDead && !enemiesInRange[i].ignorePush){
					forceDir = enemiesInRange[i].transform.position-transform.position;
					enemiesInRange[i].myRigidbody.AddForce(forceDir.normalized*forceAmt*Time.deltaTime, ForceMode.Force);
				}
			}
			if (pRange){
				if (!playerInRange.myStats.PlayerIsDead() && !playerInRange.InAttack() && !playerInRange.InWitchAnimation()){
					forceDir = playerInRange.transform.position-transform.position;
					playerInRange.myRigidbody.AddForce(forceDir.normalized*forceAmt*Time.deltaTime, ForceMode.Force);
				}
			}
		}
	
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Enemy"){
			if (other.gameObject.GetComponent<EnemyS>() != null){

				enemiesInRange.Add(other.gameObject.GetComponent<EnemyS>());
				
			}
		}
		if (other.gameObject.tag == "Player"){
			if (!playerInRange){
				playerInRange = other.gameObject.GetComponent<PlayerController>();
			}
			pRange = true;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Enemy"){
			if (other.gameObject.GetComponent<EnemyS>() != null){

				enemiesInRange.Remove(other.gameObject.GetComponent<EnemyS>());

			}
		}
		if (other.gameObject.tag == "Player"){
			if (!playerInRange){
				playerInRange = other.gameObject.GetComponent<PlayerController>();
			}
			pRange = false;
		}
	}
}
