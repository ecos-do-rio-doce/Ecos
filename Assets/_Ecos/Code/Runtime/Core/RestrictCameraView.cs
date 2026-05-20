using UnityEngine;

namespace Ecos
{
    public class RestrictCameraView : MonoBehaviour
    {
        [SerializeField] Camera cam;
        [SerializeField] Vector2 aspectRatio = new Vector2(16, 9);

        Vector2Int lastScreenValues;

        private void Start()
        {
            lastScreenValues = Vector2Int.zero;
        }

        void Update()
        {
            if (lastScreenValues.x != Screen.width || lastScreenValues.y != Screen.height)
            {
                lastScreenValues = new Vector2Int(Screen.width, Screen.height);

                UpdateCameraRect(cam);
            }
        }

        void UpdateCameraRect(Camera camera)
        {
            float targetAspect = aspectRatio.x / aspectRatio.y;
            float windowAspect = (float)Screen.width / Screen.height;
            float scaleHeight = windowAspect / targetAspect;

            if (scaleHeight < 1f)
            {
                // Add black borders on top
                Rect rect = camera.rect;
                rect.width = 1f;
                rect.height = scaleHeight;
                rect.x = 0f;
                rect.y = (1f - scaleHeight) / 2f;
                camera.rect = rect;
            }
            else
            {
                // Add black borders on sides
                float scaleWidth = 1f / scaleHeight;
                Rect rect = camera.rect;
                rect.width = scaleWidth;
                rect.height = 1f;
                rect.x = (1f - scaleWidth) / 2f;
                rect.y = 0f;
                camera.rect = rect;
            }
        }
    }
}
