// Copyright 2014-2018 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BlendModes
{
    public static class PackagePath
    {
        public static string PackageRootPath { get { return GetPackageRootPath(); } }
        public static string PackageMarkerPath { get { return Path.Combine(cachedPackageRootPath, markerSearchPattern); } }
        public static string ResourcesPath { get { return Path.Combine(PackageRootPath, "Resources"); } }
        public static string EditorResourcesPath { get { return Path.Combine(PackageRootPath, "EditorResources"); } }
        public static string ExtensionsPath { get { return Path.Combine(PackageRootPath, "Extensions"); } }
        public static string ComponentExtensionsPath { get { return Path.Combine(ExtensionsPath, "Components"); } }
        public static string ShaderExtensionsPath { get { return Path.Combine(ExtensionsPath, "Shaders"); } }
        public static string ShaderResourcesAssetPath { get { return Path.Combine(ResourcesPath, "BlendModes/ShaderResources.asset"); } }
        public static string ExtensionSourcesAssetPath { get { return Path.Combine(EditorResourcesPath, "ExtensionSources.asset"); } }

        private const string markerSearchPattern = "PackageMarker.com-elringus-blendmodes";
        private static string cachedPackageRootPath;

        public static string ToAssetsPath (string absolutePath)
        {
            absolutePath = absolutePath.Replace("\\", "/");
            return "Assets" + absolutePath.Replace(Application.dataPath, string.Empty);
        }

        public static void CreateDirectoryAsset (string fullDirectoryPath)
        {
            var assetPath = ToAssetsPath(fullDirectoryPath);
            EnsureFolderIsCreatedRecursively(assetPath);
        }

        private static string GetPackageRootPath ()
        {
            if (string.IsNullOrEmpty(cachedPackageRootPath) || !File.Exists(PackageMarkerPath))
            {
                var marker = Directory.GetFiles(Application.dataPath, markerSearchPattern, SearchOption.AllDirectories).FirstOrDefault();
                if (marker == null) { Debug.LogError("Can't find package marker file."); return null; }
                cachedPackageRootPath = Directory.GetParent(marker).Parent.FullName;
            }
            return cachedPackageRootPath;
        }

        private static void EnsureFolderIsCreatedRecursively (string targetFolder)
        {
            if (!AssetDatabase.IsValidFolder(targetFolder))
            {
                EnsureFolderIsCreatedRecursively(Path.GetDirectoryName(targetFolder));
                AssetDatabase.CreateFolder(Path.GetDirectoryName(targetFolder), Path.GetFileName(targetFolder));
            }
        }
    }
}
