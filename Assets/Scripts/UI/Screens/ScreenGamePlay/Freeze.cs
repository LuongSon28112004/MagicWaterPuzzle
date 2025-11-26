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

            // Reset rotation để đảm bảo về đúng góc ban đầu
            rect.rotation = Quaternion.identity;

            // Tạo tween xoay qua lại
            float duration = UnityEngine.Random.Range(2f, 3f);
            rect
                .DORotate(new Vector3(0, 0, 15f), duration)
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
