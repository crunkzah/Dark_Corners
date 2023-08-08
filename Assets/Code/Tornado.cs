using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    public LayerMask hurt_mask;
    Vector3 velocity;

    public Vector3 start_scale = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 end_scale = new Vector3(1.5f, 1.5f, 1.5f);

    public void Launch(Vector3 pos, Vector3 _velocity)
    {
        transform.localScale = start_scale;
        transform.position = pos;
        velocity = _velocity;
    }

    float lifeTimer = 4;

    HashSet<Collider> cols_set = new HashSet<Collider>();
    static RaycastHit[] hurt_cols = new RaycastHit[16];

    void Update()
    {
        float dt = Time.deltaTime;
        lifeTimer -= dt;
        if(lifeTimer <= 0)
        {
            Destroy(this.gameObject);
        }

        Vector3 s = transform.localScale;
        transform.localScale = Vector3.MoveTowards(transform.localScale, end_scale, dt * 3);

        Vector3 _dir = velocity.normalized;

        int len = Physics.CapsuleCastNonAlloc(transform.position, transform.position + new Vector3(0, 6, 0), 0.5f, _dir, hurt_cols, dt, hurt_mask);

        for(int i = 0; i < len; i++)
        {
            if(!cols_set.Contains(hurt_cols[i].collider))
            {
                cols_set.Add(hurt_cols[i].collider);
                ILaunchableAirbourne ila = hurt_cols[i].collider.GetComponent<ILaunchableAirbourne>();
                if(ila != null)
                {
                    ila.GetLaunched(new Vector3(0, 20, 0));
                }
            }
        }

        
        transform.Translate(velocity * dt, Space.World);
    }

}
