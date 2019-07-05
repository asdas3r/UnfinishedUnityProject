using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMPro.TMP_InputField))]
public class InputCancelSubmitHandle : MonoBehaviour
{
    public bool wasFocused = false;

    TMPro.TMP_InputField inputField;
    string tempString = null;
    
    void Start()
    {
        inputField = GetComponent<TMPro.TMP_InputField>();

        if (!string.IsNullOrEmpty(inputField.text))
        {
            tempString = inputField.text;
        }
    }

    void LateUpdate()
    {
        if (inputField.isFocused)
        {
            wasFocused = true;
        }
        else
        {
            wasFocused = false;
        }
    }

    public void OnEmptyEndEdit()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            tempString = inputField.text;
        }
        else if (string.IsNullOrEmpty(inputField.text) && !string.IsNullOrEmpty(tempString))
        {
            inputField.text = tempString;
        }
    }
}
