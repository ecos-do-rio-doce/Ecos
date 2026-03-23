using UnityEngine;

namespace Gaton.Transitions
{
    public class PlayParticleTransition : Transition
    {
        [SerializeField] bool beginActivated = false;

        ParticleSystem ps;

        public override float StartDuration => -1;

        public override float EndDuration => -1;

        public new void Awake()
        {
            ps = GetComponent<ParticleSystem>();

            if (beginActivated)
                StartEmitting();
            else
                StopEmitting();

            base.Awake();
        }

        private void StartEmitting()
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }

        private void StopEmitting()
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        public override void GoToStartState()
        {
            if (ps == null)
                return;

            StopEmitting();
        }

        public override void GoToEndState()
        {
            if (ps == null)
                return;

            StartEmitting();
        }

        public override void PlayStartTransition()
        {
            StartEmitting();
        }

        public override void PlayEndTransition()
        {
            StopEmitting();
        }

        protected override void OnSkip()
        {

        }
    }
}