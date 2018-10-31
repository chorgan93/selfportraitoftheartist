using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescentDarknessS : MonoBehaviour {

    public PlayerStatsS playerTarget;
    public DarknessPercentUIS darknessUI;

	// Use this for initialization
	void Awake () {
        playerTarget.SetDescentState(true);
        darknessUI.SetDescentState(true);
	}
}
