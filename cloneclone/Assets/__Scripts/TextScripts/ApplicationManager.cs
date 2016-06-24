using UnityEngine;
using System.Collections;

public class ApplicationManager : MonoBehaviour {

	// Use this for initialization
	void Awake () {

		TextSource.Initialize();
		PlayerCollectionS.Initialize();
	
	}
}
