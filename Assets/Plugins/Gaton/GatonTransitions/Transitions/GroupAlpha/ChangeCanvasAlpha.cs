using UnityEngine;

namespace Gaton.Transitions
{
    public abstract class ChangeCanvasAlpha : Transition
    {
        protected CanvasGroup canvasGroup;

        public new void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            base.Awake();
        }

        public virtual void Activate(bool isVisible)
        {
            canvasGroup.interactable = isVisible;

            canvasGroup.blocksRaycasts = isVisible;
        }

        public override void PlayStartTransition()
        {
            Activate(true);
        }

        public override void PlayEndTransition()
        {
            Activate(false);
        }

        public override void GoToStartState()
        {
            var canvasGroup = GetComponent<CanvasGroup>();

            canvasGroup.alpha = 0;
        }

        public override void GoToEndState()
        {
            var canvasGroup = GetComponent<CanvasGroup>();

            canvasGroup.alpha = 1;
        }
    }
}