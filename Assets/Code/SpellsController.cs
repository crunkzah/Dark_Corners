using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpellsController : MonoSingleton<SpellsController>
{
    public Animator hand_right;
    public Animator hand_left;

    public Transform hand_right_gunPoint;
    public Transform hand_left_gunPoint;
    public Transform middle_gunPoint;

    public GameObject projectile_original;
    public GameObject orb_original;
    [HideInInspector] public Orb activeOrb;
    public GameObject tornado_original;


    float spell_timer;


    float gun_timer = 0;
    const float gun_cooldown = 0.2f;

    void ShootProjectile(Vector3 pos, Vector3 vel)
    {
        // gunAudioSrc.pitch = Random.Range(0.9f, 1.1f);
        // gunAudioSrc.PlayOneShot(shoot_clip, 1);

        if(f % 2 == 0)
        {
            gunAudioSrc_L.pitch = Random.Range(0.9f, 1.1f);
            gunAudioSrc_L.PlayOneShot(shoot_clip, 1);
        }
        else
        {
            gunAudioSrc_R.pitch = Random.Range(0.9f, 1.1f);
            gunAudioSrc_R.PlayOneShot(shoot_clip, 1);
        }

        GameObject g = Instantiate(projectile_original, pos, Quaternion.LookRotation(vel.normalized, new Vector3(0, 1, 0)));
        Projectile p = g.GetComponent<Projectile>();
        p.Launch(pos, vel);
    }

    void ShootProjectileDelayed()
    {
        Ray ray = CameraController.Instance.worldCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        PlayerController.Instance.BoostVelocityAdditive(-ray.direction * 1);
        Transform _gunPoint = (f % 2 == 0 ? hand_right_gunPoint : hand_left_gunPoint);
        Vector3 proj_dir = (ray.origin + ray.direction * 1000 - _gunPoint.position).normalized;
        const float proj_speed = 72;

        ShootProjectile(_gunPoint.position, proj_dir * proj_speed);
    }

    public TrailRendererController[] hand_trails;
    public GameObject laser_original;

    void ShootLaser()
    {
        Ray ray = CameraController.Instance.worldCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if(PlayerController.Instance.IsGrounded())
        {
            PlayerController.Instance.MakeGravityZeroForXTime(0.033f);
            PlayerController.Instance.BoostVelocity(-ray.direction * 12);
        }
        else
            PlayerController.Instance.BoostVelocity(-ray.direction * 5.5f);
        CameraController.Instance.AddFov(4);
        gunAudioSrc.PlayOneShot(laser_clip);

        Vector3 laser_end_point = ray.GetPoint(200);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 200, hurtMask))
        {
            IDamagable idamagable = hit.collider.GetComponent<IDamagable>();
            if(idamagable != null)
                idamagable.TakeDamage(10);
            laser_end_point = hit.point + hit.normal * 0.01f;
        }

            

        GameObject g = Instantiate(laser_original, middle_gunPoint.position, Quaternion.identity);
        Laser laser = g.GetComponent<Laser>();
        laser.Make(middle_gunPoint.position - 0.25f * CameraController.Instance.transform.up, laser_end_point);
        Flashbang.MakeAt(laser_end_point, 0.45f, 8, 10, new Color(45f/255f, 201f/255f, 1f), false);
    }

    int f;
    void Update()
    {
        if(UberManager.Instance.state == GameState.Paused)
            return;

        float dt = Time.deltaTime;

        if(spell_timer > 0)
        {
            spell_timer -= dt;
            if(spell_timer <= 0)
                spell_timer = 0;
        }

        if(gun_timer > 0)
        {
            gun_timer -= dt;
            if(gun_timer <= 0)
                gun_timer = 0;
        }

        if(Input.GetKey(Inputs.AttackAltKey.Key))
        {
            if(gun_timer == 0)
            {
                gun_timer = gun_cooldown * 3.5f;
                hand_right.Play("Base.Cast_3", 0, 0);
                hand_left.Play("Base.Cast_3", 0, 0);
                for(int i = 0; i < hand_trails.Length; i++)
                    hand_trails[i].EmitFor(0.5f);

                Invoke(nameof(ShootLaser), 0.33f);
            }
        }

        if(gun_timer == 0)
        {
            if(Input.GetKey(Inputs.AttackKey.Key))
            {
                if(Input.GetKey(Inputs.CrouchKey.Key))
                {
                    gun_timer = gun_cooldown * 3.5f;
                    hand_right.Play("Base.Cast_3", 0, 0);
                    hand_left.Play("Base.Cast_3", 0, 0);
                    for(int i = 0; i < hand_trails.Length; i++)
                        hand_trails[i].EmitFor(0.5f);

                    Invoke(nameof(ShootLaser), 0.33f);
                }
                else
                {
                    gun_timer = gun_cooldown;
                    Animator _anim = (f % 2 == 0 ? hand_right : hand_left);

                    string _anim_name = "Base.Cast_1";
                    // for(int i = 0; i < hand_trails.Length; i++)
                    //     hand_trails[i].EmitFor(0.5f);
                    _anim.Play(_anim_name, 0, 0);

                    Invoke(nameof(ShootProjectileDelayed), 0.1f);
                    f++;
                }
            }
        }

        if(Input.GetKeyDown(Inputs.Spell_1_Key.Key))
        {
            if(!activeOrb)
            {
                if(spell_timer == 0)
                {
                    f++;
                    hand_right.Play("Base.Cast_1_slow", 0, 0);
                    hand_left.Play("Base.Cast_1_slow", 0, 0);
                    spell_timer = 1;
                    activeOrb = Instantiate(orb_original, Vector3.one * 1000, Quaternion.identity).GetComponent<Orb>();
                    Ray ray = CameraController.Instance.worldCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                    activeOrb.Launch(ray.origin + ray.direction * 0.2f + new Vector3(0, -0.75f, 0), ray.direction * 12);
                }
            }
            else
            {
                if(activeOrb != null)
                {
                //PlayerController.Instance.TeleportPlayer(activeOrb.GetTeleportPosition(), activeOrb.transform.eulerAngles.y, PlayerController.Instance.rotationX);
                    PlayerController.Instance.DoDash(activeOrb.GetTeleportPosition(), activeOrb.transform.forward);
                    activeOrb.TeleportToOrb();
                    activeOrb = null;
                }
            }
        }

        if(Input.GetKeyDown(Inputs.Spell_2_Key.Key))
        {
            if(spell_timer == 0)
            {
                f++;
                hand_right.Play("Base.Cast_1_slow", 0, 0);
                hand_left.Play("Base.Cast_1_slow", 0, 0);

                spell_timer = 1;
                Tornado _tornado = Instantiate(tornado_original, Vector3.one * 1000, Quaternion.identity).GetComponent<Tornado>();

                Ray ray = new Ray(transform.position, transform.forward);

                _tornado.Launch(ray.origin, ray.direction * 42);
            }
        }

        if(Input.GetKeyDown(Inputs.Spell_3_Key.Key))
        {
            if(spell_timer == 0)
            {
                f++;
                hand_right.Play("Base.Cast_2", 0, 0);
                hand_left.Play("Base.Cast_2", 0, 0);
                for(int i = 0; i < hand_trails.Length; i++)
                    hand_trails[i].EmitFor(2f);

                spell_timer = gun_timer = 1.44f;
                gunAudioSrc.PlayOneShot(laguna_clip);
                Invoke(nameof(CastLagunaBlade), 0.66f);
                PlayerController.Instance.MakeSlowForXTime(0.55f);
            }
        }
    }

    public void CancelAllInvokes()
    {
        CancelInvoke();
    }

    public GameObject LagunaBlade_original;
    public LayerMask hurtMask;

    void CastLagunaBlade()
    {
        CameraController.Instance.AddFov(4.5f);
        GameObject g = Instantiate(LagunaBlade_original, middle_gunPoint.position, Quaternion.identity);
        LagunaBlade lagunaBlade = g.GetComponent<LagunaBlade>();

        Ray ray = CameraController.Instance.worldCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 72, hurtMask))
        {
            
            lagunaBlade.Make(middle_gunPoint, hit.point);
        }
        else
            lagunaBlade.Make(middle_gunPoint, ray.GetPoint(16));

        PlayerController.Instance.MakeSlowForXTime(0.033f);
        
        PlayerController.Instance.MakeGravityZeroForXTime(0.033f);
        PlayerController.Instance.BoostVelocity(-ray.direction * 18);

        Debug.Log("CastLagunaBlade()");
    }


    [Header("Audio:")]
    public AudioSource gunAudioSrc;
    public AudioSource gunAudioSrc_L;
    public AudioSource gunAudioSrc_R;
    public AudioClip shoot_clip;
    public AudioClip laguna_clip;
    public AudioClip laser_clip;
}

