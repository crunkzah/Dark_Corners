using UnityEngine;

public class TrailRendererController : MonoBehaviour
{
    public TrailRenderer tr;
    
    float timer = 0;

    public bool fade = false;
    public float fade_speed = 1;
    
    void Awake()
    {
        if(!tr)
            tr = GetComponent<TrailRenderer>();
        tr.emitting = false;
    }
    
    
    
    public void EmitFor(float time)
    {
        // if(timer > 0)
        // {
        //     return;
        // }
        timer = time;
        tr.startColor = Math.Modify_Alpha(tr.startColor, 1);
        tr.endColor = Math.Modify_Alpha(tr.endColor, 1);
    }

    public void EmitStop()
    {
        timer = 0;
    }
    
    void Update()
    {
        float dt = Time.deltaTime;
        
        timer -= dt;

        if(fade)
        {
            float a = tr.startColor.a;
            a = Mathf.MoveTowards(a, 0, dt * fade_speed);
            tr.startColor = Math.Modify_Alpha(tr.startColor, a);
            tr.endColor = Math.Modify_Alpha(tr.endColor, a);
        }
        
        if(timer <= 0)
        {
            timer = 0;
            tr.emitting = false;
        }
        else
        {
            tr.emitting = true;
        }
    }
    
}
