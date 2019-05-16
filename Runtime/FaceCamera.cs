using UnityEngine;

namespace Toolbox.Runtime
{
    public class FaceCamera : MonoBehaviour
    {
        private Camera _camera;
        [SerializeField] private bool invert = false;

        private void Awake()
        {
            enabled = false;
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            transform.LookAt(_camera.transform);

            if (invert)
            {
                transform.Rotate(0,180,0);
            }
        }

        private void OnBecameVisible()
        {
            enabled = true;
        }

        private void OnBecameInvisible()
        {
            enabled = false;
        }
    }
}
