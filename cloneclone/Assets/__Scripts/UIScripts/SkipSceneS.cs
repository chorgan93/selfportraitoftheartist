using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipSceneS : MonoBehaviour
{

    public static SkipSceneS instance;
    public Image myImage;
    public Text skipText;
    public float skipTextDuration = 2f;
    private float skipTextCountdown = 0f;
    private bool showText = false;
    private string skippingSceneString = "Skipping scene...";
    private string cantSkipString = "Cannot skip scene!!";
    public GameObject cantSkipSound;


    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        myImage.enabled = false;
        skipText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (showText)
        {
            if (!myImage.enabled)
            {
                skipTextCountdown -= Time.deltaTime;
                if (skipTextCountdown <= 0)
                {
                    skipText.enabled = false;
                    showText = false;
                }
            }
        }

    }

    public void ShowMessage(bool allowSkip = true){

        if (allowSkip) { skipText.text = skippingSceneString;
            myImage.enabled = true;
        }
        else { skipText.text = cantSkipString; 
            skipTextCountdown = skipTextDuration;
            if (!showText){
                Instantiate(cantSkipSound);
            }
        }
        skipText.enabled = true;
        showText = true;
    }

    public void HideMessage(){
        showText = false;
        myImage.enabled = skipText.enabled = false;
    }
}
