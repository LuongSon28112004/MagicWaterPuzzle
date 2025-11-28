using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using master;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private LevelData levelData;
    [SerializeField] public BoardCtrl boardCtrl;
    [SerializeField] private BoxCollider2D levelBoundsCollider;
    [SerializeField] private bool startPlay = false;

    void Start()
    {
        Bounds bounds = new Bounds();
        bounds.center = levelBoundsCollider.bounds.center;
        bounds.size = levelBoundsCollider.bounds.size;
        CameraManager.Instance.FitCameraToBounds3D(bounds);
        boardCtrl.LoadLevel(levelData);
    }

    public void StartPlay()
    {
        if (!startPlay)
        {
            startPlay = true;
            CustomeEventSystem.Instance.StartPlay();
        }
    }


}