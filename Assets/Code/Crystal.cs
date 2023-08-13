using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 playerCenterPos = PlayerController.Instance.GetCenter();
        float distance_to_player = Vector3.Distance(playerCenterPos, transform.position);
        if(distance_to_player < 3)
        {
            Dissappear();
        }
    }

    void Dissappear()
    {
        this.enabled = false;
        Destroy(this.gameObject, 0.66f);
        ArenaManager.Score++;
    }


    void FixedUpdate()
    {
        Vector3 playerCenterPos = PlayerController.Instance.GetCenter();
        float distance_to_player = Vector3.Distance(playerCenterPos, transform.position);
        if(distance_to_player < 6)
        {
            Vector3 dir_to_player = (playerCenterPos - transform.position).normalized;
            Vector3 target_velocity = dir_to_player * 16;
            rb.velocity = Vector3.MoveTowards(rb.velocity, target_velocity, Time.fixedDeltaTime * 20);
        }
        else
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(0, 0.25f, 0), Time.fixedDeltaTime * 5);
        }
    }
}
