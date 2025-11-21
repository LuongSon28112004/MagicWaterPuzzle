using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    [SerializeField] private BoardCtrl boardCtrl;
    [SerializeField] private BoxCollider2D levelBoundsCollider;

    void Start()
    {
        Bounds bounds = new Bounds();
        bounds.center = levelBoundsCollider.bounds.center;
        bounds.size = levelBoundsCollider.bounds.size;
        CameraManager.Instance.FitCameraToBounds3D(bounds);
        boardCtrl.LoadLevel(levelData);
    }


}