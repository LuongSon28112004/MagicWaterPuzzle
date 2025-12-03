using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PopupBombBooster : PopupUI
{
    [SerializeField] private Transform bombBooster;
    private Coroutine playRoutine;

    private void OnEnable()
    {
        playRoutine = StartCoroutine(PlayBombBooster());
    }

    private void OnDisable()
    {
        Cleanup();
    }

    /// <summary>
    /// Hủy Tween, coroutine và object Booster khi popup bị tắt
    /// </summary>
    private void Cleanup()
    {
        // Hủy coroutine
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        // Hủy tween trên booster
        if (bombBooster != null)
            DOTween.Kill(bombBooster);

        // Xóa booster instance
        if (bombBooster != null)
        {
            Destroy(bombBooster.gameObject);
            bombBooster = null;
        }
    }


    public IEnumerator PlayBombBooster()
    {
        GameObject block = LevelManager.Instance.findObjectNearOrigin();
        if (block == null)
            yield break;

        GameObject bombPrefab = Resources.Load<GameObject>("Particles/Bomb");
        bombBooster = Instantiate(bombPrefab, block.transform.position, Quaternion.identity).transform;

        // vị trí bắt đầu
        Vector3 start = new Vector3(0, -20, 0);

        // target
        Vector3 target = block.transform.position + new Vector3(0, -1, -3);

        // tạo điểm giữa cong lên
        Vector3 mid = (start + target) / 2f;
        float value = UnityEngine.Random.Range(-4f, 4f);
        mid.x += value; // nâng đường cong cao lên

        // Waypoint path
        Vector3[] path = new Vector3[] { start, mid, target };

        bombBooster.position = start;

        bombBooster.DOPath(path, 0.7f, PathType.CatmullRom)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayOneShot("Explo", 1f);

                CameraShake.Instance.PlayShake(0.3f, 0.3f);

                GameObject particlePrefab = Resources.Load<GameObject>("Particles/BlockBombTimeEndExplodedEffect");
                GameObject particle = Instantiate(particlePrefab, block.transform.position, Quaternion.identity);

                bombBooster.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
                {
                    var effect = particle.GetComponent<BlockBombTimeEndExplodedEffect>();
                    if (effect != null)
                        effect.PlayParticle();

                    block.SetActive(false);
                    LevelManager.Instance.boardCtrl.BlockInstances.Remove(block.transform);

                    StartCoroutine(CheckWin());
                    StartCoroutine(ShowButtonScreen());
                });
            });

        yield break;
    }

    private IEnumerator CheckWin()
    {
        yield return new WaitForSeconds(0.3f);
        if (LevelManager.Instance.boardCtrl.BlockInstances.Count == 0)
        {
             StartCoroutine(GameManager.Instance.ChangeState(GameState.Win));
        }
    }

    private IEnumerator ShowButtonScreen()
    {
        yield return new WaitForSeconds(0.3f);

        ScreenGamePlay screen = UIManager.Instance.GetScreenActive<ScreenGamePlay>();
        if (screen != null)
            screen.ShowButton();
    }
}
