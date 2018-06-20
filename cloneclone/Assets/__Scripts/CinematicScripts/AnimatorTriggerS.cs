using UnityEngine;
using System.Collections;

public class AnimatorTriggerS : MonoBehaviour {

	public Animator targetAnimator;
	public string animationTrigger;

	// Use this for initialization
	void Start () {
	
		targetAnimator.SetTrigger(animationTrigger);
	}

}
