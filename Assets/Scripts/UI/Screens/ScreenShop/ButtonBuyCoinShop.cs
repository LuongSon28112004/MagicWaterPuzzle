using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBuyCoinShop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform receiverCoin;
    [SerializeField] private Sprite coinSprite;

    [Header("Config")]
    [SerializeField] private int coinAmount = 10;
    [SerializeField] private float spawnDelay = 0.01f;
    [SerializeField] private float flyDuration = 0.6f;
    [Header("Button")]
    [SerializeField] Button buttonBuy;
    [SerializeField] bool isBuy = false;
    [SerializeField] int CoinPlus = 1500;

    private void Start()
    {
        buttonBuy.onClick.AddListener(BuyCoin);
    }

    public void BuyCoin()
    {
        StartCoroutine(SpawnCoins());
    }

    private IEnumerator SpawnCoins()
    {
        if (isBuy) yield break;
        isBuy = true;
        var ui = UIManager.Instance.GetPopupActive<PopupTab>();
        ui.SetPanelLock(true);
        StartCoroutine(ui.UpdateCoin(CoinPlus));
        for (int i = 0; i < coinAmount; i++)
        {
            CreateCoin();
            yield return new WaitForSeconds(spawnDelay);
        }
        yield return new WaitForSeconds(0.5f);
        isBuy = false;
        ui.SetPanelLock(false);
        AudioManager.Instance.PlayOneShot("ReceiveCoin", 1f);
    }

    private void CreateCoin()
    {
        GameObject coin = new GameObject("CoinUI");
        coin.transform.SetParent(receiverCoin, false);

        Image img = coin.AddComponent<Image>();
        img.sprite = coinSprite;
        img.SetNativeSize();

        // Tạo Canvas riêng cho đồng xu để đè sorting, giúp trail nằm dưới đồng xu (trail thấp hơn coin)
        Canvas coinCanvas = coin.AddComponent<Canvas>();
        coinCanvas.overrideSorting = true;
        coinCanvas.sortingOrder = 101; // Lớn hơn sortingOrder của trail (100)

        // Thêm TrailRenderer để tạo vệt vàng
        TrailRenderer trail = coin.AddComponent<TrailRenderer>();
        trail.emitting = false; // Tắt lúc tản ra, chỉ bật khi bay về target
        trail.time = 0.25f; // Thời gian tồn tại của đuôi
        trail.startWidth = 4f;
        trail.endWidth = 1f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        
        // Màu vàng tươi/chói sáng và đậm rõ (Vàng to)
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.95f, 0f), 0.0f), new GradientColorKey(new Color(1f, 0.75f, 0f), 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.9f, 0.6f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        trail.colorGradient = gradient;
        trail.sortingOrder = 100; // Đảm bảo trail render đè lên UI

        RectTransform rect = coin.GetComponent<RectTransform>();

        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.zero;

        RectTransform target = UIManager.Instance
            .GetPopupActive<PopupTab>()
            .coinTarget;

        Vector2 targetLocalPos = WorldToLocalPoint(
            receiverCoin as RectTransform,
            target.position
        );

        // =========================
        // CONFIG HIỆU ỨNG
        // =========================
        Vector2 scatterOffset = Random.insideUnitCircle * 100f;
        Vector2 midPos = rect.anchoredPosition + scatterOffset;

        float rotateDir = Random.value > 0.5f ? 1f : -1f;
        float rotateAmount = Random.Range(180f, 360f) * rotateDir;

        Sequence seq = DOTween.Sequence();

        // Pop xuất hiện
        seq.Append(rect.DOScale(1.1f, 0.25f).SetEase(Ease.OutBack));
        // seq.Join(rect.DORotate(new Vector3(0, 0, rotateAmount), 0.6f, RotateMode.FastBeyond360));

        // Tản nhẹ
        seq.Join(rect.DOAnchorPos(midPos, 0.25f).SetEase(Ease.OutQuad));

        // Bật trail khi lao về đích
        seq.AppendCallback(() => trail.emitting = true);

        // Bay cong + hút về target
        seq.Append(rect
            .DOAnchorPos(targetLocalPos, flyDuration)
            .SetEase(Ease.InCubic)
        );

        // // Thu nhỏ khi chạm target
        // seq.Join(rect.DOScale(0.3f, flyDuration).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            Destroy(coin);
            AudioManager.Instance.PlayOneShot("Coin", 1f);
        });
    }




    private Vector2 WorldToLocalPoint(RectTransform parent, Vector3 worldPos)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            RectTransformUtility.WorldToScreenPoint(null, worldPos),
            null,
            out localPoint
        );
        return localPoint;
    }

}
