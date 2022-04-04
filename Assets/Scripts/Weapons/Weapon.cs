using UnityEngine;

namespace Weapons
{
    public abstract class Weapon
    {
        public abstract bool Shoot(Transform transform, int level);
        public abstract CollectableScript.CollectType GetAmmoType();

        public abstract bool IsAvailable(int level);

        public abstract bool CanShow(int level);

        protected float ModifyDamage(int level, float damage)
        {
            return damage + level / 10f;
        }
    }
}