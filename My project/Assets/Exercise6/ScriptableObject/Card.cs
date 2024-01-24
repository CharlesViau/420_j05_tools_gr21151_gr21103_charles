using UnityEngine;

namespace ScriptableObject
{
    public enum ElementalType
    {
        Fire,
        Water,
        Air,
        Earth
    }
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class Card : UnityEngine.ScriptableObject
    {
         public new string name;
         public string description;
         public ElementalType type;
         public Sprite elementalSprite;
         public int power;
         public Sprite sprite;
         public Color backgroundColor;
         public Color backgroundColor2;
         public Color borderColor;

    }
}
