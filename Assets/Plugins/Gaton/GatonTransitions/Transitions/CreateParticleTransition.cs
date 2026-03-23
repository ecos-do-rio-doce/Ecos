using System.Collections;
using UnityEngine;

namespace Gaton.Transitions
{
    public class CreateParticleTransition : Transition
    {
        [SerializeField] private ParticleSystem particlePrefab; // Drag your ParticleSystem prefab here
        [SerializeField] bool instantiateOnStart;
        [SerializeField] bool instantiateOnEnd;

        [SerializeField] private float startDuration = 5f;

        [SerializeField] private float endDuration = 5f;

        [SerializeField] float startDelay = 0;

        [SerializeField] float endDelay = 0;

        bool isInDelayBeforeInstantiatingParticles;
        bool cancelParticleInstantiation;

        ParticleSystem instantiatedParticle;

        public override float StartDuration => startDuration;

        public override float EndDuration => endDuration;

        public override void PlayStartTransition()
        {
            if (instantiateOnStart)
            {
                CreateAndDestroyParticle(startDuration, startDelay);
            }
        }

        public override void PlayEndTransition()
        {
            if (instantiateOnEnd)
            {
                CreateAndDestroyParticle(endDuration, endDelay);
            }
        }

        private void CreateAndDestroyParticle(float duration, float delay)
        {
            // Destroy the particle system after its duration
            StartCoroutine(DestroyAfterDelay(duration, delay));
        }

        private IEnumerator DestroyAfterDelay(float seconds, float delay = -1)
        {
            if (delay > 0)
            {
                isInDelayBeforeInstantiatingParticles = true;

                yield return new WaitForSeconds(delay);

                isInDelayBeforeInstantiatingParticles = false;

                if (cancelParticleInstantiation)
                {
                    cancelParticleInstantiation = false;

                    yield break;
                }
            }

            if (particlePrefab == null)
            {
                Debug.LogError("No particle has been assigned!");

                yield break;
            }

            if (instantiatedParticle != null)
            {
                Destroy(instantiatedParticle.gameObject);
            }

            // Instantiate the particle system
            instantiatedParticle = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);

            yield return new WaitForSeconds(seconds);

            if (instantiatedParticle != null)
            {
                instantiatedParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            yield return new WaitForSeconds(0.5f);

            if (instantiatedParticle != null)
            {
                Destroy(instantiatedParticle.gameObject);
            }
        }

        protected override void OnSkip()
        {
            if (instantiatedParticle != null)
            {
                Destroy(instantiatedParticle.gameObject);
            }

            if (isInDelayBeforeInstantiatingParticles)
            {
                cancelParticleInstantiation = true;
            }
        }

        public override void GoToStartState()
        {

        }
        public override void GoToEndState()
        {

        }
    }
}