using Sirenix.OdinInspector;
using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Activate Object Transition")]
    public class ActivateObjectTransition : Transition
    {
        [Title("Enablement Options")]
        [SerializeField] bool beginActive = false;

        public override float StartDuration => 0;

        public override float EndDuration => 0;


        public override void GoToStartState()
        {
            gameObject.SetActive(beginActive);
        }

        public override void GoToEndState()
        {
            gameObject.SetActive(!beginActive);
        }

        public override void PlayStartTransition()
        {
            GoToEndState();
        }

        public override void PlayEndTransition()
        {
            GoToStartState();
        }


        protected override void OnSkip()
        {

        }
    }
}