using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDetectS : MonoBehaviour {

	private List<GameObject> playerList = new List<GameObject>();

	private PlayerController playerReference;

	public PlayerController player { get { return playerReference; } }

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Player"){
			playerList.Add(other.gameObject);

			if (playerReference == null && other.gameObject.GetComponent<PlayerController>() != null){
				playerReference = other.gameObject.GetComponent<PlayerController>();
			}
		}

	}

	void OnTriggerExit(Collider other){
		
		if (other.gameObject.tag == "Player"){
			playerList.Remove(other.gameObject);
		}
		
	}

	public bool PlayerInRange(){

		return (playerList.Count > 0);

	}

	void OnDisable(){
		playerList.Clear();
	}
}
