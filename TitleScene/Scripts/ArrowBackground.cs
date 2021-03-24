using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ArrowBackground : MonoBehaviour
{
    float startPosition;
    float endXPosition;
    [SerializeField] float YPosition;
    [SerializeField] float bufferLength;
    [SerializeField] bool isFirst;
    RectTransform rectTrans;
    float initialPosition;
    float initialMoveTime;
    float width;
    float height;
    float difference;
    float moveTime = 10.0f;

    // Update is called once per frame

    void Start()
    {
        rectTrans = GetComponent<RectTransform>();
        width = rectTrans.rect.width;
        if (isFirst)
        {
            initialPosition = -bufferLength;
            initialMoveTime = moveTime / 2.0f;
        }
        else
        {
            initialPosition = -bufferLength - width;
            initialMoveTime = moveTime;
        }

        startPosition = -bufferLength - width;
        endXPosition = width - bufferLength;
        rectTrans.localPosition = new Vector3(initialPosition, YPosition, 0.0f);
        rectTrans.DOLocalMove(new Vector3(endXPosition, YPosition, 0.0f), initialMoveTime)
            .OnComplete(setLooping);
    }

    void setLooping()
    {
        difference = rectTrans.localPosition.x - endXPosition;
        rectTrans.localPosition = new Vector3(startPosition + difference, YPosition, 0.0f);
        rectTrans.DOLocalMove(new Vector3(endXPosition, YPosition, 0.0f), moveTime)
            .SetLoops(-1);
    }
}
