using UnityEditor;
//using UnityEditor.PackageManager;
//using UnityEditor.PackageManager.Requests;
using UnityEngine;
using System.Collections.Generic;

public class DefineManager
{
    //private static ListRequest request;
    //private static List<EditorApplication.CallbackFunction> updateCallbacks = new List<EditorApplication.CallbackFunction>();
    //
    //public static void CheckPackageAndSetDefine(string packageName, string defineName)
    //{
    //
    //    request = Client.List(); // 패키지 목록 요청
    //    EditorApplication.CallbackFunction callback = null;
    //    callback = () => Progress(packageName, defineName, callback);
    //    updateCallbacks.Add(callback);
    //
    //}
    //
    //private static void Progress(string packageName, string defineName, EditorApplication.CallbackFunction callback)
    //{
    //
    //    if (request.IsCompleted)
    //    {
    //        if (request.Status == StatusCode.Success)
    //        {
    //            bool found = false;
    //            foreach (var package in request.Result)
    //            {
    //                if (package.name == packageName)
    //                {
    //                    // 패키지가 설치되어 있을 경우
    //                    SetDefineSymbols(defineName, true);
    //                    found = true;
    //                    break;
    //                }
    //            }
    //
    //            if (!found)
    //            {
    //                // 패키지가 설치되어 있지 않을 경우
    //                SetDefineSymbols(defineName, false);
    //            }
    //        }
    //        else if (request.Status >= StatusCode.Failure)
    //        {
    //            Debug.Log("Failed to list packages: " + request.Error.message);
    //        }
    //
    //        EditorApplication.update -= callback;
    //        updateCallbacks.Remove(callback);
    //    }
    //}
    //
    //private static void SetDefineSymbols(string defineName, bool shouldDefine)
    //{
    //    BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
    //    string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
    //
    //    if (shouldDefine)
    //    {
    //        if (!defines.Contains(defineName))
    //        {
    //            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines + ";" + defineName);
    //            Debug.Log("Define " + defineName + " added.");
    //        }
    //    }
    //    else
    //    {
    //        if (defines.Contains(defineName))
    //        {
    //            defines = defines.Replace(defineName + ";", "").Replace(";" + defineName, "").Replace(defineName, "");
    //            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
    //            Debug.Log("Define " + defineName + " removed.");
    //        }
    //    }
    //}
}
