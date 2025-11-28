using UnityEngine;

public class SoapBubbleEmitterVariant : MonoBehaviour, IControlParticle
{
    [SerializeField] ParticleSystem particleSystem;
    public void PlayParticle()
    {
        particleSystem.Play();
    }

    public void RestartParticle()
    {
    }

    public void StopParticle()
    {
        particleSystem.Stop();
    }
}
