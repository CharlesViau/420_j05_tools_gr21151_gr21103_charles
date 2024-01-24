using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Exercise4
{
    public class PrefabManagerEditor : EditorWindow
    {
        private Vector2 _scrollPosition;
        private List<GameObject> _prefabs = new List<GameObject>();
        private List<string> _paths = new List<string>();

        private string dragBoxText = "Add your New Folder";
        private string[] _guids2;

        private List<string> _toolbarString = new List<string>();
        private int _toolbarInt;

        private int _xPosition;
        private int _zPosition;


        private List<GameObject> _gameObjects = new List<GameObject>();
        private static EditorWindow window;

        private Vector2Int _minMax;
        [MenuItem("MyWindows/Prefab Manager")]
        public static void OpenWindow()
        {
            window = GetWindow<PrefabManagerEditor>("Prefab Manager");
        }
        public void OnGUI()
        {
            _toolbarInt = GUILayout.Toolbar(_toolbarInt, _toolbarString.ToArray());

            DragBoxForPrefab();


            GUILayout.BeginHorizontal();
            //right
            GUILayout.BeginVertical();
            DragBox();
            GUILayout.BeginHorizontal();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            //end
            //left (up)
            GUILayout.BeginVertical();
            GUILayout.BeginVertical();


            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, true, true, GUILayout.Width(800), GUILayout.Height(200));
            GUILayout.BeginHorizontal();
        
            if (GUILayout.Button("Get all the Prefab in this window"))
            {
                OpenTheFolder();
            }
            foreach (var texture in _prefabs.Where(texture => GUILayout.Button(texture.name, GUILayout.Width(200), GUILayout.Height(200))))
            {
                for (var j = 0; j < _gameObjects.Count; j++)
                {
                    var obj = Instantiate(texture);
                    obj.transform.position=_gameObjects[j].transform.position;
                    DestroyImmediate(_gameObjects[j]);
                    _gameObjects.Remove(_gameObjects[j]);
                    _gameObjects.Add(obj);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DragBox()
        {
            foreach (var texture in _gameObjects)
            {
                AddAHorizontalLayer(texture);
            }
            var myRect = GUILayoutUtility.GetRect(0, 100, GUILayout.ExpandWidth(true));
            var guiStyleBoxDnd = new GUIStyle(GUI.skin.box);
            GUI.Box(myRect, "Select Your GameObject", guiStyleBoxDnd);
            if (!myRect.Contains(Event.current.mousePosition)) return;
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    DragUpdateEvent();
                    break;
                case EventType.DragPerform:
                {
                    OnDragPerformEvent();
                    break;
                }
                case EventType.Repaint:
                   return;
            }
            
            Event.current.Use();


        }

        private void OnDragPerformEvent()
        {
            foreach (var texture in DragAndDrop.objectReferences)
            {
                var obj = texture as GameObject;

                if (obj != null)
                {
                    _gameObjects.Add(obj);
                }
            }
        }

        private static void DragUpdateEvent()
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }

        private void AddAHorizontalLayer(GameObject newObject)
        {
            var index = GetIndex(_gameObjects, newObject);

            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(newObject, typeof(GameObject), true);
            if (GUILayout.Button("Delete"))
            {
                DeleteItem(_gameObjects, index);
            }
            GUILayout.EndHorizontal();
        }

        private static void DeleteItem(List<GameObject> list, int currentIndex)
        {
            list.Remove(list[currentIndex]);
        }

        private static int GetIndex(IReadOnlyList<GameObject> list, GameObject gameObj)
        {
            var index = 0;


            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] == gameObj)
                {
                    index = i;
                }
            }
            return index;
        }

        private void DragBoxForPrefab()
        {


            var myRect = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            var guiStyleBoxDnd = new GUIStyle(GUI.skin.box);
            GUI.Box(myRect, dragBoxText, guiStyleBoxDnd);

            if (!myRect.Contains(Event.current.mousePosition)) return;
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    break;
                case EventType.DragPerform:
                    DragPerformEvent();
                    break;
            }

            Event.current.Use();
        }

        private void DragPerformEvent()
        {
            foreach (var obj in DragAndDrop.objectReferences)
            {
                if (obj == null) continue;
                _paths.Add(AssetDatabase.GetAssetPath(obj).ToString());
                _toolbarString.Add(AssetDatabase.GetAssetPath(obj).ToString());

                // remove the same folders 
                _toolbarString = (List<string>) _toolbarString.Distinct().ToList();
            }
        }

        private void OpenTheFolder()
        {        // Find all Texture2Ds that have 'co' in their filename, that are labelled with 'architecture' and are placed in 'MyAwesomeProps' folder

            _guids2 = AssetDatabase.FindAssets(" t:GameObject", new[] { _paths[_toolbarInt] });

            _prefabs.Clear();
            foreach (var guid2 in _guids2)
            {

                var t = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid2));
                _prefabs.Add(t);

            }
        }
    }
}


