using System.Collections;
using UnityEngine;

public class BlockParticle : MonoBehaviour
{
    [SerializeField] GameObject BigBubble;
    [SerializeField] GameObject Idle_Bubbles_Variant;


    public IEnumerator PlayParticle()
    {
        BigBubble.SetActive(true);
        Idle_Bubbles_Variant.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        BigBubble.SetActive(false);
    }


}
