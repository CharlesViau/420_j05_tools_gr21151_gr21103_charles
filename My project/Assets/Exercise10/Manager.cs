using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Exercise10
{
    public abstract class Manager : IUpdaptable
    { 
        public abstract void FixedRefresh();
        public abstract void Init();
        public abstract void PostInit();
        public abstract void Refresh();
        public abstract void Clean();
    }

    public abstract class SingleObjectManager<T, T1> : Manager where T : class, new() where T1 : Object, IUpdaptable
    {
        #region Singleton

        private static T _instance;
        public static T Instance => _instance ??= new T();
        private SingleObjectManager() { }
        
        #endregion
        
        #region Variables & Properties
        public T1 Obj { get; private set; }
        #endregion
        
        #region Public Methods

        public override void Init()
        {
            Obj = Object.FindObjectOfType<T1>();
            Obj.Init();
        }

        public override void PostInit()
        {
            Obj.PostInit();
        }

        public override void Refresh()
        {
            Obj.Refresh();
        }

        public override void FixedRefresh()
        {
            Obj.FixedRefresh();
        }

        public override void Clean()
        {
        }

        #endregion
        
    }

    public abstract class CollectionManager<T, T1> : Manager where T : class, new() where T1 : Object, IUpdaptable
    {
        #region Singleton

        private static T _instance;
        public static T Instance => _instance ??= new T();

        protected CollectionManager()
        {
            Collection = new HashSet<T1>();
            ToAdd = new Stack<T1>();
            ToRemove = new Stack<T1>();
        }

        #endregion

        #region Variables & Properties

        protected HashSet<T1> Collection;
        protected Stack<T1> ToAdd;
        protected Stack<T1> ToRemove;

        #endregion

        #region Public Methods

        public override void Init()
        {
            FindAllObjectsOfTypeToCollection();
            InitCollection();
        }

        public override void PostInit()
        {
            PostInitCollection();
        }

        public override void Refresh()
        {
            RemoveStackItemsFromCollection();
            UpdateCollection();
            AddStackItemsToCollection();
        }

        public override void FixedRefresh()
        {
            FixedUpdateCollection();
        }

        public void Add(T1 item)
        {
            ToAdd.Push(item);
        }

        public void Remove(T1 item)
        {
            ToRemove.Push(item);
        }

        public override void Clean()
        {
            CleanManager();
        }

        #endregion

        #region Protected Methods

        //Those methods are called by default but could be used by children in case of override.
        protected void InitCollection()
        {
            foreach (var item in Collection)
            {
                item.Init();
            }
        }
        protected void PostInitCollection()
        {
            foreach (var item in Collection)
            {
                item.PostInit();
            }
        }
        protected void AddStackItemsToCollection()
        {
            while (ToAdd.Count > 0)
            {
                Collection.Add(ToAdd.Pop());
            }
        }

        protected void RemoveStackItemsFromCollection()
        {
            while (ToRemove.Count > 0)
            {
                Collection.Remove(ToRemove.Pop());
            }
        }

        protected void UpdateCollection()
        {
            foreach (var item in Collection)
            {
                item.Refresh();
            }
        }

        protected void FixedUpdateCollection()
        {
            foreach (var item in Collection)
            {
                item.FixedRefresh();
            }
        }

        protected void CleanManager()
        {
            Collection.Clear();
            ToAdd.Clear();
            ToRemove.Clear();
        }

        protected void FindAllObjectsOfTypeToCollection()
        {
            Collection = new HashSet<T1>(Object.FindObjectsOfType<T1>().ToList());
        }
        
        #endregion
    }
}