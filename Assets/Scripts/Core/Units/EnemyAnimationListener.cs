using System;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(AudioSource))]
    public class EnemyAnimationListener : MonoBehaviour
    {
        Action AttackDamageListener;
        Action AttackCompletedListener;
        Action DeathCompletedListener;


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

        public void SetDeathCompletedListener(Action onDeathCompleted)
        {
            if (DeathCompletedListener != null)
                Debug.LogWarning("Overriding player death listener");
            DeathCompletedListener = onDeathCompleted;
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

        public void OnDeathCompletedEvent()
        {
            DeathCompletedListener?.Invoke();
            DeathCompletedListener = null;
        }
    }
}