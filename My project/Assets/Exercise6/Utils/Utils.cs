using UnityEngine;

namespace Utils
{
    public class Utils : MonoBehaviour
    {
        public static Rect GetRect(float x, float y, float width, float height, Rect position)
        {
            var widthUnit = position.width / 100;
            var heightUnit = position.height / 100;

            var widthTemp = width * widthUnit;
            var heightTemp = height * heightUnit;
            var xTemp = (x * widthUnit);
            var yTemp = (y * heightUnit);

            return new Rect(xTemp, yTemp, widthTemp, heightTemp);
        }
    }
}
