using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Runtime
{
    [RequireComponent(typeof(RectTransform))]
    public class RectSpinner : MonoBehaviour
    {
        private RectTransform _transform;
        [SerializeField] private Vector3 rotationAmount;
        public Vector3 RotationAmount => rotationAmount;
        
        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            _transform.Rotate(rotationAmount);
        }
    }
}