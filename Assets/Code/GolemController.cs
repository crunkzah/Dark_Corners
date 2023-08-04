using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class GolemController : MonoBehaviour, IDamagable
{
    public EnemyState state = EnemyState.Chasing;
    Animator anim;
    NavMeshAgent agent;
    MaterialChanger materialChanger;

    public Transform hurtPoint;

    public LayerMask hurtMask;

    void Awake()
    {
        anim            = GetComponent<Animator>();
        agent           = GetComponent<NavMeshAgent>();
        materialChanger = GetComponent<MaterialChanger>();
    }

    void SetState(EnemyState _state)
    {
        if(state == EnemyState.Dead)
            return;

        switch(_state)
        {
            default:
            {
                break;
            }
        }

        state = _state;
    }

    float attack_timer;

    void Update()
    {
        float dt = Time.deltaTime;
        attack_timer -= dt;

        switch(state)
        {
            case(EnemyState.Chasing):
            {
                anim.SetFloat("MoveSpeed", agent.velocity.magnitude);
                agent.SetDestination(PlayerController.GetPosition());
                float distance_to_player = Vector3.Distance(transform.position, PlayerController.GetPosition());

                if(distance_to_player < agent.stoppingDistance * 1.1f)
                {
                    Math.RotateTowardsPositionXZ(transform, PlayerController.GetPosition(), dt * 180);

                    if(attack_timer <= 0)
                    {
                        attack_timer = 2;
                        anim.Play("UpperBody.Attack1", 1, 0);
                    }
                }
                break;
            }
            default:
            {
                break;
            }
        }
    }

    public void OnAttack1()
    {
        Debug.Log("OnAttack1");
        
        StartCoroutine(DoDamage(0.1f, 1f));
    }

    static Collider[] cols_buffer = new Collider[64];
    HashSet<Collider> cols_were_hurt = new HashSet<Collider>();

    IEnumerator DoDamage(float duration, float radius)
    {
        cols_were_hurt.Clear();

        for(float timer = 0; timer <= duration; timer += Time.deltaTime)
        {
            int len = Physics.OverlapSphereNonAlloc(hurtPoint.position, 1f, cols_buffer, hurtMask);
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

    public void TakeDamage(float damage)
    {
        materialChanger.ChangeMaterialForXTime(0.1f);
        Debug.Log(this.gameObject.name + " took " + damage + " damage");
    }
}
