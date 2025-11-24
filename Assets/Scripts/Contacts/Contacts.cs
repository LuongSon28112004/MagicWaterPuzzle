using System.Collections.Generic;
using UnityEngine;

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


    public static string HexColor(BlockColor blockColor)
    {
        switch (blockColor)
        {
            case BlockColor.Red:
                return "#ff1e00d0";
            case BlockColor.Blue:
                return "#0022ffd7";
            case BlockColor.Green:
                return "#54fc06b9";
            default: return "";
        }
    }

}
