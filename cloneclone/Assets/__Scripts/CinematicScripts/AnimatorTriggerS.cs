using UnityEngine;
using System.Collections;

public class AnimatorTriggerS : MonoBehaviour {

	public Animator targetAnimator;
	public string animationTrigger;

    public bool useBool = false;
    public string animationBool;
	// Use this for initialization
	void Start () {

        if (useBool)
        {
            targetAnimator.SetBool(animationBool, true);
        }
        else
        {
            targetAnimator.SetTrigger(animationTrigger);
        }
	}

}
