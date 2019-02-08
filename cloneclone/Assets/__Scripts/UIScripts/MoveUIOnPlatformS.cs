using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveUIOnPlatformS : MonoBehaviour
{

    private RectTransform myTransform;
    public bool moveOnSwitch;
    public Vector2 movePos;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_SWITCH
        if (moveOnSwitch){
            myTransform = GetComponent<RectTransform>();
            myTransform.anchoredPosition += movePos;
        }
#endif
    }

}
