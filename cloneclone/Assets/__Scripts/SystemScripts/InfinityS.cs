using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfinityS : MonoBehaviour {

	public SpriteRenderer spawnFlash;
	public float fadeRate = 2f;
	public float fadeInRate = 1f;
	public float minFade = 0.2f;
	private bool fadeOut = true;

	public InfinitySpawnS[] possRooms;
	private InfinitySpawnS currentSpawn;

	private bool fadeIn = false;

	private List<InfinitySpawnS> nextCheck = new List<InfinitySpawnS>();

	public BgEffectS background;

	private int difficulty = 0;

	private PlayerController playerReference;
	private Transform cameraTransform;

	// Use this for initialization
	void Start () {

		playerReference = GameObject.Find("Player").GetComponent<PlayerController>();
		cameraTransform = CameraShakeS.C.transform;

		SpawnStage();
	
	}

	void Update(){

		if (fadeOut){
			fadeIn = false;
			Color fadeCol = spawnFlash.color;
			fadeCol.a -= fadeRate*Time.deltaTime;
			if (fadeCol.a <= minFade){
				fadeCol.a = minFade;
				fadeOut = false;
			}
			spawnFlash.color = fadeCol;
		}

		if (fadeIn){
			fadeOut = false;
			Color fadeCol = spawnFlash.color;
			fadeCol.a += fadeInRate*Time.deltaTime;
			if (fadeCol.a >= 1f){
				fadeCol.a = 1f;
				fadeIn = false;
				fadeOut = true;
				SpawnStage();
			}
			spawnFlash.color = fadeCol;
		}

	}


	public void NextStage(){

		difficulty ++;

		fadeIn = true;
		Color fadeCol = spawnFlash.color;
		fadeCol = currentSpawn.flashColor;
		fadeCol.a = spawnFlash.color.a;
		spawnFlash.color = fadeCol;


	}

	private void SpawnStage(){

		if (currentSpawn != null){
			playerReference.transform.parent = null;
			Destroy(currentSpawn.gameObject);
		}

		foreach(InfinitySpawnS spawn in possRooms){
			if (spawn.CheckDifficulty(difficulty)){
				nextCheck.Add(spawn);
			}
		}

		int nextRoom = Mathf.FloorToInt(Random.Range(0, nextCheck.Count));

		GameObject newSpawn = Instantiate(possRooms[nextRoom].gameObject, playerReference.transform.position, Quaternion.identity)
			as GameObject;
		currentSpawn = newSpawn.GetComponent<InfinitySpawnS>();
		currentSpawn.SetInfinity(this);

		nextCheck.Clear();
			
			Vector3 fadePos = playerReference.transform.position;
			fadePos.z = spawnFlash.transform.position.z;
			spawnFlash.transform.position = fadePos;

		playerReference.transform.position = currentSpawn.playerSpawn.position;
		playerReference.myBuddy.transform.position = playerReference.myBuddy._buddyPos.position;
		playerReference.transform.parent = currentSpawn.transform;

		Vector3 camPos = playerReference.transform.position;
		camPos.z = cameraTransform.position.z;
		cameraTransform.position = camPos;

		background.RepositionBg(playerReference.transform.position);


	}
}
