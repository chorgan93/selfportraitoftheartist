using UnityEngine;
using System.Collections;

public class TileS : MonoBehaviour {

	public TileS upTile;
	public TileS leftTile;
	public TileS rightTile;
	public TileS downTile;
	private int currentAwareness = 0;
	public Color[] awarenessLevels;

	private SpriteRenderer myRender;

	// Use this for initialization
	void Start () {
	
		myRender = GetComponent<SpriteRenderer>();
		myRender.color = awarenessLevels[currentAwareness];

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AdjustAwareness(int awareAdjust){
		currentAwareness += awareAdjust;
		if (currentAwareness > awarenessLevels.Length-1){
			currentAwareness = awarenessLevels.Length-1;
		}
		if (currentAwareness < 0){
			currentAwareness = 0;
		}

		myRender.color = awarenessLevels[currentAwareness];
	}
}
