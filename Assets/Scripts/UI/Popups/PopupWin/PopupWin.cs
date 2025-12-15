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
    [SerializeField] Image PigIcon;
    [Header("Text Coin")]
    [SerializeField] TextMeshProUGUI textCoin;
    [SerializeField] TextMeshProUGUI textCoinPlus;
    [SerializeField] Transform targetCoin;
    private int MAX_COIN = 60;

    // Sequence

    private Sequence popupSequence;

    private void Awake()
    {
        InitCoinGame();
        StartCoroutine(InitPigReceiveCoin());
        SetupInitialState();     // Set trạng thái ban đầu
        AddAnimationWin();       // Gắn hiệu ứng xoay
        AddEventListener();
        AudioManager.Instance.PlayOneShot("Win", 1f);
        UserData.level += 1;
        GameManager.Instance.Level = UserData.level;
        SaveDataManager.Save();
    }

    private void AddAnimationTextCoin()
    {
        StartCoroutine(ShowAnimTextCoin());
    }

    private IEnumerator ShowAnimTextCoin()
    {
        yield return new WaitForSeconds(1.2f);
        textCoinPlus.transform.DOMove(targetCoin.position, 0.5f);
        textCoinPlus.transform.DOScale(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(PlusCoin());
    }

    private IEnumerator PlusCoin()
    {
        int currentCoin = UserData.coin;
        for (int i = 0; i < MAX_COIN; i++)
        {
            currentCoin += 1;
            textCoin.text = currentCoin.ToString();
            yield return new WaitForSeconds(0.015f);
        }
        UserData.coin += 60;
        SaveDataManager.Save();
        yield break;
    }

    private void InitCoinGame()
    {
        textCoin.text = UserData.coin.ToString();
    }

    private IEnumerator InitPigReceiveCoin()
    {
        int amount = 7;
        int completed = 0;

        RectTransform spawnRect = targetSpawner.GetComponent<RectTransform>();
        RectTransform pigRect = targetPig.GetComponent<RectTransform>();
        RectTransform pigIconRect = PigIcon.GetComponent<RectTransform>();

        Vector2 originalPos = pigIconRect.anchoredPosition;

        // Idle trước khi nhận tiền
        Sequence idleTween = DOTween.Sequence();
        idleTween.Append(pigIconRect.DOAnchorPosY(originalPos.y + 15f, 0.35f));
        idleTween.Join(pigIconRect.DOScale(1.1f, 0.35f));
        idleTween.Append(pigIconRect.DOAnchorPosY(originalPos.y - 15f, 0.35f));
        idleTween.Join(pigIconRect.DOScale(1f, 0.35f));
        idleTween.SetLoops(-1, LoopType.Yoyo);

        lastJumpTime = -999f;

        for (int i = 0; i < amount; i++)
        {
            GameObject coin = new GameObject("PigCoinUI", typeof(RectTransform), typeof(Image));
            RectTransform rect = coin.GetComponent<RectTransform>();
            rect.SetParent(targetSpawner, false);

            Image img = coin.GetComponent<Image>();
            img.sprite = Coin;
            img.SetNativeSize();

            rect.position = spawnRect.position;

            float timeFly = 0.7f + UnityEngine.Random.Range(-0.05f, 0.05f);
            float delay = i * 0.1f;
            Vector3 worldTarget = pigRect.position;

            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(delay);

            rect.localScale = Vector3.zero;
            seq.Append(rect.DOScale(0.6f, 0.25f).SetEase(Ease.OutBack));

            seq.Append(rect.DOMove(worldTarget, timeFly).SetEase(Ease.InQuad));
            seq.Join(rect.DORotate(new Vector3(0, 90, 0), timeFly, RotateMode.FastBeyond360));

            seq.OnComplete(() =>
            {
                Destroy(coin);
                completed++;

                // hứng coin ngay lúc coin chạm
                PigJump(pigIconRect, originalPos);

                // Coin cuối → Jump mạnh hơn
                if (completed >= amount)
                {
                    idleTween.Kill();

                    pigIconRect.DOAnchorPosY(originalPos.y + 30f, 0.25f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            pigIconRect.DOAnchorPosY(originalPos.y, 0.25f).SetEase(Ease.InQuad);
                            pigIconRect.DOScale(1f, 0.25f).SetEase(Ease.InOutQuad);
                        });
                }
            });

            yield return new WaitForSeconds(0.05f);
        }
    }

    private float lastJumpTime = -999f;
    private float jumpCooldown = 0.12f;  // tránh spam quá nhanh

    private void PigJump(RectTransform pig, Vector2 originalPos)
    {
        if (Time.time - lastJumpTime < jumpCooldown)
            return;

        lastJumpTime = Time.time;

        Sequence pigSeq = DOTween.Sequence();

        pigSeq.Append(
            pig.DOAnchorPosY(originalPos.y + 20f, 0.18f).SetEase(Ease.OutQuad)
        );
        pigSeq.Join(
            pig.DOScale(1.1f, 0.18f).SetEase(Ease.OutSine)
        );

        pigSeq.Append(
            pig.DOAnchorPosY(originalPos.y, 0.18f).SetEase(Ease.InQuad)
        );
        pigSeq.Join(
            pig.DOScale(1f, 0.18f).SetEase(Ease.InSine)
        );
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
        AddAnimationTextCoin();
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
        int amount = 15;
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
            //float delayShow = i * 0.01f;

            Sequence seq = DOTween.Sequence();

            // Scale lên
            seq.Append(rect.DOScale(1f, 0.25f).SetDelay(delay).SetEase(Ease.OutBack));
            // float x = rect.anchoredPosition.x;
            // float y = rect.anchoredPosition.y - 50f;
            // seq.Append(rect.DOAnchorPos(new Vector2(x, y), 0.05f).SetEase(Ease.OutBack));

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
            .DORotate(new Vector3(0, 0, -360), 10f, RotateMode.FastBeyond360)
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
                .DORotate(new Vector3(0, 0, -360), 10f, RotateMode.FastBeyond360)
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
