using UnityEngine;
using System.Collections;

public class GrowEffectS : MonoBehaviour {

	public float growRate = 10f;
	public Vector3 growMult = Vector3.one;
	public bool zGrow = false;

	void Update () {

		Vector3 growScale = transform.localScale;
		growScale.x += growRate*Time.deltaTime*growMult.x;
		growScale.y += growRate*Time.deltaTime*growMult.y;
		if (zGrow){
			growScale.z += growRate*Time.deltaTime*growMult.z;
		}
		transform.localScale = growScale;
	
	}
}
