using UnityEngine;
using UnityEngine.UI;

namespace Packages.Toolbox.Runtime
{
    [RequireComponent(typeof(Image))]
    public class ScrollingImage : MonoBehaviour
    {
        public bool scroll;
        public Vector2 scrollSpeed;
        private Material _material;

        private void Awake()
        {
            if (!scroll)
            {
                gameObject.GetComponent<Image>().material.mainTextureOffset = Vector2.zero;
            }
            
            _material = new Material(gameObject.GetComponent<Image>().material);
            gameObject.GetComponent<Image>().material = _material;
        }

        private void FixedUpdate()
        {
            if(scroll)
            {
                _material.mainTextureOffset = new Vector2(scrollSpeed.x, scrollSpeed.y) * Time.time;
            }
        }
    }
}