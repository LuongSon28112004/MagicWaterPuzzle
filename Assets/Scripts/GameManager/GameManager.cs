using UnityEngine;
using System;
using master;
using System.Collections;
using UnityEngine.SceneManagement;

public enum GameState
{
    None,
    Loading,
    Menu,
    GamePlay,
    Pause,
    Win,
    Lose
}

public class GameManager : SingletonDDOL<GameManager>
{
    public GameState gameState = GameState.None;

    public static event Action<GameState> OnGameStateChanged;
    public int Level = 1;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("cf_default_music"))
        {
            PlayerPrefs.SetInt("cf_default_music", 1);
        }
        if (!PlayerPrefs.HasKey("audio_music_setting"))
        {
            PlayerPrefs.SetInt("audio_music_setting", 1);
        }
        PlayerPrefs.Save();

#if UNITY_EDITOR
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
        Debug.Log("Set FPS = 120 in Editor");
#elif UNITY_ANDROID
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Debug.Log("Set FPS = 60 for Android");
#endif

        StartCoroutine(ChangeState(GameState.Loading));
        //Loading data
        SaveDataManager.Load();
        Level = UserData.level;
    }

    public IEnumerator ChangeState(GameState newState)
    {
        if (gameState == newState)
            yield break;

        gameState = newState;
        if (newState != GameState.Pause && newState != GameState.Lose && newState != GameState.Win)
        {
            Time.timeScale = 1;
        }
        OnGameStateChanged?.Invoke(gameState);

        switch (gameState)
        {
            case GameState.Loading:
                UIManager.Instance.ShowPopup<PopupLoading>(null);
                PopupLoading popupLoading = UIManager.Instance.GetPopup<PopupLoading>();
                while (popupLoading != null && !popupLoading.LoadingSuccess)
                {
                    yield return null;
                }
                UIManager.Instance.HideAllPopup();
                AudioManager.Instance.Play("MainGameplayLOOP", 0.3f, true);
                AudioManager.Instance.musicSource.volume = AudioManager.AudioMusicSetting ? 0.3f * AudioManager.Instance.Ratio_Sound : 0;
                UIManager.Instance.ShowScreen<ScreenHome>();
                UIManager.Instance.ShowPopup<PopupTab>(null);
                break;
            case GameState.Menu:
                yield return LoadSceneAndWait("Init", () =>
                {
                    UIManager.Instance.ShowScreen<ScreenHome>();
                    UIManager.Instance.ShowPopup<PopupTab>(null);
                });
                break;
            case GameState.GamePlay:
                //UIManager.Instance.ShowPopup<PopupLoadingGamePlay>(null);
                yield return LoadSceneAndWait("GamePlay", () =>
                {
                    // UIManager.Instance.HideAllPopup();
                    // LevelManager.Instance.Init();
                    UIManager.Instance.ShowScreen<ScreenGamePlay>();
                });
                break;
            case GameState.Win:
                yield return LoadSceneAndWait("PopupWin", () =>
                {

                });
                break;
            case GameState.Lose:
                // UIManager.Instance.ShowPopup<PopupLoseGame>(null);
                // yield return new WaitForSeconds(0.4f);
                // Time.timeScale = 0;
                break;

            case GameState.Pause:
                Time.timeScale = 0;
                break;
        }
    }

    public IEnumerator LoadSceneAndWait(string sceneName, Action onLoaded)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!async.isDone)
            yield return null;

        yield return null; // đợi 1 frame để scene hoàn toàn active
        onLoaded?.Invoke();
    }


    // ---- Các hàm public gọi state ----
    public void StartGame()
    {
        StartCoroutine(ChangeState(GameState.GamePlay));
    }

    public void WinGame()
    {
        StartCoroutine(ChangeState(GameState.Win));
    }

    public void LoseGame()
    {
        StartCoroutine(ChangeState(GameState.Lose));
    }

    public void BackToMenu()
    {
        StartCoroutine(ChangeState(GameState.Menu));
    }

    public void PauseGame()
    {
        StartCoroutine(ChangeState(GameState.Pause));
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        StartCoroutine(ChangeState(GameState.GamePlay));
    }
}
