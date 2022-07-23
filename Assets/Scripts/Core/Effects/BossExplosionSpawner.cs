using Core.Units;
using UnityEngine;

namespace Core.Effects
{
    public class BossExplosionSpawner : HitSpawner
    {
        [SerializeField]
        Transform CrownTransform;


        #region Unity

        void OnEnable()
        {
            GetComponent<EnemyUnit>().OnUnitDied += BossDied;
        }

        void OnDisable()
        {
            GetComponent<EnemyUnit>().OnUnitDied -= BossDied;
        }

        #endregion

        void BossDied()
        {
            SpawnHit(CrownTransform);
        }
    }
}