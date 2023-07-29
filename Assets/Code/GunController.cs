using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Animator revolverAnim;
    float gunTimer = 0;
    void Update()
    {
        gunTimer -= Time.deltaTime;
        if(gunTimer <= 0)
            gunTimer = 0;

        if(gunTimer == 0)
        {
            if(Input.GetKeyDown(Inputs.AttackKey.Key))
            {
                revolverAnim.Play("Base.Fire", 0, 0);
                gunTimer = 0.3f;
            }
        }
    }
}
