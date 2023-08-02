using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashbang : MonoBehaviour
{
    public static void MakeAt(Vector3 pos, float time, float radius, float intensity, Color col, bool shadows)
    {
        GameObject g = new GameObject("Flashbang");
        g.transform.position = pos;
        if(radius == 0)
            radius = 0.3f;
        if(time == 0)
            time = 0.011f;
        Flashbang f = g.AddComponent<Flashbang>();
        f.radius_speed = radius / time;
        Light l = g.AddComponent<Light>();
        l.intensity = intensity;
        l.range = radius;
        l.color = col;
        if(!shadows)
            l.shadows = LightShadows.None;
        else
            l.shadows = LightShadows.Hard;

        f.enabled = true;

        Destroy(g, time+0.1f);
    }


    public float radius_speed = 12;
    new Light light;

    void Start()
    {
        light = GetComponent<Light>();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        // if(light.range <= 0)
        // {
        //     this.enabled = false;
        //     Destroy(this.gameObject, 1);
        // }

        light.range = Mathf.MoveTowards(light.range, 0, dt * radius_speed);
    }
}
