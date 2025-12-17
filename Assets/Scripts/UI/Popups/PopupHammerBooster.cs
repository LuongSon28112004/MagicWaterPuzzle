using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupHammerBooster : PopupUI
{
    [SerializeField] Transform HammerBooster;
    [SerializeField] Transform Tutotial;
    [SerializeField] Transform PanelTut;
    [SerializeField] Image IconHammer;
    [SerializeField] bool isPlaying = false;
    private Tween rotateTween;

    private void Start()
    {
        ShowTutorial();
    }

    private void OnEnable()
    {
        CustomeEventSystem.Instance.UsedBoosterHammerPos += ShowBoosterHammer;
    }

    private void OnDisable()
    {
        CustomeEventSystem.Instance.UsedBoosterHammerPos -= ShowBoosterHammer;
    }

    public void InitComponent()
    {

        Tutotial.gameObject.SetActive(true);
        PanelTut.gameObject.SetActive(true);
        IconHammer.transform.gameObject.SetActive(true);
        Tutotial.transform.localScale = Vector3.one;
        PanelTut.transform.localScale = Vector3.one;
        IconHammer.transform.localScale = Vector3.one;
    }


    private void ShowTutorial()
    {
        InitComponent();
        Tutotial.localScale = Vector3.zero;
        Tutotial.transform.DOScale(Vector3.one, 0.2f);

        RectTransform rt = IconHammer.rectTransform;

        Vector2 originalPos = rt.anchoredPosition;

        // đặt icon lệch sang trái
        rt.anchoredPosition = new Vector2(-originalPos.x + 50f, originalPos.y);

        // xoay từ 0 -> 15 -> 0 liên tục
        rotateTween = rt.DORotate(
            new Vector3(0, 0, -20f),
            0.15f
        ).SetLoops(-1, LoopType.Yoyo)
         .SetEase(Ease.InOutSine);

        // move về vị trí gốc
        rt.DOAnchorPos(originalPos, 0.5f).OnComplete(() =>
        {
            // dừng xoay khi đã tới đúng vị trí
            rotateTween.Kill();
            rt.rotation = Quaternion.identity; // reset về góc 0
        });
    }

    private void HideTutorial()
    {

        PanelTut.gameObject.SetActive(false);
        RectTransform rt = IconHammer.rectTransform;

        Vector2 originalPos = rt.anchoredPosition * -1f + new Vector2(100, 0);
        LevelManager.Instance.BoosterHammerUsed = false;
        // xoay từ 0 -> 15 -> 0 liên tục
        rotateTween = rt.DORotate(
            new Vector3(0, 0, -20f),
            0.15f
        ).SetLoops(-1, LoopType.Yoyo)
         .SetEase(Ease.InOutSine);

        rt.DOAnchorPos(originalPos, 0.5f).OnComplete(() =>
        {
            IconHammer.transform.gameObject.SetActive(false);
            ScreenGamePlay screenGamePlay = UIManager.Instance.GetScreenActive<ScreenGamePlay>();
            screenGamePlay.ShowButton();
        });

    }



    public void ShowBoosterHammer(GameObject block)
    {
        StartCoroutine(PlayHammerBooster(block));
    }


    private IEnumerator PlayHammerBooster(GameObject block)
    {
        if (isPlaying) yield break;
        isPlaying = true;
        GameObject bombBoosterpre = Resources.Load<GameObject>("Particles/Hammer");
        GameObject particlePrefab = Resources.Load<GameObject>("Particles/BlockBombHammerBreakEffect");
        GameObject particle = Instantiate(particlePrefab, block.transform.position, Quaternion.identity);
        HammerBooster = Instantiate(bombBoosterpre, block.transform.position,
                    Quaternion.identity).transform;
        HammerBooster.transform.position += new Vector3(1, -3, -3);
        HammerBooster.transform.Rotate(new Vector3(-15f, -75f, 0));
        AudioManager.Instance.PlayOneShot("HammerHit", 1f);
        HammerBooster.transform.DOScale(new Vector3(0.65f, 0.65f, 0.65f), 0.8f);
        yield return new WaitForSeconds(0.8f);
        CameraShake.Instance.PlayShake(0.3f, 0.3f);
        var effect = particle.GetComponent<ParticleBombHammerBreakEffect>();
        if (effect != null)
            effect.PlayParticle();
        block.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        // break ice
        LevelManager.Instance.boardCtrl.BreakIceBlock();
        yield return new WaitForSeconds(0.1f);
        //reduce water
        StartCoroutine(ReduceWaterPipe(block));
        AudioManager.Instance.PlayOneShot("WaterPOURvar1S1", 1);
        //
        LevelManager.Instance.boardCtrl.BlockInstances.Remove(block.transform);
        StartCoroutine(CheckWin());
        HideTutorial();
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
        if (blockColor == BlockColor.Orange && waterTypeColor == WaterTypeColor.Orange) return true;
        if (blockColor == BlockColor.Darkgreen && waterTypeColor == WaterTypeColor.Darkgreen) return true;
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
}
