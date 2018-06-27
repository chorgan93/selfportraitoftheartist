using UnityEngine;
using System.Collections;

public class SnowHandlerS : MonoBehaviour {

	private SpriteRenderer[] snowflakes;
	private float[] snowLifeTimes;
	private float[] snowDriftSpeeds;
	private float[] driftXDirections;
	[Header("Spawn Properties")]
	public float snowLifeMin = 3f;
	public float snowLifeMax = 5f;
	public float snowSpawnRate = 0.4f;
	private float snowSpawnCountdown = 0f;

	private int currentSnowflake = 0;

	[Header("Transform Properties")]
	public float snowSpawnLocalZ = 2f;
	public Vector3 baseSpawnPos = new Vector3(0f,-5f,0f);
	public float xSpawnRange = 5f;
	public float ySpawnRange = 0.5f;
	private Vector3 spawnPos = Vector3.zero;

	[Header("Effect Properties")]
	public Vector3 driftDirection = new Vector3(-1f,0f,0f);
	public int changeDirCount = 2;
	private int[] changeDirCountdown;
	public float snowDriftSpeed = 10f;
	public float snowSpeedVariance = 1f;
	public float updateRate = 0.12f;
	private float updateCountdown = 0f;
	private Vector3 currentDrift = Vector3.zero;

	// Use this for initialization
	void Start () {
		transform.parent = Camera.main.transform;
		transform.localPosition = Vector3.zero;
		snowflakes = GetComponentsInChildren<SpriteRenderer>();
		snowLifeTimes = new float[snowflakes.Length];
		snowDriftSpeeds = new float[snowflakes.Length];
		driftXDirections = new float[snowflakes.Length];
		changeDirCountdown = new int[snowflakes.Length];
		for (int i = 0; i < snowflakes.Length; i++){
			snowflakes[i].gameObject.SetActive(false);
			snowflakes[i].transform.parent = transform;
			snowLifeTimes[i] = -1f;
			snowDriftSpeeds[i] = 0f;
			driftXDirections[i] = 1f;
			changeDirCountdown[i] = changeDirCount;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		snowSpawnCountdown -= Time.deltaTime;
		if (snowSpawnCountdown <= 0){
			SpawnSnowflake();
		}

		updateCountdown -= Time.deltaTime;
		if (updateCountdown <= 0){
			DriftSnowflakes();
		}

	}

	void SpawnSnowflake(){

		spawnPos = baseSpawnPos;
		spawnPos.x += xSpawnRange*Random.insideUnitCircle.x;
		spawnPos.y += ySpawnRange*Random.insideUnitCircle.y;
		spawnPos.z = snowSpawnLocalZ;

		snowflakes[currentSnowflake].transform.localPosition = spawnPos;
		snowflakes[currentSnowflake].gameObject.SetActive(true);
		snowflakes[currentSnowflake].transform.parent = null;

		snowLifeTimes[currentSnowflake] = Random.Range(snowLifeMin, snowLifeMax);
		snowDriftSpeeds[currentSnowflake] = snowDriftSpeed+Random.insideUnitCircle.x*snowSpeedVariance;

		snowSpawnCountdown = snowSpawnRate;
		currentSnowflake++;
		if (currentSnowflake > snowflakes.Length-1){
			currentSnowflake = 0;
		}
	}

	void DriftSnowflakes(){
		updateCountdown = updateRate;
		for (int i = 0; i < snowflakes.Length; i++){
			if (!snowflakes[i].gameObject.activeSelf){
				continue;
			}
			snowLifeTimes[i] -= updateRate;
			if (snowLifeTimes[i] <= 0){
				snowflakes[i].gameObject.SetActive(false);
				snowflakes[i].transform.parent = transform;
			}else{
				currentDrift = driftDirection;
				if (changeDirCountdown[i] == 1){
					currentDrift.x = 0;
				}else{
					currentDrift.x *= driftXDirections[i];
				}
				snowflakes[i].transform.localPosition += currentDrift*snowDriftSpeeds[i]*Time.deltaTime;
				changeDirCountdown[i]--;
				if (changeDirCountdown[i] <= 0){
					driftXDirections[i]*=-1f;
					changeDirCountdown[i] = changeDirCount;
				}
			}
		}
	}
}
