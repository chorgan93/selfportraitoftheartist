using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffAtNewGamePlusState : MonoBehaviour
{

    public TurnOnOffAtProgressionS.RequireMarkedState requireMarkedState;

    public GameObject[] onAtMarkedState;
    public GameObject[] offAtMarkedState;

    public bool activateOnAwake = false;

    public bool requireFamiliar = false;

    // Use this for initialization
    private void Awake()
    {
        if (requireMarkedState == TurnOnOffAtProgressionS.RequireMarkedState.None){
            requireMarkedState = TurnOnOffAtProgressionS.RequireMarkedState.NewGamePlusOnly; // fixing potential mistakes

        }
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
             && PlayerController.equippedVirtues.Contains(15) && (!requireFamiliar || (requireFamiliar && !PlayerController.killedFamiliar)))
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
