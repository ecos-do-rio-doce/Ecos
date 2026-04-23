using UnityEngine;
using UnityEngine.Events;

namespace Ecos
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private UnityEvent<float> onUpdatedPercentage;

        int currentValue;
        public int CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
                onUpdatedPercentage?.Invoke((float)currentValue/maxValue);
            }
        }

        int maxValue;
        public int MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                onUpdatedPercentage?.Invoke((float)currentValue / maxValue);
            }
        }

    }
}
