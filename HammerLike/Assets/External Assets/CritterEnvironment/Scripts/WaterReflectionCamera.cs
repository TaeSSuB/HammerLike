using Unity.VisualScripting;
using UnityEngine;

namespace PixelWater
{
    public class WaterReflectionCamera : MonoBehaviour
    {
        // [SerializeField] Texture2D defaultReflectionTexture; // Should be a white pixel with alpha at 0;
        Transform waterPlane;
        Material waterMaterial;
        Camera m_Camera;
        Camera mainCamera;

        Vector2Int waterResolution = new Vector2Int(-1, -1);
        void OnEnable()
        {
            m_Camera = GetComponent<Camera>();
            waterPlane = transform.parent;
            waterMaterial = waterPlane.GetComponent<MeshRenderer>().material;

            mainCamera = Camera.main;

            ApplyNewRenderTexture();
        }
        private void OnDisable()
        {
            if (m_Camera?.targetTexture != null)
                m_Camera.targetTexture.Release();
            // waterMaterial.SetTexture("_WaterReflectionTexture", defaultReflectionTexture);
        }

        private void LateUpdate()
        {

            transform.position = PlanarReflectionProbe.GetPosition(mainCamera.transform.position, waterPlane.position, waterPlane.up);

            transform.LookAt(transform.position + Vector3.Reflect(mainCamera.transform.forward, waterPlane.up), Vector3.Reflect(mainCamera.transform.up, waterPlane.up));

            m_Camera.projectionMatrix = PlanarReflectionProbe.GetObliqueProjection(m_Camera, waterPlane.position, waterPlane.up);

            m_Camera.orthographicSize = Camera.main.orthographicSize;
            if (mainCamera.targetTexture != null)
            {
                UpdateRenderTexture();
            }
        }

        void UpdateRenderTexture()
        {
            if (Camera.main.targetTexture == null)
            {
                Debug.Log("Not implemented error; The camera seeing the water should be rendered to a texture.");
            }
            else if (Camera.main.targetTexture.width != waterResolution.x || Camera.main.targetTexture.height != waterResolution.y)
            {
                ApplyNewRenderTexture();
            }
        }
        void ApplyNewRenderTexture()
        {
            var textureResolution = mainCamera.targetTexture == null ? new Vector2Int(mainCamera.pixelWidth, mainCamera.pixelHeight) : new Vector2Int(mainCamera.targetTexture.width, mainCamera.targetTexture.height);
            var newTexture = NewCameraTargetTexture(textureResolution);
            SetCameraTexture(m_Camera, newTexture);
            waterMaterial.SetTexture("_WaterReflectionTexture", newTexture);
            waterResolution = textureResolution;
        }

        static RenderTexture NewCameraTargetTexture(Vector2Int textureSize) // Creates similar texture as main camera has
        {
            RenderTexture newTexture = new RenderTexture(textureSize.x, textureSize.y, 32, RenderTextureFormat.ARGB32);
            newTexture.filterMode = FilterMode.Point;
            newTexture.Create();

            return newTexture;
        }

        static void SetCameraTexture(Camera camera, RenderTexture renderTexture)
        {
            if (camera?.targetTexture != null)
                camera.targetTexture.Release();

            camera.targetTexture = renderTexture;
        }
    }
}