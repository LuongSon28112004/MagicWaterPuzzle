
using UnityEngine;
using UnityEngine.UI;

public class BoosterConfig : MonoBehaviour
{
    [SerializeField] private Image Panel;
    [SerializeField] private Image Icon;
    [SerializeField] private Image PanelCount;
    [SerializeField] private Image PanelPlus;
    [SerializeField] Button button;

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


}
