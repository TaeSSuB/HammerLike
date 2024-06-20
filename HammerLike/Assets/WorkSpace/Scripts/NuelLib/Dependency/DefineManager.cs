using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using System.Collections.Generic;

public class DefineManager
{
    private static ListRequest request;
    private static List<EditorApplication.CallbackFunction> updateCallbacks = new List<EditorApplication.CallbackFunction>();

    public static void CheckPackageAndSetDefine(string packageName, string defineName)
    {
        request = Client.List(); // ��Ű�� ��� ��û
        EditorApplication.CallbackFunction callback = null;
        callback = () => Progress(packageName, defineName, callback);
        updateCallbacks.Add(callback);
        EditorApplication.update += callback;
    }

    private static void Progress(string packageName, string defineName, EditorApplication.CallbackFunction callback)
    {
        if (request.IsCompleted)
        {
            if (request.Status == StatusCode.Success)
            {
                bool found = false;
                foreach (var package in request.Result)
                {
                    if (package.name == packageName)
                    {
                        // ��Ű���� ��ġ�Ǿ� ���� ���
                        SetDefineSymbols(defineName, true);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // ��Ű���� ��ġ�Ǿ� ���� ���� ���
                    SetDefineSymbols(defineName, false);
                }
            }
            else if (request.Status >= StatusCode.Failure)
            {
                Debug.Log("Failed to list packages: " + request.Error.message);
            }

            EditorApplication.update -= callback;
            updateCallbacks.Remove(callback);
        }
    }

    private static void SetDefineSymbols(string defineName, bool shouldDefine)
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        if (shouldDefine)
        {
            if (!defines.Contains(defineName))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines + ";" + defineName);
                Debug.Log("Define " + defineName + " added.");
            }
        }
        else
        {
            if (defines.Contains(defineName))
            {
                defines = defines.Replace(defineName + ";", "").Replace(";" + defineName, "").Replace(defineName, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
                Debug.Log("Define " + defineName + " removed.");
            }
        }
    }
}
