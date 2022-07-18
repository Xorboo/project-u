using UnityEngine;

namespace Core.Units
{
    public class TemporaryObjectSpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject Prefab;

        [SerializeField]
        bool SpawnOnAwake = false;

        [SerializeField]
        bool DestroyAutomatically = false;

        [SerializeField, Min(0)]
        float DestroyDelay;


        #region Unity

        void Awake()
        {
            if (SpawnOnAwake)
            {
                SpawnObject();
            }
        }

        #endregion

        public GameObject SpawnObject()
        {
            return SpawnObject(Vector3.zero, Quaternion.identity);
        }

        public GameObject SpawnObject(Vector3 position)
        {
            return SpawnObject(position, Quaternion.identity);
        }

        public GameObject SpawnObject(Vector3 position, Quaternion rotation)
        {
            if (Prefab == null)
            {
                Debug.LogWarning("Can't spawn temporary object, prefab is not set");
                return null;
            }

            var spawnedObject = Instantiate(Prefab, position, rotation);

            if (DestroyAutomatically)
                Destroy(spawnedObject, DestroyDelay);

            return spawnedObject;
        }
    }
}