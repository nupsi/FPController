using UnityEngine;

namespace FPController.Example
{
    /// <summary>
    /// Resets players position to set point or (0, 1, 0).
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class ResetPosition : MonoBehaviour
    {
        /// <summary>
        /// Position to reset to.
        /// </summary>
        [SerializeField]
        private GameObject m_resetPosition;

        /*
         * MonoBehaviour Functions.
         */

        private void Reset()
        {
            var collider = GetComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                other.gameObject.transform.position = Position;
                var rididbody = other.gameObject.GetComponent<Rigidbody>();
                if(rididbody != null)
                {
                    rididbody.velocity = Vector3.zero;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            var collider = GetComponent<BoxCollider>();
            var position = transform.position + collider.center;
            var size = new Vector3
            {
                x = transform.localScale.x * collider.size.x,
                y = transform.localScale.y * collider.size.y,
                z = transform.localScale.z * collider.size.z
            };
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawCube(position, size);
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawWireCube(position, size);
        }

        /*
         * Accessors.
         */

        /// <summary>
        /// Getter for reset postion.
        /// Returns set point if defined or (0, 1, 0) if not.
        /// </summary>
        private Vector3 Position
        {
            get
            {
                return m_resetPosition != null
                    ? m_resetPosition.transform.position
                    : Vector3.up;
            }
        }
    }
}