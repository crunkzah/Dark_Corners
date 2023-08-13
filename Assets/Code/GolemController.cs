using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class GolemController : MonoBehaviour, IDamagable, ILaunchableAirbourne
{
    public EnemyState state = EnemyState.Chasing;
    Animator anim;
    NavMeshAgent agent;
    MaterialChanger materialChanger;

    public Transform hurtPoint;

    public LayerMask hurtMask;

    public ParticleSystem[] eyes_ps;

    Collider col;
    Rigidbody rb;

    void Awake()
    {
        anim            = GetComponent<Animator>();
        agent           = GetComponent<NavMeshAgent>();
        agent_speed_original = agent.speed;
        materialChanger = GetComponent<MaterialChanger>();
        col             = GetComponent<Collider>();
        rb              = GetComponent<Rigidbody>();
    }

    void Start()
    {
        SetState(EnemyState.Spawning);
    }

    public void GetLaunched(Vector3 _vel)
    {
        SetState(EnemyState.Airbourne);
        rb.velocity = _vel;
        launchedAirbourneTimeStamp = Time.time;
    }

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
            case(EnemyState.Dead):
            {
                materialChanger.ChangeMaterialToDead();
                col.enabled = false;
                agent.enabled = false;
                anim.Play("Base.Die", 0, 0);
                anim.SetLayerWeight(1, 0);
                golem_audioSource.PlayOneShot(die_clip);
                SpawnedObject so = GetComponent<SpawnedObject>();
                if(so)
                    so.OnObjectDied();
                Destroy(this.gameObject, 5);
                break;
            }
            case(EnemyState.Airbourne):
            {
                anim.Play("Base.Run", 0, 0);
                agent.enabled = false;
                rb.isKinematic = false;
                CancelInvoke();
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
    float agent_speed_original;

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

                if(distance_to_player < agent.stoppingDistance * 2f)
                {
                    Math.RotateTowardsPositionXZ(transform, PlayerController.GetPosition(), dt * 180);

                    if(attack_timer <= 0)
                    {
                        for(int i = 0; i < eyes_ps.Length; i++)
                            eyes_ps[i].Play();
                        attack_timer = 2;
                        agent.speed = agent_speed_original * 3.5f;
                        anim.Play("UpperBody.Attack1", 1, 0);
                    }
                }
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

    public void OnSpawnEnd()
    {
        col.enabled = true;
        SetState(EnemyState.Chasing);
        materialChanger.RevertMaterialToOriginal();
    }

    public void OnAttack1()
    {
        agent.speed = agent_speed_original;
        Debug.Log("OnAttack1");
        golem_audioSource.pitch = Random.Range(0.9f, 1.1f);
        golem_audioSource.PlayOneShot(impact_clip);
        StartCoroutine(DoDamage(0.05f, 1f));
    }

    static Collider[] cols_buffer = new Collider[64];
    HashSet<Collider> cols_were_hurt = new HashSet<Collider>();

    IEnumerator DoDamage(float duration, float radius)
    {
        cols_were_hurt.Clear();

        for(float timer = 0; timer <= duration; timer += Time.deltaTime)
        {
            int len = Physics.OverlapSphereNonAlloc(hurtPoint.position, 1.2f, cols_buffer, hurtMask);
            for(int i = 0; i < len; i++)
            {
                if(cols_were_hurt.Contains(cols_buffer[i]))
                    continue;

                PlayerController player = cols_buffer[i].GetComponent<PlayerController>();
                if(player)
                {
                    cols_were_hurt.Add(cols_buffer[i]);
                    player.TakeDamage(1);
                }
            }

            yield return null;
        }
    }

    public float Health = 15;

    public void TakeDamage(float damage)
    {
        materialChanger.ChangeMaterialForXTime(0.1f);
        Health -= damage;
        AudioManager.PlayHurtAt(hurtPoint.position, 0.9f);
        if(Health <= 0)
        {
            Health = 0;
            SetState(EnemyState.Dead);
        }
        // Debug.Log(this.gameObject.name + " took " + damage + " damage");
    }

    [Header("Audio:")]
    public AudioSource golem_audioSource;
    public AudioClip impact_clip;
    public AudioClip die_clip;

}
