using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ListBooster : MonoBehaviour
{
    [Header("ListBooster Components")]
    public Button FreezeButton;
    public Button BombButton;
    public Button HammerButton;

    [Header("ref")]
    [SerializeField] public FreezeBoosterConfig freezeBoosterConfig;
    [SerializeField] public HammerBoosterConfig hammerBoosterConfig;
    [SerializeField] public BombBoosterConfig bombBoosterConfig;



    public void ActiveAllBooster()
    {
        freezeBoosterConfig.Active();
        bombBoosterConfig.Active();
        hammerBoosterConfig.Active();
    }

    public void InActiveAllBooster()
    {
        freezeBoosterConfig.InActive();
        bombBoosterConfig.InActive();
        hammerBoosterConfig.InActive();
    }

    public void ShowBooster()
    {
        FreezeButton.transform.DOScale(Vector3.one, 0.4f);
        BombButton.transform.DOScale(Vector3.one, 0.4f);
        HammerButton.transform.DOScale(Vector3.one, 0.4f);
        ActiveAllBooster();
    }

    public void HideBooster()
    {
        FreezeButton.transform.DOScale(Vector3.zero, 0.4f);
        BombButton.transform.DOScale(Vector3.zero, 0.4f);
        HammerButton.transform.DOScale(Vector3.zero, 0.4f);
        InActiveAllBooster();
    }

    public void InitCountBooster(bool isFreeze, bool isBomb, bool isHammer, List<int> count)
    {
        if (isFreeze)
        {
            freezeBoosterConfig.ActiveCountBooster(count[0]);
        }
        else
        {
            freezeBoosterConfig.ActivePlusBooster();
        }

        if (isBomb)
        {
            bombBoosterConfig.ActiveCountBooster(count[1]);
        }
        else
        {
            bombBoosterConfig.ActivePlusBooster();
        }

        if (isHammer)
        {
            hammerBoosterConfig.ActiveCountBooster(count[2]);
        }
        else
        {
            hammerBoosterConfig.ActivePlusBooster();
        }
    }

    public void UpdateText()
    {
        freezeBoosterConfig.UpdateTextCount(UserData.listBoosterCounters[0].count);
        bombBoosterConfig.UpdateTextCount(UserData.listBoosterCounters[1].count);
        hammerBoosterConfig.UpdateTextCount(UserData.listBoosterCounters[2].count);
    }

}
