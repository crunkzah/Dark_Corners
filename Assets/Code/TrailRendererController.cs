using UnityEngine;

public class TrailRendererController : MonoBehaviour
{
    public TrailRenderer tr;
    
    float timer = 0;
    
    void Awake()
    {
        if(!tr)
            tr = GetComponent<TrailRenderer>();
        tr.emitting = false;
    }
    
    
    
    public void EmitFor(float time)
    {
        if(timer > 0)
        {
            return;
        }
        timer = time;
    }

    public void EmitStop()
    {
        timer = 0;
    }
    
    void Update()
    {
        float dt = Time.deltaTime;
        
        timer -= dt;
        
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
