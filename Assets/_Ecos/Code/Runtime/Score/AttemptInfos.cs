using UnityEngine;

namespace Ecos
{
    [System.Serializable]
    public class AttemptInfos
    {
        public FishDef fishDef;

        public bool shouldBeCaptured;

        public bool wasCaptured;

        public int missedTimings;

        public int wrongClicks;

        public AttemptInfos(FishDef fishDef, bool shouldBeCaptured)
        {
            this.fishDef = fishDef;
            this.shouldBeCaptured = shouldBeCaptured;
        }
    }
}
