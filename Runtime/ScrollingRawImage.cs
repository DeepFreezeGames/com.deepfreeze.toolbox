using UnityEngine;
using UnityEngine.UI;

namespace Packages.Toolbox.Runtime
{
    [RequireComponent(typeof(RawImage))]
    public class ScrollingRawImage : MonoBehaviour
    {
        public RawImage rawImage;
        public float verticalScrollSpeed;
        public float horizontalScrollSpeed;

        private float _posX;
        private float _posY;

        private void Start()
        {
            _posX = rawImage.uvRect.x;
            _posY = rawImage.uvRect.y;
        }

        private void Update()
        {
            _posX += horizontalScrollSpeed * Time.deltaTime;
            _posY += verticalScrollSpeed * Time.deltaTime;
            
            rawImage.uvRect = new Rect(_posX, _posY, rawImage.uvRect.size.x, rawImage.uvRect.size.y);
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (rawImage == null)
            {
                rawImage = GetComponent<RawImage>();
            }
        }
#endif
    }
}