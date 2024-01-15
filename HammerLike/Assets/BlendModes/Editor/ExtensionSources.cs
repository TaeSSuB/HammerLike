// Copyright 2014-2018 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BlendModes
{
    /// <summary>
    /// Contains sources for the available extension packages.
    /// </summary>
    public class ExtensionSources : ScriptableObject
    {
        [SerializeField] private List<ExtensionPackage> componentExtensions = new List<ExtensionPackage>();
        [SerializeField] private List<ExtensionPackage> shaderExtensions = new List<ExtensionPackage>();

        /// <summary>
        /// Loads the asset using <see cref="PackagePath.ExtensionSourcesAssetPath"/>. 
        /// Will create the asset if it doesn't exist.
        /// </summary>
        public static ExtensionSources Load ()
        {
            var assetPath = PackagePath.ExtensionSourcesAssetPath;
            var relativeAssetPath = PackagePath.ToAssetsPath(assetPath);
            if (!File.Exists(assetPath))
            {
                var extensionSources = CreateInstance<ExtensionSources>();
                PackagePath.CreateDirectoryAsset(Path.GetDirectoryName(assetPath));
                AssetDatabase.CreateAsset(extensionSources, relativeAssetPath);
                AssetDatabase.SaveAssets();
                return extensionSources;
            }
            else return AssetDatabase.LoadAssetAtPath<ExtensionSources>(relativeAssetPath);
        }

        public void SetDirtyAndSaveAssets ()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Looks for a component extension package used to extend the provided type. 
        /// </summary>
        /// <remarks>
        /// Name of the component extension packages equals to the full type name of the component they extend. 
        /// </remarks>
        public ExtensionPackage GetComponentExtensionForType (string extendedType)
        {
            return componentExtensions.FirstOrDefault(e => e.PackageName.Equals(extendedType));
        }

        /// <summary>
        /// Looks for a shader extension package of the provided shader family. 
        /// </summary>
        /// <remarks>
        /// Name of the shader extension packages equals to the shader family they represent.
        /// </remarks>
        public ExtensionPackage GetShaderExtensionByFamily (string shaderFamily)
        {
            return shaderExtensions.FirstOrDefault(e => e.PackageName.Equals(shaderFamily));
        }

        public IEnumerable<string> GetAvailableComponentExtensions ()
        {
            return componentExtensions.Select(e => e.PackageName);
        }

        public IEnumerable<string> GetAvailableShaderExtensions ()
        {
            return shaderExtensions.Select(e => e.PackageName);
        }

        public void AddComponentExtensionPackage (ExtensionPackage package)
        {
            if (!componentExtensions.Exists(p => p.PackagePath == package.PackagePath))
                componentExtensions.Add(package);
        }

        public void AddShaderExtensionPackage (ExtensionPackage package)
        {
            if (!shaderExtensions.Exists(p => p.PackagePath == package.PackagePath))
                shaderExtensions.Add(package);
        }

        public void RemoveAllPackages ()
        {
            componentExtensions.Clear();
            shaderExtensions.Clear();
        }
    }
}
