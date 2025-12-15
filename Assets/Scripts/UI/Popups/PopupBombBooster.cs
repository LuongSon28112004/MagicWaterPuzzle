using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PopupBombBooster : PopupUI
{
    [SerializeField] private Transform bombBooster;
    private Coroutine playRoutine;

    private void OnEnable()
    {
        playRoutine = StartCoroutine(PlayBombBooster());
    }

    private void OnDisable()
    {
        Cleanup();
    }

    /// <summary>
    /// Hủy Tween, coroutine và object Booster khi popup bị tắt
    /// </summary>
    private void Cleanup()
    {
        // Hủy coroutine
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        // Hủy tween trên booster
        if (bombBooster != null)
            DOTween.Kill(bombBooster);

        // Xóa booster instance
        if (bombBooster != null)
        {
            Destroy(bombBooster.gameObject);
            bombBooster = null;
        }
    }


    public IEnumerator PlayBombBooster()
    {
        GameObject block = LevelManager.Instance.findObjectNearOrigin();
        if (block == null)
            yield break;

        GameObject bombPrefab = Resources.Load<GameObject>("Particles/Bomb");
        bombBooster = Instantiate(bombPrefab, block.transform.position, Quaternion.identity).transform;

        // vị trí bắt đầu
        Vector3 start = new Vector3(0, -20, 0);

        // target
        Vector3 target = block.transform.position + new Vector3(0, -1, -3);

        // tạo điểm giữa cong lên
        Vector3 mid = (start + target) / 2f;
        float value = UnityEngine.Random.Range(-4f, 4f);
        mid.x += value; // nâng đường cong cao lên

        // Waypoint path
        Vector3[] path = new Vector3[] { start, mid, target };

        bombBooster.position = start;

        bombBooster.DOPath(path, 0.7f, PathType.CatmullRom)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayOneShot("Explo", 1f);

                CameraShake.Instance.PlayShake(0.3f, 0.3f);

                GameObject particlePrefab = Resources.Load<GameObject>("Particles/BlockBombTimeEndExplodedEffect");
                GameObject particle = Instantiate(particlePrefab, block.transform.position, Quaternion.identity);

                bombBooster.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
                {
                    var effect = particle.GetComponent<BlockBombTimeEndExplodedEffect>();
                    if (effect != null)
                        effect.PlayParticle();

                    block.SetActive(false);
                    LevelManager.Instance.boardCtrl.BlockInstances.Remove(block.transform);


                    // update ice
                    LevelManager.Instance.boardCtrl.BreakIceBlock();

                    // reduce ice
                    StartCoroutine(ReduceWaterPipe(block));
                    //sound
                    AudioManager.Instance.PlayOneShot("WaterPOURvar1S1", 1);
                    StartCoroutine(CheckWin());
                    StartCoroutine(ShowButtonScreen());
                });
            });

        yield break;
    }

    private IEnumerator ReduceWaterPipe(GameObject block)
    {
        BaseBlock baseBlock = block.GetComponent<BaseBlock>();
        BlockColor blockColor = baseBlock.BlockColorVisual;
        int remainingCapacity = baseBlock.TakeRemainingCapacity();

        //reduce pipe
        List<Transform> gates = LevelManager.Instance.boardCtrl.GateInstances;
        for (int i = 0; i < gates.Count; i++)
        {
            WaterPipe waterPipe = gates[i].GetComponent<WaterPipe>();
            for (int j = 0; j < waterPipe.WaterTypeCounters.Count; j++)
            {
                if (checkTypeColor(blockColor, waterPipe.WaterTypeCounters[j].waterTypeColor))
                {
                    int value = waterPipe.WaterTypeCounters[j].count;
                    int tmp = waterPipe.WaterTypeCounters[j].count;
                    int reduce = remainingCapacity - value;
                    if (reduce < 0)
                    {
                        value -= remainingCapacity;
                        remainingCapacity = 0;
                    }
                    else if (reduce == 0)
                    {
                        value = 0;
                        remainingCapacity = 0;
                    }
                    else
                    {
                        remainingCapacity = remainingCapacity - value;
                        value = 0;
                    }

                    waterPipe.WaterTypeCounters[j].count = value;
                    waterPipe.UpdateListWaterTypeCounter();
                    StartCoroutine(waterPipe.PipeLineCtrl.FillColor(j + 1, tmp - value));
                    if (remainingCapacity == 0)
                    {
                        yield break;
                    }
                }
            }
        }
        yield break;

    }

    public bool checkTypeColor(BlockColor blockColor, WaterTypeColor waterTypeColor)
    {
        if (blockColor == BlockColor.Red && waterTypeColor == WaterTypeColor.Red) return true;
        if (blockColor == BlockColor.Blue && waterTypeColor == WaterTypeColor.Blue) return true;
        if (blockColor == BlockColor.Brown && waterTypeColor == WaterTypeColor.Brown) return true;
        if (blockColor == BlockColor.Green && waterTypeColor == WaterTypeColor.Green) return true;
        if (blockColor == BlockColor.pink && waterTypeColor == WaterTypeColor.pink) return true;
        if (blockColor == BlockColor.purple && waterTypeColor == WaterTypeColor.purple) return true;
        if (blockColor == BlockColor.Turquoise && waterTypeColor == WaterTypeColor.Turquoise) return true;
        if (blockColor == BlockColor.Yellow && waterTypeColor == WaterTypeColor.Yellow) return true;
        return false;
    }

    private IEnumerator CheckWin()
    {
        yield return new WaitForSeconds(0.3f);
        if (LevelManager.Instance.boardCtrl.BlockInstances.Count == 0)
        {
            StartCoroutine(GameManager.Instance.ChangeState(GameState.Win));
        }
    }

    private IEnumerator ShowButtonScreen()
    {
        yield return new WaitForSeconds(0.3f);

        ScreenGamePlay screen = UIManager.Instance.GetScreenActive<ScreenGamePlay>();
        if (screen != null)
            screen.ShowButton();
    }
}
