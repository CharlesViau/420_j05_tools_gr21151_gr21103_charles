using ScriptableObject;
using UnityEngine;
using UnityEngine.UI;

namespace Exercise6
{
    public class CardUILink : MonoBehaviour
    {
        [SerializeField]private Card card;
    
        //UI FIELD
        public Text textName;
        public Text textDescription;
        public Image artwork;
        public Image elementalSymbol;
        public Text textAttack;

        private void Awake()
        {
            textName.text = card.name;
            textDescription.text = card.description;
            artwork.sprite = card.sprite;
            elementalSymbol.sprite = card.elementalSprite;
            textAttack.text = card.power.ToString();
        }
    
    }
}
