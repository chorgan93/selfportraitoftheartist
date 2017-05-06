using UnityEngine;
using System.Collections;

public class GrowEffectS : MonoBehaviour {

	public float growRate = 10f;
	public Vector3 growMult = Vector3.one;

	void Update () {

		Vector3 growScale = transform.localScale;
		growScale.x += growRate*Time.deltaTime*growMult.x;
		growScale.y += growRate*Time.deltaTime*growMult.y;
		transform.localScale = growScale;
	
	}
}
