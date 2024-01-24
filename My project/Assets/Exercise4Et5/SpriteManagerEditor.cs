using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Exercise4
{
    public class SpriteManagerEditor : EditorWindow
    {
        [Tooltip("You can drag and drop multiple folders in here  ")]
        private const string DragBoxText = "Add your New Folder";

        private static EditorWindow _window;
        private string[] _guids2;
        private int _toolbarInt;

        private readonly List<Texture2D> _textures = new List<Texture2D>();
        private readonly List<string> _paths = new List<string>();
        private List<string> _toolbarString = new List<string>();
        private Vector2 _scrollPosition;

        [MenuItem("MyWindows/Sprite Manager")]
        public static void OpenWindow()
        {
            _window = GetWindow<SpriteManagerEditor>("Sprite Manager");
        }

        public void OnGUI()
        {
            _toolbarInt = GUILayout.Toolbar(_toolbarInt, _toolbarString.ToArray());

            DragBox();

            GUILayout.BeginHorizontal();
            //right
            GUILayout.BeginVertical();
            if (GUILayout.Button("Get Textures"))
            {
                OpenTheFolder();
            }

            if (GUILayout.Button("Remove Folder"))
            {
                RemoveThisFolder();
            }

            GUILayout.EndVertical();
            GUILayout.BeginVertical();


            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, true, true, GUILayout.Width(800),
                GUILayout.Height(200));
            GUILayout.BeginHorizontal();

            foreach (var texture in _textures)
            {
                GUILayout.Button(texture, GUILayout.Width(200), GUILayout.Height(200));
            }

            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void RemoveThisFolder()
        {
            _paths.Remove(_paths[_toolbarInt]);
            _toolbarString.Remove(_toolbarString[_toolbarInt]);
            _textures.Clear();
        }

        private void OpenTheFolder()
        {
            

            _guids2 = AssetDatabase.FindAssets(" t:texture2D", new[] {_paths[_toolbarInt]});

            _textures.Clear();
            foreach (var guid2 in _guids2)
            {
                var t = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid2));
                _textures.Add(t);
            }
        }

        private void DragBox()
        {
            var myRect = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            var guiStyleBoxDnd = new GUIStyle(GUI.skin.box);
            GUI.Box(myRect, DragBoxText, guiStyleBoxDnd);

            if (!myRect.Contains(Event.current.mousePosition)) return;
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    DragUpdateEvent();
                    break;
                case EventType.DragPerform:
                    DragPerformEvent();
                    break;
                case EventType.Repaint:
                    return;
            }
            Event.current.Use();
        }

        private void DragPerformEvent()
        {
            foreach (var obj in DragAndDrop.objectReferences)
            {
                if (obj == null) continue;
                _paths.Add(AssetDatabase.GetAssetPath(obj));
                _toolbarString.Add(AssetDatabase.GetAssetPath(obj));

                // remove the same folders 
                _toolbarString = _toolbarString.Distinct().ToList();
            }
        }

        private static void DragUpdateEvent()
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }
    }
}