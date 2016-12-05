using UnityEngine;
using System.Collections;

public class PlayerDashEffectS : MonoBehaviour {

	private PlayerController myController;

	public float spawnDistance = 2f;
	private float distanceTraveled;
	private Vector2 prevDashPos;
	private Vector2 currentDashPos;

	public int maxShadows = 4;
	private int currentShadow = 0;

	private bool newDash = true;
	private float dist = 0f;

	private EffectSpawnManagerS spawnManager;


	// Use this for initialization
	void Start () {
		myController = GetComponentInParent<PlayerController>();
		spawnManager = GameObject.Find("EffectsManager").GetComponent<EffectSpawnManagerS>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {


		if (!myController.myStats.PlayerIsDead() && !myController.isBlocking && 
		    (myController.isDashing)){
			//if (currentShadow < maxShadows || !myController.isDashing){
			currentDashPos.x = myController.transform.position.x;
			currentDashPos.y = myController.transform.position.y;
			if (newDash){
				SpawnShadow();
				prevDashPos = currentDashPos;
				newDash = false;
			}else{
				dist = Vector2.Distance(prevDashPos, currentDashPos);
				distanceTraveled += dist;

				if (distanceTraveled >= spawnDistance){
					SpawnShadow();
					myController.SpawnAttackPuff();
					prevDashPos = currentDashPos;
				}

			}
			//}
		}else{
			distanceTraveled = 0;
			currentShadow = 0;
			newDash = true;
		}
	
	}

	void SpawnShadow(){

		Vector3 spawnPos = myController.transform.position;
		spawnPos.z += 1f;

		GameObject newSpawn = spawnManager.SpawnPlayerFade(spawnPos);

		SpriteRenderer newSprite = newSpawn.GetComponent<SpriteRenderer>();
		newSprite.sprite = myController.myRenderer.sprite;

		newSpawn.transform.localScale = myController.transform.localScale.x * myController.myRenderer.transform.localScale;


		distanceTraveled = 0f;
		currentShadow++;

	}
}
