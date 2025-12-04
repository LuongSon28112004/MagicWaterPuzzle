using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupWin : PopupUI
{
    [SerializeField] TextMeshProUGUI textLevel;
    [SerializeField] TextMeshProUGUI textComplete;
    [SerializeField] GameObject AnimationWin;
    [SerializeField] Image ImageAnimationWin;
    [SerializeField] Button Claim;
    [SerializeField] Button ClaimX2;
    [Header("Ctrl Popup Coin")]
    [SerializeField] GameObject ReceiverCoin;
    [SerializeField] Sprite Coin;
    [SerializeField] Transform target_1;
    [SerializeField] Transform target_2;
    [SerializeField] bool isSuccess = false;
    [Header("Pig receiver Coin")]
    [SerializeField] Transform targetSpawner;
    [SerializeField] Transform targetPig;
    [SerializeField] TextMeshProUGUI textCount;

    private Sequence popupSequence;

    private void Awake()
    {
        StartCoroutine(InitPigReceiveCoin());
        SetupInitialState();     // Set trạng thái ban đầu
        AddAnimationWin();       // Gắn hiệu ứng xoay
        AddEventListener();
        AudioManager.Instance.PlayOneShot("Win", 1f);
        UserData.level += 1;
        GameManager.Instance.Level = UserData.level;
        SaveDataManager.Save();
    }

    private IEnumerator InitPigReceiveCoin()
    {
        int amount = 7;
        RectTransform pigRect = targetPig.GetComponent<RectTransform>();

        for (int i = 0; i < amount; i++)
        {
            GameObject coin = new GameObject("PigCoinUI");
            coin.transform.SetParent(targetSpawner, false);
            coin.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

            Image img = coin.AddComponent<Image>();
            img.sprite = Coin;
            img.SetNativeSize();

            RectTransform rect = coin.GetComponent<RectTransform>();

            // random nhẹ cho đẹp
            // rect.anchoredPosition += new Vector2(UnityEngine.Random.Range(-40f, 40f), 0);

            float fallTime = 0.5f;

            Vector2 targetPos = pigRect.anchoredPosition; // <-- CHỖ QUAN TRỌNG

            Sequence seq = DOTween.Sequence();

            seq.Append(
                rect.DOAnchorPos(targetPos, fallTime)
                    .SetEase(Ease.OutQuad)
            );

            // seq.Join(
            //     rect.DORotate(new Vector3(0, 0, -90), fallTime, RotateMode.FastBeyond360)
            //         .SetEase(Ease.Linear)
            // );

            seq.OnComplete(() =>
            {
                Destroy(coin);
            });

            yield return new WaitForSeconds(0.1f);
        }
    }



    private void OnEnable()
    {
        PlayPopupAnimation();    // Chạy animation khi popup bật lên
    }

    private void AddEventListener()
    {
        Claim.onClick.AddListener(ClaimClick);
    }

    private void ClaimClick()
    {
        ReceiverCoin.gameObject.SetActive(true);
        StartCoroutine(InitCoinReceiver());
    }

    private IEnumerator InitCoinReceiver()
    {
        ReceiverCoin.transform.DOScale(Vector3.one, 0.25f).OnComplete(() =>
        {
            StartCoroutine(ShowCoin());
        });
        yield break;
    }

    private IEnumerator ShowCoin()
    {
        yield return new WaitForSeconds(0.7f);
        int amount = 20;
        int completed = 0;

        float radius = 120f;    // bán kính để tản coin ra đều

        for (int i = 0; i < amount; i++)
        {
            GameObject coin = new GameObject("CoinUI");
            coin.transform.SetParent(ReceiverCoin.transform, false);

            Image img = coin.AddComponent<Image>();
            img.sprite = Coin;
            img.SetNativeSize();

            RectTransform rect = coin.GetComponent<RectTransform>();

            // =============== TẢN ĐỀU GÓC ================
            // float angle = (360f / amount) * i;  // chia đều góc
            // float rad = angle * Mathf.Deg2Rad;

            // // đặt vị trí theo vòng tròn
            // rect.anchoredPosition = new Vector2(
            //     Mathf.Cos(rad) * radius,
            //     Mathf.Sin(rad) * radius
            // );

            // rect.localScale = Vector3.zero;
            rect.anchoredPosition = new Vector2(UnityEngine.Random.Range(-50f, 150f), UnityEngine.Random.Range(-50f, 150f));
            rect.localScale = Vector3.zero;

            float delay = i * 0.03f;
            float delayShow = i * 0.01f;

            Sequence seq = DOTween.Sequence();

            // Scale lên
            seq.Append(rect.DOScale(1f, 0.25f).SetDelay(delay).SetEase(Ease.OutBack));

            // Bay về target_2
            seq.Append(rect.DOMove(target_2.position, 0.2f)
                .SetDelay(delay)
                .SetEase(Ease.InOutSine));

            // Bay về target_1
            seq.Append(rect.DOMove(target_1.position, 0.35f)
                .SetEase(Ease.Linear));

            seq.OnComplete(() =>
            {
                Destroy(coin);
                completed++;
                AudioManager.Instance.PlayOneShot("Coin", 1f);

                if (completed >= amount - 8 && !isSuccess)
                {
                    AudioManager.Instance.PlayOneShot("ReceiveCoin", 1f);
                    isSuccess = true;
                }

                if (completed >= amount)
                {
                    GameManager.Instance.BackToMenu();
                }
            });
        }
    }




    // -----------------------------
    // 1. Set trạng thái ban đầu
    // -----------------------------
    private void SetupInitialState()
    {
        // Text ẩn và scale nhỏ
        SetAlpha(textLevel, 0);
        SetAlpha(textComplete, 0);
        textLevel.transform.localScale = Vector3.zero;
        textComplete.transform.localScale = Vector3.zero;

        // AnimationWin ẩn
        AnimationWin.transform.localScale = Vector3.zero;
        AnimationWin.SetActive(false);

        // Buttons ẩn
        Claim.transform.localScale = Vector3.zero;
        ClaimX2.transform.localScale = Vector3.zero;
    }

    private void SetAlpha(TextMeshProUGUI txt, float a)
    {
        var c = txt.color;
        c.a = a;
        txt.color = c;
    }

    // -----------------------------
    // 2. Hiệu ứng xoay AnimationWin
    // -----------------------------
    private void AddAnimationWin()
    {
        RectTransform rect = ImageAnimationWin.GetComponent<RectTransform>();

        rect
            .DORotate(new Vector3(0, 0, -360), 3f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
        //.Pause();  // Quan trọng: dừng trước, chỉ chạy sau khi object hiện lên
    }

    // -----------------------------
    // 3. Chuỗi animation popup
    // -----------------------------
    private void PlayPopupAnimation()
    {
        popupSequence?.Kill();
        popupSequence = DOTween.Sequence();

        // ================================
        // STEP 1 — TEXT LEVEL (POP BOUNCE)
        // ================================
        popupSequence.Append(textLevel.DOFade(1, 0.15f)); // hiện nhẹ

        popupSequence.Join(
            textLevel.transform.DOScale(1.2f, 0.22f).SetEase(Ease.OutBack) // phóng to
        );
        popupSequence.Append(
            textLevel.transform.DOScale(0.9f, 0.12f) // co 1 chút
        );
        popupSequence.Append(
            textLevel.transform.DOScale(1f, 0.12f) // ổn định
        );

        // ================================
        // STEP 2 — TEXT COMPLETE (POP BOUNCE)
        // ================================
        popupSequence.Append(textComplete.DOFade(1, 0.15f));

        popupSequence.Join(
            textComplete.transform.DOScale(1.2f, 0.22f).SetEase(Ease.OutBack)
        );
        popupSequence.Append(
            textComplete.transform.DOScale(0.9f, 0.12f)
        );
        popupSequence.Append(
            textComplete.transform.DOScale(1f, 0.12f)
        );

        // ================================
        // STEP 3 — ANIMATION WIN
        // ================================
        popupSequence.AppendCallback(() => AnimationWin.SetActive(true));

        popupSequence.Append(
            AnimationWin.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack)
        )
        .OnComplete(() =>
        {
            // bật xoay khi hiện ra
            ImageAnimationWin.transform
                .DORotate(new Vector3(0, 0, -360), 3f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1)
                .Play();
        });

        // ================================
        // STEP 4 — BUTTONS APPEAR
        // ================================
        popupSequence.Append(
            Claim.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack)
        );
        popupSequence.Join(
            ClaimX2.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack)
        );

        popupSequence.Play();
    }

}
