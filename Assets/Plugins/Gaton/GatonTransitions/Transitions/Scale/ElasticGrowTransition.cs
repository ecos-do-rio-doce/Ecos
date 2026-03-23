using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Elastic Grow", order: -8)]
    public class ElasticGrowTransition : Transition
    {
        [Title("Grow Values")]
        [SerializeField] float growDuration = 0.8f;
        [SerializeField] float shrinkDuration = 0.1f;

        [SerializeField] float overshoot = 0.1f;

        [SerializeField] bool setEasePeriod = false;

        [SerializeField, ShowIf(nameof(setEasePeriod))] float easePeriod = 1f;

        [Title("Tween Values")]

        [SerializeField] float delayToStart = 0;

        [Title("Scale Values")]
        [SerializeField] Vector3 startScale = Vector3.zero;
        [SerializeField] Vector3 endScale = Vector3.one;

        Tween changeScaleTween;

        public override float StartDuration => growDuration + DelayToStart;
        public override float EndDuration => shrinkDuration;

        public float DelayToStart { get => delayToStart; set => delayToStart = value; }
        public float ShrinkDuration { set => shrinkDuration = value; }
        public float GrowDuration { set => growDuration = value; }
        public float Overshoot { set => overshoot = value; }
        public float EasePeriod { set => easePeriod = value; }
        public Vector3 EndScale { get => endScale; set => endScale = value; }

        public new void Awake()
        {
            transform.localScale = startScale;

            base.Awake();
        }

        public override void PlayStartTransition()
        {
            if (changeScaleTween != null)
                changeScaleTween.Kill();



            changeScaleTween = transform.DOScale(EndScale, growDuration).SetEase(Ease.OutElastic).OnComplete(OnFinish);

            changeScaleTween.easeOvershootOrAmplitude = overshoot;

            if (setEasePeriod)
                changeScaleTween.easePeriod = easePeriod;

            if (DelayToStart > 0)
                changeScaleTween.SetDelay(DelayToStart);

            TransitionsUtilities.SetTweenToUnscaled(changeScaleTween, unscaledTime);
        }

        public override void PlayEndTransition()
        {
            if (changeScaleTween != null)
                changeScaleTween.Kill();

            changeScaleTween = transform.DOScale(startScale, shrinkDuration).OnComplete(OnFinish);

            TransitionsUtilities.SetTweenToUnscaled(changeScaleTween, unscaledTime);
        }

        protected override void OnSkip()
        {
            if (changeScaleTween != null)
                changeScaleTween.Complete(true);
        }

        private void OnFinish()
        {
            changeScaleTween = null;
        }

        public override void GoToStartState()
        {
            transform.localScale = startScale;
        }

        public override void GoToEndState()
        {
            transform.localScale = EndScale;
        }

        private void OnDestroy()
        {
            if (changeScaleTween != null)
                changeScaleTween.Kill();
        }
    }
}