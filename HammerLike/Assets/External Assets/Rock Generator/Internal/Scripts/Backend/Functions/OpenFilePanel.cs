using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JPBotelho
{
    public static class FilePanel
    {
        public static string Open()
        {
            string t = EditorUtility.SaveFilePanel("Save Rock", Application.dataPath, "Procedural Rock.obj", "Obj");

            if (!string.IsNullOrEmpty(t))
            {
                return t;
            }
            else
            {
                //Debug.LogError("Export Cancelled");
                return "";            
            }
        }
    }
}