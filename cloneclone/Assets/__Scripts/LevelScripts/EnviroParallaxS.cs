using UnityEngine;
using System.Collections;

public class EnviroParallaxS : MonoBehaviour {

	private Vector3 startPosition;
	private Vector3 currentPosition;
	public Transform targetTransform;
	public SpawnPosManager spawnManager;
	private float targetStartX;
	private float targetStartY;
	public float parallaxMultX = -5f;
	public Vector3 xDirectionMove;
	public float parallaxMultY = -5f;
	public Vector3 yDirectionMove;

	// Use this for initialization
	void Start () {
	
		startPosition = transform.position;
		if (SpawnPosManager.whereToSpawn < spawnManager.spawnPts.Length){
		targetStartX = spawnManager.spawnPts[SpawnPosManager.whereToSpawn].position.x;
			targetStartY = spawnManager.spawnPts[SpawnPosManager.whereToSpawn].position.y;
		}else{
			targetStartX = spawnManager.spawnPts[0].position.x;
			targetStartY = spawnManager.spawnPts[0].position.y;
		}


	}
	
	// Update is called once per frame
	void Update () {
	
		currentPosition = startPosition;
		currentPosition += (targetStartX-targetTransform.position.x)*parallaxMultX*xDirectionMove;
		currentPosition += (targetStartY-targetTransform.position.y)*parallaxMultY*yDirectionMove;
		transform.position = currentPosition;

	}
}
