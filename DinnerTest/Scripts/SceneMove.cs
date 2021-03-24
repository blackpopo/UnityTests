using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneMove : MonoBehaviour
{
        [SerializeField] Button sceneButton;
        
        [SerializeField] Text buttonText;

        [SerializeField] RawImage photoPanel;
        
        [SerializeField] Button decisionButton;
        [SerializeField] Button oneMoreButton;
        [SerializeField] Button captureButton;

        [SerializeField] Image hiddenImage;

        public void Start()
        {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
                hiddenImage.color = Color.magenta;
                sceneButton.image.color = Color.magenta;
                buttonText.text = "食事開始";
        }
        public void ButtonClick()
        {
                if (hiddenImage.color == Color.blue) //食事中
                {
                        hiddenImage.color = Color.cyan;
                        sceneButton.image.color = Color.cyan;
                        buttonText.text = "食事終了";

                }else if (hiddenImage.color == Color.cyan)//　食事終了 
                {
                        hiddenImage.color = Color.magenta;
                        sceneButton.image.color = Color.magenta;
                        buttonText.text = "食事開始";
                        captureButton.gameObject.SetActive(true);
                        photoPanel.gameObject.SetActive(false);
                }
                else if (hiddenImage.color == Color.magenta) // 食事前
                {
                        if (photoPanel.gameObject.activeSelf == true &&
                            decisionButton.gameObject.activeSelf == false &&
                            oneMoreButton.gameObject.activeSelf == false &&
                            captureButton.gameObject.activeSelf == false 
                            ) //撮影されてなければ開始されない
                        {
                                hiddenImage.color = Color.blue;
                                sceneButton.image.color = Color.blue;
                                buttonText.text = "食事中";
                        }
                }
                else
                {
                        Debug.Log("Unexpected State!");
                }

        }
}
