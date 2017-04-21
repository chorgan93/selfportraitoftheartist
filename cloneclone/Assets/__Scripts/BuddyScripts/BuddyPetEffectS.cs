using UnityEngine;
using System.Collections;

public class BuddyPetEffectS : MonoBehaviour {

	private SpriteRenderer myRender;
	public SpriteRenderer[] particleSprites;
	private int currentParticle = -1;
	private float spawnParticleCount;
	public float spawnParticleRate = 0.12f;
	private Vector3 currentSpawnPos;
	public float spawnXRange = 0.25f;
	private bool activated = false;
	public float heartDriftYSpeed = 0.5f;
	public float heartDriftXSpeed = 0.9f;

	// Use this for initialization
	void Start () {

		myRender = GetComponent<SpriteRenderer>();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (activated){
			spawnParticleCount -= Time.deltaTime;
			if (spawnParticleCount <= 0){
				spawnParticleCount = spawnParticleRate;
				currentParticle++;
				if (currentParticle >= particleSprites.Length){
					activated = false;
				}else{
					spawnParticle();
				}
			}
		}
	
	}

	void spawnParticle(){
		currentSpawnPos = transform.position;
		currentSpawnPos.z -= 1f;
		currentSpawnPos.x += spawnXRange*Random.insideUnitCircle.x;
		particleSprites[currentParticle].transform.position = currentSpawnPos;
		particleSprites[currentParticle].transform.parent = null;
		particleSprites[currentParticle].gameObject.SetActive(true);
	}

	public void FireEffect(Color spriteCol, bool flipX = false){
		if (!myRender){

			myRender = GetComponent<SpriteRenderer>();
		}
		myRender.color = spriteCol;

		FadeSpriteObjectS fadeSpriteRef;
		for (int i =0; i < particleSprites.Length; i++){
			particleSprites[i].color = spriteCol;
			fadeSpriteRef = particleSprites[i].GetComponent<FadeSpriteObjectS>();

			if (!flipX){
				fadeSpriteRef.SetYDrift(heartDriftYSpeed);
			}
			else{
				fadeSpriteRef.SetYDrift(heartDriftYSpeed*-1f);
			}
				
			fadeSpriteRef.SetDrift(heartDriftXSpeed);

			particleSprites[i].gameObject.SetActive(false);
		}

		currentParticle = -1;
		spawnParticleCount = spawnParticleRate;
		activated = true;
	}
}
