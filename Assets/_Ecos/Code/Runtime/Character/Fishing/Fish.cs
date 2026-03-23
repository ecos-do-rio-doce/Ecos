using Sirenix.OdinInspector;
using UnityEngine;

namespace Ecos
{
    public class Fish : MonoBehaviour
    {
        [SerializeField, OnValueChanged(nameof(UpdateImageSize))] float fishSize;
        public float FishSize => fishSize;

        RectTransform rectTransform;
        public RectTransform RectTransform => rectTransform;

        public float FishPos => rectTransform.localPosition.x;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void UpdateImageSize()
        {
#if UNITY_EDITOR
            rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(fishSize, rectTransform.sizeDelta.y);
#endif
        }
    }
}
