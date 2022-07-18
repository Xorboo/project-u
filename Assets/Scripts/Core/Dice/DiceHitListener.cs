using Core.Units;
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
        TemporaryObjectSpawner ParticleSpawner;

        [SerializeField]
        Vector3 ParticlePositionShift = new Vector3(0, -0.5f, 0);

        AudioSource AudioSource;
        float LastPlayTime = float.MinValue;


        #region Unity

        void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
        }

        void OnCollisionEnter(Collision col)
        {
            if (Time.time > LastPlayTime + MinRepeatDelay)
            {
                ParticleSpawner.SpawnObject(transform.position + ParticlePositionShift);

                AudioSource.pitch = Random.Range(1 - PitchDiff, 1 + PitchDiff);
                AudioSource.PlayOneShot(HitClip, Volume);

                LastPlayTime = Time.time;
            }
        }

        #endregion

        Vector3 GetContactCenter(Collision col)
        {
            var firstPoint = col.contacts[0].point;
            Vector3 min = firstPoint, max = firstPoint;
            foreach (var contact in col.contacts)
            {
                var p = contact.point;

                min.x = Mathf.Min(min.x, p.x);
                min.y = Mathf.Min(min.y, p.y);
                min.z = Mathf.Min(min.z, p.z);

                max.x = Mathf.Max(max.x, p.x);
                max.y = Mathf.Max(max.y, p.y);
                max.z = Mathf.Max(max.z, p.z);
            }

            return (min + max) / 2f;
        }
    }
}