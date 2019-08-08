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
            StartCoroutine(TurnOnAnimBool());
        }
        else
        {
            targetAnimator.SetTrigger(animationTrigger);
        }
	}

    IEnumerator TurnOnAnimBool(){
        targetAnimator.SetBool(animationBool, true);
        yield return null;

        targetAnimator.SetBool(animationBool, false);
    }

}
