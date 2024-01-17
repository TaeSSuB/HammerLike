// Copyright 2014-2018 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BlendModes
{
    /// <summary>
    /// Represents an extension package sources.
    /// </summary>
    [System.Serializable]
    public class ExtensionPackage
    {
        /// <summary>
        /// Relative (rooting at <see cref="PackagePath.ExtensionsPath"/>) path to the extension package directory.
        /// </summary>
        public string PackagePath { get { return packagePath.Replace('\\', Path.DirectorySeparatorChar); } }
        /// <summary>
        /// Name of the extension package (equals to the last directory name in the package path).
        /// </summary>
        public string PackageName { get { return Path.GetFileName(PackagePath); } }
        /// <summary>
        /// Files contained in the extension package directory.
        /// </summary>
        public List<ExtensionFile> Files { get { return files; } }

        [SerializeField] private string packagePath;
        [SerializeField] private List<ExtensionFile> files;

        public ExtensionPackage (string packagePath, List<ExtensionFile> files)
        {
            this.packagePath = packagePath;
            this.files = files;
        }
    }
}
