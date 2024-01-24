using System;
using UnityEngine;

namespace Exercise7
{
    public class RollingSphere : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private Color _color;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _color =  GetComponent<MeshRenderer>().material.color = new Color(
                                         UnityEngine.Random.Range(0f, 1f),
                                         UnityEngine.Random.Range(0f, 1f),
                                         UnityEngine.Random.Range(0f, 1f)
                                     );
        }


        private void FixedUpdate()
        {
            if(_rigidbody.velocity.magnitude < 1)
                _rigidbody.AddForce(new Vector3(UnityEngine.Random.Range(50,100),0,UnityEngine.Random.Range(50,100)));
        }

        public RollingSphereData Save()
        {
            return new RollingSphereData(transform.position, _color, _rigidbody.velocity);
        }

        public void Load(RollingSphereData data)
        {
            transform.position = data.position;
            _color = data.color;
            _rigidbody.velocity = data.velocity;
        }
    }
}
