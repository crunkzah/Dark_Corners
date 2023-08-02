using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RampGeneratorV2))]
public class RampGeneratorV2_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate ramp"))
        {
            RampGeneratorV2 t = (RampGeneratorV2)target;
            t.UpdateRampTexture();
        }
    }
}
