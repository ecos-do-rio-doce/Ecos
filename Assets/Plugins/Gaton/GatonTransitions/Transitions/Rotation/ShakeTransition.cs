using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Shake", order: 2)]
    public class ShakeTransition : Transition
    {
        [Title("Tween")]
        [SerializeField] float delay = 0;

        [Title("Shake Values")]
        [SerializeField] float duration = 0.5f;

        [SerializeField] Vector3 strenght = new Vector3(90, 90, 90);

        [SerializeField] int vibrato = 10;

        [SerializeField] float elasticity = 1f;

        Quaternion startRotation;

        Tween rotationTween;

        public override float StartDuration => duration;
        public override float EndDuration => duration;

        public new void Awake()
        {
            base.Awake();

            startRotation = transform.localRotation;
        }

        public override void PlayStartTransition()
        {
            if (rotationTween != null)
                rotationTween.Kill();

            transform.localRotation = startRotation;

            rotationTween = transform.DOPunchRotation(strenght, duration, vibrato, elasticity);

            rotationTween.OnComplete(OnFinish);

            if (delay > 0)
                rotationTween.SetDelay(delay);

            TransitionsUtilities.SetTweenToUnscaled(rotationTween, unscaledTime);
        }

        public override void PlayEndTransition()
        {

        }

        private void OnFinish()
        {
            rotationTween = null;
        }

        public override void GoToStartState()
        {

        }

        public override void GoToEndState()
        {

        }
        protected override void OnSkip()
        {
            if (rotationTween != null)
                rotationTween.Complete();
        }







    }
}