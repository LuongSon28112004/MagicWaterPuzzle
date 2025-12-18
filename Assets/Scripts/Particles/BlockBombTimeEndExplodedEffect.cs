using UnityEngine;

public class BlockBombTimeEndExplodedEffect : MonoBehaviour, IControlParticle
{
    [SerializeField] private ParticleSystem particleSystems;
    [SerializeField] private GameObject TNT_EXplode;
    public void PlayParticle()
    {
        TNT_EXplode.gameObject.SetActive(true);
        particleSystems.Play();
    }

    public void RestartParticle()
    {

    }

    public void StopParticle()
    {
        TNT_EXplode.gameObject.SetActive(false);
        particleSystems.Stop();
    }
}
