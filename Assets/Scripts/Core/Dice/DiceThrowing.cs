using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Core.Dice
{
    [RequireComponent(typeof(Rigidbody))]
    public class DiceThrowing : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        bool ReadyForThrowing = false;
        bool FollowingMouse = false;

        [SerializeField]
        LayerMask LayerMaskTake, LayerMaskThrow;

        Rigidbody Body;
        Vector3 RotatingDirection;

        [SerializeField]
        float ThrowingForce = 3f;

        [SerializeField]
        float GravityForceFactor = 5f;

        bool Throwing = false;
        Vector3 UpVectorOld;
        int FramesCount = 0;

        [SerializeField]
        Transform DicePositionAtCamera;

        Camera Camera;


        #region Unity

        void Awake()
        {
            Camera = Camera.main;
            Body = GetComponent<Rigidbody>();
        }

        void OnEnable()
        {
            GameManager.Instance.OnDieWaitingChanged += DieWaitingChanged;
            DieWaitingChanged(GameManager.Instance.IsWaitingForDie);
        }

        void OnDisable()
        {
            if (GameManager.Exists())
                GameManager.Instance.OnDieWaitingChanged -= DieWaitingChanged;
        }

        void Start()
        {
            Body.isKinematic = true;
        }

        void FixedUpdate()
        {
            // Custom gravity to adjust for level scale
            Body.AddForce(GravityForceFactor * Body.mass * Physics.gravity);
        }

        void Update()
        {
            if (ReadyForThrowing)
            {
                transform.position = DicePositionAtCamera.position;
                transform.rotation = DicePositionAtCamera.rotation;
            }

            if (FollowingMouse)
            {
                Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 100, LayerMaskThrow))
                {
                    Vector3 mousePosition = hit.point;
                    //_body.AddForce((mouse_position - transform.position) * Mathf.Lerp(0, throwing_force, dist / 10f));
                    Body.velocity = (mousePosition - transform.position) * ThrowingForce;
                    //transform.position = hit.point;
                    Body.AddTorque(RotatingDirection * Body.mass);
                }
            }

            if (Throwing)
            {
                float angleRotationLimit = 0.1f;
                int idleFramesLimit = 30;

                Vector3 upVectorNew = transform.up;
                if (Vector3.Angle(upVectorNew, UpVectorOld) < angleRotationLimit)
                {
                    FramesCount++;
                    if (FramesCount > idleFramesLimit)
                    {
                        Throwing = false;
                        ThrowingResult();
                    }
                }
                else
                {
                    FramesCount = 0;
                    UpVectorOld = upVectorNew;
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (Throwing)
                return;

            ReadyForThrowing = false;
            Body.isKinematic = false;
            FollowingMouse = true;
            //Каждый бросок кубик будет крутиться в случайном направлении, чтобы игрок не приноровился кидать шестерки
            RotatingDirection = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (!FollowingMouse)
                return;

            FollowingMouse = false;
            Throwing = true;
            UpVectorOld = transform.rotation.eulerAngles;
            FramesCount = 0;
        }

        #endregion


        void DieWaitingChanged(bool isWaiting)
        {
            ReadyForThrowing = isWaiting;
        }

        void ThrowingResult()
        {
            int dieResult = 0;

            float angleLimitDegrees = 10f;

            List<Vector3> directions = new()
                { transform.up, -transform.right, transform.forward, -transform.forward, transform.right, -transform.up };
            var angles = directions.Select(dir => Vector3.Angle(dir, Vector3.up)).ToList();
            float minAngle = angles.Min();
            if (minAngle < angleLimitDegrees)
            {
                int minIndex = angles.IndexOf(minAngle);
                dieResult = minIndex + 1;
            }

            Debug.Log($"Die result: {dieResult}");

            if (dieResult > 0)
            {
                GameManager.Instance.OnDieThrown(dieResult);
            }
            else //Например, куб уперся в коллизию и остановился под углом. Значит надо перебросить.
            {
                Debug.Log($"Reset cube");
                ReadyForThrowing = true;
                FollowingMouse = false;
                Throwing = false;
            }
        }
    }
}