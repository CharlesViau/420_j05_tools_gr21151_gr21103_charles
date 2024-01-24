using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Exercise8
{
    public class Enemy : MonoBehaviour, IUpdaptable
    {

        private Material _material;
        private EnemyState _state = EnemyState.Normal;
        private Transform _playerTransform;
        private float _speed;
        public UnityEvent onFear;
        private IEnumerator _onSuperPlayer;

        private void Awake()
        {
            _material = GetComponent<MeshRenderer>().material;
            _material.color = Color.green;
            _speed = Random.Range(3, 7);
            onFear.AddListener(()=>StartCoroutine(OnSuperPlayer()));
        }

        private void Start()
        {
            _playerTransform = Player.Player1;
        }

        public void Init()
        {
        }

        public void PostInit()
        {
        }

        public void Refresh()
        {
            switch (_state)
            {
                case EnemyState.Normal:
                    MoveToPlayer();
                    break;
                case EnemyState.Fear:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void FixedRefresh()
        {
           
        }

        private void MoveToPlayer()
        {
            transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, _speed * Time.deltaTime);
        }

        private void MoveOppositeToPlayer()
        {
            transform.position = Vector3.MoveTowards(transform.position, -(_playerTransform.position), _speed * Time.deltaTime);
        }

        
        private IEnumerator OnSuperPlayer()
        {
            _state = EnemyState.Fear;
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            _material.color = Color.yellow;

            for (var i = Player.SuperTimeDuration; i > 0; i-= Time.deltaTime)
            {
                MoveOppositeToPlayer();
                yield return null;
            }
            
            transform.localScale = new Vector3(1, 1, 1);
            _material.color = Color.green;
            _state = EnemyState.Normal;
        }
    }

    public enum EnemyState
    {
        Normal,
        Fear
    }
}
