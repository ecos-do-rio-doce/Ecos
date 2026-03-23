using DG.Tweening;
using UnityEngine;

namespace Gaton.Transitions
{
    public static class TransitionsUtilities
    {
        public static void SetTweenToUnscaled(Tween tween, bool isUnscaled)
        {
            if (isUnscaled)
                tween.SetUpdate(true);
        }
    }
}
