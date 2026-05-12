using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ecos
{
    public class ReelingState : State
    {
        public override void OnEnterState()
        {
            worldFishing.ShowFishAppearedIndicator?.Invoke(false);

            var fishes = worldFishing.CurrentFishSource.FishesInSource;
            FishDef fishDef = fishes[Random.Range(0, fishes.Count)];

            PlayerScore.Instance.currentAttempt = new AttemptInfos(fishDef, worldFishing.CurrentFishSource.IsAllowed(fishDef));


            FishingManager.Instance.StartFishing(fishDef);

            FishingManager.Instance.onFinishFishingAction += FinishedFishing;
        }

        private void FinishedFishing(FishDef fish)
        {
            worldFishing.ChangeState(new CaughtFishState());
        }

        public override void Update()
        {
            
        }

        public override void OnExitState()
        {
            FishingManager.Instance.onFinishFishingAction -= FinishedFishing;
        }
    }
}
