using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gaton.Transitions
{
    public class TransitionsCollection : MonoBehaviour
    {
        List<Transition> transitions;

        #region Callbacks

        public Action<TransitionsCollection> OnStartTransitions;

        public Action<TransitionsCollection> OnEndTransitions;

        public Action<TransitionsCollection> OnSkipTransitions;

        #endregion

        #region Duration
        public float StartDuration
        {
            get
            {
                float duration = 0;

                foreach (var transition in Transitions)
                {
                    if (transition == null || transition.gameObject.activeInHierarchy == false)
                        continue;

                    if (transition.SkipStartTransition)
                        continue;

                    if (transition.StartDuration > duration)
                        duration = transition.StartDuration;
                }

                return duration;
            }
        }
        public float EndDuration
        {
            get
            {
                float duration = 0;

                foreach (var transition in Transitions)
                {
                    if (transition == null || transition.gameObject.activeInHierarchy == false)
                        continue;

                    if (transition.SkipEndTransition)
                        continue;

                    if (transition.EndDuration > duration)
                        duration = transition.EndDuration;
                }

                return duration;
            }
        }
        #endregion

        public List<Transition> Transitions
        {
            get
            {
                if (transitions == null)
                    transitions = new List<Transition>();

                return transitions;
            }

            set => transitions = value;
        }

        public void PlayTransitions(bool start)
        {
            if (start)
            {
                PlayStartTransitions();
            }
            else
            {
                PlayEndTransitions();
            }
        }

        [Title("PLAY MODE BUTTONS")]
        [Button]
        public void PlayStartTransitions()
        {
            foreach (Transition transition in Transitions)
            {
                if (transition == null)
                    continue;

                transition.RequestPlayStartTransition();
            }

            OnStartTransitions?.Invoke(this);
        }

        [Button]
        public void PlayEndTransitions()
        {
            foreach (Transition transition in Transitions)
            {
                if (transition == null)
                    continue;

                transition.RequestPlayEndTransition();
            }

            OnEndTransitions?.Invoke(this);
        }

        public void SkipTransitions()
        {
            foreach (Transition transition in Transitions)
            {
                if (transition == null)
                    continue;

                transition.RequestSkip();
            }

            OnSkipTransitions?.Invoke(this);
        }

        #region List Functions
        public void Add(Transition transition)
        {
            Transitions.Add(transition);
        }

        public void Remove(Transition transition)
        {
            Transitions.Remove(transition);
        }
        #endregion

        #region Editor

        [Title("EDITOR DEBUG BUTTONS")]
        [Button]
        public void SendAllTransitionsToStartState()
        {
            foreach (var transition in GetComponentsInChildren<Transition>())
            {
                transition.GoToStartState();
            }
        }

        [GUIColor(0, 1, 0)]
        [Button]
        public void SendAllTransitionsToEndState()
        {
            foreach (var transition in GetComponentsInChildren<Transition>())
            {
                transition.GoToEndState();
            }
        }

        #endregion
    }
}