using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageOverlay : MonoSingleton<DamageOverlay> 
{
    private Image _image;
    public float maxAlpha = 0.5f;
    public float decaySpeed = 1.5f;

    void Start() 
    {
        _image = GetComponent<Image>();
    }

    void Update() 
    {
        Color c = _image.color;
        float percentage = 1;
        float alpha_target = 0; 
        if(percentage < 0.25f)
            alpha_target = 0.5f;
        c.a = Mathf.MoveTowards(c.a, alpha_target, decaySpeed * Time.deltaTime);
        _image.color = c;
    }

    public void ShowOverlay(float intensity) 
    {
        intensity = Mathf.Clamp01(intensity);
        Color c = _image.color;
        c.a += maxAlpha * intensity;
        _image.color = c;
    }
}
