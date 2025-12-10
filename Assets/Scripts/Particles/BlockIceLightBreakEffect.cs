using UnityEngine;

public class BlockIceLightBreakEffect : MonoBehaviour, IControlParticle
{
    [Header("Block Ice Break Component")]
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] GameObject RockCrack;
    public void PlayParticle()
    {
        RockCrack.SetActive(true);
        particleSystem.Play();
    }

    public void RestartParticle()
    {

    }

    public void StopParticle()
    {
        RockCrack.SetActive(false);
        particleSystem.Play();
    }
}
