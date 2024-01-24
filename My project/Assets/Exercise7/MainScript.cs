using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Exercise7
{
    public class MainScript : MonoBehaviour
    {
        [SerializeField] private Transform ballsParentTransform;
        [SerializeField] private GameObject ballPrefab;

        public List<GameObject> Spheres { get; private set; }

        private void Awake()
        {
            Spheres = new List<GameObject>();
        }

        private void Start()
        {
            for (var i = 0; i < 50 - Spheres.Count; i++)
            {
                var obj = Instantiate(ballPrefab, ballsParentTransform);

                Spheres.Add(obj);
            }
        }

        public void DestroySpheres()
        {
            foreach (Transform child in ballsParentTransform)
            {
                Destroy(child.gameObject);
            }
        }

        public List<RollingSphereData> SaveSpheres()
        {
            return Spheres.Select(sphere => sphere.GetComponent<RollingSphere>().Save()).ToList();
        }

        public void Load(List<RollingSphereData> list)
        {
            foreach (var obj in list)
            {
                var ball = Instantiate(ballPrefab, ballsParentTransform);
                ball.GetComponent<RollingSphere>().Load(obj);
            }
        }
    }
}