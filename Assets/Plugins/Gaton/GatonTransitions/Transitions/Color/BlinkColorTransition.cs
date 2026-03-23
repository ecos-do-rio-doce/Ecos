using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Blink Color", order: 990)]
    public class BlinkColorTransition : Transition
    {
        [Title("Color Values")]
        [SerializeField] Color startColor;

        [SerializeField] Color endColor;

        [Title("Tween Values")]
        [SerializeField] float blinkingTime = 0.7f;

        [Title("Duration")]
        [SerializeField] float endBlinkingAfterTime = -1;

        Tween blinkColorTween;

        public override float StartDuration => -1;

        public override float EndDuration => -1;

        public override void PlayStartTransition()
        {
            if (blinkColorTween != null)
                blinkColorTween.Kill();

            var rend = GetComponent<Renderer>();

            if (rend != null)
            {
                blinkColorTween = rend.material.DOColor(endColor, blinkingTime)
                          .From(startColor)
                          .SetLoops(-1, LoopType.Yoyo);
            }

            var image = GetComponent<Image>();

            if (image != null)
            {
                blinkColorTween = image.DOColor(endColor, blinkingTime)
                          .From(startColor)
                          .SetLoops(-1, LoopType.Yoyo);
            }

            if (endBlinkingAfterTime > 0)
            {
                CancelInvoke();

                Invoke(nameof(PlayEndTransition), endBlinkingAfterTime);
            }

        }

        public override void PlayEndTransition()
        {
            if (blinkColorTween != null)
                blinkColorTween.Kill();

            blinkColorTween = null;

            var rend = GetComponent<Renderer>();

            if (rend != null)
            {
                rend.material.DOColor(startColor, blinkingTime);
            }

            var image = GetComponent<Image>();

            if (image != null)
            {
                image.DOColor(startColor, blinkingTime);
            }
        }

        public override void GoToStartState()
        {
            var rend = GetComponent<Renderer>();

            if (rend != null)
            {
                rend.material.color = startColor;
            }

            var image = GetComponent<Image>();

            if (image != null)
            {
                image.color = startColor;
            }
        }

        public override void GoToEndState()
        {
            var rend = GetComponent<Renderer>();

            if (rend != null)
            {
                rend.material.color = endColor;
            }

            var image = GetComponent<Image>();

            if (image != null)
            {
                image.color = endColor;
            }
        }

        protected override void OnSkip()
        {

        }

    }
}