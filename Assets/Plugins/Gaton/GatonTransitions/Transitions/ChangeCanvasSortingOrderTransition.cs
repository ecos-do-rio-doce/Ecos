using Sirenix.OdinInspector;
using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Canvas/Change Canvas Sorting Order", order: 999)]
    public class ChangeCanvasSortingOrderTransition : Transition
    {
        [Title("Sorting Orders")]
        [SerializeField] int sortOrderWhenClosed = -1;
        [SerializeField] int sortOrderWhenOpened = 1;

        [SerializeField] bool startsClosed = true;

        Canvas canvas;

        public override float StartDuration => 0;
        public override float EndDuration => 0;

        public new void Awake()
        {
            canvas = GetComponentInParent<Canvas>();

            base.Awake();
        }

        public override void PlayStartTransition()
        {
            canvas.sortingOrder = sortOrderWhenOpened;
        }

        public override void PlayEndTransition()
        {
            canvas.sortingOrder = sortOrderWhenClosed;
        }

        protected override void OnSkip()
        {
            // Is Instant
        }

        public override void GoToStartState()
        {
            var canvas = GetComponentInParent<Canvas>();

            canvas.sortingOrder = startsClosed ? sortOrderWhenClosed : sortOrderWhenOpened;
        }

        public override void GoToEndState()
        {
            var canvas = GetComponentInParent<Canvas>();

            canvas.sortingOrder = startsClosed ? sortOrderWhenClosed : sortOrderWhenOpened;
        }
    }
}