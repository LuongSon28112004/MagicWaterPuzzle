
using UnityEngine;

public class BombBoosterConfig : BoosterConfig
{
    [Header("Bomb Booster Config")]
    [SerializeField] private Transform lockBooster;
    [SerializeField] private Transform unlockBooster;

    public void LockBooster()
    {
        lockBooster.gameObject.SetActive(true);
        unlockBooster.gameObject.SetActive(false);
        InActive();
    }

    public void UnlockBooster()
    {
        lockBooster.gameObject.SetActive(false);
        unlockBooster.gameObject.SetActive(true);
        Active();
    }
}
