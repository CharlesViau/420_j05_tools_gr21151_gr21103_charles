using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TextureEditor : EditorWindow
    {
        private MeshRenderer _meshRenderer;
        private Texture _colorTexture;
        private Color _brushColor = Color.white;
        private Color _eraseColor = Color.white;
        private int _rowNumber = 3;
        private int _colNumber = 3;

        private List<GameObject> _gameObjects = new List<GameObject>();
        private Dictionary<Vector2, Color> _textureMap;

        private Vector2Int _minMax;


        [MenuItem("MyWindows/TextureEditor")]
        public static void OpenWindow()
        {
            GetWindow<TextureEditor>("TextureEditor(Exercise2)");
        }

        private void OnEnable()
        {
            _minMax = new Vector2Int(0, 0);
            CheckDictSizeWithDrawingBoardSize();


            _colorTexture = EditorGUIUtility.whiteTexture;
            _textureMap = CreateNewDictionary(_rowNumber, _colNumber);
        }

        public void OnGUI()
        {
            //Window Start
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            DrawRightSide();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            DrawLeftSide();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawRightSide()
        {
            DragBox();
            ChangeTheBrushColor();
            ChangeTheBrushEraser();
            if (GUILayout.Button("Submit"))
            {
                ChangeMaterial();
            }
        }


        private void DrawLeftSide()
        {
            var saveColor = GUI.color;

            DrawRowField();
            DrawColumnField();
            CheckDictSizeWithDrawingBoardSize();
            DrawDrawingBoard();

            GUI.color = saveColor;
        }

        private void DrawDrawingBoard()
        {
            EditorGUILayout.BeginVertical();

            for (var row = 0; row < _rowNumber; row++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var col = 0; col < _colNumber; col++)
                {
                    var key = new Vector2(row, col);
                    GUI.color = _textureMap[key];

                    var colorRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none,
                        GUILayout.ExpandWidth(true),
                        GUILayout.ExpandHeight(true));

                    GUI.DrawTexture(colorRect, _colorTexture);

                    HandleDrawingEvents(colorRect, key);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void HandleDrawingEvents(Rect colorRect, Vector2 key)
        {
            var currentEvent = Event.current;

            if (!(colorRect.Contains(currentEvent.mousePosition) &
                  currentEvent.type == EventType.MouseDrag)) return;
            _textureMap[key] = currentEvent.button switch
            {
                0 => _brushColor,
                1 => _eraseColor,
                _ => _textureMap[key]
            };

            currentEvent.Use();
        }


        private void CheckDictSizeWithDrawingBoardSize()
        {
            if (_minMax.x == _rowNumber && _minMax.y == _colNumber) return;
            _minMax.x = _rowNumber;
            _minMax.y = _colNumber;
            _textureMap = CreateNewDictionary(_rowNumber, _colNumber);
        }

        private void DrawColumnField()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Number of Column : ");
            _colNumber = EditorGUILayout.IntField(_colNumber);
            GUILayout.EndHorizontal();
        }

        private void DrawRowField()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Number of Row : ");
            _rowNumber = EditorGUILayout.IntField(_rowNumber);
            GUILayout.EndHorizontal();
        }

        private Dictionary<Vector2, Color> CreateNewDictionary(int row, int col)
        {
            var dictionary = new Dictionary<Vector2, Color>();

            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < row; j++)
                {
                    dictionary.Add(new Vector2(i, j), Color.white);
                }
            }

            return dictionary;
        }

        private void ChangeMaterial()
        {
            foreach (var obj in _gameObjects)
            {
                obj.TryGetComponent<MeshRenderer>(out var renderer);

                if (!renderer)
                {
                    Debug.Log("Item " + _meshRenderer.gameObject.name + " Doesnt have mesh renderer");
                    return;
                }

                var t2d = new Texture2D(_minMax.x, _minMax.y)
                {
                    filterMode = FilterMode.Point //Simplest non-blend texture mode
                };

                //Materials require Shaders as an argument, Diffuse is the most basic type
                renderer.material = new Material(Shader.Find("Diffuse"));
                //sharedMaterial is the MAIN RESOURCE MATERIAL. Changing this will change ALL objects using it, .material will give you the local instance
                renderer.sharedMaterial.mainTexture = t2d;

                for (var i = 0; i < _minMax.x; i++)
                {
                    for (var j = 0; j < _minMax.y; j++)
                    {
                        // int index = j + i * minMax.y;
                        t2d.SetPixel(i, _minMax.y - 1 - j, _textureMap[new Vector2(i, j)]);
                        //Color every pixel using our color table, the texture is 8x8 pixels large, but stretches to fit
                    }
                }

                t2d.Apply();
            }
        }

        private void ChangeTheBrushColor()
        {
            _brushColor = EditorGUILayout.ColorField("Brush:", _brushColor);
        }

        private void ChangeTheBrushEraser()
        {
            _eraseColor = EditorGUILayout.ColorField("Erase:", _eraseColor);
        }

        private void DragBox()
        {
            CheckMissingReference();

            for (var i = _gameObjects.Count - 1; i >= 0; i--)
            {
                AddAHorizontalLayer(_gameObjects[i]);
            }

            var myRect = GUILayoutUtility.GetRect(0, 100, GUILayout.ExpandWidth(true));
            var guiStyleBoxDnd = new GUIStyle(GUI.skin.box);

            GUI.Box(myRect, "Select Your GameObject", guiStyleBoxDnd);

            if (!myRect.Contains(Event.current.mousePosition)) return;
            {
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        break;
                    case EventType.DragPerform:
                        HandleDragPerformOfDragBoxEvent();
                        break;
                    case EventType.Repaint:
                        return;
                }

                Event.current.Use();
            }
        }

        private void HandleDragPerformOfDragBoxEvent()
        {
            foreach (var obj in DragAndDrop.objectReferences)
            {
                if (obj as GameObject)
                {
                    _gameObjects.Add(obj as GameObject);
                }
            }
        }

        private void AddAHorizontalLayer(GameObject newObject)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(newObject, typeof(GameObject), true);

            if (GUILayout.Button("Delete"))
            {
                _gameObjects.Remove(newObject);
            }

            GUILayout.EndHorizontal();
        }

        private void CheckMissingReference()
        {
            for (var i = _gameObjects.Count - 1; i >= 0; i--)
            {
                if (!_gameObjects[i])
                {
                    _gameObjects.Remove(_gameObjects[i]);
                }
            }
        }
    }
}