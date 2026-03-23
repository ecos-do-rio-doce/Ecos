using UnityEngine;

namespace Ecos
{
    public class SetAnimatorParams : MonoBehaviour
    {
        public enum FishingAnimState
        {
            Idle = 0,
            Walking = 1,
            WaitingToBite = 2,
            CaughtFish = 3,
        }

        [SerializeField] string paramName;

        [SerializeField] Animator animator;

        public Animator Animator
        {
            get
            {
                if (animator == null)
                {
                    animator = GetComponentInChildren<Animator>();
                }

                return animator;
            }
        }

        public void SetBoolParameter(bool value)
        {
            Animator.SetBool(paramName, value);
        }

        public void SetIntParameter(int value)
        {
            Animator.SetInteger(paramName, value);
        }

        public void SetTriggerParamter(string triggerName)
        {
            Animator.SetTrigger(triggerName);
        }
    }
}
