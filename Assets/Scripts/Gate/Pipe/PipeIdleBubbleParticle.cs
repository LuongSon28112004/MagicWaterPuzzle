using UnityEngine;

public class PipeIdleBubbleParticle : MonoBehaviour, IControlParticle
{
    [SerializeField] private GameObject IdleBubble;
    [SerializeField] bool isPlayParticle = false;
    public void PlayParticle()
    {
        if (!isPlayParticle)
        {
            isPlayParticle = true;
            IdleBubble.SetActive(true);
        }
    }

    public void StopParticle()
    {
        if (isPlayParticle)
        {
            isPlayParticle = false;
            IdleBubble.SetActive(false);
        }
    }

    public void RestartParticle()
    {

    }
}
