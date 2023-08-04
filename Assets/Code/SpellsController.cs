using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpellsController : MonoSingleton<SpellsController>
{
    public Animator hand_right;
    public Animator hand_left;

    public Transform hand_right_gunPoint;
    public Transform hand_left_gunPoint;

    public GameObject projectile_original;
    public GameObject orb_original;
    [HideInInspector] public Orb activeOrb;
    public GameObject tornado_original;


    float spell_timer;


    float gun_timer = 0;
    const float gun_cooldown = 0.2f;

    void ShootProjectile(Vector3 pos, Vector3 vel)
    {
        GameObject g = Instantiate(projectile_original, pos, Quaternion.LookRotation(vel.normalized, new Vector3(0, 1, 0)));
        Projectile p = g.GetComponent<Projectile>();
        p.Launch(pos, vel);
    }

    int f;
    void Update()
    {
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

        if(gun_timer == 0)
        {
            if(Input.GetKey(Inputs.AttackKey.Key))
            {
                gun_timer = gun_cooldown;
                Animator _anim = (f % 2 == 0 ? hand_right : hand_left);
                Transform _gunPoint = (f % 2 == 0 ? hand_right_gunPoint : hand_left_gunPoint);

                //string _anim_name = (f % 4 == 0 ? "Base.Cast_1" : "Base.Cast_2");
                string _anim_name = "Base.Cast_1";
                _anim.Play(_anim_name, 0, 0);

                Ray ray = CameraController.Instance.worldCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                Vector3 proj_dir = (ray.origin + ray.direction * 1000 - _gunPoint.position).normalized;
                const float proj_speed = 72;

                ShootProjectile(_gunPoint.position, proj_dir * proj_speed);
                //Debug.Log("KEK: " + _gunPoint.gameObject.name);
                f++;
            }
        }

        if(Input.GetKeyDown(Inputs.Spell_1_Key.Key))
        {
            f++;
            hand_right.Play("Base.Cast_1_slow", 0, 0);
            hand_left.Play("Base.Cast_1_slow", 0, 0);
            //Debug.Log("Mek");
            
            if(!activeOrb)
            {
                if(spell_timer == 0)
                {
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

                Ray ray = new Ray();
                ray.direction = transform.forward;
                ray.origin = transform.position;

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

                spell_timer = 1;
                // Tornado _tornado = Instantiate(tornado_original, Vector3.one * 1000, Quaternion.identity).GetComponent<Tornado>();

                // Ray ray = new Ray();
                // ray.direction = transform.forward;
                // ray.origin = transform.position;

                // _tornado.Launch(ray.origin, ray.direction * 42);
            }
        }
    }    
}
