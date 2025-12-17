
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterConfig : MonoBehaviour
{
    [Header("Booster Config")]
    [SerializeField] protected Image Panel;
    [SerializeField] protected Image Icon;
    [SerializeField] protected Image PanelCount;
    [SerializeField] protected Image PanelPlus;
    [SerializeField] GameObject CountBooster;
    [SerializeField] GameObject PlusBooster;
    [SerializeField] TextMeshProUGUI textCountBooster;
    [SerializeField] protected Button button;
    [Header("Hand Booster Config")]
    [SerializeField] protected Image handTut;
    [SerializeField] protected Transform targetTut;
    [SerializeField] Transform Targetnew;
    [SerializeField] Transform TargetOld;

    public void Active()
    {
        SetAlpha(Panel, 1f);
        SetAlpha(Icon, 1f);
        SetAlpha(PanelCount, 1f);
        SetAlpha(PanelPlus, 1f);
        button.interactable = true;
    }

    public void InActive()
    {
        SetAlpha(Panel, 0.2f);
        SetAlpha(Icon, 0.2f);
        SetAlpha(PanelCount, 0.2f);
        SetAlpha(PanelPlus, 0.2f);
        button.interactable = false;
    }


    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
    public void PlayTutorial()
    {
        transform.SetParent(Targetnew, true);
        handTut.gameObject.SetActive(true);
        targetTut.gameObject.SetActive(true);
        Active();
        handTut.rectTransform.DOAnchorPos(targetTut.GetComponent<RectTransform>().anchoredPosition, 2f).SetLoops(-1);
    }

    public void StopTutorial()
    {
        transform.SetParent(TargetOld, true);
        handTut.gameObject.SetActive(false);
        targetTut.gameObject.SetActive(false);
    }


    public void ActiveCountBooster(int count)
    {
        CountBooster.gameObject.SetActive(true);
        PlusBooster.gameObject.SetActive(false);
        UpdateTextCount(count);
    }

    public void ActivePlusBooster()
    {
        PlusBooster.gameObject.SetActive(true);
        CountBooster.gameObject.SetActive(false);
    }

    public void UpdateTextCount(int count)
    {
        textCountBooster.text = count.ToString();
    }


}
