using System;
using UnityEngine;

namespace Weapons
{
    public class Pistol : Weapon
    {
        private float _rate = 0.8f;
        private float _nextFire = 0f;
        private float _bulletDamage = 3f;
        private float _rateMod = 1f;
        private float _damageMod = 3f;
        private static readonly GameObject Bullet = Resources.Load<GameObject>("Bullet");
        //TODO add gameobject of weapon
        
        public override bool Shoot(Transform transform, int level)
        {
            if (Time.time > _nextFire)
            {
                _nextFire = Time.time + _rate - _rateMod * 0.15f;
                BulletScript bulletScript = GameObject.Instantiate(Bullet, transform.position, transform.rotation)
                    .GetComponent<BulletScript>();
                bulletScript.damage = ModifyDamage(level, _bulletDamage * _damageMod);

                return true;
            }

            return false;
        }

        public override CollectableScript.CollectType GetAmmoType()
        {
            return CollectableScript.CollectType.PistolAmmo;
        }

        public override bool IsAvailable(int level)
        {
            return true;
        }

        public override bool CanShow(int level)
        {
            return false;
        }
    }
}