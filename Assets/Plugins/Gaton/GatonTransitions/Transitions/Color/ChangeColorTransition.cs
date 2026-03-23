using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Change Color", order: 1)]
    public class ChangeColorTransition : Transition
    {
        [SerializeField] Color colorWhenHidden = Color.white;
        [SerializeField] Color colorWhenOpened = Color.white;

        [SerializeField] float timeToChangeColor = 0.3f;

        [SerializeField] float delayToStart = 0f;

        [SerializeField] Ease startEase;
        [SerializeField] Ease endEase;

        Graphic image;

        Tween colorChange;

        public Color ColorWhenHidden { get => colorWhenHidden; set => colorWhenHidden = value; }
        public Color ColorWhenOpenned { get => colorWhenOpened; set => colorWhenOpened = value; }

        public override float StartDuration => timeToChangeColor;
        public override float EndDuration => timeToChangeColor;

        public new void Awake()
        {
            image = GetComponent<Graphic>();

            base.Awake();
        }

        public override void PlayStartTransition()
        {
            TryToKillPrevious();

            colorChange = image.DOColor(ColorWhenOpenned, timeToChangeColor).OnComplete(KillTween).SetEase(startEase);

            if (delayToStart > 0f)
                colorChange.SetDelay(delayToStart);

            TransitionsUtilities.SetTweenToUnscaled(colorChange, unscaledTime);
        }

        public override void PlayEndTransition()
        {
            TryToKillPrevious();

            colorChange = image.DOColor(ColorWhenHidden, timeToChangeColor).OnComplete(KillTween).SetEase(endEase);

            TransitionsUtilities.SetTweenToUnscaled(colorChange, unscaledTime);
        }



        protected override void OnSkip()
        {
            if (colorChange != null)
                colorChange.Complete(true);
        }

        private void TryToKillPrevious()
        {
            if (colorChange != null)
                colorChange.Kill();
        }

        private void KillTween()
        {
            colorChange = null;
        }

        public override void GoToStartState()
        {
            var image = GetComponent<Graphic>();

            image.color = ColorWhenHidden;
        }

        public override void GoToEndState()
        {
            var image = GetComponent<Graphic>();

            image.color = ColorWhenOpenned;
        }
    }
}