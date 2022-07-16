using System;
using DG.Tweening;
using UnityEngine;

namespace Core.Map
{
    public class TileRevealController : MonoBehaviour
    {
        [SerializeField]
        RevealParameters Parameters;

        [SerializeField]
        GameObject HiddenSide;

        [SerializeField]
        GameObject MainSide;


        public bool IsRevealed { get; private set; } = false;


        #region Unity

        void Awake()
        {
            IsRevealed = false;
            HiddenSide.SetActive(true);
            MainSide.SetActive(false);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (HiddenSide == null)
            {
                var uncovered = transform.Find("UncoveredSide");
                HiddenSide = uncovered != null ? uncovered.gameObject : null;
            }

            if (MainSide == null)
            {
                var root = transform.Find("Root");
                MainSide = root != null ? root.gameObject : null;
            }
        }
#endif

        #endregion


        public void Reveal()
        {
            IsRevealed = true;
            HiddenSide.SetActive(false);
            MainSide.SetActive(true);

            // Animate scale
            var t = MainSide.transform;
            Vector3 defaultScale = t.localScale;
            t.localScale = new Vector3(defaultScale.x, 0f, defaultScale.z);
            t.DOScaleY(defaultScale.y, Parameters.Duration).SetEase(Parameters.EaseType);

            // Add particles
            var particlesTransform = Instantiate(Parameters.ParticlePrefab, transform).transform;
            particlesTransform.localPosition = new Vector3(0f, Parameters.ParticleVerticalShift, 0f);
            particlesTransform.localScale = Parameters.RequiredScale3D;
            Destroy(particlesTransform.gameObject, Parameters.ParticlesDestroyDelay);
        }
    }
}