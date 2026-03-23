using Sirenix.OdinInspector;
using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Bounce", order: -1)]
    public class BounceTransition : Transition
    {
        [Title("Bounce Moments")]
        [SerializeField] bool bounceOnStart = true;
        [SerializeField] bool bounceOnEnd = false;

        [Title("Values")]
        [SerializeField] BounceTweenValues bounceValues;

        BounceTween bounceTween;

        public override float StartDuration => BounceValues.animationDuration;

        public override float EndDuration => 0f;

        public BounceTweenValues BounceValues { get => bounceValues; set => bounceValues = value; }

        public new void Awake()
        {
            base.Awake();

            CreateBounceTween();
        }

        public override void GoToStartState()
        {
            if (bounceTween != null)
                transform.localScale = new Vector3(bounceTween.StartScale, bounceTween.StartScale, bounceTween.StartScale);
        }
        public override void GoToEndState()
        {
            if (bounceTween != null)
                transform.localScale = new Vector3(bounceTween.StartScale, bounceTween.StartScale, bounceTween.StartScale);
        }
        public override void PlayStartTransition()
        {
            if (!bounceOnStart)
                return;

            bounceTween.DoBounce(BounceValues, unscaledTime);
        }

        public override void PlayEndTransition()
        {
            if (!bounceOnEnd)
                return;

            bounceTween.DoBounce(BounceValues, unscaledTime);
        }

        protected override void OnSkip()
        {
            bounceTween.Skip();
        }

        [Button]
        void CreateBounceTween()
        {
            bounceTween = new BounceTween(transform);
        }


    }
}