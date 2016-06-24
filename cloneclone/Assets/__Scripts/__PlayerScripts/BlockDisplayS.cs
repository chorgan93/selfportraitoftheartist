using UnityEngine;
using System.Collections;

public class BlockDisplayS : MonoBehaviour {

	private SpriteRenderer myRenderer;
	private PlayerController myPlayer;

	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<SpriteRenderer>();
		myPlayer = GetComponentInParent<PlayerController>();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (myPlayer.isBlocking && myPlayer.myStats.currentMana > 0){

			myRenderer.enabled = true;
		}
		else{

			myRenderer.enabled = false;

		}
	
	}
}
