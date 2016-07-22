using UnityEngine;
using System.Collections;

public class ProjectileTrailS : MonoBehaviour {

	private ProjectileS myProjectile;

	public GameObject particleObj;
	public float projScale = 0.4f;

	public float minSpawnRate;
	public float maxSpawnRate;
	private float activeSpawnRate;
	private float spawnCountdown;

	// Use this for initialization
	void Start () {

		myProjectile = GetComponentInParent<ProjectileS>();
		activeSpawnRate = minSpawnRate;
		spawnCountdown = activeSpawnRate;
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (myProjectile.rangeRef > 0){
			SpawnParticle();
		}

	}

	private void SpawnParticle(){

		spawnCountdown -= activeSpawnRate;
		if (spawnCountdown <= 0){
			spawnCountdown = activeSpawnRate;

			Vector3 spawnPos = transform.position;
			spawnPos.x += Random.insideUnitCircle.x*transform.localScale.x*transform.parent.localScale.x;
			spawnPos.y += Random.insideUnitCircle.y*transform.localScale.y*transform.parent.localScale.x;

			GameObject newParticle = Instantiate(particleObj, spawnPos, myProjectile.transform.rotation)
				as GameObject;
			SpriteRenderer newRender = newParticle.GetComponent<SpriteRenderer>();
			newRender.sprite = myProjectile.projRenderer.sprite;
			newRender.color = myProjectile.projRenderer.color;

			newParticle.transform.localScale = projScale*Vector3.one;
		}

	}
}
