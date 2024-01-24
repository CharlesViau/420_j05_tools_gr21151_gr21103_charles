using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Exercise10
{
    public class GameManager : MonoBehaviour
    {
        private readonly List<Manager> _managers = new List<Manager>();

        #region GameFlow (MainEntry)

        private void Awake()
        {
            AddManagersToList();
            InitManagers();
        }

        private void Start()
        {
            PostInitManagers();
        }

        private void Update()
        {
            RefreshManagers();
        }

        private void FixedUpdate()
        {
            FixedRefreshManagers();
        }

        private void OnDestroy()
        {
            CleanManagers();
        }

        #endregion

        #region Class Methods

        private void InitManagers()
        {
            foreach (var manager in _managers)
            {
                manager.Init();
            }
        }

        private void PostInitManagers()
        {
            foreach (var manager in _managers)
            {
                manager.PostInit();
            }
        }

        private void RefreshManagers()
        {
            foreach (var manager in _managers)
            {
                manager.Refresh();
            }
        }

        private void FixedRefreshManagers()
        {
            foreach (var manager in _managers)
            {
                manager.FixedRefresh();
            }
        }

        private void CleanManagers()
        {
            foreach (var manager in _managers)
            {
                manager.Clean();
            }
        }
        
        private void AddManagersToList()
        {
            foreach (var type in
                     Assembly.GetAssembly(typeof(Manager)).GetTypes().Where(myType =>
                         myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Manager))))
            {
                _managers.Add((Manager) type.GetProperty("Instance")?.GetValue(type));
            }
        }

        #endregion
    }
}