using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonPress : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonClickSound);
    }

    public void ButtonClickSound()
    {
        AudioManager.instance.Play("ButtonClick");
    }
}
