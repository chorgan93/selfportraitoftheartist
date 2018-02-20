using UnityEngine;
using System.Collections;

public class MatchPlayerColorS : MonoBehaviour {

	public PlayerController playerRef;
	public SpriteRenderer targetSprite;

	// Use this for initialization
	void Awake () {
		targetSprite.color = playerRef.EquippedWeapon().swapColor;
	}
	

}
