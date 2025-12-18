using UnityEngine;

public class BlockIceBreakEffect : MonoBehaviour, IControlParticle
{
    [Header("Block Ice Break Component")]
    [SerializeField] ParticleSystem particleSystems;
    [SerializeField] GameObject RockBreak;

    public void PlayParticle()
    {
        RockBreak.SetActive(true);
        particleSystems.Play();
    }

    public void RestartParticle()
    {

    }

    public void StopParticle()
    {
        RockBreak.SetActive(false);
        particleSystems.Play();
    }
}
