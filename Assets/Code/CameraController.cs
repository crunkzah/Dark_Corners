using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    public Camera fpsCam;
    public Camera worldCam;
    public float cam_height = 2;

    public override void Init()
    {
        base.Init();
        worldCam = GetComponent<Camera>();
    }

    public void LateUpdate()
    {
        transform.localPosition = Math.Modify_Y(transform.localPosition, cam_height);
    }
}
