using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ArrowColorDotween : MonoBehaviour
{
    Image image;

    [SerializeField, Range(1, 9)] int StartColorNumber;

    Color[] colors =
    {
        new Color(0.25f, 0.25f, 1.0f),
        new Color(0, 0.5f, 1.0f),
        new Color(0, 1.0f, 0.5f),
        new Color(0.125f, 1.0f, 0.125f),
        new Color(0.5f, 1.0f, 0.0f),
        new Color(1.0f, 0.5f, 0.0f),
        new Color(1.0f, 0.25f, 0.25f),
        new Color(1.0f, 0, 0.5f),
        new Color(0.5f, 0, 1.0f),

    };
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        image.color = colors[StartColorNumber];
        Sequence colorSequence = DOTween.Sequence();
        for (int i = StartColorNumber - 1; i <colors.Length; i++)
        {
            colorSequence.Append(image.DOColor(colors[i], 1.0f));
        }

        for (int i = 0; i <= StartColorNumber; i++) {
            colorSequence.Append(image.DOColor(colors[i], 1.0f));
        }
        colorSequence.SetLoops(-1);
        colorSequence.SetEase(Ease.Linear);
        colorSequence.Play();

    }


    // Update is called once per frame
}
