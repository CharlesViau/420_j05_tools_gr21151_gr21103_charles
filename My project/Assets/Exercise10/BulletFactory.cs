using UnityEngine;

namespace Exercise10
{
    public enum BulletType
    {
        Bullet
    }
    public class BulletFactory : Factory<BulletFactory, BulletType>
    {
        public override void PostInit()
        {
            
        }
    }
}
