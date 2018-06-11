using UnityEngine;
using System.Collections;

public class AddTechS : MonoBehaviour {

	public int techNum = -1;

	// Use this for initialization
	void Start () {
	
		if (techNum > -1){
			PlayerInventoryS.I.AddEarnedTech(techNum);
		}

	}

}
