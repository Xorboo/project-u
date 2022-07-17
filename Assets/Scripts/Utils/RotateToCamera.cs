using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    GameObject _camera;
    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    
    void Update()
    {
        transform.LookAt(_camera.transform.position);
        transform.Rotate(0, 180, 0, Space.Self);
    }
}
