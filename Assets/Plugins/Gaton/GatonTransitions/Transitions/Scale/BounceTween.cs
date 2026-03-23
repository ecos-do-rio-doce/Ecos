using DG.Tweening;
using UnityEngine;

namespace Gaton.Transitions
{
    public class BounceTween
    {
        Transform transform;

        Tween growAndShrinkTween;

        bool isBouncing = false;

        public float StartScale { get; set; }
        public bool IsBouncing { get => isBouncing; set => isBouncing = value; }
        public Transform Transform { get => transform; set => transform = value; }

        public BounceTween(Transform transform)
        {
            this.Transform = transform;

            StartScale = transform.localScale.x;
        }

        public void DoBounce(BounceTweenValues values, bool unscaledTime = false)
        {
            DoBounce(values.scaleIncrease, values.animationDuration, values.vibrato, values.elasticity, unscaledTime);
        }

        public void DoBounce(float scaleIncrease, float animationDuration, int vibrato, float elasticity, bool unscaledTime = false)
        {
            if (growAndShrinkTween != null && growAndShrinkTween.IsPlaying())
            {
                growAndShrinkTween.Kill();
            }

            Transform.localScale = new Vector3(StartScale, StartScale, StartScale);

            Vector3 punch = new Vector3(scaleIncrease, scaleIncrease, scaleIncrease);

            growAndShrinkTween = Transform.DOPunchScale(punch, animationDuration, vibrato, elasticity)
                                          .OnKill(OnFinishTween);

            TransitionsUtilities.SetTweenToUnscaled(growAndShrinkTween, unscaledTime);

            IsBouncing = true;
        }

        public void Skip()
        {
            if (growAndShrinkTween != null && growAndShrinkTween.IsPlaying())
            {
                growAndShrinkTween.Complete(true);
            }
        }

        void OnFinishTween()
        {
            growAndShrinkTween = null;

            IsBouncing = false;
        }
    }

    [System.Serializable]
    public class BounceTweenValues
    {
        public float scaleIncrease = 0.05f;

        public float animationDuration = 0.5f;

        public int vibrato = 9;

        public float elasticity = 1;
    }
}