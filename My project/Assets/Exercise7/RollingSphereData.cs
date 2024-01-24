using System;
using System.Collections.Generic;
using UnityEngine;

namespace Exercise7
{
    [Serializable]
    public class RollingSphereData
    { 
        public Vector3 position;
        public Color color;
        public Vector3 velocity;

        public RollingSphereData(Vector3 position, Color color, Vector3 velocity)
        {
            this.position = position;
            this.color = color;
            this.velocity = velocity;
        }
    }
}
