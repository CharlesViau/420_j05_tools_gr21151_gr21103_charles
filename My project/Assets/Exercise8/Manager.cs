using System.Collections.Generic;
using Exercise8;
using UnityEngine;

public abstract class Manager : IUpdaptable
{
    public abstract void FixedRefresh();
    public abstract void Init();
    public abstract void PostInit();
    public abstract void Refresh();
    public abstract void Clean();
}
public abstract class Manager<T, E> : Manager, IManager<E> where T : class, new() where E : IUpdaptable {

    #region Singleton
    private static T instance;
    public static T Instance => instance ??= new T();
    protected Manager() 
    {
        collection = new HashSet<E>();
        toAdd = new Stack<E>();
        toRemove = new Stack<E>();
    }
    #endregion

    #region Variables & Properties

    protected HashSet<E> collection;
    protected Stack<E> toAdd;
    protected Stack<E> toRemove;

    #endregion

    #region Public Methods
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

    public void Add(E item)
    {
        toAdd.Push(item);
    }

    public void Remove(E item)
    {
        toRemove.Push(item);
    }

    public override void Clean()
    {
        CleanManager();
    }
   
    #endregion

    #region Protected Methods
    //Those methods are called by default but could be used by children in case of override.
    protected void AddStackItemsToCollection()
    {
        while (toAdd.Count > 0)
        {
            collection.Add(toAdd.Pop());
        }
    }
    protected void RemoveStackItemsFromCollection()
    {
        while (toRemove.Count > 0)
        {
            collection.Remove(toRemove.Pop());
        }
    }
    protected void UpdateCollection()
    {
        foreach (var item in collection)
        {
            item.Refresh();
        }
    }
    protected void FixedUpdateCollection()
    {
        foreach (var item in collection)
        {
            item.FixedRefresh();
        }
    }

    protected void CleanManager()
    {
        collection.Clear();
        toAdd.Clear();
        toRemove.Clear();
    }

    #endregion
}
