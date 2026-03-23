using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Ecos
{
    public class WaterDetection : MonoBehaviour
    {
        [SerializeField] private Transform referenceTransform;
        [SerializeField] private bool showDebugInformations = true;

        [Title("Raycast")]
        [SerializeField] private float detectWaterDistance = 1f;
        [SerializeField] private float raycastDistance = 2f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask waterLayer;

        private void Reset()
        {
            referenceTransform = transform.root;
        }
        private void Awake()
        {
            if (referenceTransform == null)
            {
                Reset();
            }
        }
        public bool IsInWater()
        {
            TryDetectSurface(out RaycastHit hit, out bool isWater);
            return isWater;
        }

        public bool TryDetectSurface(out RaycastHit hit, out bool isWater)
        {
            Vector3 origin = GetDetectionPoint();

            int combinedMask = groundLayer | waterLayer;

            if (Physics.Raycast(origin, Vector3.down, out hit, raycastDistance, combinedMask, QueryTriggerInteraction.Ignore))
            {
                int hitLayerMask = 1 << hit.collider.gameObject.layer;


                if((waterLayer & (hitLayerMask)) != 0)
                {
                    isWater = true;
                }
                else
                {
                    isWater = false;
                }

                return true;
            }

            hit = default;
            isWater = false;
            return false;
        }

        private Vector3 GetDetectionPoint()
        {
            return referenceTransform.position + (referenceTransform.forward * detectWaterDistance) + new Vector3(0, 1f, 0f);
        }

        private void OnDrawGizmos()
        {
            if (!showDebugInformations)
                return;

            if(TryDetectSurface(out RaycastHit hit, out bool isWater))
            {
                Gizmos.color = isWater ? Color.green : Color.red;
                Gizmos.DrawWireSphere(hit.point, 0.1f);
            }

            
        }
    }
}
