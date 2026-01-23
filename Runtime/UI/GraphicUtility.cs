using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.UI
{
    public static class GraphicUtility 
    {
        /// <summary>
        /// Adjust the alpha value of the graphics color to given value (0-255).
        /// </summary>
        public static void SetTransparency(this Graphic graphic, float value)
        {
            Color color = graphic.color;
            color.a = value == 0f ? 0f : value / 255f;

            graphic.color = color;
        }

        public static void SetInvisible(this Graphic graphic) => SetTransparency(graphic, 0f);
        public static void SetVisible(this Graphic graphic) => SetTransparency(graphic, 255f);
    }
}
