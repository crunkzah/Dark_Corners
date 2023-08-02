using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RampGeneratorV2 : MonoBehaviour
{

    public Gradient procedrualGradientRamp;
    public bool procedrualGradientEnabled = false;
    public bool updateEveryFrame = false;
    public Renderer[] renderers;

    private Texture2D rampTexture;
    private Texture2D tempTexture;
    private float width = 256;
    private float height = 64;

    void Start()
    {
        if (procedrualGradientEnabled == true)
        {
            UpdateRampTexture();
        }
    }

    [ExecuteInEditMode]
    void Update()
    {
        if (procedrualGradientEnabled == true)
        {
            if (updateEveryFrame == true)
            {
                UpdateRampTexture();
            }
        }
    }

    

    // Generating a texture from gradient variable
    Texture2D GenerateTextureFromGradient(Gradient grad)
    {
        if (tempTexture == null)
        {
            tempTexture = new Texture2D((int)width, (int)height);
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color col = grad.Evaluate(0 + (x / width));
                tempTexture.SetPixel(x, y, col);
            }
        }
        tempTexture.wrapMode = TextureWrapMode.Clamp;
        tempTexture.Apply();
        return tempTexture;
    }

    // Update procedural ramp textures and applying them to the shaders
    public void UpdateRampTexture()
    {
        rampTexture = GenerateTextureFromGradient(procedrualGradientRamp);
        int len = renderers.Length;
        for(int i = 0; i < len; i++)
        {
            int len2 = renderers[i].sharedMaterials.Length;
            for(int j = 0; j < len2; j++)
            {
                if(renderers[i].sharedMaterials[j].HasProperty("_Ramp"))
                    renderers[i].sharedMaterials[j].SetTexture("_Ramp", rampTexture);
            }
        }

#if UNITY_EDITOR
        //if(Input.GetKey(KeyCode.R))
        {
            byte[] bytes = rampTexture.EncodeToPNG();
            var dirPath = Application.dataPath + "/SineVFX/MatCapPro/Resources/Textures/RampsGenerated/";
            // if(!Directory.Exists(dirPath)) 
            // {
            //     Directory.CreateDirectory(dirPath);
            // }

            string ramp_name = "GeneratedReaver_Ramp_" + Random.Range(10, 9999).ToString();
            File.WriteAllBytes(dirPath + ramp_name + ".png", bytes);
            Debug.Log(string.Format("<color=yellow>Ramp <color=green>{0}</color> generated!</color>", ramp_name));
        }
        
#endif
        // foreach (Renderer rend in renderers)
        // {
        //     foreach (Material mat in rend.materials)
        //     {
        //         mat.SetTexture("_Ramp", rampTexture);
        //     }
        // }
        
    }
}
