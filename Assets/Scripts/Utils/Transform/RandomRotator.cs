using System;
using UnityEngine;
using Utils.Execution;
using Random = UnityEngine.Random;

namespace Utils.Transform
{
    public class RandomRotator : AutoExecutionFromFieldBehaviour
    {
        enum RotationType
        {
            None,
            Degrees90,
            DegreesRandom
        }

        [Flags]
        enum ScaleAxis
        {
            None = 0,
            X = 1,
            Y = 2,
            Z = 4
        }

        [SerializeField]
        RotationType Rotation = RotationType.Degrees90;

        [SerializeField]
        Vector3 RotationAxis = Vector3.up;

        [SerializeField]
        ScaleAxis Scale = ScaleAxis.X | ScaleAxis.Z;


        #region Unity

        #endregion


        protected override void Execute()
        {
            DoRotate();
            DoScale();
        }

        void DoRotate()
        {
            float degreesValue = 0;

            switch (Rotation)
            {
                case RotationType.None:
                    break;
                case RotationType.Degrees90:
                    degreesValue = Random.Range(0, 4) * 90;
                    break;
                case RotationType.DegreesRandom:
                    degreesValue = Random.Range(0f, 360f);
                    break;
                default:
                    Debug.LogError($"Unsupported rotation type [{Rotation}]", gameObject);
                    break;
            }

            if (degreesValue == 0)
                return;

            transform.Rotate(RotationAxis, degreesValue);
        }

        void DoScale()
        {
            if (Scale == ScaleAxis.None)
                return;

            Vector3 scale = Vector3.one;

            if (Scale.HasFlag(ScaleAxis.X))
                scale.x = RandomSide();
            if (Scale.HasFlag(ScaleAxis.Y))
                scale.y = RandomSide();
            if (Scale.HasFlag(ScaleAxis.Z))
                scale.z = RandomSide();

            if (scale == Vector3.one)
                return;

            transform.localScale = Vector3.Scale(transform.localScale, scale);

            float RandomSide() => Random.value > 0.5f ? 1 : -1;
        }
    }
}