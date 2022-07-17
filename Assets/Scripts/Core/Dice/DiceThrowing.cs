using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class DiceThrowing : MonoBehaviour
    {
        bool ready_for_throwing = false;
        bool following_mouse = false;

        [SerializeField] LayerMask layerMask_take, layerMask_throw;
        Rigidbody _body;
        Vector3 rotating_direction;
        [SerializeField] float throwing_force = 3f;

        bool throwing = false;
        Vector3 rotation_angles_old;
        int frames_count = 0;

        [SerializeField] Transform DicePositionAtCamera;


        #region Unity

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

        #endregion

        void Start()
        {
            _body = GetComponent<Rigidbody>();
            _body.isKinematic = true;
        }

        void DieWaitingChanged(bool isWaiting)
        {
            ready_for_throwing = isWaiting;            
        }

        void Update()
        {
            if (ready_for_throwing)
            {
                transform.position = DicePositionAtCamera.position;
                transform.rotation = DicePositionAtCamera.rotation;
            }

            if (Input.GetMouseButtonDown(0) && !throwing)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, layerMask_take))
                {
                    if (hit.transform.CompareTag("Dice"))
                    {
                        ready_for_throwing = false;
                        _body.isKinematic = false;
                        following_mouse = true;
                        rotating_direction = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3)); //Каждый бросок кубик будет крутиться в случайном направлении, чтобы игрок не приноровился кидать шестерки
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && following_mouse)
            {
                following_mouse = false;
                throwing = true;
                rotation_angles_old = transform.rotation.eulerAngles;
                frames_count = 0;
            }

            if (following_mouse)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, layerMask_throw))
                {
                    Vector3 mouse_position = hit.point;
                    //_body.AddForce((mouse_position - transform.position) * Mathf.Lerp(0, throwing_force, dist / 10f));
                    _body.velocity = (mouse_position - transform.position) * throwing_force;
                    //transform.position = hit.point;                
                    _body.AddTorque(rotating_direction);
                }
            }

            if (throwing)
            {
                Vector3 rotation_angles_new = transform.rotation.eulerAngles;
                if (rotation_angles_new == rotation_angles_old)
                {
                    frames_count++;
                    if (frames_count > 30)
                    {
                        throwing = false;
                        ThrowingResult();
                    }
                }
                else
                {
                    frames_count = 0;
                }
                rotation_angles_old = rotation_angles_new;
            }
        }

        void ThrowingResult()
        {
            float x = Mathf.Round(transform.rotation.eulerAngles.x);
            float z = Mathf.Round(transform.rotation.eulerAngles.z);
            int dice_value = 0;

            if (z == 0)
                dice_value = 1;
            if (z == 270)
                dice_value = 2;
            if (x == 270)
                dice_value = 3;
            if (x == 90)
                dice_value = 4;
            if (z == 90)
                dice_value = 5;
            if (z == 180)
                dice_value = 6;

            //print("Dice value: " + dice_value);

            if (dice_value > 0)
                GameManager.Instance.OnDieThrown(dice_value);
            else //Например, куб уперся в коллизию и остановился под углом. Значит надо перебросить.
            {
                ready_for_throwing = true;
                following_mouse = false;
                throwing = false;
            }
        }
    }
}