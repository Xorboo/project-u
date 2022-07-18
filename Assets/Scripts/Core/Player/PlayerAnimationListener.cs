using System;
using Core.Units;
using UnityEngine;
using Utils;

namespace Core.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerAnimationListener : MonoBehaviour
    {
        [SerializeField]
        AudioClip StepClip;

        [SerializeField]
        float StepPitchDiff = 0.2f;

        [SerializeField]
        TemporaryObjectSpawner StepSpawner;

        [SerializeField]
        AudioPlayer AudioPlayer;

        Action AttackDamageListener;
        Action AttackCompletedListener;


        #region Unity

        #endregion

        public void SetAttackListener(Action onAttackDamage, Action onAttackCompleted)
        {
            if (AttackDamageListener != null)
                Debug.LogWarning("Overriding player attack damage listener");
            AttackDamageListener = onAttackDamage;

            if (AttackCompletedListener != null)
                Debug.LogWarning("Overriding player attack complete listener");
            AttackCompletedListener = onAttackCompleted;
        }

        public void OnWalkStepEvent()
        {
            StepSpawner.SpawnObject(transform.position);
            if (StepClip != null)
                AudioPlayer.PlayOneShot(StepClip, StepPitchDiff);
        }

        public void OnAttackDamageEvent()
        {
            AttackDamageListener?.Invoke();
            AttackDamageListener = null;
        }

        public void OnAttackCompletedEvent()
        {
            AttackCompletedListener?.Invoke();
            AttackCompletedListener = null;
        }
    }
}