using UnityEngine;

public class PipeClearEffect : MonoBehaviour, IControlParticle
{
    [Header("Effect Components")]
    [SerializeField] ParticleSystem clearEffectParticles;

    public void PlayParticle()
    {
        if (clearEffectParticles != null)
        {
            clearEffectParticles.Play();
        }
    }

    public void RestartParticle()
    {

    }

    public void StopParticle()
    {
        if (clearEffectParticles != null)
        {
            clearEffectParticles.Stop();
        }
    }
}
