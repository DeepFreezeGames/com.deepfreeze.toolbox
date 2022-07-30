using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Runtime
{
    public class Spinner : MonoBehaviour
    {
        private Transform _transform;
        [SerializeField] private bool pauseWhenHidden = false;
        [SerializeField] private Vector3 rotationAmount = Vector3.zero;
        public Vector3 RotationAmount => rotationAmount;

        private void Awake()
        {
            _transform = transform;
            enabled = false;
        }

        private void Update()
        {
            _transform.Rotate(rotationAmount);
        }

        private void OnBecameVisible()
        {
            enabled = true;
        }

        private void OnBecameInvisible()
        {
            if (pauseWhenHidden)
            {
                enabled = false;
            }
        }
    }
}