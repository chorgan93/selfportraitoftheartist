using UnityEngine;
using System.Collections;

public class BusS : MonoBehaviour {

	public float yChange = 15f;
	private float startY;
	private Vector3 currentPos;
	private Vector3 startPos;
	public float travelTime = 3f;
	private float currentTime = 0f;
	private bool moving = false;

	public bool leavingBus = false;
	private static bool comingFromBus = false;

	// Use this for initialization
	void Start () {

		if (leavingBus && !comingFromBus){
			gameObject.SetActive(false);
		}else{
		startPos = transform.position;
		startY = startPos.y;
		moving = true;
		}
		comingFromBus = false;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (moving){
			
			currentTime += Time.deltaTime;

			if (currentTime >= travelTime){
				currentTime = travelTime;
				moving = false;
				if (!leavingBus){
					comingFromBus = true;
				}
			}

			currentPos = startPos;
			if (!leavingBus){
			currentPos.y = AnimCurveS.QuadEaseOut(currentTime, startY, yChange, travelTime);
			}else{
				currentPos.y = AnimCurveS.QuadEaseIn(currentTime, startY, yChange, travelTime);
			}
			transform.position = currentPos;
		}
	
	}
}
