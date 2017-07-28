using UnityEngine;
using System.Collections;

public class ChangeTrainDirectionS : MonoBehaviour {

	public int newDir = 1;
	public SetTrainStopS[] targetTriggers;

	// Use this for initialization
	void OnEnable () {
		for (int i = 0; i < targetTriggers.Length; i++){
			targetTriggers[i].trainDirection = newDir;
		}
		CameraShakeS.C.MicroShake();
	}

}
