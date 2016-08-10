using UnityEngine;
using System.Collections;

public class BleedingS : MonoBehaviour {

	[Header("Blood Properties")]
	public GameObject bloodPrefab;
	public GameObject deathBloodPrefab;
	public GameObject deathBloodPrefabAlt;
	public float maxBloodTime = 0.2f;
	public float bloodYDifference = 3f;
	public float bloodZPos = 4f;
	private int currentSpawn = 0; 

	private float addBloodTime = 0f;

	[Header("Spread Properties")]
	public int bloodPerHit = 5;
	public int bloodOnDeathMult = 2;
	public float distanceMin;
	public float distanceMax;

	public float varianceMin;
	public float varianceMax;

	[Header("Death Properties")]
	public float deathBleedTime = 1f;
	private float deathBleedCountdown;
	public float onDeathSpawnRate = 0.08f;
	public float endDeathSpawnRate = 0.4f;
	private float currentDeathSpawnRate;
	public float onDeathSpawnVariance = 0.3f;
	private bool deathBleeding = false;

	Vector3 spawnPos;

	void Start(){
		deathBleedCountdown = deathBleedTime;
		currentDeathSpawnRate = onDeathSpawnRate;
	}

	void Update(){
		if (deathBleeding){
			deathBleedCountdown -= Time.deltaTime;
			if (deathBleedCountdown <= 0){
				deathBleeding = false;
			}

			currentDeathSpawnRate -= Time.deltaTime;
			if (currentDeathSpawnRate <= 0){
				Vector3 deathSpawnPos = transform.position;
				deathSpawnPos += Random.insideUnitSphere*onDeathSpawnVariance;
				deathSpawnPos.y -= bloodYDifference/2f;
				deathSpawnPos.z = bloodZPos;

				if (deathBloodPrefabAlt){
					if (deathBleedCountdown < deathBleedTime*0.6f){
						Instantiate(deathBloodPrefabAlt, deathSpawnPos, Quaternion.identity);
					}else{
						Instantiate(deathBloodPrefab, deathSpawnPos, Quaternion.identity);
					}
				}else{
				Instantiate(deathBloodPrefab, deathSpawnPos, Quaternion.identity);
				}
				currentDeathSpawnRate = endDeathSpawnRate + (onDeathSpawnRate-endDeathSpawnRate)*(deathBleedCountdown/deathBleedTime);
			}
		}
	}

	public void StartDeath(){
		currentDeathSpawnRate = onDeathSpawnRate;
		deathBleedCountdown = deathBleedTime;
		deathBleeding = true;
	}

	
	public void SpawnBlood  (Vector3 spawnDir, bool bigBlood = false){

		int bloodAmt = bloodPerHit;
		if (bigBlood){
			bloodAmt *= bloodOnDeathMult;
		}

		currentSpawn = 1;
		addBloodTime =  maxBloodTime/(bloodAmt*1f);

		GameObject newBlood;
		Vector3 currentSpawnDir;

		for (int i = 0; i < bloodAmt; i++){

			currentSpawnDir = spawnDir;
			currentSpawnDir += Random.insideUnitSphere*(varianceMin+((varianceMax-varianceMin)*((currentSpawn*1f)/(bloodAmt*1f))));
			currentSpawnDir = currentSpawnDir.normalized;

			spawnPos = transform.position;
			spawnPos += currentSpawnDir*(distanceMin+((distanceMax-distanceMin)*((currentSpawn*1f)/(bloodAmt*1f))));
			spawnPos.z = bloodZPos;
			spawnPos.y -= bloodYDifference;

			newBlood = Instantiate(bloodPrefab, spawnPos, Quaternion.identity) as GameObject;
			newBlood.GetComponent<BloodS>().AddWaitTime(addBloodTime*(currentSpawn*1f-1f));

			currentSpawn++;
		}
	}
}
