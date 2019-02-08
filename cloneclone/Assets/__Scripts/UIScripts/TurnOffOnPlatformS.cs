using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffOnPlatformS : MonoBehaviour
{

    public bool turnOffOnSwitch = false;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_SWITCH
        if (turnOffOnSwitch)
        {
            gameObject.SetActive(false);
        }
#endif
    }

}
