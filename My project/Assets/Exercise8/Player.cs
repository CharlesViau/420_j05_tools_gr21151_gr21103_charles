using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Exercise8
{
    public class Player : MonoBehaviour
    {
        public UnityEvent onSuperPlayer;
        private Material _material;
        private Rigidbody _rigidbody;
        public float speed = 5;
        public const float SuperTimeDuration = 5;
        private const float SuperTimeCooldown = 15;
        private float _currentCooldown;
        private bool CanCast => _currentCooldown <= 0;

        public static Transform Player1;
        

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _material = GetComponent<MeshRenderer>().material;
            Player1 = GetComponent<Transform>();
        }

        private void Start()
        {
            onSuperPlayer.AddListener(() => StartCoroutine(SuperMode()));
            onSuperPlayer.AddListener(()=>EnemyManager.Instance.NotifySuperPlayer());
            _material.color = Color.blue;
        }

        // Update is called once per frame
        private void Update()
        {
            _rigidbody.velocity =
                new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed);

            if (Input.GetKeyDown(KeyCode.Space) && CanCast)
            {
                onSuperPlayer?.Invoke();
            }
        }

        private IEnumerator SuperMode()
        {
            _material.color = Color.red;
            transform.localScale = new Vector3(2, 2, 2);
            speed *= 2;

            for (float time = 0; time < SuperTimeDuration; time += Time.deltaTime)
            {
                yield return null;
            }

            transform.localScale = new Vector3(1, 1, 1);
            _material.color = Color.blue;
            speed /= 2;
            StartCoroutine(CountCooldown());
        }

        private IEnumerator CountCooldown()
        {
            _currentCooldown = SuperTimeCooldown;
            
            for (var time = SuperTimeCooldown; time > 0; time-=Time.deltaTime)
            {
                _currentCooldown -= Time.deltaTime;
                yield return null;
            }

            _currentCooldown = 0;
        }
    }
}