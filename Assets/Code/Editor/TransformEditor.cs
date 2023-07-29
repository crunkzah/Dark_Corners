using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class TransformEditor
{
    [MenuItem("CONTEXT/Transform/Round scale")]
    static void Func()
    {

    }

    [MenuItem("CONTEXT/Transform/Round scale")]
    private static void RoundScale(MenuCommand cmd)
    {
        Transform t = (Transform)cmd.context;
        RoundScale_On_Transform(t);
        UnityEditor.EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent("Scale rounded"));

        EditorUtility.SetDirty(t.gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
    
    [MenuItem("CONTEXT/Transform/Round coordinates")]
    private static void RoundCoordinates(MenuCommand cmd)
    {
        Transform t = (Transform)cmd.context;
        RoundCoordinates_On_Transform(t);
        UnityEditor.EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent("Coordinates rounded"));
        EditorUtility.SetDirty(t.gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    static void RoundCoordinates_On_Transform(Transform t)
    {
        Vector3 pos = t.localPosition;

        pos.x = Mathf.Round(pos.x * 2) / 2;
        pos.y = Mathf.Round(pos.y * 2) / 2;
        pos.z = Mathf.Round(pos.z * 2) / 2;


        Undo.RecordObject(t, "Round coordinates");
        t.localPosition = pos;
    }

    static void RoundScale_On_Transform(Transform t)
    {
        Vector3 scale = t.localScale;

        scale.x = Mathf.Round(scale.x * 2) / 2;
        scale.y = Mathf.Round(scale.y * 2) / 2;
        scale.z = Mathf.Round(scale.z * 2) / 2;

        Undo.RecordObject(t, "Round scale");
        t.localScale = scale;

        UnityEditor.EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent("Scale rounded"));
    }

    [MenuItem("Tools/Round coordinates _4")]
    private static void RoundCoordinates_FromTools()
    {
        if(EditorApplication.isPlaying)
            return;

        foreach(GameObject obj in Selection.gameObjects)
        {
            RoundCoordinates_On_Transform(obj.transform);
            
            Texture tex = (Texture)Resources.Load("round_icon");
            UnityEditor.EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent("Coordinates rounded", tex));

            EditorUtility.SetDirty(obj);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    [MenuItem("Tools/Round scale _5")]
    private static void RoundScale_FromTools()
    {
        if(EditorApplication.isPlaying)
            return;
            
        foreach(GameObject obj in Selection.gameObjects)
        {
            RoundScale_On_Transform(obj.transform);

            Texture tex = (Texture)Resources.Load("round_icon2");
            UnityEditor.EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent("Scale rounded", tex));
            EditorUtility.SetDirty(obj);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    [MenuItem("Tools/Round rotation _7")]
    private static void RoundRotation_FromTools()
    {
        if(EditorApplication.isPlaying)
            return;
            
        foreach(GameObject obj in Selection.gameObjects)
        {
            RoundRotation_On_Transform(obj.transform);

            Texture tex = (Texture)Resources.Load("round_icon3");
            UnityEditor.EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent("Rotation rounded", tex));
            EditorUtility.SetDirty(obj);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    static void RoundRotation_On_Transform(Transform t)
    {
        Vector3 r = t.eulerAngles;

        r.x = (float)(int)(r.x);
        r.y = (float)(int)(r.y);
        r.z = (float)(int)(r.z);

        Undo.RecordObject(t, "Round rotation");
        t.eulerAngles = r;

        UnityEditor.EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent("Scale rounded"));
    }
}
