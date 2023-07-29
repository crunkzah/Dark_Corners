using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    public Camera fpsCam;
    public float cam_height = 2;
    public void LateUpdate()
    {
        transform.localPosition = Math.Modify_Y(transform.localPosition, cam_height);
    }
}
