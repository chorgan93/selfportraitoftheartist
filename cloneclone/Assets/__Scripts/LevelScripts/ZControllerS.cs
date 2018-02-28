using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZControllerS : MonoBehaviour {

	public static ZControllerS Z;
	private float minZ = -1;
	private float maxZ = 2;

	private List<ZControlTargetS> allTargets;
	private List<ZControlTargetS> currentList;

	Vector3 placePos = Vector3.zero;

	// Use this for initialization
	void Awake () {
	
		Z = this;

		allTargets = new List<ZControlTargetS>();
		currentList = new List<ZControlTargetS>();

	}

	void LateUpdate () {
		CleanList();
		OrderList();
	
	}

	void Update(){
		if (Input.GetKeyDown(KeyCode.K)){
			Debug.Log(currentList.Count);
		}
	}

	private void CleanList(){

		for (int i = 0; i < allTargets.Count; i++){
			if (allTargets[i] == null){
				allTargets.RemoveAt(i);
			}
		}

	}

	private void OrderList(){

		currentList.Clear();

		bool zPlaced = false;
		for (int j = 0; j < allTargets.Count; j++){ 
			zPlaced = false;
			if (currentList.Count > 0){
				for (int i = 0; i < currentList.Count; i++){
					if (currentList[i].GetCurrentY() < allTargets[j].GetCurrentY()){
						if (i == currentList.Count-1 && !zPlaced){
							currentList.Add(allTargets[j]);
							zPlaced = true;
						}
					}else{
						if (!zPlaced){
							currentList.Insert(i, allTargets[j]);
							zPlaced = true;
						}
					}
				}
			}else{
				currentList.Add(allTargets[j]);
			}
		}

		for(int j = 0; j < currentList.Count; j++){
			placePos = currentList[j].transform.position;
			if (currentList.Count > 1){
			placePos.z = (minZ + (maxZ-minZ))*((j*1f)/((currentList.Count-1)*1f));
			}else{
				placePos.z = minZ;
			}
			currentList[j].transform.position = placePos;
		}

	}

	public void AddTarget(ZControlTargetS zTarget){
		if (!allTargets.Contains(zTarget)){
			allTargets.Add(zTarget);
		}
	}
}
