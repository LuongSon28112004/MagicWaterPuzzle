using System;
using System.Collections;
using Coffee.UIExtensions;
using UnityEngine;

public class ShinyEffect : MonoBehaviour
{
    [SerializeField] ShinyEffectForUGUI shinyEffectForUGUI;
    private void Start()
    {
        shinyEffectForUGUI = GetComponent<ShinyEffectForUGUI>();
        StartCoroutine(PlayShinyEffect());
    }
    private IEnumerator PlayShinyEffect()
    {
        while (true)
        {
            shinyEffectForUGUI.Play(2.5f);
            yield return new WaitForSeconds(2.5f);
        }
    }
}
