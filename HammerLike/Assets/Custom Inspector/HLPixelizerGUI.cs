// HLPixelizerGUI.cs
using Sirenix.OdinInspector;
using UnityEngine;

public class HLPixelizerGUI : MonoBehaviour
{
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    public int ColoredInt1;
    
    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    public int ColoredInt2;
    
    [GUIColor("#FF0000")]
    public int Hex1;
    
    [GUIColor("#FF000077")]
    public int Hex2;
    
    [GUIColor("RGB(0, 1, 0)")]
    public int Rgb;
    
    [GUIColor("RGBA(0, 1, 0, 0.5)")]
    public int Rgba;
    
    [GUIColor("orange")]
    public int NamedColors;
    
    [ButtonGroup]
    [GUIColor(0, 1, 0)]
    private void Apply()
    {
    }
    
    [ButtonGroup]
    [GUIColor(1, 0.6f, 0.4f)]
    private void Cancel()
    {
    }
    
    [InfoBox("You can also reference a color member to dynamically change the color of a property.")]
    [GUIColor("GetButtonColor")]
    [Button("I Am Fabulous", ButtonSizes.Gigantic)]
    private static void IAmFabulous()
    {
    }
    
    [Button(ButtonSizes.Large)]
    [GUIColor("@Color.Lerp(Color.red, Color.green, Mathf.Abs(Mathf.Sin((float)EditorApplication.timeSinceStartup)))")]
    private static void Expressive()
    {
    }
    
#if UNITY_EDITOR // Editor-related code must be excluded from builds
    private static Color GetButtonColor()
    {
        Sirenix.Utilities.Editor.GUIHelper.RequestRepaint();
        return Color.HSVToRGB(Mathf.Cos((float)UnityEditor.EditorApplication.timeSinceStartup + 1f) * 0.225f + 0.325f, 1, 1);
    }
#endif
}