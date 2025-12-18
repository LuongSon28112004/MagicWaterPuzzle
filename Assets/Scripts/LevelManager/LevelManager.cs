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

        // Sort theo số phía sau "Level_"
        var levelDatas = levels
            .OrderBy(l =>
            {
                string name = l.name.Replace("Level_", "");
                return int.Parse(name);
            })
            .ToList();

        LevelData levelData;

        int levelIndex = GameManager.Instance.Level;

        if (levelIndex > levelDatas.Count)
            levelIndex = levelDatas.Count;

        levelData = levelDatas[levelIndex - 1];

        boardCtrl.LoadLevel(levelData);

        if (levelData.IsEvenX || levelData.IsEvenY)
        {
            boardCtrl.transform.position += new Vector3(levelData.IsEvenX == true ? -1f : 0f, levelData.IsEvenY == true ? -1f : 0f, 0);
            CameraManager.Instance.transform.position += new Vector3(levelData.IsEvenX == true ? -1f : 0f, levelData.IsEvenY == true ? -1f : 0f, 0);
        }

        CameraManager.Instance.InitBoxCam(levelData.BoundCam);
    }

    public void StartPlay()
    {
        if (!startPlay)
        {
            startPlay = true;
            CustomeEventSystem.Instance.StartPlay();
            if (GameManager.Instance.Level == 1)
            {
                var screenGamePlay = UIManager.Instance.GetScreen<ScreenGamePlay>();
                screenGamePlay.DeactiveTut();
            }
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

    public GameObject findObjectHasKeyColor(BlockColor color)
    {
        List<Transform> transforms = boardCtrl.GateInstances;
        for (int i = 0; i < transforms.Count; i++)
        {
            WaterPipe gate = transforms[i].GetComponent<WaterPipe>();
            if (gate != null && gate.PipeKeyLock.IsLocked && gate.PipeKeyLock.KeyInfor.ColorKey == color)
            {
                return transforms[i].gameObject;
            }
        }
        return null;
    }


}