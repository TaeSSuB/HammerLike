using UnityEditor;
using UnityEngine;

/// <summary>
/// ItemCustomEditor : 아이템 데이터 에디터 커스텀
/// - 아이템 데이터의 아이콘을 에디터 상에서 미리보기 가능하도록 구현
/// </summary>
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
