using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GiantController : MonoBehaviour
{
    public EnemyState state;

    Animator anim;
    NavMeshAgent agent;

    public AudioSource audioSource;

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }


    void SetState(EnemyState _state)
    {
        switch(_state)
        {
            default:
            {
                break;
            }
        }
        state = _state;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        {
            switch(state)
            {
                default:
                {
                    break;
                }
            }
        }
    }
}
