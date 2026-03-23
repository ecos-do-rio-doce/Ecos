using Sirenix.OdinInspector;
using UnityEngine;

namespace Gaton.Transitions
{
    public abstract class Transition : MonoBehaviour
    {
        [Title("Callbacks")]
        [Tooltip("Adds the 'SubscribeTransitionToCollection' to the object. \nAutomatically searches for the 'transition collection' component on obj and parent and tries to subscribe to its open/close callbacks")]
        [SerializeField] bool subscribeToTransitionsCollection = true;

        [Title("Skip Parameters")]
        [Tooltip("Can be skippable in certain conditions? E.g: Button presses to finish animation.")]
        [SerializeField] bool isSkippable = true;

        [Tooltip("If true, doesn't play start animation. Skips it.")]
        [SerializeField] bool skipStartTransition = false;

        [Tooltip("If true, doesn't play end animation. Skips it.")]
        [SerializeField] bool skipEndTransition = false;


        [Title("Time Scale")]
        [Tooltip("Animation plays independent of delta time.")]
        [SerializeField] protected bool unscaledTime = false;

        public void Awake()
        {
            if (subscribeToTransitionsCollection)
            {
                var subscribe = gameObject.AddComponent<SubscribeTransitionToCollection>();

                subscribe.Transition = this;
            }
        }

        /// <summary>
        /// Estimated duration of transition. 
        /// <para></para><b>A return value of 0 means that it is instant.</b>
        /// <para></para><b>A negative return value means that the duration cannot be calculated.</b>
        /// </summary>
        public abstract float StartDuration { get; }
        public abstract float EndDuration { get; }
        public bool SkipStartTransition { get => skipStartTransition; set => skipStartTransition = value; }
        public bool SkipEndTransition { get => skipEndTransition; }

        /// <summary>
        /// Use this function to request start transition. Some objs are going to skip them because of the "skip parameters".
        /// </summary>
        public void RequestPlayStartTransition()
        {
            if (SkipStartTransition)
            {
                GoToEndState();

                return;
            }

            PlayStartTransition();
        }

        /// <summary>
        /// Use this function to request end transition. Some objs are going to skip them because of the "skip parameters".
        /// </summary>
        public void RequestPlayEndTransition()
        {
            if (SkipEndTransition)
            {
                GoToStartState();

                return;
            }

            PlayEndTransition();
        }

        /// <summary>
        /// Transitions of: opening menu, focusing button...
        /// </summary>
        [Title("PLAY MODE BUTTONS")]
        [Button]
        public abstract void PlayStartTransition();

        /// <summary>
        /// Transitions of: closing menu, unfocusing button...
        /// </summary>
        [Button]
        public abstract void PlayEndTransition();

        /// <summary>
        /// Auto-completes transition. Some of them doesn't need to implement this function (they're instant)
        /// </summary>
        protected abstract void OnSkip();

        /// <summary>
        /// Mostly used at editor time, so don't expect variables that are assigned at awake - instead use 'GetComponent'.
        /// <para>Instantly goes to start state.</para>
        /// </summary>
        [Title("Editor Debug Buttons")]
        [Button]
        public abstract void GoToStartState();

        /// <summary>
        /// Mostly used at editor time, so don't expect variables that are assigned at awake - instead use 'GetComponent'
        /// <para>Instantly goes to start state.</para>
        /// </summary>
        [Button]
        public abstract void GoToEndState();

        /// <summary>
        /// Tries to request skip. - Some transitions are unskippable -
        /// </summary>
        public void RequestSkip()
        {
            if (isSkippable)
                OnSkip();
        }

        private void Reset()
        {
            // Sets subscription based if script is child of a menu obj
            subscribeToTransitionsCollection = GetComponentInParent<TransitionsCollection>() != null;
        }
    }
}