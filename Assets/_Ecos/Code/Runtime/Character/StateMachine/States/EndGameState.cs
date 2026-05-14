using UnityEngine;

namespace Ecos
{
    public class EndGameState : State
    {
        public override void OnEnterState()
        {
            worldFishing.SetCanWalk = false;
            worldFishing.SetAnimatorState(SetAnimatorParams.FishingAnimState.Idle);
        }

        public override void OnExitState()
        {
            
        }

        public override void Update()
        {
            
        }
    }
}
