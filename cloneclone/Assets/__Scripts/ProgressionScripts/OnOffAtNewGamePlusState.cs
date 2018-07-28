using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffAtNewGamePlusState : MonoBehaviour
{

    public TurnOnOffAtProgressionS.RequireMarkedState requireMarkedState;

    public GameObject[] onAtMarkedState;
    public GameObject[] offAtMarkedState;

    public bool activateOnAwake = false;

    // Use this for initialization
    private void Awake()
    {
        if (activateOnAwake)
        {
            CheckMark();
        }
    }
    void Start()
    {
        if (!activateOnAwake){
            CheckMark();
        }
    }

    void CheckMark()
    {
        if ((requireMarkedState == TurnOnOffAtProgressionS.RequireMarkedState.NewGamePlusOnly
            && PlayerController.equippedVirtues.Contains(15))
            || (requireMarkedState == TurnOnOffAtProgressionS.RequireMarkedState.FirstPlaythroughOnly
                 && !PlayerController.equippedVirtues.Contains(15))){
            if (onAtMarkedState != null){
                for (int i = 0; i < onAtMarkedState.Length; i++){
                    onAtMarkedState[i].SetActive(true);
                }
            }
            if (offAtMarkedState != null)
            {
                for (int i = 0; i < offAtMarkedState.Length; i++)
                {
                    offAtMarkedState[i].SetActive(false);
                }
            }
        }
    }
}
