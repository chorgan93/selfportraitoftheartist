using UnityEngine;
using System.Collections;

public class ZoomOnStartS : MonoBehaviour {

	public bool setZoom = true;
	// Use this for initialization
	void Start () {
		CameraFollowS.F.SetDialogueZoomIn(setZoom);
	}

}
