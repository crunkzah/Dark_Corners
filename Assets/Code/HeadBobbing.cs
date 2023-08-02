using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoSingleton<HeadBobbing>
{
    public float amplitude = 0.02f;
    public float period = 1;
    float x;
    float v;

    public void UpdateMe(Vector3 velocity)
    {
        if(velocity.x * velocity.z != 0)
            velocity *= 0.70710678118F;
        if(velocity.x * velocity.z < 0)
            velocity.x *= -1;

        float _v = velocity.x + velocity.z;
        if(_v == 0)
            v = Mathf.MoveTowards(v, velocity.x + velocity.z, Time.deltaTime * 1);
        else
            v = _v;

        if(v != 0)
            x += v * Time.deltaTime * period;

        float xCoord = amplitude * Mathf.Cos(x);
        float yCoord = amplitude * Mathf.Abs(Mathf.Cos(x));
        //float zCoord = z_mult * Mathf.Cos(z);
        float zCoord = 0;

        transform.localPosition = new Vector3(xCoord, yCoord, zCoord);
    }
}
