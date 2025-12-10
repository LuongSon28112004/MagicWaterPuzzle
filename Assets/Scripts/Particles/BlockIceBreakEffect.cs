using UnityEngine;

public class BlockIceBreakEffect : MonoBehaviour, IControlParticle
{
    [Header("Block Ice Break Component")]
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] GameObject RockBreak;

    public void PlayParticle()
    {
        RockBreak.SetActive(true);
        particleSystem.Play();
    }

    public void RestartParticle()
    {

    }

    public void StopParticle()
    {
        RockBreak.SetActive(false);
        particleSystem.Play();
    }
}
