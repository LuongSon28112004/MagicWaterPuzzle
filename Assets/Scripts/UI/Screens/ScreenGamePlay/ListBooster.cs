using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListBooster : MonoBehaviour
{
    public Button FreezeButton;
    public Button BombButton;
    public Button HammerButton;

    [SerializeField] private FreezeBoosterConfig freezeBoosterConfig;
    [SerializeField] private HammerBoosterConfig hammerBoosterConfig;
    [SerializeField] private BombBoosterConfig bombBoosterConfig;

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
}
