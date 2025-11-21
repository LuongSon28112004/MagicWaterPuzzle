using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLoading : PopupUI
{
    [SerializeField] private Slider sliderLoading;
    [SerializeField] Image Logo;
    [SerializeField] private TextMeshProUGUI textLoading;
    [SerializeField] private bool loadingSuccess = false;

    public bool LoadingSuccess { get => loadingSuccess; }

    private void Start()
    {
        StartCoroutine(LoadingGame());
    }

    private IEnumerator LoadingGame()
    {
        float progress = 0f;
        while (progress < 100f)
        {
            progress += 1f;
            SetProgress(progress);
            if (progress < 70)
            {
                yield return new WaitForSeconds(0.05f);
            }
            else if (progress < 90)
            {
                yield return new WaitForSeconds(0.15f);
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }
        }

        SetProgress(100f);
        yield return new WaitForSeconds(0.5f);
        loadingSuccess = true;
    }


    public void SetProgress(float progress)
    {
        if (sliderLoading != null)
        {
            sliderLoading.value = progress / 100f;
        }

        if (textLoading != null)
        {
            textLoading.text = progress.ToString() + "%";
        }
    }
}
