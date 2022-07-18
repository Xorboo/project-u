using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(Camera))]
    public class CameraColliderSpawner : MonoBehaviour
    {
        [SerializeField]
        LayerMask PlaneLayer;

        [SerializeField]
        PhysicMaterial PlaneMaterial;

        [SerializeField]
        string Tag = "CameraPlane";

        #region Unity

        void Awake()
        {
            var camera = GetComponent<Camera>();

            // Calculate the planes from the main camera's view frustum
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            int layer = GetLayerNumber();

            // Create a "Plane" GameObject aligned to each of the calculated planes
            for (int i = 0; i < 6; ++i)
            {
                GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
                p.name = $"Plane {i}";
                p.tag = "CameraPlane";
                p.transform.position = -planes[i].normal * planes[i].distance;
                p.transform.rotation = Quaternion.FromToRotation(Vector3.up, planes[i].normal);
                p.transform.localScale = new Vector3(100, 100, 100);
                p.transform.SetParent(transform, true);
                p.GetComponent<MeshCollider>().material = PlaneMaterial;
                Destroy(p.GetComponent<MeshRenderer>());

                p.layer = layer;
            }
        }

        #endregion

        int GetLayerNumber()
        {
            int layerNumber = 0;
            int layer = PlaneLayer.value;
            while (layer > 0)
            {
                layer >>= 1;
                layerNumber++;
            }

            return layerNumber - 1;
        }
    }
}