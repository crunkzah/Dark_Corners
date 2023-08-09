using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer lr;

    public void Make(Vector3 start, Vector3 end)
    {
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    void Update()
    {
        lr.startWidth = Mathf.MoveTowards(lr.startWidth, 0, Time.deltaTime * 1);
        lr.endWidth = Mathf.MoveTowards(lr.endWidth, 0, Time.deltaTime * 1);
    }
}
