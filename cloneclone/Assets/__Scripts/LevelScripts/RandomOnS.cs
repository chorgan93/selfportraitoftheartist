using UnityEngine;
using System.Collections;

public class RandomOnS : MonoBehaviour {

	public GameObject[] potentialSpawns;
	public bool unParent = true;

	// Use this for initialization
	void Start () {
	
		int oneToSpawn = Mathf.FloorToInt(Random.Range(0, potentialSpawns.Length));


		if (unParent){
			potentialSpawns[oneToSpawn].transform.parent = null;
		}

		potentialSpawns[oneToSpawn].gameObject.SetActive(true);
		Destroy(gameObject);

	}
}
