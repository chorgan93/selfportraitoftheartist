using UnityEngine;
using System.Collections;

public class ZoomOnStartS : MonoBehaviour {

	public bool setZoom = true;
	public bool setSlowZoom = false;
	// Use this for initialization
	void Start () {
		CameraFollowS.F.SetZoomIn(setZoom, setSlowZoom);
	}

}
