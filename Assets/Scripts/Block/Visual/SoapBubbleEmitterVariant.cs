using UnityEngine;

public class SoapBubbleEmitterVariant : MonoBehaviour, IControlParticle
{
    [SerializeField] ParticleSystem particleSystems;
    public void PlayParticle()
    {
        particleSystems.Play();
    }

    public void RestartParticle()
    {
    }

    public void StopParticle()
    {
        particleSystems.Stop();
    }
}
