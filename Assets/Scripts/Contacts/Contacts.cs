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


}
