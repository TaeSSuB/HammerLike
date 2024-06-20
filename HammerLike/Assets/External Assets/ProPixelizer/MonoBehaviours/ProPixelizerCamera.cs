// Copyright Elliot Bentine, 2018-
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProPixelizer
{
    /// <summary>
    /// The ProPixelizerCamera performs all functionality required for ProPixelizer:
    ///  - (i) ProPixelizerCamera handles all snapping of cameras and objects to eliminate pixel creep.
    ///  - (ii) Handles resizing of the camera viewport to maintain desired world-space pixel size.
    ///  - (iii) Calculates deltas of the low-res target to enable sub-pixel camera movement in the low-res target.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("ProPixelizer/ProPixelizerCamera")]
    //[ExecuteAlways]
    public class ProPixelizerCamera : MonoBehaviour
    {
        IEnumerable<ObjectRenderSnapable> _snapables;
        Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            CannotMaintainWSPixelSize = false;
        }

        public enum PixelSizeMode
        {
            FixedWorldSpacePixelSize,
            FixedDownscalingRatio
        }

        //https://github.com/ProPixelizer/ProPixelizer/issues/181
        [Tooltip("Clamps the orthographic size of the camera to maintain fixed world-space pixel size when the low-resolution target " +
            "is at maximum resolution. When disabled, the camera ortho size may increase beyond this limit, but world space pixel size will not be constrained.")]
        public bool ClampCameraOrthoSize = true;

        /// <summary>
        /// Camera ortho size before clamping is applied.
        /// </summary>
        private float _OriginalCameraOrthoSize;

        /// <summary>
        /// Set to true when current projection doesn't maintain desired WS pixel size, eg. due to pixelsize and camera ortho size.
        /// 
        /// (Debug Info for user and editor purposes)
        /// </summary>
        public bool CannotMaintainWSPixelSize { get; private set; }

        /// <summary>
        /// Mode to use this camera snap SRP in.
        /// </summary>
        public PixelSizeMode Mode;

        /// <summary>
        /// World-space unit size of a pixel in the low-res render target.
        /// 
        /// (This is a user-controlled parameter. If you want the actual world space pixel size, see LowResPixelSizeWS)
        /// </summary>
        [Min(0.001f)]
        [Tooltip("World-space unit size of a pixel from a pixelated object.")]
        public float PixelSize = 0.032f;

        /// <summary>
        /// World-space unit size of a pixel in the low-res render target.
        /// </summary>
        public float LowResPixelSizeWS { get; private set; }

        /// <summary>
        /// World-space unit size of a screen pixel.
        /// </summary>
        public float ScreenPixelSizeWS { get; private set; }

        public int LowResTargetWidth { get; private set; }
        public int LowResTargetHeight { get; private set; }

        public float OrthoLowResWidth { get; private set; }
        public float OrthoLowResHeight { get; private set; }

        [Tooltip("Scale of the low-resolution target.")]
        [Range(0.01f, 1.0f)]
        public float TargetScale = 1f;

        [Tooltip("Look-up table to use for full-screen color grading.")]
        public Texture2D FullscreenColorGradingLUT;

        [Tooltip("Should ProPixelizer be used for this camera? You can use this to disable the ProPixelizer rendering passes on specific cameras.")]
        public bool EnableProPixelizer = true;

        public void UpdateCamera()
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
                if (_camera == null)
                    return;
            }

            PixelSize = Mathf.Abs(PixelSize);
            _OriginalCameraOrthoSize = _camera.orthographicSize;

            int width, height;
            switch (Mode)
            {
                case PixelSizeMode.FixedWorldSpacePixelSize:
                    LowResPixelSizeWS = PixelSize;
                    ScreenPixelSizeWS = 2.0f * _camera.orthographicSize / _camera.scaledPixelHeight;

                    float scale = ScreenPixelSizeWS / LowResPixelSizeWS;
                    // For performance reasons, we restrict the size of the low-res target to no greater than the screen size.
                    scale = Mathf.Clamp01(scale);

                    ProPixelizerUtils.CalculateLowResTargetSize(_camera.scaledPixelWidth, _camera.scaledPixelHeight, scale, out width, out height);
                    LowResTargetWidth = width;
                    LowResTargetHeight = height;

                    // we do however need to cap the orthographic size to make sure the camera cannot stretch outside the max view of the pixelated target.
                    // this limit must exclude the margin size - because in principle we aren't supposed to see the margin, which may include border outline
                    // errors, etc.
                    float cameraOrthoSizeLimit = (LowResTargetHeight - 2 * ProPixelizerUtils.LOW_RESOLUTION_TARGET_MARGIN) / 2f * LowResPixelSizeWS;
                    if (cameraOrthoSizeLimit < _camera.orthographicSize)
                    {
                        if (ClampCameraOrthoSize)
                            _camera.orthographicSize = Mathf.Max(Mathf.Min(_camera.orthographicSize, cameraOrthoSizeLimit), 0f);
                        else
                        {
                            // If we don't clamp ortho size, we must adjust the low-res pixel size WS instead.    
                            LowResPixelSizeWS = 2f * _camera.orthographicSize / LowResTargetHeight;
                            CannotMaintainWSPixelSize = true;
                        }
                    }
                    break;
                case PixelSizeMode.FixedDownscalingRatio:
                    // manually set low-resolution target
                    ProPixelizerUtils.CalculateLowResTargetSize(_camera.scaledPixelWidth, _camera.scaledPixelHeight, TargetScale, out width, out height);
                    LowResTargetWidth = width;
                    LowResTargetHeight = height;
                    LowResPixelSizeWS = 2f * _camera.orthographicSize / LowResTargetHeight;
                    PixelSize = LowResPixelSizeWS;
                    break;
            }

            OrthoLowResWidth = LowResTargetWidth * LowResPixelSizeWS;
            OrthoLowResHeight = LowResTargetHeight * LowResPixelSizeWS;
        }

        public void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
            RenderPipelineManager.endCameraRendering += EndCameraRendering;
            RenderPipelineManager.beginFrameRendering += BeginFrameRendering;
            RenderPipelineManager.endFrameRendering += EndFrameRendering;
        }

        public void OnDisable() => Unsubscribe();

        public void Unsubscribe()
        {
            RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= EndCameraRendering;
            RenderPipelineManager.beginFrameRendering -= BeginFrameRendering;
            RenderPipelineManager.endFrameRendering -= EndFrameRendering;
        }

        public void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera == _camera)
                Snap();
        }

        public void EndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera == _camera)
                Release();
        }

        public void BeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
        {
            UpdateCamera();
        }

        public void EndFrameRendering(ScriptableRenderContext context, Camera[] cameras)
        {
            // Undo any orthographic size clamping that was performed to maintain WS pixel size.
            if (_camera != null)
                _camera.orthographicSize = _OriginalCameraOrthoSize;
        }

        public Vector3 PreSnapCameraPosition { get; private set; }

        public Quaternion PreSnapCameraRotation { get; private set; }

        /// <summary>
        /// Coordinates denoting the centre of the screen camera's view on the low-res target.
        /// UV-space.
        /// </summary>
        public Vector2 LowResCameraDeltaUV {  get; private set; }

        /// <summary>
        /// Shift of the low-resolution (phantom) camera relative to the high-screen screen camera.
        /// UV-space.
        /// </summary>
        public Vector3 LowResCameraDeltaWS { get; private set; }

        public void Snap()
        {
            if (_camera == null)
            {
                // This happens if the camera object gets deleted - but rendering pipeline asset still exists.
                Unsubscribe();
                return;
            }

            var pipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (pipelineAsset == null)
                throw new System.Exception("ProPixelizer requires the Universal Render Pipeline, but no UniversalRenderPipelineAsset is currently in use. Check you have assigned a RenderPipelineAsset in QualitySettings and/or GraphicsSettings.");
            float renderScale = pipelineAsset.renderScale;

            PreSnapCameraPosition = transform.position;
            PreSnapCameraRotation = transform.rotation;

            //Find all objects required to snap and release.
#if UNITY_2022_1_OR_NEWER
            _snapables = new List<ObjectRenderSnapable>(FindObjectsByType<ObjectRenderSnapable>(FindObjectsSortMode.None));
#else
            _snapables = new List<ObjectRenderSnapable>(FindObjectsOfType<ObjectRenderSnapable>());
#endif

            //Sort snapables so that parents are snapped before their children.
            ((List<ObjectRenderSnapable>)_snapables).Sort((comp1, comp2) => comp1.TransformDepth.CompareTo(comp2.TransformDepth));

            foreach (ObjectRenderSnapable snapable in _snapables)
                snapable.SaveTransform();

            foreach (ObjectRenderSnapable snapable in _snapables)
                if (snapable.enabled)
                    snapable.SnapAngles(this);

            // We want to calculate low-resolution transforms based on the unsnapped camera postion.
            // Although ProPixelizerCamera doesn't require ObjectRenderSnapable on the same transform,
            // there's nothing to prevent a user from adding it, or nesting the camera in a heirachy.
            // Thus we assert here that the camera position and rotation must be the _unsnapped_ versions
            // for the calculations.
            transform.rotation = PreSnapCameraRotation;
            transform.position = PreSnapCameraPosition;

            _camera.ResetWorldToCameraMatrix();
            Matrix4x4 camToWorld = _camera.cameraToWorldMatrix;
            Matrix4x4 worldToCam = _camera.worldToCameraMatrix;

            // Shift the camera so that the origin aligns with a pixel in the low-res target.
            float bias = 0.5f;
            Vector3 originPosCS = worldToCam.MultiplyPoint(new Vector3());
            var roundedOriginPosCS = LowResPixelSizeWS * new Vector3(
                        Mathf.RoundToInt(originPosCS.x / LowResPixelSizeWS) + bias,
                        Mathf.RoundToInt(originPosCS.y / LowResPixelSizeWS) + bias,
                        Mathf.RoundToInt(originPosCS.z / LowResPixelSizeWS) + bias
                );
            LowResCameraDeltaWS = camToWorld.MultiplyPoint(roundedOriginPosCS);
            camToWorld = Matrix4x4.Translate(-LowResCameraDeltaWS) * _camera.cameraToWorldMatrix;
            worldToCam = _camera.worldToCameraMatrix * Matrix4x4.Translate(LowResCameraDeltaWS);

            // delta sized in world-space units, in the space of the camera.
            var LowResCameraDeltaCS = roundedOriginPosCS - originPosCS;
            LowResCameraDeltaUV = new Vector2(
                LowResCameraDeltaCS.x / OrthoLowResWidth,
                LowResCameraDeltaCS.y / OrthoLowResHeight
                );
            // compare this to LowResTargetWidth and Height to determine the centre of the origin-snapped low-res target relative
            // to the unsnapped screen target camera. This coordinate shift should be added to 0.5, 0.5 and used as centre.

            // Note that low res target render matrix should also use the shifted position in WS units.

            // Snap all objects to integer pixels in camera space.
            foreach (ObjectRenderSnapable snapable in _snapables)
            {
                if (!snapable.enabled)
                    continue;
                Vector3 snappedPosition;
                if (snapable.AlignPixelGrid)
                {
                    var rootCamSpace = worldToCam.MultiplyPoint(snapable.PixelGridReferencePosition);
                    var snapableCamSpace = worldToCam.MultiplyPoint(snapable.WorldPositionPreSnap);
                    var delta = (snapableCamSpace - rootCamSpace);
                    var snapLength = snapable.GetPixelSize() * LowResPixelSizeWS;
                    var roundedDelta = new Vector3(
                        Mathf.RoundToInt(delta.x / snapLength) + snapable.OffsetBias,
                        Mathf.RoundToInt(delta.y / snapLength) + snapable.OffsetBias,
                        Mathf.RoundToInt(delta.z / snapLength) + snapable.OffsetBias
                        );
                    var roundedRootCamSpace = new Vector3(
                        Mathf.RoundToInt(rootCamSpace.x / LowResPixelSizeWS),
                        Mathf.RoundToInt(rootCamSpace.y / LowResPixelSizeWS),
                        Mathf.RoundToInt(rootCamSpace.z / LowResPixelSizeWS)
                        );
                    // roundedDelta includes an offset bias to snap to half-integer (ie, centre pixel) positions.
                    snappedPosition = camToWorld.MultiplyPoint(LowResPixelSizeWS * roundedRootCamSpace + roundedDelta * snapLength);
                }
                else
                {
                    Vector3 pixelPosCamSpace = worldToCam.MultiplyPoint(snapable.transform.position) / LowResPixelSizeWS;
                    var roundedPos = new Vector3(
                        Mathf.RoundToInt(pixelPosCamSpace.x) + snapable.OffsetBias,
                        Mathf.RoundToInt(pixelPosCamSpace.y) + snapable.OffsetBias,
                        Mathf.RoundToInt(pixelPosCamSpace.z) + snapable.OffsetBias
                        );
                    snappedPosition = camToWorld.MultiplyPoint(roundedPos * LowResPixelSizeWS);
                }


                if (snapable.SnapPosition && LowResPixelSizeWS > float.Epsilon)
                    snapable.transform.position = snappedPosition;
            }

            // Restore camera position to unsnapped location.
            transform.position = PreSnapCameraPosition;
            transform.rotation = PreSnapCameraRotation;
        }

        public void Release()
        {
            // Note: the `release' loop is run in reverse.
            // This prevents shaking and movement from occuring when
            // parent and child transforms are both Snapable.
            //  e.g. For a heirachy A>B:
            //   * Snap A, then B.
            //   * Unsnap B, then A.
            // Doing Unsnap A, then unsnap B will produce jerking.
            foreach (ObjectRenderSnapable snapable in _snapables.Reverse())
                snapable.RestoreTransform();
            // Also reset camera explicitly. Otherwise, when camera is child of a rotation or position snapped object,
            // The 'unsnapping' of the camera before render will leave the camera unsnapped when the object transform
            // is snapped, and restore transform will create drift.
            transform.position = PreSnapCameraPosition;
            transform.rotation = PreSnapCameraRotation;
        }

        public void ModifySettings(ref ProPixelizerSettings settings)
        {
            // camera specific overrides of settings.
            if (overrideColorGrading && FullscreenColorGradingLUT != null)
                settings.FullscreenLUT = FullscreenColorGradingLUT;
            settings.Enabled = EnableProPixelizer;
            if (overridePixelisationFilter)
                settings.Filter = PixelizationFilter;
            if (overrideUsePixelExpansion)
                settings.UsePixelExpansion = UsePixelExpansion;
        }

        public bool overrideUsePixelExpansion;
        public bool overridePixelisationFilter;
        public bool overrideColorGrading;
        [Tooltip("Whether to perform ProPixelizer's 'Pixel Expansion' method of pixelation. This is required for per-object pixelisation and for moving pixelated objects at apparent sub-pixel resolutions. See ProPixelizer Render Feature.")]
        public bool UsePixelExpansion = true;
        [Tooltip("Controls which objects in the scene should be pixelated. See ProPixelizer Render Feature.")]
        public PixelizationFilter PixelizationFilter;
    }
}