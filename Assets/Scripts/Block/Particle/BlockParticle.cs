using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockParticle : MonoBehaviour
{
    [SerializeField] GameObject BigBubble;
    [SerializeField] GameObject Idle_Bubbles_Variant;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] BoxCollider2D boxTriggerCollider;

    private void Awake()
    {
        List<BoxCollider2D> colliders = new List<BoxCollider2D>(GetComponents<BoxCollider2D>());
        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i].isTrigger == true)
            {
                boxTriggerCollider = colliders[i];
            }
            else
            {
                boxCollider = colliders[i];
            }
        }
    }


    public IEnumerator PlayParticle()
    {
        BigBubble.SetActive(true);
        Idle_Bubbles_Variant.SetActive(true);
        yield return new WaitForSeconds(1f);
        BigBubble.SetActive(false);
    }

    public void SetNonClick()
    {
        boxCollider.enabled = false;
    }


}
