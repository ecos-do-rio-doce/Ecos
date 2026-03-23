using Sirenix.OdinInspector;
using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Grow", order: -9)]
    public class GrowTransition : Transition
    {
        [Title("Growth Values")]
        [SerializeField] float increaseValue = 1f;
        [SerializeField] float duration = 0.4f;

        [Title("Scale")]
        [SerializeField] bool setScaleToZeroAtAwake = true;

        [Title("Tween")]
        [SerializeField] float startDelay = 0;
        [SerializeField] float endDelay = 0;

        GrowTween growTween;

        public override float StartDuration => duration;
        public override float EndDuration => duration;

        public GrowTween GrowTween
        {
            get
            {
                if (growTween == null)
                    CreateGrowTween();

                return growTween;
            }
            set
            {
                growTween = value;
            }
        }

        [Button]
        public void CreateGrowTween()
        {
            growTween = new GrowTween(transform, increaseValue, duration, transform.localScale, speedBased: false, unscaledTime: unscaledTime, startDelay: startDelay, endDelay: endDelay);
        }

        public new void Awake()
        {
            if (setScaleToZeroAtAwake)
                transform.localScale = Vector3.zero;

            CreateGrowTween();

            base.Awake();
        }

        public override void PlayStartTransition()
        {
            GrowTween.Grow();
        }

        public override void PlayEndTransition()
        {
            GrowTween.Shrink();
        }

        protected override void OnSkip()
        {
            GrowTween.SkipGrow();
        }

        public override void GoToStartState()
        {
            transform.localScale = Vector3.zero;
        }

        public override void GoToEndState()
        {
            transform.localScale = Vector3.one * increaseValue;
        }

        private void OnDestroy()
        {
            GrowTween.DestroyTweens();
        }
    }
}