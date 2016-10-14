using UnityEngine;
using System.Collections;

public class PlayerShadowS : MonoBehaviour {

	private PlayerController myController;
	private SpriteRenderer myRenderer;

	private Color shadowColor;

	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<SpriteRenderer>();
		myController = GetComponentInParent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {

		if (myRenderer.enabled){
			if (myController.myStats.PlayerIsDead() || !myController.myRenderer.enabled || myController.isWaking){
				myRenderer.enabled = false;
			}else{
				if (shadowColor != myController.myRenderer.color){
					shadowColor = myController.myRenderer.color;
					myRenderer.material.SetColor("_FlashColor", shadowColor);
				}
			}
		}else{
			if (!myController.myStats.PlayerIsDead() && myController.myRenderer.enabled && !myController.isWaking){
				myRenderer.enabled = true;
			}
		}
	}
}
