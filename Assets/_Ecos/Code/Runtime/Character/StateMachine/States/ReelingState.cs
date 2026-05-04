using System;
using UnityEngine;

namespace Ecos
{
    public class ReelingState : State
    {
        public override void OnEnterState()
        {
            worldFishing.ShowFishAppearedIndicator?.Invoke(false);

            FishingManager.Instance.StartFishing();

            FishingManager.Instance.onFinishFishingAction += FinishedFishing;
        }

        private void FinishedFishing()
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
