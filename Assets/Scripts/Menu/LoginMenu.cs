using UnityEngine;
using UnityEngine.EventSystems;

public class LoginMenu : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;

    Settings setter;
    string nameP;

    void Start()
    {
        //inputField.ActivateInputField();
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        setter = Settings.singleton;
    }

    public void SetProfileName(string name)
    {
        nameP = name;
    }

    public void SetProfileNameAndSettings()
    {
        Debug.Log("name = " + nameP);
        if (nameP != null && nameP.Trim() != "")
        {
            setter.playerName = nameP;
            setter.WriteValues();
            MenuNavigation.singleton.MainMenu();
        }
    }
}
