using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace VolumetricLights {

    public class VolumetricLightsTranslucentShadowMapFeature : ScriptableRendererFeature {

        static class ShaderParams {
            public static int MainTex = Shader.PropertyToID("_MainTex");
            public static int CustomTranspBaseMap = Shader.PropertyToID("_BaseMap");
            public static int TranslucencyIntensity = Shader.PropertyToID("_TranslucencyIntensity");
        }


        public class TranspRenderPass : ScriptableRenderPass {

            public VolumetricLightsTranslucentShadowMapFeature settings;

            const string m_ProfilerTag = "TranslucencyPrePass";
            const string m_TranspOnlyShader = "Hidden/VolumetricLights/TransparentMultiply";

            Material transpOverrideMat;
            Material[] transpOverrideMaterials;
            VolumetricLight light;
            RTHandle m_TranslucencyMap;
            ScriptableRenderer renderer;
#if UNITY_2022_2_OR_NEWER
            RTHandle cameraDepth;
#else
            RenderTargetIdentifier cameraDepth;
#endif


            public TranspRenderPass(VolumetricLightsTranslucentShadowMapFeature settings) {
                this.settings = settings;
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            }

            public void Setup(ScriptableRenderer renderer, VolumetricLight light) {
                this.renderer = renderer;
                this.light = light;
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
                base.OnCameraSetup(cmd, ref renderingData);
#if UNITY_2022_2_OR_NEWER
                    cameraDepth = renderer.cameraDepthTargetHandle;
#else
                cameraDepth = renderer.cameraDepthTarget;
#endif

            }
            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                m_TranslucencyMap = RTHandles.Alloc(light.translucentMap);
                ConfigureTarget(m_TranslucencyMap, cameraDepth);
                ConfigureClear(ClearFlag.Color, Color.white);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                if (transpOverrideMat == null) {
                    Shader transpShader = Shader.Find(m_TranspOnlyShader);
                    transpOverrideMat = new Material(transpShader);
                }

                int renderersCount = VolumetricLightsTranslucency.objects.Count;
                if (transpOverrideMaterials == null || transpOverrideMaterials.Length < renderersCount) {
                    transpOverrideMaterials = new Material[renderersCount];
                }

                float intensity = 10f * light.shadowTranslucencyIntensity / (light.brightness + 0.0001f);
                for (int k = 0; k < renderersCount; k++) {
                    VolumetricLightsTranslucency transpObject = VolumetricLightsTranslucency.objects[k];
                    if (transpObject == null || transpObject.theRenderer == null || !transpObject.theRenderer.isVisible) continue;
                    if (transpObject.intensityMultiplier <= 0) continue;

                    Material mat = transpObject.theRenderer.sharedMaterial;
                    if (mat == null) continue;
                    if (transpOverrideMaterials[k] == null) {
                        transpOverrideMaterials[k] = Instantiate(transpOverrideMat);
                    }
                    Material overrideMaterial = transpOverrideMaterials[k];
                    if (mat.HasProperty(ShaderParams.CustomTranspBaseMap)) {
                        overrideMaterial.SetTexture(ShaderParams.MainTex, mat.GetTexture(ShaderParams.CustomTranspBaseMap));
                    } else if (mat.HasProperty(ShaderParams.MainTex)) {
                        overrideMaterial.SetTexture(ShaderParams.MainTex, mat.GetTexture(ShaderParams.MainTex));
                    } else {
                        continue;
                    }
                    overrideMaterial.SetVector(ShaderParams.TranslucencyIntensity, new Vector4(intensity * transpObject.intensityMultiplier, light.shadowTranslucencyBlend, 0, 0));

                    cmd.DrawRenderer(transpObject.theRenderer, overrideMaterial);
                }

                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd) {
            }

            public void CleanUp() {
                RTHandles.Release(m_TranslucencyMap);
                if (transpOverrideMaterials != null) {
                    for (int k = 0; k < transpOverrideMaterials.Length; k++) {
                        DestroyImmediate(transpOverrideMaterials[k]);
                    }
                }
            }

        }

        TranspRenderPass m_ScriptablePass;

        public override void Create() {
            m_ScriptablePass = new TranspRenderPass(this);
        }

        private void OnDestroy() {
            if (m_ScriptablePass != null) {
                m_ScriptablePass.CleanUp();
            }
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {

            int renderersCount = VolumetricLightsTranslucency.objects.Count;
            if (renderersCount == 0) return;


            Camera cam = renderingData.cameraData.camera;
            Transform parent = cam.transform.parent;
            if (parent == null) return;
            VolumetricLight light = parent.GetComponent<VolumetricLight>();
            if (light == null || !light.shadowTranslucency || light.translucentMap == null) return;

            m_ScriptablePass.Setup(renderer, light);
            renderer.EnqueuePass(m_ScriptablePass);
        }

    }



}

