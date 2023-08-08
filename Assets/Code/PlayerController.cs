using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerState
{
    Spawning,
    Alive,
    Dead
}

public class PlayerController : MonoSingleton<PlayerController>, IDamagable
{
    [HideInInspector]public CharacterController controller;
    

    public PlayerState state = PlayerState.Alive;
    public CameraController cam;
    [Header("Stats:")]
    public float moveSpeed = 12;
    public float acceleration = 64;
    //public float floorFriction = 32;
    public float jumpForce = 7.5f;

    public Vector3 velocity;

    public override void Init()
    {
        Inputs.ReadSens();
        PLAYER_LAYER = LayerMask.NameToLayer("Player");
        NPC_LAYER = LayerMask.NameToLayer("NPC");
        
        rotationY = transform.eulerAngles.y;
        controller = GetComponent<CharacterController>();

        this.enabled = false;
        Invoke(nameof(EnablePlayer), 0.1f);
    }

    void EnablePlayer()
    {
        this.enabled = true;
    }

    const float GRAVITY_Y = -15;
    float GetGravity()
    {
        if(gravity_zero_timer > 0)
            return 0;
        return GRAVITY_Y;
    }

    float gravity_zero_timer;
    void MakeGravityZeroForXTime(float x)
    {
        gravity_zero_timer = x;
    }

    float slow_timer;
    public void MakeSlowForXTime(float x)
    {
        slow_timer = x;
    }
    const float VELOCITY_Y_GROUNDED = -15;
    float rotationY = 0;
    [HideInInspector]public float rotationX = 0;
 

    public Vector3 GetClosestPointOnCollider(Vector3 point)
    {
        return controller.ClosestPointOnBounds(point);
    }    

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
    }

    

    void OnJump()
    {
        
    }

    public static Vector3 GetPosition()
    {
        return Instance.transform.position;
    }


    float airbourne_timestamp;
    const float JUMP_COYOTE_TIMEWINDOW = 0.125f;

    void OnBecomeAirbourne()
    {
        secondJumpHappened = false;
        airbourne_timestamp = Time.time;
        if(velocity.y < 0)
        {
            velocity.y = 0;
        }
        //Debug.Log("OnBecomeAirbourne()");
    }

    void OnBecomeGrounded()
    {
        if(Time.time - airbourne_timestamp > 0.2f)
        {
            //player_audio.PlayOneShot(landing_clip);
        }
    }

    public void BoostVelocity(Vector3 _vel)
    {
        velocity = _vel;
    }

    bool wasGroundedBefore = false;

    Quaternion dump_q;

    public Vector3 externalVelocity;


    static int PLAYER_LAYER = -1;
    static int NPC_LAYER = -1;

    const float externalVelocityDeceleration = 12;

    float dot_xz_vel_desiredVel;


    Vector3 holding_move_input;
    public bool IsHoldingMoveInput()
    {
        if(holding_move_input.sqrMagnitude == 0)
            return false;

        return true;
    }

    bool secondJumpHappened = false;
    void Jump()
    {
        if(controller.height < 1.9f)
            return;
        if(controller.isGrounded)
        {
            velocity.y = jumpForce;
            OnJump();
        }
        else
        {
            if(Time.time - airbourne_timestamp < JUMP_COYOTE_TIMEWINDOW)
            {
                velocity.y = jumpForce;
                OnJump();
            }
            else
            {
                if(!secondJumpHappened)
                {
                    velocity.y = jumpForce * 1.25f;    
                    secondJumpHappened = true;
                }
            }
        }
    }

    float controller_target_height = 2;

    public Light playerLight;

    bool isDashing = false;
    Vector3 dash_position;
    Vector3 dash_position_direction;
    public const float DASH_SPEED = 144 * 3;
    
    Vector3 velocity_before_dash;

    public void DoDash(Vector3 _dash_position, Vector3 _dash_position_direction)
    {
        dash_position_direction = _dash_position_direction;
        velocity_before_dash = velocity;
        dash_position = _dash_position;
        isDashing = true;
    }

    void UpdateAlive(float dt)
    {
        gravity_zero_timer -= dt;
        if(gravity_zero_timer <= 0)
            gravity_zero_timer = 0;

        slow_timer -= dt;
        if(slow_timer <= 0)
            slow_timer = 0;

        float targetIntensity = 0;
        if((Math.Get_XZ(velocity) + Math.Get_XZ(externalVelocity)).sqrMagnitude > 0)
            targetIntensity = 3;
        playerLight.intensity = Mathf.MoveTowards(playerLight.intensity, targetIntensity, 16 * dt);

        HUDManager.HideLabelText();
        RaycastHit interactionHit;        
        Ray interactionRay = CameraController.Instance.fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if(Physics.Raycast(interactionRay, out interactionHit, 2.5f, UberManager.GetInteractionMask()))
        {
            Interactable interactable = interactionHit.collider.gameObject.GetComponent<Interactable>();
            if(interactable != null)
            {
                if(Input.GetKeyDown(Inputs.InteractKey.Key))
                    interactable.Interact();
            }
            ObjectInfo objectInfo = interactionHit.collider.gameObject.GetComponent<ObjectInfo>();
            if(objectInfo)
                HUDManager.SetLabelText(objectInfo.Text);
        }

        

        //HUDManager.SetLabelText()


        Vector3 input = MovementInputWatcher.Instance.GetMovementInput;
        holding_move_input = input;
        float d_rotationY = Input.GetAxisRaw("Mouse X") * Inputs.MOUSE_SENS;
        float d_rotationX = Input.GetAxisRaw("Mouse Y") * Inputs.MOUSE_SENS;
        // if(Cursor.visible)
        //     d_rotationX = d_rotationY = 0;
        rotationY += d_rotationY;
        rotationX += d_rotationX;
        //Debug.Log("RotationXY: " + rotationX + ", " + rotationY);
        //Debug.Log("Kek");

        rotationX = Mathf.Clamp(rotationX, -89.9F, 89.9F);
        transform.localEulerAngles = Math.Modify_Y(transform.localEulerAngles, rotationY);
        cam.transform.localEulerAngles = Math.Modify_X(cam.transform.localEulerAngles, -rotationX);

        Vector3 localInputXZ = new Vector3(0, 0, 0);
        if(input.sqrMagnitude > 0)
        {
            localInputXZ = transform.TransformDirection(input).normalized;
        }

        float desiredMoveSpeed      = moveSpeed;
        if(slow_timer > 0)
        {
            desiredMoveSpeed *= 0.5f;
        }
        Vector3 desiredVelocityXZ   = localInputXZ * desiredMoveSpeed;
        desiredVelocityXZ = localInputXZ * desiredMoveSpeed;

        Vector3 velocityXZ          = Math.Get_XZ(velocity);
        float _acceleration         = acceleration;

        if(controller.isGrounded && Input.GetKey(Inputs.CrouchKey.Key))
        {
            controller_target_height = 0.5f;
            desiredVelocityXZ *= 0.5f;
        }
        else
        {
            controller_target_height = 2;
        }

        Ray crouchRay = new Ray(transform.position + new Vector3(0, controller.height, 0), new Vector3(0, 1, 0));
        RaycastHit crouchHit;
        if(Physics.SphereCast(crouchRay, controller.radius, out crouchHit, 14 * dt, UberManager.GetPlayerCollisionMask()))
            controller_target_height = controller.height;

        controller.height = Mathf.MoveTowards(controller.height, controller_target_height, 14 * dt);
        controller.center = new Vector3(0, controller.height / 2, 0);
        CameraController.Instance.cam_height = controller.height;

        if(controller.isGrounded)
        {
            velocity.y = VELOCITY_Y_GROUNDED;

            externalVelocity = Vector3.MoveTowards(externalVelocity, new Vector3(0, 0, 0), externalVelocityDeceleration * 4 * dt);
        }
        else
        {
            externalVelocity = Vector3.MoveTowards(externalVelocity, new Vector3(0, 0, 0), externalVelocityDeceleration * dt);

            if(localInputXZ.x == 0 && localInputXZ.z == 0)
                _acceleration = acceleration * 0.1f;
            else
                _acceleration = acceleration * 0.66f;
            if(velocity.y < 0)
                velocity.y += GetGravity()  * dt; //Apply gravity second time if we are falling down
        }
        if(Input.GetKeyDown(Inputs.JumpKey.Key))
            Jump();

        if(!controller.isGrounded && input.x == 0 && input.z == 0)
        {
            const float airFriction = 3;
            desiredVelocityXZ = velocityXZ;
            desiredVelocityXZ = Vector3.MoveTowards(desiredVelocityXZ, new Vector3(0, 0, 0), dt * airFriction);
        }
        velocityXZ = Vector3.MoveTowards(velocityXZ, desiredVelocityXZ, dt * _acceleration);

        velocity.x = velocityXZ.x;
        velocity.z = velocityXZ.z;
        velocity.y += GetGravity()  * dt;
        

        if(wasGroundedBefore && !controller.isGrounded)
        {
            OnBecomeAirbourne();
        }
        else if(!wasGroundedBefore && controller.isGrounded)
        {
            OnBecomeGrounded();
        }

        wasGroundedBefore = controller.isGrounded;
        Vector3 motion = externalVelocity + velocity;

        if(isDashing)
        {
            velocity = new Vector3(0, 0, 0);
            transform.position = Vector3.MoveTowards(transform.position, dash_position, dt * DASH_SPEED);
            UberManager.Instance.timeScale = 0.1f;
            if(Vector3.Distance(transform.position, dash_position) < 0.02f)
            {
                isDashing = false;
                MakeGravityZeroForXTime(0.1f);
                velocity = dash_position_direction * Math.Get_XZ(velocity_before_dash).magnitude + new Vector3(0, 1, 0); 
            }
            //controller.Move(motion * dt);
        }
        else
        {
            UberManager.Instance.timeScale = 1f;
            controller.Move(motion * dt);
            HeadBobbing.Instance.UpdateMe(transform.InverseTransformDirection(velocity));
        }
    }

    
    float spawning_timer;

    void Update()
    {
        if(UberManager.GetState() == GameState.Paused)
            return;

        float dt = Time.deltaTime;

        // controller.height = Mathf.MoveTowards(controller.height, 2, 2 * dt);
        // controller.center = new Vector3(0, controller.height * 0.5f + 0.075f, 0);

        switch(state)
        {
            case(PlayerState.Spawning):
            {
                break;
            }
            case(PlayerState.Alive):
            {
                UpdateAlive(dt);
                break;
            }
            case(PlayerState.Dead):
            {
                break;
            }
        }
    }

    void SetState(PlayerState _state)
    {
        if(state == PlayerState.Dead)
            return;

        switch(_state)
        {
            case(PlayerState.Spawning):
            {
                break;
            }
            case(PlayerState.Alive):
            {
                break;
            }
            case(PlayerState.Dead):
            {
                SpellsController.Instance.CancelAllInvokes();
                SpellsController.Instance.enabled = false;
                CameraController.Instance.fpsCam.enabled = false;
                CameraController.Instance.OnDie();
                break;
            }
        }

        state = _state;
    }


    public bool IsGrounded()
    {
        return controller.isGrounded;
    }

    public Vector3 GetCenter()
    {
        return transform.position + new Vector3(0, 1, 0) * controller.height * 0.5f;
    }

    public Vector3 GetHead()
    {
        return GetHeadPosition();
    }

    public Transform GetHeadTransform()
    {
        return cam.transform;
    }

    public Vector3 GetGroundPosition()
    {
        return transform.localPosition;
    }

    public Vector3 GetHeadPosition()
    {
        return cam.transform.position;
    }


    void Die()
    {
    }

    public void TeleportPlayer(Vector3 pos, float angle_y, float angle_x)
    {
        //Debug.Log("TeleportPlayer at " + pos);
        rotationX = angle_x;
        rotationY = angle_y;
        controller.enabled = false;
        transform.position = pos;
        controller.enabled = true;
    }

    void OnGUI()
    {
        // if(state != PlayerState.Alive)
        //     return;

        // int size = 72;
        // float posX = cam.cam.pixelWidth / 2 - size / 4;
        // float posY = cam.cam.pixelHeight / 2 - size / 2;
        // GUI.Label(new Rect(posX, posY, size, size), "*");

        // GUIStyle style = new GUIStyle();
        // style.alignment = TextAnchor.MiddleCenter;
        
        // float w = 350;
        // float h = 200;
        
        // style.normal.textColor = Color.green;
        // GUI.Label(new Rect(Screen.width/2 - w/2, Screen.height - h*2.25f, w, h),  "dot: " + dot_xz_vel_desiredVel.ToString());
    }

    [HideInInspector] public float Health = 50;

    public void TakeDamage(float damage)
    {
        if(UberManager.GodMode)
        {
            return;
        }

        Health -= damage;
        if(Health <= 0)
        {
            Health = 0;
            SetState(PlayerState.Dead);
        }
        else
            DamageOverlay.Instance.ShowOverlay(0.33f);
        
        
        
        
        Debug.Log("Player took " + damage + " damage");
    }

    public AudioSource player_audio;
    public AudioSource hurt_audio;

    public AudioClip jump_clip;
    public AudioClip landing_clip;
    public AudioClip hurt_clip;
    public AudioClip death_clip;
}
