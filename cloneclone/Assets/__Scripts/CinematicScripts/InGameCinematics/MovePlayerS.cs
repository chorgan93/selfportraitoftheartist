using UnityEngine;
using System.Collections;

public class MovePlayerS : MonoBehaviour {

	public PlayerController playerRef;
	public Transform targetTransform;

	public PlayerAnimationFaceS.PlayerFaceState setFace = PlayerAnimationFaceS.PlayerFaceState.noFace;

	// Use this for initialization
	void Start () {
		playerRef.transform.position = targetTransform.position;
		if (setFace != PlayerAnimationFaceS.PlayerFaceState.noFace){
			playerRef.SetFaceDirection(setFace);
		}
	}
	

}
