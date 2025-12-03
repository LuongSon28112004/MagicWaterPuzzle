using UnityEngine;

public class BlockBombTimeEndExplodedEffect : MonoBehaviour, IControlParticle
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private GameObject TNT_EXplode;
    public void PlayParticle()
    {
        TNT_EXplode.gameObject.SetActive(true);
        particleSystem.Play();
    }

    public void RestartParticle()
    {

    }

    public void StopParticle()
    {
        TNT_EXplode.gameObject.SetActive(false);
        particleSystem.Stop();
    }
}
