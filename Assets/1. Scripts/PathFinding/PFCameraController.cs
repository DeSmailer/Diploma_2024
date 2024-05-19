using UnityEngine;

namespace PathFinding
{

    public class CameraController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float zoomSpeed = 10f;
        public float minZoom = 2f;
        public float maxZoom = 10f;

        private Camera cam;

        void Start()
        {
            cam = Camera.main;
        }

        void Update()
        {
            float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            transform.Translate(new Vector3(moveX, moveY, 0));

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if(scroll != 0.0f)
            {
                float newSize = cam.orthographicSize - scroll * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
        }
    }
}
