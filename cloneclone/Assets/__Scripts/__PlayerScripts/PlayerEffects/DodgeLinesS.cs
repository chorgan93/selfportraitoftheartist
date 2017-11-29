using UnityEngine;
using System.Collections;

public class DodgeLinesS : MonoBehaviour {

	public AnimObjS[] animLines;
	public float activeTimeMax = 1f;
	private float activeTimeCount;
	private bool useMainCol = true;
	private float startRotate = 0f;
	public float placeOffset = 3f;

	// Use this for initialization
	void Start () {

		transform.parent = null;
		startRotate = transform.rotation.eulerAngles.z;
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		activeTimeCount -= Time.deltaTime;
		if (activeTimeCount <= 0){
			gameObject.SetActive(false);
		}
	}

	public void TriggerEffect(Color mainCol, Color subCol, Vector3 newPos, Vector3 matchVelocity){
		activeTimeCount = activeTimeMax;
		useMainCol = true;
		matchVelocity.z = 0f;
		FaceDirection(matchVelocity);
		for (int i = 0; i < animLines.Length; i++){
			if (useMainCol){
				animLines[i].SetColor(mainCol);
			}else{
				animLines[i].SetColor(subCol);
			}
			animLines[i].ResetAnimation();
			useMainCol = !useMainCol;
		}
		transform.position = newPos+matchVelocity.normalized*placeOffset;
		gameObject.SetActive(true);
	}

	private void FaceDirection(Vector3 direction){

		float rotateZ = 0;

		Vector3 targetDir = direction.normalized;

		if(targetDir.x == 0){
			if (targetDir.y > 0){
				rotateZ = 90;
			}
			else{
				rotateZ = -90;
			}
		}
		else{
			rotateZ = Mathf.Rad2Deg*Mathf.Atan((targetDir.y/targetDir.x));
		}	


		if (targetDir.x < 0){
			rotateZ += 180;
		}



		transform.rotation = Quaternion.Euler(new Vector3(0,0,rotateZ+startRotate));


	}
}
