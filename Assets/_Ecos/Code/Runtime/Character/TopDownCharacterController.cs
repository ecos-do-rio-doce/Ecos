using UnityEngine;
using UnityEngine.Events;

namespace Ecos
{
    [RequireComponent(typeof(CharacterController))]
    public class TopDownCharacterController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float gravity = -20f;

        [SerializeField] private UnityEvent onCharacterWalked;
        [SerializeField] private UnityEvent onCharacterStopped;

        private bool canWalk;

        private CharacterController characterController;
        private Vector3 verticalVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (!canWalk)
                return;

            Move();
        }

        public void SetCanWalk(bool value)
        {
            canWalk = value;
        }

        private void Move()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

            if (inputDirection.sqrMagnitude > 1f)
            {
                inputDirection.Normalize();
            }

            Vector3 moveDirection = inputDirection;
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            if (moveDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                onCharacterWalked?.Invoke();
            }
            else
            {
                onCharacterStopped?.Invoke();
            }

            if (characterController.isGrounded && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -2f;
            }

            verticalVelocity.y += gravity * Time.deltaTime;
            characterController.Move(verticalVelocity * Time.deltaTime);
        }
    }
}
