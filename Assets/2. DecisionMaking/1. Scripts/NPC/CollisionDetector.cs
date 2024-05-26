using UnityEngine;

namespace DecisionMaking
{
    public class CollisionDetector : MonoBehaviour
    {

        bool isDetected;
        private const string groundTag = "Ground";

        public bool IsDetected
        {
            get
            {
                if(isDetected)
                {
                    isDetected = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            private set
            {
                isDetected = value;
            }
        }

        public Vector3 PushDirection { get; private set; }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("CollisionDetector.OnCollisionEnter1 " + collision.gameObject.name);

            if(!collision.gameObject.CompareTag(groundTag))
            {
                if(collision.contacts.Length > 0)
                {
                    ContactPoint contact = collision.contacts[0];

                    Vector3 pushDirection = transform.position - contact.point;
                    pushDirection = pushDirection.normalized;
                    PushDirection = pushDirection;

                    Debug.Log("IsDetected " + collision.gameObject.name);
                    IsDetected = true;
                }
            }
        }
    }
}
