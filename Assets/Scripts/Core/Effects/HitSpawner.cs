using Core.Units;
using UnityEngine;

namespace Core.Effects
{
    public class HitSpawner : TemporaryObjectSpawner
    {
        [SerializeField]
        float CameraShiftDistance = 0.7f;


        #region Unity

        #endregion


        public void SpawnHit(Transform unit)
        {
            Vector3 pos = unit.position;
            Vector3 cameraDir = (Camera.main.transform.position - pos).normalized;
            SpawnObject(pos + cameraDir * CameraShiftDistance);
        }
    }
}