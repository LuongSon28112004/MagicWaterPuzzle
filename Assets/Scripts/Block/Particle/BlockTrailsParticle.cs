using UnityEngine;

public class BlockTrailsParticle : MonoBehaviour, IControlParticle
{
    // [SerializeField] ParticleSystem sparcle;
    // [SerializeField] ParticleSystem glow;
    [SerializeField] GameObject sparcleObj;
    [SerializeField] GameObject glowObj;
    public void PlayParticle()
    {
        sparcleObj.SetActive(true);
        glowObj.SetActive(true);
    }

    public void RestartParticle()
    {

    }

    public void StopParticle()
    {
        sparcleObj.SetActive(false);
        glowObj.SetActive(false);
    }
}
