using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Rotate Transition")]
    public class RotateTransition : Transition
    {
        [Title("Play")]
        [SerializeField] bool playOnStart = false;

        [Title("Rotate Values")]
        [SerializeField] bool localRotation = false;

        [SerializeField] float duration = 0.5f;

        [SerializeField] Vector3 startRotation = Vector3.zero;

        [SerializeField] Vector3 endRotation = Vector3.zero;

        [Title("Delay")]
        [SerializeField] float startDelay = 0;

        [SerializeField] float endDelay = 0;

        [Title("Ease")]
        [SerializeField] Ease startEase;

        [SerializeField] Ease endEase;

        [SerializeField] RotateMode rotateMode = RotateMode.Fast;

        public override float StartDuration => duration;

        public override float EndDuration => duration;


        private void Start()
        {
            if (playOnStart)
                PlayStartTransition();
        }

        public override void GoToStartState()
        {
            if (localRotation)
                transform.localRotation = Quaternion.Euler(startRotation);
            else
                transform.rotation = Quaternion.Euler(startRotation);
        }
        public override void GoToEndState()
        {
            if (localRotation)
                transform.localRotation = Quaternion.Euler(endRotation);
            else
                transform.rotation = Quaternion.Euler(endRotation);
        }

        public override void PlayStartTransition()
        {
            Tween tween;
            if (localRotation)
                tween = transform.DOLocalRotate(endRotation, duration, rotateMode).SetEase(startEase).SetDelay(startDelay);
            else
                tween = transform.DORotate(endRotation, duration, rotateMode).SetEase(startEase).SetDelay(startDelay);

            TransitionsUtilities.SetTweenToUnscaled(tween, unscaledTime);
        }

        public override void PlayEndTransition()
        {
            Tween tween;
            if (localRotation)
                tween = transform.DOLocalRotate(startRotation, duration, rotateMode).SetEase(endEase).SetDelay(endDelay);
            else
                tween = transform.DORotate(startRotation, duration, rotateMode).SetEase(endEase).SetDelay(endDelay);

            TransitionsUtilities.SetTweenToUnscaled(tween, unscaledTime);
        }

        protected override void OnSkip()
        {
            transform.DOComplete();
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}