using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        AudioSource AudioSource
        {
            get
            {
                if (!AudioSourceValue)
                {
                    AudioSourceValue = GetComponent<AudioSource>();
                    DefaultPitch = AudioSourceValue.pitch;
                }

                return AudioSourceValue;
            }
        }

        AudioSource AudioSourceValue;
        float DefaultPitch = 1f;

        #region Unity

        #endregion

        public void PlayOneShot(AudioClip clip)
        {
            AudioSource.PlayOneShot(clip);
        }

        public void PlayOneShot(AudioClip clip, float pitchDiff)
        {
            AudioSource.pitch = Random.Range(DefaultPitch - pitchDiff, DefaultPitch + pitchDiff);
            AudioSource.PlayOneShot(clip);
        }
    }
}