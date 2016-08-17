using UnityEngine;
using System.Collections;

public class GrowEffectS : MonoBehaviour {

	public float growRate = 10f;

	void Update () {

		Vector3 growScale = transform.localScale;
		growScale.x += growRate*Time.deltaTime;
		growScale.y = growScale.x;
		transform.localScale = growScale;
	
	}
}
