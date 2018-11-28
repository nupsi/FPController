using UnityEngine;

namespace FPController.Example
{
    [RequireComponent(typeof(BoxCollider))]
    public class ResetPosition : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_resetPosition;

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