using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerColors : MonoBehaviour
{
    static Color[] playerColors = new Color[] 
    {
            new Color(0f, 0f, 0f),
            new Color(1f, 1f, 1f),
            new Color(0.9f, 0.1f, 0.1f),
            new Color(0.9f, 0.5f, 0f),
            new Color(0.8f, 0.8f, 0.1f),
            new Color(0.1f, 0.8f, 0.1f),
            new Color(0.1f, 0.1f, 0.9f),
            new Color(0.5f, 0.1f, 0.7f)
    }; static string[] colorNames = new string[]
     {
            "BBC",
            "Extremist",
            "Coco",
            "Orange",
            "PeeBoi",
            "Soja",
            "Marine",
            "Daronne Mystic"
     };
    static Vector3 highlightedVariant = new Vector3(1f, 1f, 0.8f);
    static Vector3 pressedVariant = new Vector3(1f, 0.6f, 0.6f);
    static Vector3 selectedVariant = new Vector3(1f, 0.8f, 0.8f);
    static Color disableVariant = new Color(0.5f, 0.5f, .5f);

    static public ColorBlock SetButtonColor(Color color)
    {
        //create new ColorBlock for the new button
        var colorBlock = new ColorBlock();

        //Get the given color in HSV
        var h = 0f;
        var s = 0f;
        var v = 0f;
        Color.RGBToHSV(color, out h, out s, out v);

        //Set colors to the new ColorBlock using variant Vector3
        colorBlock.normalColor = color;
        colorBlock.highlightedColor = Color.HSVToRGB(h * highlightedVariant.x, s * highlightedVariant.y, v * highlightedVariant.z);
        colorBlock.pressedColor = Color.HSVToRGB(h * pressedVariant.x, s * pressedVariant.y, v * pressedVariant.z);
        colorBlock.selectedColor = Color.HSVToRGB(h * selectedVariant.x, s * selectedVariant.y, v * selectedVariant.z);
        colorBlock.disabledColor = disableVariant;
        colorBlock.colorMultiplier = 1f;
        //Change the button ColorBlock with the new one
        return colorBlock;
    }
    static public Color[] GetPlayerColors()
    {
        return playerColors;
    }
    static public string[] GetPlayerNames()
    {
        return colorNames;
    }
}
