using UnityEngine;
using System.Collections;

public class ParticleSystemS : MonoBehaviour {

	private ParticleSystem mySystem;

	// Use this for initialization
	void Start () {

		transform.parent = null;
		mySystem = GetComponent<ParticleSystem>();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!mySystem.isPlaying){
			Destroy(gameObject);
		}
	
	}
}
