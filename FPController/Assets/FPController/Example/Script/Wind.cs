using UnityEngine;

namespace FPController.Example
{
    [RequireComponent(typeof(BoxCollider))]
    public class Wind : MonoBehaviour
    {
        [SerializeField]
        private float m_force = 2f;

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            var body = other.GetComponent<Rigidbody>();
            if(body != null)
            {
                body.AddForce(transform.forward * m_force, ForceMode.Impulse);
            }
        }

        private void OnDrawGizmosSelected()
        {
            var start = transform.position;
            var end = transform.position + transform.forward;
            var endOffset = transform.position + (transform.forward * 0.75f);
            var offset = transform.right * 0.25f;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawLine(end, endOffset + offset);
            Gizmos.DrawLine(end, endOffset - offset);
        }
    }
}