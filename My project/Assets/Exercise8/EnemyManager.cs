using UnityEngine;

namespace Exercise8
{
    public class EnemyManager : Manager<EnemyManager, Enemy>
    {
        private GameObject _enemyPrefab;
        public override void Init()
        {
            _enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
            Add(Object.Instantiate(_enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Enemy>());

            foreach (var item in collection)
            {
                item.Init();
            }
        }

        public override void PostInit()
        {
            foreach (var item in collection)
            {
                item.PostInit();
            }
        }

        public void NotifySuperPlayer()
        {
            foreach (var item in collection)
            {
                item.onFear?.Invoke();
            }
        }
    }
}