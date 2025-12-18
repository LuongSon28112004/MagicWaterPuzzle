using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class KeyLock : MonoBehaviour
{
    [SerializeField] private RectTransform key;
    [SerializeField] private TextMeshProUGUI text;

    private Vector2 keyStartPos;

    public void ShowIntro()
    {
        transform.gameObject.SetActive(true);
        keyStartPos = key.anchoredPosition;
        StartCoroutine(PlayIntro());
    }

    public IEnumerator PlayIntro()
    {
        // reset trạng thái
        key.anchoredPosition = keyStartPos + new Vector2(0, 200);
        key.localScale = Vector3.zero;
        key.localRotation = Quaternion.identity;

        text.alpha = 0;
        text.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        // 1. Key xuất hiện + rơi xuống
        seq.Append(key.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
        seq.Join(key.DOAnchorPos(keyStartPos, 0.5f).SetEase(Ease.OutBounce));

        // 2. Key lắc nhẹ
        seq.Append(key.DORotate(new Vector3(0, 0, 15f), 0.15f)
            .SetLoops(4, LoopType.Yoyo));

        // 3. Text hiện ra
        seq.Append(text.DOFade(1f, 0.3f));
        seq.Join(text.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack));

        // 4. Key nhấn mạnh "mở khóa"
        seq.AppendInterval(0.3f);
        seq.Append(key.DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 0.8f));
        seq.Join(key.DORotate(new Vector3(0, 0, -20f), 0.2f)
            .SetLoops(2, LoopType.Yoyo));

        seq.Play();
        yield return seq.WaitForCompletion();
        yield return new WaitForSeconds(2f);
        CloseTut();
    }

    public void CloseTut()
    {
        transform.gameObject.SetActive(false);
    }
}
