using UnityEngine;
using System.Collections;

public class SpawnPosManager : MonoBehaviour {

	public static int whereToSpawn = 0;
	public static int tempWhereToSpawn = -1;
	public static bool spawningFromDeath = false;
	public static bool spawningFromTeleport = false;
	private GameObject pRef;

	public CheckpointS sceneCheckpoint;
	public Transform[] spawnPts;

	public AllyFollowS placeAlly;

	// Use this for initialization
	void Awake () {
		
		if (spawnPts.Length > 0){
			pRef = GameObject.Find("Player");
			if (tempWhereToSpawn > -1 && tempWhereToSpawn <= spawnPts.Length-1){
				pRef.transform.position = spawnPts[tempWhereToSpawn].position;
				if (placeAlly){
					placeAlly.PlaceAlly(tempWhereToSpawn);
				}
			}
			else if (whereToSpawn > spawnPts.Length-1){
				pRef.transform.position = spawnPts[0].position;
				if (placeAlly){
					placeAlly.PlaceAlly(0);
				}
			}else{
				pRef.transform.position = spawnPts[whereToSpawn].position;
				if (placeAlly){
					placeAlly.PlaceAlly(whereToSpawn);
				}
			}
			tempWhereToSpawn = -1;
		}
	
	}

	void Start(){

		if (sceneCheckpoint){
			if (sceneCheckpoint.gameObject.activeSelf){
			if (spawningFromDeath || spawningFromTeleport){
				sceneCheckpoint.ActivateMusic();
			}
			spawningFromTeleport = false;
			spawningFromDeath = false;
			GameOverS.reviveScene = Application.loadedLevelName;
			GameOverS.revivePosition = sceneCheckpoint.spawnNum;
			SaveLoadS.OverwriteCurrentSave();
			}
		}

	}

	public void ResetPlayerPos(){
		if (whereToSpawn > spawnPts.Length-1){
			pRef.transform.position = spawnPts[0].position;
			CameraFollowS.F.CutTo(spawnPts[0].position);
		}else{
			pRef.transform.position = spawnPts[whereToSpawn].position;
			CameraFollowS.F.CutTo(spawnPts[whereToSpawn].position);
		}
		pRef.GetComponent<PlayerController>().ResetBuddyPos(); 
	}
}
