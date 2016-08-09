using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectSpawnManagerS : MonoBehaviour {

	public GameObject playerDashEffectPrefab;
	private List<GameObject> playerDashes = new List<GameObject>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject SpawnPlayerFade(Vector3 spawnPos){

		GameObject spawnObj;

		if (playerDashes.Count > 0){
			spawnObj = playerDashes[0];
			playerDashes.Remove(spawnObj);
			spawnObj.transform.position = spawnPos;
			spawnObj.SetActive(true);
		}else{
			spawnObj = Instantiate(playerDashEffectPrefab, spawnPos, Quaternion.identity) as GameObject;
		}

		spawnObj.transform.parent = null;
		spawnObj.GetComponent<FadeSpriteObjectS>().SetManager(this, 1);

		return spawnObj;
	}

	public void Despawn(GameObject target, int spawnCode){

		target.SetActive(false);
		target.transform.parent = transform;

		if (spawnCode == 1){
			playerDashes.Add(target);
		}

	}
}
