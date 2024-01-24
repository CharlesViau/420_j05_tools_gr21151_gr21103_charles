using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObject;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CardEditor : EditorWindow
    {
        private static Card _card;

        private static Dictionary<ElementalType, Sprite> _elementalDictionary;


        private static EditorWindow _window;

        [MenuItem("MyWindows/CardEditor")]
        public static void OpenWindow()
        {
            CreateElementalDictionary();
            _card = CreateInstance<Card>();
            _window = GetWindow<CardEditor>("CardEditor(Exercise6)");
        }

        public void OnGUI()
        {
            DrawEditorWindow();
        }

        private static void DrawEditorWindow()
        {
            GUILayout.BeginHorizontal();
            DrawLeftLayout();
            DrawRightLayout();
            GUILayout.EndHorizontal();
        }

        private static void CreateElementalDictionary()
        {
            _elementalDictionary = new Dictionary<ElementalType, Sprite>();
            var elementalSprites = AssetDatabase
                .LoadAllAssetRepresentationsAtPath("Assets/Resources/Sprites/ElementalSymbol/Element.png")
                .OfType<Sprite>().ToArray();

            foreach (var element in Enum.GetNames(typeof(ElementalType)))
            {
                foreach (var sprite in elementalSprites)
                {
                    if (sprite.name == element)
                    {
                        _elementalDictionary.Add((ElementalType) Enum.Parse(typeof(ElementalType), element), sprite);
                    }
                }
            }
        }

        private static void DrawLeftLayout()
        {
            var leftRect = Utils.Utils.GetRect(0, 0, 50, 95, _window.position);

            GUILayout.BeginArea(leftRect);
            GUILayout.BeginVertical();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Card Info");
            EditorGUILayout.Space();

            _card.name = EditorGUILayout.TextField("Name :", _card.name);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Description : ");
            _card.description = EditorGUILayout.TextArea(_card.description, GUILayout.ExpandWidth(true),
                GUILayout.MinHeight(5), GUILayout.ExpandHeight(true));
            EditorGUILayout.Space();

            _card.power = EditorGUILayout.IntField("Attack Power :", _card.power);
            EditorGUILayout.Space();

            _card.type = (ElementalType) EditorGUILayout.EnumPopup("Elemental Type : ", _card.type);
            _card.elementalSprite = _elementalDictionary[_card.type];
            EditorGUILayout.Space();

            _card.backgroundColor = EditorGUILayout.ColorField("Background Color : ", _card.backgroundColor);
            _card.backgroundColor.a = 1;
            EditorGUILayout.Space();

            _card.backgroundColor2 = EditorGUILayout.ColorField("Text Field Color : ", _card.backgroundColor2);
            _card.backgroundColor2.a = 1;
            EditorGUILayout.Space();

            _card.borderColor = EditorGUILayout.ColorField("Border Color : ", _card.borderColor);
            _card.borderColor.a = 1;
            EditorGUILayout.Space();

            _card.sprite = (Sprite) EditorGUILayout.ObjectField("Artwork", _card.sprite, typeof(Sprite), true);
            EditorGUILayout.Space();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private static void DrawRightLayout()
        {
            var rightRect = Utils.Utils.GetRect(52, 0, 48, 100, _window.position);
            GUILayout.BeginArea(rightRect);
            GUILayout.BeginVertical();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Card Preview");
            DrawCardPreview(rightRect);


            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private static void DrawCardPreview(Rect rightRect)
        {
            var cardBorderRect = Utils.Utils.GetRect(0, 10, 55, 80, rightRect);
            GUI.color = _card.borderColor;
            GUI.DrawTexture(cardBorderRect, Texture2D.whiteTexture);

            var cardBackgroundRect = Utils.Utils.GetRect(5, 16, 90, 93, cardBorderRect);
            GUI.color = _card.backgroundColor;
            GUI.DrawTexture(cardBackgroundRect, Texture2D.whiteTexture);

            var borderNameRect = Utils.Utils.GetRect(10, 20, 91, 18, cardBackgroundRect);
            GUI.color = _card.borderColor;
            GUI.DrawTexture(borderNameRect, Texture2D.whiteTexture);

            var backgroundNameRect = Utils.Utils.GetRect(15, 121, 93, 80, borderNameRect);
            GUI.color = _card.backgroundColor2;
            GUI.DrawTexture(backgroundNameRect, Texture2D.whiteTexture);

            var borderImageRect = Utils.Utils.GetRect(10, 40, 91, 34, cardBackgroundRect);
            GUI.color = _card.borderColor;
            GUI.DrawTexture(borderImageRect, Texture2D.whiteTexture);

            if (_card.sprite)
            {
                var imageRect = Utils.Utils.GetRect(12, 41, 86, 32, cardBackgroundRect);
                GUI.color = Color.white;
                GUI.DrawTexture(imageRect, _card.sprite.texture);
            }

            var borderDescription = Utils.Utils.GetRect(10, 76, 91, 34, cardBackgroundRect);
            GUI.color = _card.borderColor;
            GUI.DrawTexture(borderDescription, Texture2D.whiteTexture);

            var backgroundDescription = Utils.Utils.GetRect(13, 78, 85, 30, cardBackgroundRect);
            GUI.color = _card.backgroundColor2;
            GUI.DrawTexture(backgroundDescription, Texture2D.whiteTexture);


            var elementalRect = Utils.Utils.GetRect(12, 111, 10, 5, cardBackgroundRect);
            GUI.color = Color.white;
            GUI.DrawTexture(elementalRect, AssetPreview.GetAssetPreview(_card.elementalSprite));
        }
    }
}