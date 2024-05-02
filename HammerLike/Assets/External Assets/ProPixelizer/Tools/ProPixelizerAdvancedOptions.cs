// Copyright Elliot Bentine, 2018-
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.IO;

namespace ProPixelizer
{

    /// <summary>
    /// A tool for putting ProPixelizer into Advanced Mode.
    /// </summary>
    public class ProPixelizerAdvancedOptions : EditorWindow
    {
        [MenuItem("Window/ProPixelizer/Advanced Options")]
        public static void ShowWindow()
        {
            ProPixelizerAdvancedOptions window = (ProPixelizerAdvancedOptions)GetWindow(typeof(ProPixelizerAdvancedOptions));
            window.StatusListRequest = window.GetProjectPackages();
        }

        bool _showSGTarget = true;

        void OnGUI()
        {
            //var style = new GUIStyle(EditorStyles.toolbarButton);
            //style.normal.textColor = Color.green;
            //style.fontSize = 18;

            //GUILayout.Label("TextureIndexer", EditorStyles.largeLabel);
            EditorGUILayout.LabelField("ProPixelizer | Advanced Options", EditorStyles.boldLabel);
            if (GUILayout.Button("User Guide")) Application.OpenURL(ProPixelizerUtils.USER_GUIDE_URL);
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Advanced options for the ProPixelizer package.", MessageType.None);
            EditorGUILayout.Space();
            _showSGTarget = EditorGUILayout.BeginFoldoutHeaderGroup(_showSGTarget, "ShaderGraph SubTarget");
            EditorGUILayout.HelpBox("Enable the ShaderGraph SubTarget for ProPixelizer.\n" +
                "\nNote that because this requires access to classes marked as internal to URP, this requires that " +
                "these packages are embedded into the project and ProPixelizer added to `InternalsVisibleTo`. " +
                "\n\nThis tool will automate this process, but backup your project before using it. You may need " +
                "to run the tool a few times, as Unity Package Manager can sometimes interrupt execution as it loads the packages.",
                MessageType.None);


            EditorGUILayout.LabelField("Explicit Dependency:", EditorStyles.boldLabel);
            GUI.enabled = false;
            EditorGUILayout.ToggleLeft("shadergraph", _SGHasDirectDependency);
            GUI.enabled = true;

            EditorGUILayout.LabelField("Package embed status:", EditorStyles.boldLabel);
            GUI.enabled = false;
            EditorGUILayout.ToggleLeft("universal", _URPEmbedded);
            EditorGUILayout.ToggleLeft("shadergraph", _SGEmbedded);
            GUI.enabled = true;

            EditorGUILayout.LabelField("ProPixelizer added to InternalsVisibleTo:", EditorStyles.boldLabel);
            GUI.enabled = false;
            EditorGUILayout.ToggleLeft("universal", _URPModded);
            EditorGUILayout.ToggleLeft("shadergraph", _SGModded);
            GUI.enabled = true;

            EditorGUILayout.LabelField("Scripting Define Symbols:", EditorStyles.boldLabel);
            GUI.enabled = false;
#if PROPIXELIZER_SHADERGRAPH
            EditorGUILayout.Toggle("scripting define", true);
            EditorGUILayout.HelpBox("If you later upgrade or remove the embedded packages, don't forget to remove the `PROPIXELIZER_SHADERGRAPH` scripting define symbol.", MessageType.Info);
#else
            if (_URPEmbedded && _URPModded && _SGEmbedded && _SGModded)
                EditorGUILayout.HelpBox("Final step: In Project Settings/Player/Other Settings/Scripting Define Symbols, add a define for " +
                    "`PROPIXELIZER_SHADERGRAPH`.\n(Note that these definitions are per-platform, so you need to add them for each platform).",
                    MessageType.Info);
            EditorGUILayout.ToggleLeft("PROPIXELIZER_SHADERGRAPH", false);
#endif
            GUI.enabled = true;

            var style = new GUIStyle(EditorStyles.miniButton);
            GUI.enabled = false;
            if (ShaderGraphSubtargetEnabled())
            {
                style.normal.textColor = Color.green;
                GUILayout.Label("Subtarget is enabled.", style);
            }
            else
            {
                GUILayout.Label("Subtarget is not enabled.", style);
                GUI.enabled = true;
                if (GUILayout.Button("Enable (may need to run multiple times)", EditorStyles.miniButton))
                {
                    WorkflowStatus = EmbedWorkflowStatus.Start;
                }
                if (WorkflowStatus != EmbedWorkflowStatus.None)
                {
                    GUILayout.Label("Workflow in progress: " + WorkflowStatus);
                }
            }
            GUI.enabled = true;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        enum EmbedWorkflowStatus
        {
            Start,
            EmbedURPPackage,
            CheckSGDependency,
            AddSGDependency,
            EmbedSGPackage,
            ModifyPackages,
            AddProPixelizerSGDefine,
            None
        }

        EmbedWorkflowStatus WorkflowStatus = EmbedWorkflowStatus.None;

        void UpdateWorkflowStatus()
        {
            switch (WorkflowStatus)
            {
                case EmbedWorkflowStatus.Start:
                    WorkflowStatus = EmbedWorkflowStatus.EmbedURPPackage;
                    break;
                case EmbedWorkflowStatus.EmbedURPPackage:
                    if (_URPEmbedded)
                        WorkflowStatus = EmbedWorkflowStatus.CheckSGDependency;
                    else
                    {
                        if (URPEmbedRequest == null)
                            URPEmbedRequest = Client.Embed(UniversalPackageName);
                        else if (URPEmbedRequest.IsCompleted)
                        {
                            URPEmbedRequest = null;
                            WorkflowStatus = EmbedWorkflowStatus.CheckSGDependency;
                        }
                    }
                    break;
                case EmbedWorkflowStatus.CheckSGDependency:
                    if (!_SGHasDirectDependency)
                        WorkflowStatus = EmbedWorkflowStatus.AddSGDependency;
                    else
                        WorkflowStatus = EmbedWorkflowStatus.EmbedSGPackage;
                    break;
                case EmbedWorkflowStatus.AddSGDependency:
                    if (SGPackageListRequest == null)
                        SGPackageListRequest = GetProjectPackages();
                    else {
                        if (TryAddShaderGraphDirectDependency(SGPackageListRequest))
                        {
                            SGPackageListRequest = null;
                            WorkflowStatus = EmbedWorkflowStatus.EmbedSGPackage;
                        }
                    }
                    break;
                case EmbedWorkflowStatus.EmbedSGPackage:
                    if (_SGEmbedded)
                        WorkflowStatus = EmbedWorkflowStatus.ModifyPackages;
                    else
                    {
                        if (SGEmbedRequest == null)
                            SGEmbedRequest = Client.Embed(ShaderGraphPackageName);
                        else if (SGEmbedRequest.IsCompleted)
                        {
                            SGEmbedRequest = null;
                            WorkflowStatus = EmbedWorkflowStatus.ModifyPackages;
                        }
                    }
                    break;
                case EmbedWorkflowStatus.ModifyPackages:
                    if (ModificationListRequest == null)
                        ModificationListRequest = GetProjectPackages();
                    if (HandleModificationListRequest(ModificationListRequest))
                    {
                        ModificationListRequest = null;
                        WorkflowStatus = EmbedWorkflowStatus.AddProPixelizerSGDefine;
                    }
                    break;
                case EmbedWorkflowStatus.AddProPixelizerSGDefine:
                    WorkflowStatus = EmbedWorkflowStatus.None;
                    break;
                case EmbedWorkflowStatus.None:
                    if (StatusListRequest != null)
                    {
                        if (TryHandleProjectPackageListRequest(StatusListRequest))
                            StatusListRequest = null;
                    } else
                    {
                        idleUpdateCounter++;
                        if (idleUpdateCounter > 10)
                        {
                            idleUpdateCounter = 0;
                            StatusListRequest = GetProjectPackages();
                        }
                    }
                    break;
            }
        }

        int idleUpdateCounter;

        private void OnInspectorUpdate()
        {
            UpdateWorkflowStatus();
            Repaint();
        }

        bool _URPEmbedded = false;
        bool _SGHasDirectDependency = false;
        bool _SGEmbedded = false;
        bool _URPModded = false;
        bool _SGModded = false;
        /// <summary>
        /// List request used to query package status.
        /// </summary>
        ListRequest StatusListRequest;
        /// <summary>
        /// List request used to identify and modify packages.
        /// </summary>
        ListRequest ModificationListRequest;
        ListRequest SGPackageListRequest;
        EmbedRequest URPEmbedRequest;
        EmbedRequest SGEmbedRequest;
        public const string UniversalPackageName = "com.unity.render-pipelines.universal";
        public const string ShaderGraphPackageName = "com.unity.shadergraph";

        // Editor Coroutines would be a great thing to use here. However, they aren't shipped with base
        // Unity, and I don't want to enforce a package dependency on users for the sake of a one-time-use
        // tool. So, instead, I have to rely on the Editor update loop and a state machine.

        ListRequest GetProjectPackages()
        {
            return Client.List(true, true);
        }

        bool TryHandleProjectPackageListRequest(ListRequest request)
        {
            if (!request.IsCompleted)
                return false;

            if (request.Status == StatusCode.Failure)
            {
                return true;
            }

            foreach (var package in request.Result)
            {
                if (package.name == UniversalPackageName)
                    CheckPackageModificationStatus(package, true);
                if (package.name == ShaderGraphPackageName)
                {
                    CheckPackageModificationStatus(package, false);
                    _SGHasDirectDependency = package.isDirectDependency;
                }
            }
            return true;
        }

        bool TryAddShaderGraphDirectDependency(ListRequest request)
        {
            if (!request.IsCompleted)
                return false;

            if (request.Status == StatusCode.Failure)
                return true;

            foreach (var package in request.Result)
            {
                if (package.name == ShaderGraphPackageName)
                {
                    Client.Add(package.packageId);
                }
            }
            return true;
        }

        /// <summary>
        /// Attempt to determine if current packages have been embedded and modified.
        /// </summary>
        /// <returns>Returns true if the SearchRequest has been handled</returns>
        bool CheckPackageModificationStatus(UnityEditor.PackageManager.PackageInfo package, bool packageIsURP)
        {
            var isEmbedded = (package.source == PackageSource.Embedded);
            if (packageIsURP)
                _URPEmbedded = isEmbedded;
            else
                _SGEmbedded = isEmbedded;
            bool allModded = false;
            if (isEmbedded)
            {
                allModded = true;
                foreach (var relativePath in GetPathsForInternalsVisibleToModification(packageIsURP))
                {
                    var fullPath = Path.Join(package.resolvedPath, relativePath);
                    if (!IsProPixelizerAddedToInternalsVisibleTo(fullPath))
                        allModded = false;
                }
                if (packageIsURP)
                    _URPModded = allModded;
                else
                    _SGModded = allModded;
            } else
            {
                if (packageIsURP)
                    _URPModded = false;
                else
                    _SGModded = false;
            }
            return true;
        }

        /// <summary>
        /// Attempt to modify an embedded package to add ProPixelizer to project InternalsVisibleTo.
        /// </summary>
        /// <returns>Returns true if the EmbedRequest has been handled</returns>
        bool TryPerformEmbedModifications(UnityEditor.PackageManager.PackageInfo package, bool packageIsURP)
        {
            var path = package.resolvedPath;
            foreach (var relativePath in GetPathsForInternalsVisibleToModification(packageIsURP))
            {
                var fullPath = Path.Join(path, relativePath);
                if (IsProPixelizerAddedToInternalsVisibleTo(fullPath))
                    Debug.Log(string.Format("Checking {0}: found ProPixelizer reference already added.", fullPath));
                else
                {
                    AddProPixelizerToInternalsVisibleTo(fullPath);
                    Debug.Log(string.Format("Checking {0}: added ProPixelizer reference.", fullPath));
                }
            }
            if (packageIsURP)
                _URPModded = true;
            else
                _SGModded = true;

            // Trigger searching packages again to detect changes.
            StatusListRequest = GetProjectPackages();

            return true;
        }

        /// <summary>
        /// Attempts to modify embedded packages.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool HandleModificationListRequest(ListRequest request)
        {
            if (!request.IsCompleted)
                return false;

            if (request.Status != StatusCode.Success)
                return true;

            foreach (var package in request.Result)
            {
                // only consider embedded packages for modification.
                if (package.source != PackageSource.Embedded) continue;
                if (package.name == UniversalPackageName)
                    TryPerformEmbedModifications(package, true);
                if (package.name == ShaderGraphPackageName)
                    TryPerformEmbedModifications(package, false);
            }
            return true;
        }

        string[] GetPathsForInternalsVisibleToModification(bool packageIsURP)
        {
            if (packageIsURP)
                return new string[] { "Editor/AssemblyInfo.cs", "Runtime/AssemblyInfo.cs" };
            else
                return new string[] { "Editor/AssemblyInfo.cs" };
        }

        const string InternalsVisibleToProPixelizerLine = "[assembly: InternalsVisibleTo(\"ProPixelizer\")]";

        bool IsProPixelizerAddedToInternalsVisibleTo(string path)
        {
            using (var contents = File.OpenText(path))
            {
                return contents.ReadToEnd().Contains(InternalsVisibleToProPixelizerLine);
            }
        }

        void AddProPixelizerToInternalsVisibleTo(string path)
        {
            using (var stream = new StreamWriter(path, true))
            {
                stream.WriteLine(InternalsVisibleToProPixelizerLine);
            }
        }

#if PROPIXELIZER_SHADERGRAPH
        bool ShaderGraphSubtargetEnabled() => true;
#else
        bool ShaderGraphSubtargetEnabled() => false;
#endif
    }
}
#endif