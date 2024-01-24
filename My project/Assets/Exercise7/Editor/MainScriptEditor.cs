using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Exercise7.Editor
{
    [CustomEditor(typeof(MainScript))]
    public class MainScriptEditor : UnityEditor.Editor
    {
        private MainScript _mainScript;
        private void Awake()
        {
            _mainScript = target as MainScript;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Save"))
            {
                Save();
            }

            if (GUILayout.Button("Load"))
            {
                Load();
            }
        }

        private void Save()
        {
            
            var listString = JsonUtility.ToJson(new Wrapper<RollingSphereData>(_mainScript.SaveSpheres()));
            Debug.Log(Application.streamingAssetsPath);
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "balls.json"), listString);
        }

        private void Load()
        {
            if (!Directory.Exists(Application.streamingAssetsPath)) return;
            _mainScript.Spheres?.Clear();
            _mainScript.DestroySpheres();
            
            var data = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "balls.json"));
            var spheres = JsonUtility.FromJson<Wrapper<RollingSphereData>>(data);

           _mainScript.Load(spheres.List);
        }

        private class Wrapper<T>
        {
            public List<T> List;
            public Wrapper(List<T> list)
            {
                List = list;
            }
        }
    }
}
