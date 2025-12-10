using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using master;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private List<LevelData> levelDatas;
    [SerializeField] public BoardCtrl boardCtrl;
    [SerializeField] private bool startPlay = false;
    [SerializeField] private bool boosterHammerUsed = false;

    public bool BoosterHammerUsed { get => boosterHammerUsed; set => boosterHammerUsed = value; }

    void Start()
    {
        LoadListLevelSO();
    }

    private void LoadListLevelSO()
    {
        LevelData[] levels = Resources.LoadAll<LevelData>("LevelData");
        List<LevelData> levelDatas = levels.ToList();
        LevelData levelData;
        if (GameManager.Instance.Level > 9)
        {
            levelData = levelDatas[8];
        }
        else
        {
            levelData = levelDatas[GameManager.Instance.Level - 1];
        }
        boardCtrl.LoadLevel(levelData);
        if (levelData.IsEven)
        {
            boardCtrl.transform.position += new Vector3(-1f, -1f, 0);
            CameraManager.Instance.transform.position += new Vector3(-1f, -1f, 0);
        }
        CameraManager.Instance.InitBoxCam(levelData.BoundCam);
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