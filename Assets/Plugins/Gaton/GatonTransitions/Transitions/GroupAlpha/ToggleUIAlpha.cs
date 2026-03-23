using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Canvas/Toggle Canvas Alpha", order: 998)]
    public class ToggleUIAlpha : ChangeCanvasAlpha
    {
        public override float StartDuration => 0;
        public override float EndDuration => 0;

        public override void Activate(bool isVisible)
        {
            base.Activate(isVisible);

            float alpha = isVisible ? 1 : 0;

            canvasGroup.alpha = alpha;
        }


        protected override void OnSkip()
        {
            // Is Instant
        }
    }
}