using UnityEngine;
using System.Collections;

public class AllowRetryS : MonoBehaviour {

    public int serahFightFix = -1;

	// Use this for initialization
	void Start () {
		RetryFightUI.allowRetry = true;
        if (serahFightFix > -1 && RetryFightUI.fightRef != null){
            RetryFightUI.fightRef.addProgressOnRestart = serahFightFix;
        }
	}

}
