using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    Vector3 velocity;
    public void Launch(Vector3 pos, Vector3 _velocity)
    {
        transform.position = pos;
        velocity = _velocity;
    }

    float lifeTimer = 4;

    void Update()
    {
        float dt = Time.deltaTime;
        lifeTimer -= dt;
        if(lifeTimer <= 0)
        {
            Destroy(this.gameObject);
        }
        
        //transform.Rotate(new Vector3(0, 1 * 720 * dt, 0), Space.Self);
        transform.Translate(velocity * dt, Space.World);
    }

}
