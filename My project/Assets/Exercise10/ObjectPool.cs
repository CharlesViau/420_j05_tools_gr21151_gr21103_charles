using System;
using System.Collections.Generic;

namespace Exercise10
{
    public class ObjectPool
    {
        #region Singleton
        private static ObjectPool _instance;

        private ObjectPool()
        {
            _poolDict = new Dictionary<Type, Stack<IPoolable>>();
        }

        public static ObjectPool Instance
        {
            get { return _instance ??= new ObjectPool(); }
        }
        #endregion

        private Dictionary<Type, Stack<IPoolable>> _poolDict;
        
        public void Pool(Type componentType, IPoolable toPool)
        {
            if(!_poolDict.ContainsKey(componentType))
                _poolDict.Add(componentType, new Stack<IPoolable>());
            
            _poolDict[componentType].Push(toPool);
            toPool.Pool();
        }
        

        public IPoolable Depool(Type componentType)
        {
            if (!_poolDict.ContainsKey(componentType))
                return null; 

            if (_poolDict[componentType].Count <= 0) return null;
            _poolDict[componentType].Peek().Depool();
            return _poolDict[componentType].Pop();
        }
    }
}
