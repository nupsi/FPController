using UnityEngine;

namespace FPController.Example
{
    [RequireComponent(typeof(BoxCollider))]
    public class Trampoline : MonoBehaviour
    {
        [SerializeField]
        private float m_force = 100f;

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var body = collision.gameObject.GetComponent<Rigidbody>();
            if(body != null)
            {
                body.AddForce(transform.up * m_force, ForceMode.Impulse);
            }
        }
    }
}