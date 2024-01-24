using System.Collections.Generic;
using UnityEngine;

namespace Exercise8
{
    public class GameManager : MonoBehaviour
    {
        private readonly List<Manager> _managers = new List<Manager>();

        #region MainEntry
        private void Awake()
        {
            _managers.Add(EnemyManager.Instance);

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
        #endregion
    }
}
