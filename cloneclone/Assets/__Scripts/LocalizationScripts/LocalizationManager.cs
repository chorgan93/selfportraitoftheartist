using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{

    public static LocalizationManager instance;

    private Dictionary<string, string> localizedTextEN, localizedTextES;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    public TextAsset masterText_EN;
    public TextAsset masterText_ES;

    public LocalizationMenu locMenu;

    public static int currentLanguage = 0; // 0 = English, 1 = Español

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadLocalizedText(); 
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    bool DEBUG_FORCE_MENU = false;
    public void LoadLocalizedText()
    {
#if !UNITY_EDITOR
        DEBUG_FORCE_MENU = false;
#endif
        localizedTextEN = new Dictionary<string, string>();
        localizedTextES = new Dictionary<string, string>();

        // if english
        if (masterText_EN != null)
        {
            string dataAsJson = masterText_EN.text;
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedTextEN.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
        }
        if (masterText_ES != null) {
            string dataAsJson = masterText_ES.text;
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedTextES.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
        }

        isReady = true;

        if (locMenu) {
            StartCoroutine(DetectSavedLanguage());
        }
    }

    IEnumerator DetectSavedLanguage() {
        yield return null;
#if UNITY_SWITCH
        while (NintendoSwitchSaveObjS.singleton == null) {
#if UNITY_EDITOR
            Debug.Log("Switch singleton not ready for load.");
#endif
            yield return null;
        }
#endif
        int savedLanguage = SaveLoadS.SavedLanguage();
#if UNITY_EDITOR
        Debug.Log("Detected saved language: " + savedLanguage);
#endif
        if (SaveLoadS.SavedLanguage() > -1 && !DEBUG_FORCE_MENU)
        {
            locMenu.Complete(savedLanguage);
        }
        else
        {
            locMenu.TurnOn();
        }
    }

    public string GetLocalizedValue(string key, bool displayOnFail = false)
    {
        string result = missingTextString;
        if (key == ""){
            result = "";
        }
        if (currentLanguage == 0)
        {
            if (localizedTextEN.ContainsKey(key))
            {
                result = localizedTextEN[key];
            }
            else if (displayOnFail)
            {
                result = key;
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Could not find key: " + key);
#endif
            }
        }
        else if (currentLanguage == 1) {
            if (localizedTextES.ContainsKey(key))
            {
                result = localizedTextES[key];
            }
            else if (displayOnFail)
            {
                result = key;
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Could not find key: " + key);
#endif
            }
        }

        return result;

    }

    public bool GetIsReady()
    {
        return isReady;
    }

}