using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaton.Transitions
{
    public class InvokeRepeatingTransitionsCollection : MonoBehaviour
    {
        [SerializeField] Vector2 delayRange;

        bool keepPlaying = true;

        TransitionsCollection transitionsCollection;

        public bool KeepPlaying { get => keepPlaying; set => keepPlaying = value; }
        public Vector2 FrequencyRange { get => delayRange; set => delayRange = value; }

        private void Start()
        {
            transitionsCollection = GetComponent<TransitionsCollection>();

            StartCoroutine(RepeatCoroutine());
        }

        IEnumerator RepeatCoroutine()
        {
            while (KeepPlaying)
            {
                float interval = Random.Range(FrequencyRange.x, FrequencyRange.y);

                yield return new WaitForSeconds(interval);

                transitionsCollection.PlayStartTransitions();
            }
        }
    }
}