using System;
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

    private Sequence popupSequence;

    private void Awake()
    {
        SetupInitialState();     // Set trạng thái ban đầu
        AddAnimationWin();       // Gắn hiệu ứng xoay
        AddEventListener();
        AudioManager.Instance.PlayOneShot("Win", 1f);
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
        GameManager.Instance.BackToMenu();
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
