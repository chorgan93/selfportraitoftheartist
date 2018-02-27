using UnityEngine;
using System.Collections;

public class FakeParallaxS : MonoBehaviour {

	private Vector3 startPosition;
	private Vector3 currentPosition;
	public Transform targetTransform;
	public SpawnPosManager spawnManager;
	private float targetStartX;
	public float parallaxMult = -5f;

	// Use this for initialization
	void Start () {
	
		startPosition = transform.position;
		targetStartX = spawnManager.spawnPts[SpawnPosManager.whereToSpawn].position.x;

		Debug.Log(targetStartX + " " + targetTransform.position);

	}
	
	// Update is called once per frame
	void Update () {
	
		currentPosition = startPosition;
		currentPosition.x += (targetStartX-targetTransform.position.x)*parallaxMult;
		transform.position = currentPosition;

	}
}
