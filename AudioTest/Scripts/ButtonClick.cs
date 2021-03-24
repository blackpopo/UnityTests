using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject[] colliders;
    
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
    }

    // Update is called once per frame

    public void ConfigButtonClick()
    {
        
        panel.SetActive(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].SetActive(false);
        }
    }

    public void BuckButtonClick()
    {
        panel.SetActive(false);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].SetActive(true);
        }
    }
    
}
