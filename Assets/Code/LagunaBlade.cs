using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LagunaBlade : MonoBehaviour
{
    public LayerMask hurt_mask;

    LineRenderer lr;
    const int POINTS_NUM = 16;
    Vector3[] positions = new Vector3[POINTS_NUM];

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = POINTS_NUM;
    }
    float lifeTimer = 1;
    public float scale = 0.75f;

    float updateLRtimer = 0;
    float updateLRFreq = 0.033f;

    void Update()
    {
        if(lifeTimer <= 0)
        {
            Destroy(this.gameObject);
            this.enabled = false;
        }
        else
        {
            lifeTimer -= Time.deltaTime;
        }

        scale = Mathf.MoveTowards(scale, 0.05f, Time.deltaTime * 1.0f);
        lr.startWidth = Mathf.MoveTowards(lr.startWidth, 0.0f, Time.deltaTime * 1.0f);
        lr.endWidth = Mathf.MoveTowards(lr.endWidth, 0.0f, Time.deltaTime * 1.0f);

        //positions[0] = start_tr.position;
    }

    void FixedUpdate()
    {
        positions[0] = start;
        positions[POINTS_NUM-1] = end;
        Vector3 a = positions[0];
        Vector3 b= positions[POINTS_NUM-1];
        float step = 1f / (POINTS_NUM-2);
        float t = 0;
        for(int i = 1; i <= POINTS_NUM-2; i++)
        {
            t += step;
            positions[i] = Vector3.Lerp(a, b, t) + Random.onUnitSphere * Random.Range(scale * 0.5f, scale);
        }
        lr.SetPositions(positions);
    }

    Transform start_tr;
    Vector3 start;
    Vector3 end;

    

    public void Make(Transform _start_tr, Vector3 _end)
    {
        start_tr = _start_tr;
        start = start_tr.position;
        end = _end;

        positions[0] = start_tr.position;
        positions[POINTS_NUM-1] = end;
        Vector3 a = positions[0];
        Vector3 b= positions[POINTS_NUM-1];
        float step = 1f / (POINTS_NUM-2);
        float t = 0;
        for(int i = 1; i <= POINTS_NUM-2; i++)
        {
            t += step;
            positions[i] = Vector3.Lerp(a, b, t) + Random.onUnitSphere * Random.Range(0.3f, 0.75f);
        }
        lr.SetPositions(positions);

        Flashbang.MakeAt(Vector3.Lerp(a, b, 0.975f), 0.45f, 16, 10, new Color(45f/255f, 201f/255f, 1f), false);

        int enemies_hit_cnt = Physics.OverlapSphereNonAlloc(b, 2f, hurt_cols, hurt_mask);
        for(int i = 0; i < enemies_hit_cnt; i++)
        {
            IDamagable idamagable = hurt_cols[i].GetComponent<IDamagable>();
            if(idamagable != null)
            {
                idamagable.TakeDamage(10);
            }
        }
    }

    static Collider[] hurt_cols = new Collider[64];
}
