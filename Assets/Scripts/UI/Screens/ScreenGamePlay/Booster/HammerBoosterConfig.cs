using UnityEngine;

public class HammerBoosterConfig : BoosterConfig
{
    [Header("Hammer Booster Config")]
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
