using System;
using System.Collections;
using Coffee.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Mode
{
    LoadingGame,
    LoadingLevel
}

public class PopupLoading : PopupUI
{
    [Header("Popup Loading ref")]
    [SerializeField] private Mode mode = Mode.LoadingGame;
    [SerializeField] private Slider sliderLoading;
    [SerializeField] Image Logo;
    [SerializeField] private TextMeshProUGUI textLoadingPercent;
    [SerializeField] private TextMeshProUGUI textLoadingTitle;
    [SerializeField] private bool loadingSuccess = false;
    [Header("Shiny Effect Logo")]
    [SerializeField] ShinyEffectForUGUI shinyEffectForUGUI;

    public bool LoadingSuccess { get => loadingSuccess; set => loadingSuccess = value; }
    public Mode Mode { get => mode; set => mode = value; }

    private void Start()
    {
        if (mode == Mode.LoadingGame)
        {
            StartCoroutine(LoadingGame());
        }
        else if (mode == Mode.LoadingLevel)
        {
            StartCoroutine(LoadingLevel());
        }
        StartCoroutine(ShinyEffectPlay());
    }

    private IEnumerator ShinyEffectPlay()
    {
        while (true)
        {
            shinyEffectForUGUI.Play(1.5f);
            yield return new WaitForSeconds(1.5f);
        }
    }

    private void OnEnable()
    {
        if (mode == Mode.LoadingGame)
        {
            StartCoroutine(LoadingGame());
        }
        else if (mode == Mode.LoadingLevel)
        {
            StartCoroutine(LoadingLevel());
        }
    }


    //Loading Game
    private IEnumerator LoadingGame()
    {
        float progress = 0f;
        while (progress < 100f)
        {
            progress += 1f;
            SetProgress(progress);
            if (progress < 20)
            {
                yield return new WaitForSeconds(0.07f);
            }
            else if (progress < 90)
            {
                yield return new WaitForSeconds(0.02f);
            }
            else
            {
                yield return new WaitForSeconds(0.01f);
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

        if (textLoadingPercent != null)
        {
            textLoadingPercent.text = progress.ToString() + "%";
        }
    }

    //Loading Level
    private IEnumerator LoadingLevel()
    {
        textLoadingTitle.gameObject.SetActive(false);
        textLoadingPercent.gameObject.SetActive(false);
        sliderLoading.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        loadingSuccess = true;
    }
}
