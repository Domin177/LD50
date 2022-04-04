using UnityEngine;

namespace Weapons
{
    public class MachineGun : Weapon
    {
        private float _rate = 0.225f;
        private float _nextFire = 0f;
        private float _bulletDamage = 2.5f;
        private float _rateMod = 1f;
        private float _damageMod = 2f;
        public static int AvailableFromLevel = 2;
        private static readonly GameObject Bullet = Resources.Load<GameObject>("Bullet");
        
        public override bool Shoot(Transform transform, int level)
        {
            if (Time.time > _nextFire)
            {
                _nextFire = Time.time + _rate - _rateMod * 0.15f;
                BulletScript bulletScript = GameObject.Instantiate(Bullet, transform.position, transform.rotation)
                    .GetComponent<BulletScript>();
                bulletScript.damage = ModifyDamage(level,_bulletDamage * _damageMod);
                return true;
            }

            return false;
        }

        public override CollectableScript.CollectType GetAmmoType()
        {
            return CollectableScript.CollectType.RifleAmmo;
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