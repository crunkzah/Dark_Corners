using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
    private static T _instance = null;
    public static T Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<T>();
                if (_instance == null) {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
    
    protected virtual void Awake() {
        if (_instance == null || _instance == this) {
            _instance = this as T;
            Init();
        } else {
            Debug.Log("Destroyed Singleton " + name);
            Destroy(gameObject);
        }
    }
    
    protected virtual void OnDestroy() {
        if (_instance == this) {
            _instance = null;
        }
    }
    
    public virtual void Init() { }
    
    protected virtual void OnApplicationQuit() {
        _instance = null;
    }
}
