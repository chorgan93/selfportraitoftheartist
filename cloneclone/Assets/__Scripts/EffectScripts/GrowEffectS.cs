using UnityEngine;
using System.Collections;

public class GrowEffectS : MonoBehaviour {

	public float growRate = 10f;
	public Vector3 growMult = Vector3.one;
	public bool zGrow = false;

	public bool useUpdate = false;
	public float updateRate = 0.1f;
	private float updateCountdown;
	private Vector3 growScale = Vector3.zero;

	void Update () {

		if (useUpdate){
			updateCountdown -= Time.deltaTime;
			if (updateCountdown <= 0){
				updateCountdown = updateRate;
				growScale = transform.localScale;
				growScale.x += growRate*Time.deltaTime*growMult.x;
				growScale.y += growRate*Time.deltaTime*growMult.y;
				if (zGrow){
					growScale.z += growRate*Time.deltaTime*growMult.z;
				}
				transform.localScale = growScale;
			}
		}else{
		growScale = transform.localScale;
		growScale.x += growRate*Time.deltaTime*growMult.x;
		growScale.y += growRate*Time.deltaTime*growMult.y;
		if (zGrow){
			growScale.z += growRate*Time.deltaTime*growMult.z;
		}
		transform.localScale = growScale;
		}
	
	}
}
