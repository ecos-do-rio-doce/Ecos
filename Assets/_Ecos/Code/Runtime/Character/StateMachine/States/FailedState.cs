using UnityEngine;

namespace Ecos
{
    public class FailedState : State
    {
        public float FAIL_DURATION = 0f;

        public override void OnEnterState()
        {
            worldFishing.SetCanWalk = false;
        }
        public override void Update()
        {
            FAIL_DURATION -= Time.deltaTime;

            if (FAIL_DURATION < 0)
            {
                worldFishing.ChangeState(new NotFishingState());
            }
        }

        public override void OnExitState()
        {
            
        }

    }
}
