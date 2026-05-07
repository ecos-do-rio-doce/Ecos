using UnityEngine;

namespace Ecos
{
    public class NotFishingState : State
    {
        public override void OnEnterState()
        {
            worldFishing.SetCanWalk = true;
            worldFishing.SetAnimatorState(SetAnimatorParams.FishingAnimState.Idle);
        }

        public override void OnExitState()
        {
            
        }

        public override void Update()
        {
            if (worldFishing.DetectedMainActionButton())
            {
                FishSource source = worldFishing.WaterDetection.TryToGetFishSource();

                if(source != null)
                {
                    worldFishing.CurrentFishSource = source;
                    worldFishing.ChangeState(new WaitingToBiteState());
                }
                else
                {
                    worldFishing.ChangeState(new FailedState());
                }
            }
        }

        private void DetectFishingStart()
        {
            
        }
    }
}
