using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_Item), true)]
public class ItemCustomEditor : Editor
{
    SO_Item itemData;

    private void OnEnable()
    {
        itemData = target as SO_Item;
    }

    public override void OnInspectorGUI()
    {
        if (itemData.itemIcon != null)
        {
            GUILayout.Label("Preview", GUILayout.Height(20), GUILayout.Width(120));

            Texture2D texture = AssetPreview.GetAssetPreview(itemData.itemIcon);

            GUILayout.Label("", GUILayout.Height(120), GUILayout.Width(120));

            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
        }

        base.OnInspectorGUI();
    }
}
