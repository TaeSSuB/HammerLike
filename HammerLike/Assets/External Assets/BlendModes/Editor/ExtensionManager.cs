// Copyright 2014-2018 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BlendModes
{
    /// <summary>
    /// Provides APIs to manage extension packages and related assets in the editor.
    /// </summary>
    public static class ExtensionManager
    {
        /// <summary>
        /// Cached reference to the <see cref="BlendModes.ExtensionSources"/> asset.
        /// </summary>
        public static ExtensionSources ExtensionSources { get { return cachedExtensionSources ?? (cachedExtensionSources = ExtensionSources.Load()); } }
        /// <summary>
        /// Cached reference to the <see cref="BlendModes.ShaderResources"/> asset.
        /// </summary>
        public static ShaderResources ShaderResources { get { return cachedShaderResources ?? (cachedShaderResources = ShaderResources.Load()); } }

        private static ExtensionSources cachedExtensionSources;
        private static ShaderResources cachedShaderResources;

        /// <summary>
        /// Adds references to all the currently installed shader extensions to the <see cref="ShaderResources"/> asset.
        /// Will create the resources asset if it doesn't exist.
        /// </summary>
        public static void UpdateShaderResources ()
        {
            ShaderResources resources;

            if (!File.Exists(PackagePath.ShaderResourcesAssetPath))
            {
                resources = ScriptableObject.CreateInstance<ShaderResources>();
                PackagePath.CreateDirectoryAsset(Path.GetDirectoryName(PackagePath.ShaderResourcesAssetPath));
                AssetDatabase.CreateAsset(resources, PackagePath.ToAssetsPath(PackagePath.ShaderResourcesAssetPath));
                AssetDatabase.SaveAssets();
            }
            else resources = ShaderResources.Load();

            resources.RemoveAllShaders();

            var shaderPaths = Directory.GetFiles(PackagePath.ShaderExtensionsPath, "*.shader", SearchOption.AllDirectories).ToList();
            foreach (var additionalPath in resources.GetAdditionalPaths())
            {
                var fullPath = Path.GetFullPath(additionalPath);
                if (!Directory.Exists(fullPath)) continue;
                shaderPaths.AddRange(Directory.GetFiles(fullPath, "*.shader", SearchOption.AllDirectories));
            }
            foreach (var shaderPath in shaderPaths)
            {
                var relativeShaderPath = PackagePath.ToAssetsPath(shaderPath);
                var shader = AssetDatabase.LoadAssetAtPath<Shader>(relativeShaderPath);
                if (IsValidShader(shader)) resources.AddShader(shader);
            }

            EditorUtility.SetDirty(resources);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Returns names of the currently installed component extension packages.
        /// </summary>
        public static IEnumerable<string> GetInstalledComponentExtensions ()
        {
            return Directory.GetDirectories(PackagePath.ComponentExtensionsPath).Select(p => Path.GetFileName(p));
        }

        /// <summary>
        /// Returns names of the currently installed shader extension packages.
        /// </summary>
        public static IEnumerable<string> GetInstalledShaderExtensions ()
        {
            return Directory.GetDirectories(PackagePath.ShaderExtensionsPath).Select(p => Path.GetFileName(p));
        }

        /// <summary>
        /// Whether a component extension package for the provided type is currently installed.
        /// </summary>
        public static bool IsComponentExtensionForTypeInstalled (string componentType)
        {
            if (!Directory.Exists(PackagePath.ComponentExtensionsPath)) return false;
            var installedPackageNames = Directory.GetDirectories(PackagePath.ComponentExtensionsPath).Select(p => Path.GetFileName(p));
            return installedPackageNames.Contains(componentType);
        }

        /// <summary>
        /// Whether a shader extension package for the provided shader family is currently installed.
        /// </summary>
        public static bool IsShaderExtensionInstalled (string shaderFamily)
        {
            if (!Directory.Exists(PackagePath.ShaderExtensionsPath)) return false;
            var installedPackageNames = Directory.GetDirectories(PackagePath.ShaderExtensionsPath).Select(p => Path.GetFileName(p));
            return installedPackageNames.Contains(shaderFamily);
        }

        /// <summary>
        /// Whether a component extension package for the provided type is available in the <see cref="BlendModes.ExtensionSources"/> asset.
        /// </summary>
        public static bool IsComponentTypeSupported (string componentType)
        {
            return ExtensionSources.GetComponentExtensionForType(componentType) != null;
        }

        /// <summary>
        /// Whether a shader extension package of the provided shader family is available in the <see cref="BlendModes.ExtensionSources"/> asset.
        /// </summary>
        public static bool IsShaderExtensionAvailable (string shaderFamily)
        {
            return ExtensionSources.GetShaderExtensionByFamily(shaderFamily) != null;
        }

        /// <summary>
        /// Whether provided game object contains a component for which a <see cref="ComponentExtension"/> is available.
        /// </summary>
        public static bool IsObjectContainsSupportedComponent (GameObject extendedObject)
        {
            return extendedObject.GetComponents<Component>().Any(c => IsComponentTypeSupported(c.GetType().FullName));
        }

        /// <summary>
        /// Return the first <see cref="Component"/> attached to the provided game object for which a <see cref="ComponentExtension"/> is available.
        /// </summary>
        public static Component GetSupportedComponentOnObject (GameObject extendedObject)
        {
            return extendedObject.GetComponents<Component>().First(c => IsComponentTypeSupported(c.GetType().FullName));
        }

        /// <summary>
        /// Installs a component extension package for the component of provided type (when available).
        /// </summary>
        public static void InstallComponentExtension (string extendedComponentType)
        {
            if (!IsComponentTypeSupported(extendedComponentType))
            {
                Debug.LogError(string.Format("'{0}' type is not supported.", extendedComponentType));
                return;
            }

            if (IsComponentExtensionForTypeInstalled(extendedComponentType))
                RemoveComponentExtension(extendedComponentType);

            var extensionPackage = ExtensionSources.GetComponentExtensionForType(extendedComponentType);
            var packagePath = Path.Combine(PackagePath.ExtensionsPath, extensionPackage.PackagePath);
            foreach (var file in extensionPackage.Files)
            {
                var filePath = Path.Combine(packagePath, file.FilePath);
                var parentDirectory = Directory.GetParent(filePath).FullName;
                PackagePath.CreateDirectoryAsset(parentDirectory);
                File.WriteAllText(filePath, file.FileContent, Encoding.UTF8);
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Installs a shader extension package of the provided shader family (when available).
        /// </summary>
        public static void InstallShaderExtension (string shaderFamily)
        {
            if (!IsShaderExtensionAvailable(shaderFamily))
            {
                Debug.LogError(string.Format("'{0}' shader family is not available.", shaderFamily));
                return;
            }

            if (IsShaderExtensionInstalled(shaderFamily))
                RemoveShaderExtension(shaderFamily);

            var extensionPackage = ExtensionSources.GetShaderExtensionByFamily(shaderFamily);
            var packagePath = Path.Combine(PackagePath.ExtensionsPath, extensionPackage.PackagePath);
            foreach (var file in extensionPackage.Files)
            {
                var filePath = Path.Combine(packagePath, file.FilePath);
                var parentDirectory = Directory.GetParent(filePath).FullName;
                PackagePath.CreateDirectoryAsset(parentDirectory);
                File.WriteAllText(filePath, file.FileContent, Encoding.UTF8);
            }

            AssetDatabase.Refresh();
            UpdateShaderResources();
        }

        /// <summary>
        /// Removes a component extension package for the component of provided type (is installed).
        /// </summary>
        public static void RemoveComponentExtension (string extendedComponentType)
        {
            var packagePath = Path.Combine(PackagePath.ComponentExtensionsPath, extendedComponentType);
            if (Directory.Exists(packagePath))
            {
                Directory.Delete(packagePath, true);
                File.Delete(packagePath + ".meta");
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Removes a shader extension package of the provided shader family.
        /// </summary>
        public static void RemoveShaderExtension (string shaderFamily)
        {
            var packagePath = Path.Combine(PackagePath.ShaderExtensionsPath, shaderFamily);
            if (Directory.Exists(packagePath))
            {
                Directory.Delete(packagePath, true);
                File.Delete(packagePath + ".meta");
            }

            AssetDatabase.Refresh();
            UpdateShaderResources();
        }

        /// <summary>
        /// Checks whether provided shader name complies with the extension shaders pattern.
        /// </summary>
        public static bool IsValidShader (Shader shader)
        {
            return shader.name.StartsWith("Hidden/BlendModes/");
        }
    }
}
