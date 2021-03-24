using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class hukidashiText : MonoBehaviour
{
    [SerializeField] TMP_Text _text;

    [SerializeField] Image hukidashi;

    RectTransform imageRect;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    public void Line(String line, float duration=1.5f, float fadeTime=0.3f)
    {
        Debug.Log("Line script is called : "+ line);
        _text.text = line;
        Sequence seq = DOTween.Sequence();
        seq.Append(imageRect.DOScale(new Vector3(0.95f, 0.95f, 0.95f), fadeTime).SetEase(Ease.Linear));
        seq.Append(hukidashi.DOFade(0.0f, duration).SetDelay(duration).SetEase(Ease.Linear));
        imageRect.localScale = new Vector3(0, 0, 0);
        seq.Play().OnComplete(Initialize);
    }

    void Initialize()
    {
        imageRect = hukidashi.rectTransform;
        imageRect.localScale = new Vector3(0, 0, 0);
        hukidashi.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Debug.Log("Hukidashi script is Alive");
    }
}
