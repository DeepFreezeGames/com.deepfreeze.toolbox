using UnityEngine.UI;

namespace Toolbox.Runtime.Extensions
{
    public static class ButtonExtensions
    {
        public static bool Clickable(this Button button)
        {
            return button.gameObject.activeInHierarchy && button.interactable && button.enabled;
        }
    }
}