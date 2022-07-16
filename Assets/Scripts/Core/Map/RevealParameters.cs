using DG.Tweening;
using UnityEngine;

namespace Core.Map
{
    [CreateAssetMenu]
    public class RevealParameters : ScriptableObject
    {
        public float Duration = 0.5f;
        public float ParticlesDestroyDelay = 2f;
        public Ease EaseType = Ease.OutElastic;

        public ParticleSystem ParticlePrefab;
        public float ParticleVerticalShift = 0.16f;

        [SerializeField]
        MapParameters MapParameters;

        [SerializeField]
        float ParticleDefaultSize = 2f;

        public float RequiredScale => MapParameters.TileWorldSize / ParticleDefaultSize;
        public Vector3 RequiredScale3D => new Vector3(RequiredScale, RequiredScale, RequiredScale);
    }
}