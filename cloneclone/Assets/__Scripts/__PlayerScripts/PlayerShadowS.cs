using UnityEngine;
using System.Collections;

public class PlayerShadowS : MonoBehaviour {

	private PlayerController myController;
	private SpriteRenderer myRenderer;

	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<SpriteRenderer>();
		myController = GetComponentInParent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {

		if (myRenderer.enabled){
			if (myController.myStats.PlayerIsDead() || !myController.myRenderer.enabled){
				myRenderer.enabled = false;
			}
		}else{
			if (!myController.myStats.PlayerIsDead() && myController.myRenderer.enabled){
				myRenderer.enabled = true;
			}
		}
	}
}
