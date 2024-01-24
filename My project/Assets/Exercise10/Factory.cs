using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Exercise10
{
    public interface IFactory
    {
        public void Init();
        public void PostInit();
    }

    public abstract class Factory<T, E> : IFactory where T : class, new() where E : System.Enum
    {
        #region Singleton

        private static T _instance;
        public static T Instance => _instance ??= new T();

        protected Factory()
        {
            _prefabDictionary = new Dictionary<E, GameObject>();
        }

        #endregion
        
        #region Properties and Variables
        
        protected readonly string PrefabLocation = "Prefabs/" + typeof(E) + "/";
        private readonly Dictionary<E, GameObject> _prefabDictionary;
        
        #endregion
        
        #region Public Methods
        public void Init()
        {
            var allPrefabTypes = System.Enum.GetValues(typeof(E)).Cast<E>().ToArray();

            foreach (var element in allPrefabTypes)
            {
                _prefabDictionary.Add(element, Resources.Load<GameObject>(PrefabLocation + element));
            }
        }

        public abstract void PostInit();

        /*public E Create(E type)
        {
            return null;
        }*/

        #endregion
    }
}