using UnityEngine;
using System.Collections;

public class ChangeCameraSizeOnTouch : MonoBehaviour {


		public float newSize = 1f;
	public float changeTime = 1f;
	private bool activated = false;

		void OnTriggerEnter(Collider other){
		if (!activated){
			if (other.gameObject.tag == "Player"){
				CameraFollowS.F.ChangeOrthoSizeMult(newSize, changeTime);
			activated = true;
			}
			}
		}
	}
