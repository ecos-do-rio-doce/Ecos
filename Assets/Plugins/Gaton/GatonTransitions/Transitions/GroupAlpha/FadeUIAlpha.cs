using DG.Tweening;
using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Canvas/Fade Canvas Alpha", order: 997)]
    public class FadeUIAlpha : ChangeCanvasAlpha
    {
        [SerializeField] float duration = 0.2f;

        [SerializeField] float targetAlpha = 0;

        public override float StartDuration => duration;
        public override float EndDuration => duration;

        public override void Activate(bool isVisible)
        {
            base.Activate(isVisible);

            var tween = canvasGroup.DOFade(isVisible ? 1 : targetAlpha, duration);

            TransitionsUtilities.SetTweenToUnscaled(tween, unscaledTime);
        }

        protected override void OnSkip()
        {
            // TODO
        }
    }
}