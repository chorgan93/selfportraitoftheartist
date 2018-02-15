using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileAdjustS : MonoBehaviour {

	public int tileAdd = 1;
	private List<TileS> currentTiles = new List<TileS>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Ground"){
			other.GetComponent<TileS>().AdjustAwareness(tileAdd);
			currentTiles.Add(other.GetComponent<TileS>());
		}

	}

	void OnTriggerExit(Collider other){

		if (other.gameObject.tag == "Ground"){
			other.GetComponent<TileS>().AdjustAwareness(-tileAdd);
			currentTiles.Remove(other.GetComponent<TileS>());
		}

	}

	void AdjustList(){
		for (int i = 0; i < currentTiles.Count; i++){
			currentTiles[i].AdjustAwareness(-tileAdd);
		}
	}

	void OnDestroy(){
		AdjustList();
	}
}
