using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inputs 
{
    public static float MASTER_VOLUME = 0.1F;
    public static float EFFECTS_VOLUME = 0.1F;
    public static float FIELD_OF_VIEW = 103;
    
    public static readonly GameInput ForwardKey = new GameInput("Forward", KeyCode.W);
    public static readonly GameInput BackwardsKey = new GameInput("Backwards", KeyCode.S);
    public static readonly GameInput LeftKey = new GameInput("Left", KeyCode.A);
    public static readonly GameInput RightKey = new GameInput("Right", KeyCode.D);

    public static readonly GameInput JumpKey = new GameInput("Jump", KeyCode.Space);
    public static readonly GameInput InteractKey = new GameInput("Interact", KeyCode.E);
    public static readonly GameInput AttackKey = new GameInput("Attack", KeyCode.Mouse0);
    public static readonly GameInput CrouchKey = new GameInput("Crouch", KeyCode.LeftControl);

    public static void ReadFov()
    {
        FIELD_OF_VIEW = PlayerPrefs.GetFloat("Fov", 103);
    }

    public static float MOUSE_SENS = 1;
   
    public static void ReadSens()
    {
        MOUSE_SENS = PlayerPrefs.GetFloat("Sens", 1) * 2;
    }
}

public class MovementInputWatcher : MonoSingleton<MovementInputWatcher> {
    private const float TweenSpeed = 2f;

    private float _horizontalTarget = 0f;
    private float _verticalTarget = 0f;
    private float _horizontal = 0f;
    private float _vertical = 0f;

    private Vector3 vector = Vector3.zero;

    private void Update() {
        _horizontalTarget = 0f;
        _verticalTarget = 0f;
        if (Input.GetKey(Inputs.RightKey.Key)) _horizontalTarget += 1f;
        if (Input.GetKey(Inputs.LeftKey.Key)) _horizontalTarget -= 1f;
                         
        if (Input.GetKey(Inputs.ForwardKey.Key)) _verticalTarget += 1f;
        if (Input.GetKey(Inputs.BackwardsKey.Key)) _verticalTarget -= 1f;
        
        /*_horizontal = Mathf.MoveTowards(_horizontal, _horizontalTarget, TweenSpeed * Time.deltaTime);
        _vertical = Mathf.MoveTowards(_vertical, _verticalTarget, TweenSpeed * Time.deltaTime);*/
        
        vector.x = _horizontalTarget;
        vector.z = _verticalTarget;
    }

    public Vector3 GetMovementInput {
       get 
        {
            return vector; 
        }
    }
}

public class GameInput {
    private string key;
    private KeyCode defaultKey;
    private KeyCode bindedKey;

    public string EntryKey { get { return key; } }

    public KeyCode DefaultKey
    {
        get
        {
            return defaultKey;
        }
    }

    public KeyCode Key
    {
        set
        {
            PlayerPrefs.SetInt($"KeyBinding_{EntryKey}", (int) Convert.ChangeType(value, value.GetTypeCode()));
            bindedKey = value;
        }
        get
        {
            return this.bindedKey;
        }
    }

    public GameInput(string key, KeyCode defaultKey) {
        this.key = key;
        this.defaultKey = defaultKey;
        this.bindedKey = (KeyCode) PlayerPrefs.GetInt($"KeyBinding_{key}", (int) Convert.ChangeType(defaultKey, defaultKey.GetTypeCode()));
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (key != null ? key.GetHashCode() : 0);
            hash = hash * 23 + defaultKey.GetHashCode();
            hash = hash * 23 + bindedKey.GetHashCode();
            return hash;
        }
    }

    public override bool Equals(object obj)
    {
        if (!(obj is GameInput))
            return false;

        GameInput other = (GameInput)obj;
        return key == other.key && defaultKey == other.defaultKey && bindedKey == other.bindedKey;
    }
}
