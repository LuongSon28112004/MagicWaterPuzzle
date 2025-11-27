using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    [SerializeField] List<GameObject> listIconFreeze;

    private void Start()
    {
        addAnimationIntroIconFreeze();
    }

    private void addAnimationIntroIconFreeze()
    {
        foreach (GameObject icon in listIconFreeze)
        {
            RectTransform rect = icon.GetComponent<RectTransform>();

            // Đặt góc ban đầu về -15 độ
            rect.rotation = Quaternion.Euler(0, 0, -15f);

            // Tween xoay qua lại giữa -15 và 15
            rect
                .DORotate(new Vector3(0, 0, 15f), 2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }



    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
