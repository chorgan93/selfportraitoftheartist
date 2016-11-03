using UnityEngine;
using System.Collections;

public class RandomSpriteS : MonoBehaviour {

	public Sprite[] possSprites;

	// Use this for initialization
	void Start () {
	
		int mySpriteNum = Mathf.RoundToInt(Random.Range(0, possSprites.Length-1));
		GetComponent<SpriteRenderer>().sprite = possSprites[mySpriteNum];

	}
}
