using DG.Tweening;
using UnityEngine;

namespace Core.Units
{
    public class LookRotation : MonoBehaviour
    {
        [SerializeField]
        float ExtraAngle = 0f;

        [SerializeField]
        Transform RotationRoot;

        [SerializeField]
        float RotationTime = 0.5f;


        Tween RotationTween = null;


        #region Unity

        #endregion


        public void RotateTo(Transform target)
        {
            if (RotationTween != null)
            {
                Debug.LogWarning($"Rotating while past rotation is still active");
                RotationTween.Kill();
                RotationTween = null;
            }

            var currentRotation = RotationRoot.rotation.eulerAngles;
            Quaternion tempRotation = Quaternion.LookRotation(target.transform.position - RotationRoot.position);
            var desiredRotation = Quaternion.Euler(currentRotation.x, tempRotation.eulerAngles.y + ExtraAngle, currentRotation.z);

            RotationTween = RotationRoot
                .DORotateQuaternion(desiredRotation, RotationTime)
                .OnComplete(() => RotationTween = null);
        }
    }
}