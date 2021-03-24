using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FlowBackground : MonoBehaviour
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
    float jumpPower = 30.0f;
    float moveTime = 30.0f;

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
        rectTrans.DOLocalJump(new Vector3(endXPosition, YPosition, 0.0f), jumpPower, (int) (initialMoveTime / 5), initialMoveTime)
            .OnComplete(setLooping);
    }

    void setLooping()
    {
        difference = rectTrans.localPosition.x - endXPosition;
        rectTrans.localPosition = new Vector3(startPosition + difference, YPosition, 0.0f);
        rectTrans.DOLocalJump(new Vector3(endXPosition, YPosition, 0.0f), jumpPower, (int) (moveTime /5), moveTime)
            .SetLoops(-1);
    }
}
