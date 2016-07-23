using UnityEngine;
using System.Collections;

public class PlayerDashEffectS : MonoBehaviour {

	public GameObject dashShadow;
	private PlayerController myController;

	public float spawnRate = 0.08f;
	private float spawnCountdown = 0f;

	private float endFadeTime = 0.04f;
	private float endFadeCountdown;

	// Use this for initialization
	void Start () {
		myController = GetComponentInParent<PlayerController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (myController.myStats.speedAmt >= 3f){

			if (myController.isDashing && endFadeCountdown <= 0){
				endFadeCountdown = endFadeTime+myController.dashDuration;
			}

			if (endFadeCountdown > 0){
				spawnCountdown -= Time.deltaTime;
				if (endFadeCountdown > (myController.dashDuration+endFadeTime)*0.6f){
					spawnCountdown -= Time.deltaTime*2f;
				}
				if (spawnCountdown <= 0f){
					spawnCountdown = spawnRate;
					SpawnShadow();
				}
				endFadeCountdown -= Time.deltaTime;
			}else{
				spawnCountdown = spawnRate;
			}
		}
	
	}

	void SpawnShadow(){

		Vector3 spawnPos = myController.transform.position;
		spawnPos.z += 1f;

		GameObject newSpawn = Instantiate(dashShadow, spawnPos, Quaternion.identity)
			as GameObject;

		SpriteRenderer newSprite = newSpawn.GetComponent<SpriteRenderer>();
		newSprite.sprite = myController.myRenderer.sprite;
		newSprite.material = myController.myRenderer.material;

		dashShadow.transform.localScale = myController.transform.localScale.x * myController.myRenderer.transform.localScale;

	}
}
