using UnityEngine;

public class ParticleBombHammerBreakEffect : MonoBehaviour, IControlParticle
{
    [SerializeField] ParticleSystem particleSystems;
    [SerializeField] GameObject TNTPop;
    public void PlayParticle()
    {
        TNTPop.SetActive(true);
        particleSystems.Play();
    }

    public void RestartParticle()
    {

    }

    public void StopParticle()
    {
        TNTPop.SetActive(false);
        particleSystems.Stop();
    }
}
