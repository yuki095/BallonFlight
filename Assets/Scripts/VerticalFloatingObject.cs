using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  //DoTween処理を実装するための宣言

public class VerticalFloatingObject : MonoBehaviour
{

    public float moveTime;      //上下の移動にかかる時間
    public float moveRange;     //上下幅

    void Start()
    {
        transform.DOMoveY(transform.position.y - moveRange, moveTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    void Update()
    {
        
    }
}
