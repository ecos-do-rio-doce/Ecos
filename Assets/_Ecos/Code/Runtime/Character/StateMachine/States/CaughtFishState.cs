using UnityEngine;

namespace Ecos
{
    public class CaughtFishState : State
    {
        public float timeToReturn = 3f;
        public override void OnEnterState()
        {
            worldFishing.ShowFishAppearedIndicator?.Invoke(false);
            worldFishing.SetAnimatorState(SetAnimatorParams.FishingAnimState.CaughtFish);
        }

        public override void Update()
        {
            timeToReturn -= Time.deltaTime;

            if(timeToReturn < 0f)
            {
                worldFishing.ChangeState(new NotFishingState());
            }
        }

        public override void OnExitState()
        {
            
        }

        
    }
}
