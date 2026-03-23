using UnityEngine;

namespace Ecos
{
    public class WaitingToBiteState : State
    {
        private Vector2 RANGE_TO_BITE = new Vector2(3, 6);
        private float biteDetectionTime = 1f;

        float auxTime = 0;

        bool canGetFish = false;

        public override void OnEnterState()
        {
            Debug.Log("Waiting for fish");
            worldFishing.SetCanWalk = false;
            worldFishing.SetAnimatorState(SetAnimatorParams.FishingAnimState.WaitingToBite);
            ResetAppearFishTime();
        }

        private void ResetAppearFishTime()
        {
            auxTime = Random.Range(RANGE_TO_BITE.x, RANGE_TO_BITE.y);
            Debug.Log($"Looking for fish, time to get: {auxTime}");
        }

        public override void Update()
        {
            if (canGetFish)
            {
                if (worldFishing.DetectedMainActionButton())
                {                    
                    worldFishing.ChangeState(new CaughtFishState());

                    return;
                }

                auxTime -= Time.deltaTime;
                
                if (auxTime < 0f)
                {
                    canGetFish = false;
                    ResetAppearFishTime();

                    worldFishing.ShowFishAppearedIndicator?.Invoke(false);
                }
            }
            else
            {
                auxTime -= Time.deltaTime;

                if(auxTime < 0)
                {
                    canGetFish = true;
                    auxTime = biteDetectionTime;
                    worldFishing.ShowFishAppearedIndicator?.Invoke(true);
                }
            }
            
        }

        public override void OnExitState()
        {
            
        }

    }
}
