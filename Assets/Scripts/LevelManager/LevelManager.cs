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

    public GameObject findObjectNearOrigin()
    {
        List<Transform> transforms = boardCtrl.BlockInstances;
        float min = 1000f;
        GameObject resuit = null;
        for (int i = 0; i < transforms.Count; i++)
        {
            float distance = Vector3.Magnitude(transforms[i].position - Vector3.zero);
            if (distance < min)
            {
                min = distance;
                resuit = transforms[i].gameObject;
            }
        }
        return resuit;
    }


}