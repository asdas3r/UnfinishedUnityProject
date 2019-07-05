using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationText : MonoBehaviour
{
    [SerializeField] string keyString;

    Text textObject;
    TMPro.TMP_Text tmpTextObject;
    LocalizationManager localizationManager;

    void Start()
    {
        textObject = GetComponent<Text>();
        tmpTextObject = GetComponent<TMPro.TMP_Text>();
        localizationManager = LocalizationManager.singleton;
    }

    private void Update()
    {
        if (localizationManager.isReady == true)
        {
            if (textObject)
            {
                textObject.text = localizationManager.GetLocalizedValue(keyString);
            }

            if (tmpTextObject)
            {
                tmpTextObject.text = localizationManager.GetLocalizedValue(keyString);
            }

            //localizationManager.isReady = false;
        }
    }
}
