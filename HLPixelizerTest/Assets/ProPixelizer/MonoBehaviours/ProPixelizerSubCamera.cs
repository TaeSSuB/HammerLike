// Copyright Elliot Bentine, 2018-
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProPixelizer
{
    /// <summary>
    /// ProPixelizer's subcamera is used for rendering a low-resolution view of the scene.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class ProPixelizerSubCamera : MonoBehaviour
    {
        Camera _camera;
        RTHandle _Color;
        RTHandle _Depth;
        ProPixelizerCamera _proPixelizerCamera;

        public RTHandle Color { get { return _Color; } }
        public RTHandle Depth { get { return _Depth; } }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _proPixelizerCamera = transform.parent.GetComponent<ProPixelizerCamera>();
            _proPixelizerCamera.SubCamera = this;
            var parentCamera = transform.parent.GetComponent<Camera>();
            if (parentCamera != null)
            {
                _camera.depth = parentCamera.depth - 1;
            }
            Configure();
        }

        public void Configure()
        {
            var data = _camera.GetUniversalAdditionalCameraData();
            data.renderPostProcessing = false;
            data.antialiasing = AntialiasingMode.None;
            data.requiresColorTexture = false;
            data.requiresDepthTexture = false;
        }

        public void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
            RenderPipelineManager.endCameraRendering += EndCameraRendering;
        }

        public void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= EndCameraRendering;
            _camera.targetTexture = null;
            _Color?.Release();
            _Depth?.Release();
        }

        public void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera != _camera)
                return;

            // perform snapping of associated ProPixelizer camera
            _proPixelizerCamera.Snap();

            // For an orthographic camera, adjust parameters to match the low-resolution target settings.
            if (camera.orthographic)
            {
                camera.orthographicSize = _proPixelizerCamera.OrthoLowResHeight / 2f;
                transform.localPosition = _proPixelizerCamera.LowResCameraDeltaWS;
            }

            var colorDescriptor = new RenderTextureDescriptor(
                _proPixelizerCamera.LowResTargetWidth, _proPixelizerCamera.LowResTargetHeight, RenderTextureFormat.Default, 0);
            RenderingUtils.ReAllocateIfNeeded(
                ref _Color, in colorDescriptor,
                name: "SubCameraColor",
                filterMode: FilterMode.Point,
                wrapMode: TextureWrapMode.Clamp
                );
            var depthDescriptor = colorDescriptor;
            depthDescriptor.colorFormat = RenderTextureFormat.Depth;
            depthDescriptor.depthBufferBits = 32;
            RenderingUtils.ReAllocateIfNeeded(
                ref _Depth, in depthDescriptor,
                name: "SubCameraDepth",
                filterMode: FilterMode.Point,
                wrapMode: TextureWrapMode.Clamp
                );

            camera.targetTexture = _Color;
        }

        public void EndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera != _camera)
                return;

            // unsnap associated ProPixelizer camera
            _proPixelizerCamera.Release();
        }
    }
}