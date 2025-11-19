using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    [SerializeField] private BoardCtrl boardCtrl;

    void Start()
    {
        boardCtrl.LoadLevel(levelData);
    }

   
}