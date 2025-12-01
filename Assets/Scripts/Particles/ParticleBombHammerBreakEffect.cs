using UnityEngine;

public class ParticleBombHammerBreakEffect : MonoBehaviour, IControlParticle
{
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] GameObject TNTPop;
    public void PlayParticle()
    {
        TNTPop.SetActive(true);
        particleSystem.Play();
    }

    public void RestartParticle()
    {

    }

    public void StopParticle()
    {
        TNTPop.SetActive(false);
        particleSystem.Stop();
    }
}
