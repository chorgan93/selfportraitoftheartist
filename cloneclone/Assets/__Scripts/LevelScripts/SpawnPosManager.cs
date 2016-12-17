using UnityEngine;
using System.Collections;

public class SpawnPosManager : MonoBehaviour {

	public static int whereToSpawn = 0;
	public static bool spawningFromDeath = false;
	private GameObject pRef;

	public CheckpointS sceneCheckpoint;
	public Transform[] spawnPts;

	// Use this for initialization
	void Awake () {
		
		if (spawnPts.Length > 0){
			pRef = GameObject.Find("Player");
			pRef.transform.position = spawnPts[whereToSpawn].position;
		}
	
	}

	void Start(){

		if (sceneCheckpoint){
			if (spawningFromDeath){
				sceneCheckpoint.ActivateMusic();
			}
		}

	}
}
