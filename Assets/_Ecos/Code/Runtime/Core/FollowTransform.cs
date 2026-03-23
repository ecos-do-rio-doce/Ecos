using UnityEngine;

namespace Ecos
{
    public class FollowTransform : MonoBehaviour
    {
        [SerializeField] Vector3 offset;
        [SerializeField] Transform targetTransform;

        private void LateUpdate()
        {
            this.transform.position = targetTransform.position + offset;
        }
    }
}
