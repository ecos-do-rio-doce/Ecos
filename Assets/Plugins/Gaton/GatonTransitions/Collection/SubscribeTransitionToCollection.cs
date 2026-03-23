using System;
using UnityEngine;

namespace Gaton.Transitions
{
    public class SubscribeTransitionToCollection : MonoBehaviour
    {
        [SerializeField] TransitionsCollection[] transitionsCollections = null;

        Transition transition;

        public Transition Transition
        {
            get => transition;

            set
            {
                transition = value;

                SetupMenusArray();

                Array.ForEach(transitionsCollections, SubscribeToCallbacks);
            }
        }

        private void Start()
        {
            if (Transition == null)
                Transition = GetComponent<Transition>();
        }

        private void OnDestroy()
        {
            Array.ForEach(transitionsCollections, UnsubscribeToCallbacks);
        }

        private void SetupMenusArray()
        {
            if (transitionsCollections == null || transitionsCollections.Length <= 0)
                transitionsCollections = new TransitionsCollection[] { GetComponentInParent<TransitionsCollection>() };
        }

        void SubscribeToCallbacks(TransitionsCollection transitionsCollection)
        {
            if (transitionsCollection == null)
                return;

            transitionsCollection.Add(Transition);
        }

        void UnsubscribeToCallbacks(TransitionsCollection transitionsCollection)
        {
            if (transitionsCollection == null)
                return;

            transitionsCollection.Remove(Transition);
        }
    }
}