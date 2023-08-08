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

    public void LateUpdate()
    {
        transform.localPosition = Math.Modify_Y(transform.localPosition, cam_height);
    }
}
