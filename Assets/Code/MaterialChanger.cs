using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    public Renderer[] rends;
    public Material hurt_material;
    Material saved_material;

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

    void RevertMaterialToOriginal()
    {
        int len = rends.Length;
        for(int i = 0; i < len; i++)
        {
            rends[i].sharedMaterial = saved_material;
        }
    }
    
}
