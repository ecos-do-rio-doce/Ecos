using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Change Scale by Axis", order: 3)]
    public class ChangeAxisScaleTransition : Transition
    {
        [Title("Grow Values")]
        [SerializeField] Vector3 startValue;

        [SerializeField] Vector3 endValue = Vector3.one;

        [SerializeField] float duration = 0.5f;

        [SerializeField] Ease ease;

        [Title("Tween Values")]
        [SerializeField] float delayToStart = 0f;

        public override float StartDuration => duration;
        public override float EndDuration => duration;

        public new void Awake()
        {
            transform.localScale = startValue;

            base.Awake();
        }

        public override void PlayStartTransition()
        {
            transform.DOKill(true);

            var tween = transform.DOScale(endValue, duration).From(startValue).SetEase(ease);

            if (delayToStart > 0f)
                tween.SetDelay(delayToStart);

            TransitionsUtilities.SetTweenToUnscaled(tween, unscaledTime);
        }

        public override void PlayEndTransition()
        {
            transform.DOKill(true);

            var tween = transform.DOScale(startValue, duration);

            TransitionsUtilities.SetTweenToUnscaled(tween, unscaledTime);
        }

        protected override void OnSkip()
        {
            transform.DOComplete(true);
        }

        public override void GoToStartState()
        {
            transform.localScale = startValue;
        }

        public override void GoToEndState()
        {
            transform.localScale = endValue;
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}