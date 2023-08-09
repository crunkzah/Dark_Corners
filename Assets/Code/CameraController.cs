using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

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

    public PostProcessingProfile dead_profile;

    public void OnDie()
    {   
        PostProcessingBehaviour ppb = GetComponent<PostProcessingBehaviour>();
        if(ppb)
            ppb.profile = dead_profile;
    }

    public void AddFov(float fov)
    {
        worldCam.fieldOfView = Inputs.FIELD_OF_VIEW + fov;
    }

    public void LateUpdate()
    {
        worldCam.fieldOfView = Mathf.MoveTowards(worldCam.fieldOfView, Inputs.FIELD_OF_VIEW, Time.deltaTime * 10);
        transform.localPosition = Math.Modify_Y(transform.localPosition, cam_height);
    }
}
