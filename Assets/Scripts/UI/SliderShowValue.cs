using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SliderShowValue : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text valueText;
    [SerializeField] bool isInteger = false;

    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        if (isInteger)
        {
            valueText.text = String.Format("{0}", (int)slider.value);
        }
        else
        {
            valueText.text = String.Format("{0:0.00}", slider.value);
        }
    }

    /*public void UpdateText(float value)
    {
        valueText.text = String.Format("{0:0.00}", value);
    }*/
}
