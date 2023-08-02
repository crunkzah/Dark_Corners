using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    SphereCollider col;

    void Awake()
    {
        col = GetComponent<SphereCollider>();
    }

    public TrailRenderer trail;
    public ParticleSystem ps_loop;

    void StopEmitting()
    {
        if(trail)
            trail.emitting = false;
        if(ps_loop)
            ps_loop.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    void Disappear()
    {
        col.enabled = false;
        StopEmitting();
        Destroy(this.gameObject, 1);
    }


    public LayerMask collisionMask;
    public Vector3 velocity;

    public void Launch(Vector3 pos, Vector3 vel)
    {
        transform.position = pos;
        velocity = vel;
        lifeTimer = 4;
    }

    float lifeTimer;
    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        lifeTimer-= dt;
        if(lifeTimer <= 0)
        {
            Disappear();
            return;
        }

        // if(!isWorking)
        //     return;

        if(velocity.sqrMagnitude > 0)
        {
            RaycastHit hit;
            if(Physics.SphereCast(transform.position, col.radius, velocity.normalized, out hit, dt * velocity.magnitude))
            {
                velocity = Vector3.Reflect(velocity.normalized, hit.normal) * velocity.magnitude;
            }
            transform.forward = velocity.normalized;
        }

        transform.Translate(velocity * dt, Space.World);
    }
}
