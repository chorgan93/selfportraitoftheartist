using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffOnKillFamiliarS : MonoBehaviour
{


    public GameObject[] onAtKillFamiliar;
    public GameObject[] offAtKillFamiliar;

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
        if (PlayerController.killedFamiliar){
            if (onAtKillFamiliar != null){
                for (int i = 0; i < onAtKillFamiliar.Length; i++){
                    onAtKillFamiliar[i].SetActive(true);
                }
            }
            if (offAtKillFamiliar != null)
            {
                for (int i = 0; i < offAtKillFamiliar.Length; i++)
                {
                    offAtKillFamiliar[i].SetActive(false);
                }
            }
        }
    }
}
