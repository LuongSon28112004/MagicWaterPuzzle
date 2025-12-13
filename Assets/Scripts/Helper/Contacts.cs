using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ColorMaterialCongig
{
    public BlockColor colorID;
    public Material Frame_01;
    public Material Glass_01;
}

public class Contacts : MonoBehaviour
{
    public static Contacts Instance { get; private set; }

    [SerializeField] private List<ColorMaterialCongig> Materials;
    [SerializeField] public Material ice;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public static ColorMaterialCongig GetColorMat(BlockColor colorID)
    {
        if (Instance == null || Instance.Materials == null || Instance.Materials.Count == 0)
            return null;

        switch (colorID)
        {
            case BlockColor.Red:
                return Instance.Materials.Count > 1 ? Instance.Materials[1] : null;
            case BlockColor.Blue:
                return Instance.Materials[0];
            case BlockColor.Green:
                return Instance.Materials[2];
            case BlockColor.Yellow:
                return Instance.Materials[3];
            case BlockColor.purple:
                return Instance.Materials[4];
            case BlockColor.pink:
                return Instance.Materials[5];
            case BlockColor.Brown:
                return Instance.Materials[6];
            case BlockColor.Turquoise:
                return Instance.Materials[7];
            default:
                return null;
        }
    }

    public static ColorMaterialCongig GetColorMatPipe(WaterTypeColor colorID)
    {
        if (Instance == null || Instance.Materials == null || Instance.Materials.Count == 0)
            return null;

        switch (colorID)
        {
            case WaterTypeColor.Red:
                return Instance.Materials.Count > 1 ? Instance.Materials[1] : null;
            case WaterTypeColor.Blue:
                return Instance.Materials[0];
            case WaterTypeColor.Green:
                return Instance.Materials[2];
            case WaterTypeColor.Yellow:
                return Instance.Materials[3];
            case WaterTypeColor.purple:
                return Instance.Materials[4];
            case WaterTypeColor.pink:
                return Instance.Materials[5];
            case WaterTypeColor.Brown:
                return Instance.Materials[6];
            case WaterTypeColor.Turquoise:
                return Instance.Materials[7];
            default:
                return null;
        }
    }


    public static string HexColor(BlockColor blockColor)
    {
        switch (blockColor)
        {
            case BlockColor.Red:
                return "#ff1e00ff";
            case BlockColor.Blue:
                return "#0022ffff";
            case BlockColor.Green:
                return "#51ff00ff";
            case BlockColor.Yellow:
                return "#eaff00ff";
            case BlockColor.purple:
                return "#a600ffff";
            case BlockColor.pink:
                return "#ff00a6ff";
            case BlockColor.Brown:
                return "#9f4000ff";
            case BlockColor.Turquoise:
                return "#00fbffff";
            default: return "";
        }
    }

    public static string HexColorSplash(WaterTypeColor waterTypeColor)
    {
        switch (waterTypeColor)
        {
            case WaterTypeColor.Red:
                return "#ff353578";
            case WaterTypeColor.Blue:
                return "#4059fc84";
            case WaterTypeColor.Green:
                return "#53f9406b";
            case WaterTypeColor.Yellow:
                return "#eaff00ff";
            case WaterTypeColor.purple:
                return "#a600ffff";
            case WaterTypeColor.pink:
                return "#ea00ffff";
            case WaterTypeColor.Brown:
                return "#ff6600ff";
            case WaterTypeColor.Turquoise:
                return "#00fbffff";

            default: return "";
        }
    }

    public static string formatTime(float timeLeft)
    {
        TimeSpan ts = TimeSpan.FromSeconds(timeLeft);

        if (timeLeft > 60f)
        {
            // Hiện dạng mm:ss
            return ts.ToString(@"mm\:ss");
        }
        else
        {
            // Hiện chỉ giây (ss)
            return ts.ToString(@"ss");
        }
    }

    public Vector2 WorldToUIPosition(Canvas canvas, Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            null,
            out Vector2 uiPos
        );

        return uiPos;
    }


}
