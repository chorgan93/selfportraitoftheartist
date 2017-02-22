using UnityEngine;
using System.Collections;

public class EnemyShadowS : MonoBehaviour {

	private EnemyS enemyRef;
	private SpriteRenderer myRender;
	private Color myColor;
	private float fadeRate = 2.4f;

	private Color shadowColor;

	public bool matchAlpha = false;
	public bool overrideColor = false;

	// Use this for initialization
	void Start () {
	
		enemyRef = GetComponentInParent<EnemyS>();
		myRender = GetComponent<SpriteRenderer>();
		if (!overrideColor){
		//myRender.color = enemyRef.bloodColor;
			Color setFade = enemyRef.bloodColor;
			setFade.a = 0.4f;
			myRender.color = setFade;
			myRender.material.SetColor("_FlashColor", enemyRef.bloodColor);
		}

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
			}else{
				if (matchAlpha){
					myRender.color = enemyRef.myRenderer.color;
				}
			}
		}
	
	}

	public void Reinitialize(){
		if (!overrideColor){
			//myRender.color = enemyRef.bloodColor;
			Color setFade = enemyRef.bloodColor;
			setFade.a = 0.4f;
			myRender.color = setFade;
			myRender.material.SetColor("_FlashColor", enemyRef.bloodColor);
		}
		myRender.enabled = true;
	}
}
