using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class LinaController : MonoBehaviour, IDamagable, ILaunchableAirbourne
{
    public EnemyState state = EnemyState.Chasing;
    Animator anim;
    NavMeshAgent agent;
    MaterialChanger materialChanger;

    public GameObject projectile_original;

    Collider col;

    public float Health = 10;

    public Transform hurtPoint;

    public LayerMask hurtMask;
    Rigidbody rb;

    void Awake()
    {
        anim            = GetComponent<Animator>();
        agent           = GetComponent<NavMeshAgent>();
        agent_speed_original = agent.speed;
        materialChanger = GetComponent<MaterialChanger>();
        col = GetComponent<Collider>();
        rb              = GetComponent<Rigidbody>();
        
    }

    void Start()
    {
        SetState(EnemyState.Spawning);
    }

    

    public GameObject revolver;
    float flee_timeStamp;

    void SetState(EnemyState _state)
    {
        if(state == EnemyState.Dead)
            return;

        switch(_state)
        {
            case(EnemyState.Spawning):
            {
                col.enabled = false;
                materialChanger.ChangeMaterialToSpawning();
                break;
            }
            case(EnemyState.Aiming):
            {
                attack_timer = attack_cooldown;
                agent.speed = 0;
                //anim.SetTrigger("Shoot");
                anim.Play("Base.Attack1", 0, 0);
                lina_audioSource.PlayOneShot(aim_clip);
                break;
            }
            case(EnemyState.Chasing):
            {
                agent.speed = agent_speed_original;
                break;
            }
            case(EnemyState.Fleeing):
            {
                Debug.Log("SetState Fleeing");
                for(int tries = 0; tries < 8; tries++)
                {
                    //Debug.Log("try " + tries);
                    Vector3 pos = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * Random.Range(6f, 16f);
                    NavMeshHit navMeshHit;
                    if(NavMesh.SamplePosition(pos, out navMeshHit, 4, NavMesh.AllAreas))
                    {
                        tries = 99;
                        agent.SetDestination(navMeshHit.position);
                    }
                }

                agent.speed = agent_speed_original * 1.5f;

                break;
            }
            case(EnemyState.Airbourne):
            {
                CancelInvoke();
                anim.Play("Base.Run", 0, 0);
                agent.enabled = false;
                rb.isKinematic = false;
                break;
            }
            case(EnemyState.Dead):
            {
                agent.enabled = false;
                col.enabled = false;
                anim.Play("Base.Die", 0, 0);
                materialChanger.ChangeMaterialToDead();
                SpawnedObject so = GetComponent<SpawnedObject>();
                revolver.transform.SetParent(null, true);
                Rigidbody revolver_rb = revolver.GetComponent<Rigidbody>();
                revolver_rb.isKinematic = false;
                revolver_rb.AddForce(new Vector3(Random.Range(-1f, 1f), 3, Random.Range(-1f, 1f)));
                revolver_rb.AddTorque(Random.onUnitSphere * 3);
                
                Destroy(revolver, 5);

                if(so)
                    so.OnObjectDied();
                Destroy(this.gameObject, 4);

                
                break;
            }
            default:
            {
                break;
            }
        }

        state = _state;
    }

    float attack_timer;
    public float attack_cooldown = 0.5f;
    float agent_speed_original;
    const float attack_distance = 144;

    float launchedAirbourneTimeStamp;

    void OnCollisionStay(Collision collisionInfo)
    {
        if(state == EnemyState.Airbourne && (Time.time - launchedAirbourneTimeStamp > 0.1f) && collisionInfo.collider.gameObject.isStatic)
        {
            rb.isKinematic = true;
            agent.enabled = true;
            SetState(EnemyState.Chasing);
        }
    }

    public void GetLaunched(Vector3 _vel)
    {
        SetState(EnemyState.Airbourne);
        rb.velocity = _vel;
        launchedAirbourneTimeStamp = Time.time;
    }

    public void OnSpawnEnd()
    {
        col.enabled = true;
        SetState(EnemyState.Chasing);
        materialChanger.RevertMaterialToOriginal();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        attack_timer -= dt;

        switch(state)
        {
            case(EnemyState.Spawning):
            {
                break;
            }
            case(EnemyState.Chasing):
            {
                anim.SetFloat("MoveSpeed", agent.velocity.magnitude);
                agent.SetDestination(PlayerController.GetPosition());
                float distance_to_player = Vector3.Distance(transform.position, PlayerController.GetPosition());

                if(attack_timer <= 0)
                {
                    if(distance_to_player < attack_distance)
                        SetState(EnemyState.Aiming);
                }

                if(distance_to_player < agent.stoppingDistance * 1.1f)
                    Math.RotateTowardsPositionXZ(transform, PlayerController.GetPosition(), dt * 4);
                break;
            }
            case(EnemyState.Fleeing):
            {
                anim.SetFloat("MoveSpeed", agent.velocity.magnitude);
                if((agent.remainingDistance < 1) || (Time.time - flee_timeStamp > 15))
                    SetState(EnemyState.Chasing);
                break;
            }
            case(EnemyState.Aiming):
            {
                anim.SetFloat("MoveSpeed", agent.velocity.magnitude);
                float distance_to_player = Vector3.Distance(transform.position, PlayerController.GetPosition());
                Math.RotateTowardsPositionXZ(transform, PlayerController.GetPosition(), dt * 8);

                break;
            }
            case(EnemyState.Airbourne):
            {
                transform.Rotate(new Vector3(0, 480 * dt, 0));
                break;
            }
            default:
            {
                break;
            }
        }
    }

    public Transform gunpoint;

    public void OnAttack1()
    {
        lina_audioSource.pitch = Random.Range(0.9f, 1.1f);
        lina_audioSource.PlayOneShot(shot_clip);
        
        GameObject g = Instantiate(projectile_original, gunpoint.position, Quaternion.identity) ;
        Projectile p = g.GetComponent<Projectile>();

        Vector3 _dir = (PlayerController.Instance.GetCenter() - gunpoint.position).normalized;
        Vector3 _vel = _dir * 36;

        p.Launch(gunpoint.position, _vel);
    }

    public void OnAttack1End()
    {
       //SetState(EnemyState.Chasing);
       SetState(EnemyState.Fleeing);
       anim.Play("Base.Run", 0, 0);
       Debug.Log("OnAttack1End");
    }
    

    public void TakeDamage(float damage)
    {
        materialChanger.ChangeMaterialForXTime(0.1f);
        AudioManager.PlayHurtAt(transform.position + new Vector3(0, 1.5f, 0), 1.05f);
        Health -= damage;
        if(Health <= 0)
        {
            Health = 0;
            SetState(EnemyState.Dead);
        }
        //Debug.Log(this.gameObject.name + " took " + damage + " damage");
    }

    [Header("Audio:")]
    public AudioSource lina_audioSource;
    public AudioClip aim_clip;
    public AudioClip shot_clip;

}
