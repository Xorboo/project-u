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


        #region Unity

        #endregion


        public void OnWalkStepEvent()
        {
            StepSpawner.SpawnObject(transform.position);
            if (StepClip != null)
                AudioPlayer.PlayOneShot(StepClip, StepPitchDiff);
        }
    }
}