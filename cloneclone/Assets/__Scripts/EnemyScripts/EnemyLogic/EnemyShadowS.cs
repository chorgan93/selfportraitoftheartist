using UnityEngine;
using System.Collections;

public class EnemyShadowS : MonoBehaviour {

	private EnemyS enemyRef;
	private SpriteRenderer myRender;
	private Color myColor;
	private float fadeRate = 2.4f;

	// Use this for initialization
	void Start () {
	
		enemyRef = GetComponentInParent<EnemyS>();
		myRender = GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {

		if (myRender.enabled){
			if (enemyRef.isDead){
				myColor = myRender.color;
				myColor.a -= fadeRate*Time.deltaTime;
				if (myColor.a <= 0){
					myRender.enabled = false;
				}else{
					myRender.color = myColor;
				}
			}
		}
	
	}
}
