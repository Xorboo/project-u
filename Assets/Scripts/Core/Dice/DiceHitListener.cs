using UnityEngine;

namespace Core.Dice
{
    [RequireComponent(typeof(AudioSource))]
    public class DiceHitListener : MonoBehaviour
    {
        [SerializeField]
        AudioClip HitClip;

        [SerializeField]
        float PitchDiff = 0.2f;

        [SerializeField]
        float MinRepeatDelay = 0.3f;

        [SerializeField, Range(0, 1)]
        float Volume = 0.5f;

        [SerializeField]
        GameObject EffectPrefab;

        AudioSource AudioSource;
        float LastPlayTime = float.MinValue;


        #region Unity

        void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
        }

        void OnCollisionEnter(Collision col)
        {
            var point = col.contacts[0].point;

            if (Time.time > LastPlayTime + MinRepeatDelay)
            {
                var effect = Instantiate(EffectPrefab);
                effect.transform.position = transform.position;
                Destroy(effect.gameObject, 1f);

                AudioSource.pitch = Random.Range(1 - PitchDiff, 1 + PitchDiff);
                AudioSource.PlayOneShot(HitClip, Volume);

                LastPlayTime = Time.time;
            }
        }

        #endregion
    }
}