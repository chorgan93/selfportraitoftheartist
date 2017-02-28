using UnityEngine;
using System.Collections;

public class MatchFlashColorS : MonoBehaviour {

	public SpriteRenderer targetSprite;
	private SpriteRenderer mySprite;

	// Use this for initialization
	void Start () {
	
		mySprite = GetComponent<SpriteRenderer>();
		mySprite.material.SetColor("_FlashColor", targetSprite.color);
	}
}
