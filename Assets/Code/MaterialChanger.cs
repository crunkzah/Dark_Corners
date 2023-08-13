using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    public Renderer[] rends;
    public Material hurt_material;
    public Material dead_material;
    public Material spawning_material;

    Material saved_material;

    void Awake()
    {
        saved_material = rends[0].sharedMaterial;
    }

    public void ChangeMaterialForXTime(float x)
    {
        if(saved_material == null)
        {
            saved_material = rends[0].sharedMaterial;
        }

        int len = rends.Length;
        for(int i = 0; i < len; i++)
        {
            rends[i].sharedMaterial = hurt_material;
        }

        CancelInvoke();
        Invoke(nameof(RevertMaterialToOriginal), x);
    }

    public void ChangeMaterialToSpawning()
    {
        CancelInvoke();
        int len = rends.Length;
        for(int i = 0; i < len; i++)
        {
            rends[i].sharedMaterial = spawning_material;
        }
    }

    public void ChangeMaterialToDead()
    {
        CancelInvoke();
        int len = rends.Length;
        for(int i = 0; i < len; i++)
        {
            rends[i].sharedMaterial = dead_material;
        }
    }

    public void RevertMaterialToOriginal()
    {
        int len = rends.Length;
        for(int i = 0; i < len; i++)
        {
            rends[i].sharedMaterial = saved_material;
        }
    }
    
}
