using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hger
{

    public class Utils : MonoBehaviour
    {

        // Âü°í : https://youtu.be/waEsGu--9P8
        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, float characterSize = 1f, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 50000)
        {
            if (color == null) color = Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, characterSize, (Color)color, textAnchor, textAlignment, sortingOrder);
        }
        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, float characterSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject obj = new GameObject("World_Text", typeof(TextMesh));

            Transform obj_transform = obj.transform;
            obj_transform.SetParent(parent, false);
            obj_transform.localPosition = localPosition;

            TextMesh textMesh = obj.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.characterSize = characterSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return textMesh;
        }

        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;

            return vec;
        }

        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }
        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCam)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCam);
        }
        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPos, Camera worldCam)
        {
            Vector3 worldPos = worldCam.ScreenToWorldPoint(screenPos);
            return worldPos;
        }
    }

}