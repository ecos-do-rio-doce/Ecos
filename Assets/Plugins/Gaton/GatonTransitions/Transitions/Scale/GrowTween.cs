using DG.Tweening;
using UnityEngine;

namespace Gaton.Transitions
{
    public class GrowTween
    {
        Transform transform;

        Tween growTween;
        Tween shrinkTween;

        float increaseValue;
        float growSpeed;

        Vector3 startScale;

        bool isSpeedBased;

        bool unscaledTime;

        float startDelay;
        float endDelay;

        public GrowTween(Transform transform, float increaseValue, float growSpeed, Vector3 startScale, bool speedBased = true, bool unscaledTime = false, float startDelay = 0, float endDelay = 0)
        {
            this.transform = transform;

            this.increaseValue = increaseValue;
            this.growSpeed = growSpeed;

            transform.localScale = startScale;

            this.startScale = startScale;

            this.isSpeedBased = speedBased;

            this.unscaledTime = unscaledTime;

            this.startDelay = startDelay;

            this.endDelay = endDelay;
        }

        public void Grow()
        {
            TryToKill(growTween);

            TryToKill(shrinkTween);

            growTween = transform.DOScale(startScale.x + increaseValue, growSpeed)
                .OnKill(OnFinishGrow);

            if (startDelay > 0)
                growTween.SetDelay(startDelay);

            if (isSpeedBased)
                growTween.SetSpeedBased();

            TransitionsUtilities.SetTweenToUnscaled(growTween, unscaledTime);
        }

        public void Shrink()
        {
            TryToKill(growTween);

            TryToKill(shrinkTween);

            shrinkTween = transform.DOScale(startScale, growSpeed)
                .OnKill(OnFinishShrink);

            if (endDelay > 0)
                shrinkTween.SetDelay(endDelay);

            if (isSpeedBased)
                shrinkTween.SetSpeedBased();

            TransitionsUtilities.SetTweenToUnscaled(shrinkTween, unscaledTime);
        }

        private void OnFinishGrow()
        {
            growTween = null;
        }

        private void OnFinishShrink()
        {
            shrinkTween = null;
        }

        public void SkipGrow()
        {
            growTween.Pause();

            shrinkTween.Pause();

            transform.localScale = startScale + new Vector3(increaseValue, increaseValue, increaseValue);
        }

        public void DestroyTweens()
        {
            if (growTween != null)
                growTween.Kill();

            if (shrinkTween != null)
                shrinkTween.Kill();
        }

        void TryToKill(Tween tween)
        {
            if (tween != null)
                tween.Kill();
        }
    }
}