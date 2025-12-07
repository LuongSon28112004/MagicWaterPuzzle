using UnityEngine;
using UnityEngine.UI;

public class CanvasAutoScaler : MonoBehaviour
{
    public CanvasScaler scaler;

    void Start()
    {
        float aspect = (float)Screen.width / Screen.height;

        // Debug xem tỷ lệ thực tế
        Debug.Log("Aspect Ratio: " + aspect);

        if (aspect > 0.7)
        {
            // Siêu cao, rất hẹp (iPhone X, 11 Pro, 12 Mini)
            scaler.matchWidthOrHeight = 1f;
        }
        else if (aspect >= 0.5625 && aspect < 0.7)
        {
            scaler.matchWidthOrHeight = 0.5f;
        }
        else
        {
            scaler.matchWidthOrHeight = 0;
        }

    }
}
