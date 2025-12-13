using UnityEngine;

public class BlockImpactParticle : MonoBehaviour, IControlParticle
{
    [SerializeField] ParticleSystem impact;
    [SerializeField] GameObject impactObj;

    public void PlayParticle()
    {
        impactObj.SetActive(true);
        impact.Play();
    }

    public void RestartParticle()
    {

    }

    public void StopParticle()
    {
        impactObj.SetActive(false);
        impact.Stop();
    }
}
