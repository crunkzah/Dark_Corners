using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway settings:")]
    public float smoothTime = 0.1f;
    public float swayMultiplier = 0.005f;

    Vector3 forward_dir;
    public Vector3 zero_dir = new Vector3(0, 0, 1);
    Vector3 v_velocity;

    void Update()
    {
        if(UberManager.Instance.state == GameState.Paused)
            return;

        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = -Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        forward_dir.y += mouseY;
        forward_dir.x += mouseX;
        forward_dir.Normalize();

        forward_dir = Vector3.SmoothDamp(forward_dir, zero_dir, ref v_velocity, smoothTime, 255);
        forward_dir.Normalize();
        
        transform.localRotation = Quaternion.LookRotation(forward_dir, new Vector3(0, 1, 0));
    }

}
