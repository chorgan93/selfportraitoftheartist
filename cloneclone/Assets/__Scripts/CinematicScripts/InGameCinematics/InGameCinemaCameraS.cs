using UnityEngine;
using System.Collections;

public class InGameCinemaCameraS : MonoBehaviour {

	public float moveTime = -1;
	public int myCinemaStep = 0;
	public GameObject newPoi;

	// Use this for initialization
	void Start () {
	
		CameraFollowS.F.SetNewPOI(newPoi);

	}
}
