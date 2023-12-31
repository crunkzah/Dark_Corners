using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public LayerMask hitMask;
    public ParticleSystem ps_loop;
    HashSet<Collider> hit_cols = new HashSet<Collider>();

    float speed = 7;
    public Light _light;
    public AudioSource audioSrcLoop;
    float targetIntensity;
    float targetVolume;
    float lifeTimer;

    Vector3 velocity;
    SphereCollider col;

    public void Launch(Vector3 pos, Vector3 vel)
    {
        col = GetComponent<SphereCollider>();
        
        targetIntensity = _light.intensity;
        _light.intensity = 0;
        transform.position = pos;
        velocity = vel;
        lifeTimer = 4;
        targetVolume = 1;
    }

    public Vector3 GetTeleportPosition()
    {
        return transform.position;
    }

    public void TeleportToOrb()
    {
        Disappear();
    }
    bool isWorking = true;
    void Disappear()
    {
        ps_loop.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        targetIntensity = 0;
        targetVolume = 0;
        isWorking = false;
        if(SpellsController.Instance.activeOrb && SpellsController.Instance.activeOrb == this)
            SpellsController.Instance.activeOrb = null;

        Destroy(this.gameObject, 3);
    }


    void Update()
    {
        float dt = Time.deltaTime;
        _light.intensity = Mathf.MoveTowards(_light.intensity, targetIntensity, 4 * dt);
        audioSrcLoop.volume = Mathf.MoveTowards(audioSrcLoop.volume, targetVolume, 4 * dt);

        lifeTimer-= dt;
        if(lifeTimer <= 0)
        {
            Disappear();
        }

        if(!isWorking)
            return;

        if(velocity.sqrMagnitude > 0)
        {
            RaycastHit hit;
            if(Physics.SphereCast(transform.position, col.radius, velocity.normalized, out hit, dt * velocity.magnitude, hitMask))
            {
                if(hit.collider.gameObject.isStatic)
                    velocity = Vector3.Reflect(velocity.normalized, hit.normal) * velocity.magnitude;
                else
                {
                    if(!hit_cols.Contains(hit.collider))
                    {
                        IDamagable idamagable = hit.collider.GetComponent<IDamagable>();
                        if(idamagable != null)
                        {
                            idamagable.TakeDamage(1f);
                            hit_cols.Add(hit.collider);
                        }
                    }
                }
            }
            transform.forward = velocity.normalized;
        }

        transform.Translate(velocity * dt, Space.World);
    }
}
