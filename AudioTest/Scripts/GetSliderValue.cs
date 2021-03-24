using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetSliderValue : MonoBehaviour
{
    [SerializeField] Slider slider;

    [SerializeField] Text volume;
    // Start is called before the first frame update

    void Start()
    {
        volume.text = "" + slider.value + "";
    }

    public void ValueChanged()
    {
        volume.text = "" + slider.value + "";
    }
    
}
