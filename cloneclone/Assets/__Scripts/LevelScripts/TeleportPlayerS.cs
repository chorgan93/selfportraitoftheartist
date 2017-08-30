using UnityEngine;
using System.Collections;

public class TeleportPlayerS : MonoBehaviour {

	public bool activateOnce = false;
	private bool activated = false;
	public GameObject targetPos;

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			if ((activateOnce && !activated) || !activateOnce){
				activated = true;
				other.gameObject.transform.position = targetPos.transform.position;
				other.gameObject.GetComponent<PlayerController>().myBuddy.transform.position = targetPos.transform.position;
				CameraFollowS.F.CutTo(targetPos.transform.position);
			}
		}
	}
}
