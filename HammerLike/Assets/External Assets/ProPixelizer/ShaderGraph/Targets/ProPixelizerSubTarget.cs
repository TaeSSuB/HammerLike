// Copyright Elliot Bentine, 2018-
#if UNITY_2022_3_OR_NEWER && PROPIXELIZER_SHADERGRAPH && UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Legacy;
using static UnityEditor.Rendering.Universal.ShaderGraph.SubShaderUtils;
using static Unity.Rendering.Universal.ShaderUtils;
using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using System.Linq;
using UnityEngine.Rendering.Universal;
using System.Reflection;

namespace ProPixelizer.Universal.ShaderGraph
{
    sealed class ProPixelizerSubTarget : UniversalSubTarget, ILegacyTarget
    {
        static readonly GUID kSourceCodeGuid = new GUID("4c8c5b55e54009246a543f08ebc553bd"); // matches .cs

        public override int latestVersion => 2;

        public ProPixelizerSubTarget()
        {
            displayName = "ProPixelizer";
        }

        protected override ShaderID shaderID => ShaderID.SG_Lit;

        public override bool IsActive() => true;

        public override void Setup(ref TargetSetupContext context)
        {
            context.AddAssetDependency(kSourceCodeGuid, AssetCollection.Flags.SourceDependency);
            base.Setup(ref context);

            var universalRPType = typeof(UniversalRenderPipelineAsset);
            if (!context.HasCustomEditorForRenderPipeline(universalRPType))
            {
                var gui = typeof(ShaderGraphLitGUI);
#if HAS_VFX_GRAPH
                if (TargetsVFX())
                    gui = typeof(VFXShaderGraphLitGUI);
#endif
                context.AddCustomEditorForRenderPipeline(gui.FullName, universalRPType);
            }

            // Process SubShaders
            context.AddSubShader(PostProcessSubShader(SubShaders.ProPixelizerSubShader(target, target.renderType, target.renderQueue, target.disableBatching)));
        }

        public override void ProcessPreviewMaterial(Material material)
        {
            if (target.allowMaterialOverride)
            {
                // copy our target's default settings into the material
                // (technically not necessary since we are always recreating the material from the shader each time,
                // which will pull over the defaults from the shader definition)
                // but if that ever changes, this will ensure the defaults are set
                material.SetFloat(Property.CastShadows, target.castShadows ? 1.0f : 0.0f);
                material.SetFloat(Property.ReceiveShadows, target.receiveShadows ? 1.0f : 0.0f);
                material.SetFloat(Property.CullMode, (int)target.renderFace);
            }

            // We always need these properties regardless of whether the material is allowed to override
            // Queue control & offset enable correct automatic render queue behavior
            // Control == 0 is automatic, 1 is user-specified render queue
            material.SetFloat(Property.QueueOffset, 0.0f);
            material.SetFloat(Property.QueueControl, (float)BaseShaderGUI.QueueControl.Auto);

            // call the full unlit material setup function
            ShaderGraphLitGUI.UpdateMaterial(material, MaterialUpdateType.CreatedNewMaterial);
        }

        public override void GetFields(ref TargetFieldContext context)
        {
            base.GetFields(ref context);

            var descs = context.blocks.Select(x => x.descriptor);

            context.AddField(UniversalFields.NormalDropOffTS);
            context.AddField(UniversalFields.Normal, descs.Contains(BlockFields.SurfaceDescription.NormalOS) ||
                descs.Contains(BlockFields.SurfaceDescription.NormalTS) ||
                descs.Contains(BlockFields.SurfaceDescription.NormalWS));
        }

        public override void GetActiveBlocks(ref TargetActiveBlockContext context)
        {
            context.AddBlock(BlockFields.SurfaceDescription.Smoothness);
            context.AddBlock(BlockFields.SurfaceDescription.NormalTS);
            context.AddBlock(BlockFields.SurfaceDescription.Emission);
            context.AddBlock(BlockFields.SurfaceDescription.Occlusion);
            context.AddBlock(BlockFields.SurfaceDescription.Specular);
            context.AddBlock(BlockFields.SurfaceDescription.Alpha);
            context.AddBlock(BlockFields.SurfaceDescription.AlphaClipThreshold);
        }

        public override void CollectShaderProperties(PropertyCollector collector, GenerationMode generationMode)
        {
            // if using material control, add the material property to control workflow mode
            if (target.allowMaterialOverride)
            {
                collector.AddFloatProperty(Property.SpecularWorkflowMode, (float)WorkflowMode.Specular);
                collector.AddFloatProperty(Property.CastShadows, target.castShadows ? 1.0f : 0.0f);
                collector.AddFloatProperty(Property.ReceiveShadows, target.receiveShadows ? 1.0f : 0.0f);

                // setup properties using the defaults
                collector.AddFloatProperty(Property.CullMode, (float)target.renderFace);    // render face enum is designed to directly pass as a cull mode

                bool enableAlphaToMask = (target.alphaClip && (target.surfaceType == SurfaceType.Opaque));
                collector.AddFloatProperty(Property.AlphaToMask, enableAlphaToMask ? 1.0f : 0.0f);
            }

            // We always need these properties regardless of whether the material is allowed to override other shader properties.
            // Queue control & offset enable correct automatic render queue behavior.  Control == 0 is automatic, 1 is user-specified.
            // We initialize queue control to -1 to indicate to UpdateMaterial that it needs to initialize it properly on the material.
            collector.AddFloatProperty(Property.QueueOffset, 0.0f);
            collector.AddFloatProperty(Property.QueueControl, -1.0f);
        }

        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo)
        {
            var universalTarget = (target as UniversalTarget);
            universalTarget.AddDefaultMaterialOverrideGUI(ref context, onChange, registerUndo);
        }

        protected override int ComputeMaterialNeedsUpdateHash()
        {
            int hash = base.ComputeMaterialNeedsUpdateHash();
            hash = hash * 23 + target.allowMaterialOverride.GetHashCode();
            return hash;
        }

        public bool TryUpgradeFromMasterNode(IMasterNode1 masterNode, out Dictionary<BlockFieldDescriptor, int> blockMap)
        {
            blockMap = null;
            return false;
        }

        internal override void OnAfterParentTargetDeserialized()
        {
            Assert.IsNotNull(target);
        }

        #region SubShader
        static class SubShaders
        {
            public static SubShaderDescriptor ProPixelizerSubShader(UniversalTarget target, string renderType, string renderQueue, string disableBatchingTag)
            {
                // Eurgh - why is everything private!
                // Use reflection to create a SubShader using the UniversalLitSubTarget as inspiration.
                var name = typeof(UniversalLitSubTarget).FullName + "+SubShaders," + typeof(UniversalLitSubTarget).Assembly.FullName;
                Type type = Type.GetType(name);
                MethodInfo method = type.GetMethod("LitSubShader", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                var ssd = (SubShaderDescriptor)method.Invoke(null, new object[] { target, WorkflowMode.Specular, renderType, renderQueue, disableBatchingTag, false, false });

                // Add the ProPixelizerPass
                ssd.passes.Add(Passes.ProPixelizerMetadata(target, CoreBlockMasks.Vertex, CoreBlockMasks.FragmentDepthNormals, CorePragmas.Forward, CoreKeywords.ShadowCaster));
                ssd.renderType = "ProPixelizer";

                // Enumerate through passes:
                var newPasses = new PassCollection();
                foreach (var originalPass in ssd.passes)
                {
                    var pass = originalPass.descriptor;
                    switch (pass.referenceName)
                    {
                        case "SHADERPASS_FORWARD":
                            // we could change the includes to redirect to a different lighting calc...
                            break;
                    }

                    // Enforce specular workflow by adding the keyword to defines.
                    pass.defines.Add(new KeywordDescriptor()
                    {
                        displayName = "Specular Setup",
                        referenceName = "_SPECULAR_SETUP",
                        type = KeywordType.Boolean,
                        definition = KeywordDefinition.ShaderFeature,
                        scope = KeywordScope.Local,
                        stages = KeywordShaderStage.Fragment
                    }, 1);

                    switch (pass.displayName)
                    {
                        case "SceneSelectionPass":
                        case "ScenePickingPass":
                            // Remove the scene selection and scene picking passes as they are bugged for us, fallbacks work better.
                            break;
                        default:
                            newPasses.Add(pass);
                            break;
                    }

                }

                ssd.passes = newPasses;
                return ssd;
            }
        }
        #endregion

        #region Passes
        public static class Passes
        {
            public static PassDescriptor ProPixelizerMetadata(
                UniversalTarget target,
                BlockFieldDescriptor[] vertexBlocks,
                BlockFieldDescriptor[] pixelBlocks,
                PragmaCollection pragmas,
                KeywordCollection keywords)
            {
                var result = new PassDescriptor
                {
                    // Definition
                    displayName = "ProPixelizerPass",
                    referenceName = "(111)", // Normally this would be a defined name from ShaderPass.hlsl, e.g. SHADERPASS_FORWARD where `#define SHADERPASS_FORWARD (0)`.
                    lightMode = "ProPixelizer",
                    useInPreview = true,

                    // Template
                    passTemplatePath = UniversalTarget.kUberTemplatePath,
                    sharedTemplateDirectories = UniversalTarget.kSharedTemplateDirectories,

                    // Port Mask
                    validVertexBlocks = vertexBlocks,
                    validPixelBlocks = pixelBlocks,

                    // Fields
                    structs = CoreStructCollections.Default,
                    requiredFields = CoreRequiredFields.ShadowCaster,
                    fieldDependencies = CoreFieldDependencies.Default,

                    // Conditional State
                    renderStates = CoreRenderStates.UberSwitchedRenderState(target),
                    pragmas = CorePragmas.Instanced,
                    defines = new DefineCollection { },
                    keywords = new KeywordCollection { CoreKeywords.ShadowCaster },
                    includes = new IncludeCollection { Includes.ProPixelizerMetadata },

                    // Custom Interpolator Support
                    customInterpolators = CoreCustomInterpDescriptors.Common
                };
                result.defines.Add(CoreKeywordDescriptors.AlphaTestOn, 1);
                CorePasses.AddLODCrossFadeControlToPass(ref result, target);
                return result;
            }
        }
        #endregion

        #region Includes
        public static class Includes
        {
            private const string MetadataPassPath = "/ShaderGraph/Includes/ProPixelizerMetadataPass.hlsl";
            private const string PackingUtilsPath = "/SRP/PackingUtils.hlsl";
            public static readonly IncludeCollection ProPixelizerMetadata = new IncludeCollection
            {
                // Pre-graph
                { CoreIncludes.DOTSPregraph },
                { CoreIncludes.CorePregraph },
                { CoreIncludes.ShaderGraphPregraph },

                // Post-graph
                { CoreIncludes.CorePostgraph },
                { Utils.PackageLocation + PackingUtilsPath, IncludeLocation.Postgraph },
                { Utils.PackageLocation + MetadataPassPath, IncludeLocation.Postgraph },
            };
        }
        #endregion
    }
}
#endif