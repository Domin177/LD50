using UnityEngine;

namespace Weapons
{
    public class Shotgun : Weapon
    {
        private const float Rate = 0.95f;
        private const float BulletDamage = 2.5f;
        private const float RateMod = 1f;
        private const float DamageMod = 4f;
        private const float BulletAmount = 5;

        private const float AngleConst = 0.1f;

        public static int AvailableFromLevel = 5;
        
        private float _nextFire = 0f;
        
        private static readonly GameObject Bullet = Resources.Load<GameObject>("Bullet");
        //TODO add gameobject of weapon
        
        public override bool Shoot(Transform transform, int level)
        {
            if (Time.time > _nextFire)
            {
                Quaternion mainRotation = transform.rotation;
                _nextFire = Time.time + Rate - RateMod * 0.15f;
                float angle = mainRotation.z - (((BulletAmount - 1) / 2) * AngleConst);
                for (int i = 1; i <= BulletAmount; i++)
                {
                    Quaternion rotation = new Quaternion(mainRotation.x, mainRotation.y, angle, mainRotation.w);
                    
                    BulletScript bulletScript = GameObject.Instantiate(Bullet, transform.position, rotation)
                        .GetComponent<BulletScript>();
                    bulletScript.damage = ModifyDamage(level,BulletDamage * DamageMod);
                    angle += AngleConst;
                }

                return true;
            }

            return false;
        }

        public override CollectableScript.CollectType GetAmmoType()
        {
            return CollectableScript.CollectType.ShotgunAmmo;
        }

        public override bool IsAvailable(int level)
        {
            return level >= AvailableFromLevel;
        }

        public override bool CanShow(int level)
        {
            return level == AvailableFromLevel;
        }
    }
}