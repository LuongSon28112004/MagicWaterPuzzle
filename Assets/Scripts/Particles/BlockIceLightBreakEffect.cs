using UnityEngine;

public class BlockIceLightBreakEffect : MonoBehaviour, IControlParticle
{
    [Header("Block Ice Break Component")]
    [SerializeField] ParticleSystem particleSystems;
    [SerializeField] GameObject RockCrack;
    public void PlayParticle()
    {
        RockCrack.SetActive(true);
        particleSystems.Play();
    }

    public void RestartParticle()
    {

    }

    public void StopParticle()
    {
        RockCrack.SetActive(false);
        particleSystems.Play();
    }
}
