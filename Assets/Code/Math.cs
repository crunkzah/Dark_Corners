using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class Math 
{
    public static int Multiply(int a, float b)
    {
        return (int)(((float)a) * b);
    }

    public static Vector3 Modify_Y(Vector3 v, float y)
    {
        Vector3 Result = v;
        Result.y = y;

        return Result;
    }

    public static Vector3 Modify_X(Vector3 v, float x)
    {
        Vector3 Result = v;
        Result.x = x;

        return Result;
    }

    public static Vector3 Get_XZ(Vector3 v)
    {
        Vector3 Result = v;
        Result.y = 0;

        return Result;
    }

    public static Vector3 RandomPositiveDirXZ()
    {
        Vector3 v = new Vector3(Random.Range(0, 1f), 0, Random.Range(0, 1f));
        return v.normalized;
    }

    public static float Abs(float a)
    {
        if(a < 0)
            a = -a;
        return a;
    }

    public static void RotateTowardsPosition(Transform tr, Vector3 pos, float delta)
    {
        Vector3 p = tr.position;
        pos.y = p.y = 0;
        Vector3 dir = (pos - tr.position).normalized;

        if(dir.sqrMagnitude == 0)
            return;
        Vector3 Result = Vector3.RotateTowards(tr.forward, dir, delta, 25);
        Result.y = 0;
        tr.forward = Result.normalized;
    }

    public static Color Modify_Alpha(Color col, float a)
    {
        col.a = a;
        return col;
    }

    public static Vector3 DirectionTo(Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }

    public static bool SphereAndRaycast(Ray ray, float radius, out RaycastHit hit, float distance, int mask)
    {
        if(Physics.Raycast(ray, out hit, distance, mask))
            return true;

        if(Physics.SphereCast(ray, radius, out hit, distance, mask))
            return true;

        return false;
    }

    public static bool SphereAndRaycast(Ray ray, float radius, float distance, int mask)
    {
        if(Physics.Raycast(ray, distance, mask))
            return true;

        if(Physics.SphereCast(ray, radius, distance, mask))
            return true;

        return false;
    }

    public static float SqrDistance(Vector3 a, Vector3 b)
    {
        float Result = 0;

        Result += (a.x-b.x)*(a.x-b.x);
        Result += (a.y-b.y)*(a.y-b.y);
        Result += (a.z-b.z)*(a.z-b.z);

        return Result;
    }

    public static float Decimal_to_IntPercentage(float v)
    {
        return (int)(v * 100f + 0.01f);
    }

    public static int RNG()
    {
        return Random.Range(0, 100);
    }

    public static int RNG(int max_range)
    {
        return Random.Range(0, max_range);
    }


    public static bool DoWeSeePlayer(Vector3 origin, Vector3 target_pos, float distance, int layerMask)
    {
        if(distance < 0)
            return true;

        if(Vector3.SqrMagnitude(target_pos - origin) > distance * distance)
        {
            return false;
        }
        
        Ray ray = new Ray(origin, (target_pos - origin).normalized);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, distance, layerMask))
        {
            if(hit.distance > Vector3.Distance(origin, target_pos))
            {
                return true;
            }
        }

        return false;
    }

    public static float Clamp(float val, float min, float max)
    {
        if(min < max)
        {
            float t = min;
            min = max;
            max = t;
        }

        if(val < min)
            return min;
        if(val > max)
            return max;
            
        return val;
    }

    public static float ClampToZero(float val)
    {
        if(val < 0)
            val = 0;

        return val;
    }

    public static float Min(float a, float b)
    {
        if(b < a)
            return b;
        
        return a;
    }

    public static float Max(float a, float b)
    {
        if(b > a)
            return b;

        return a;
    }
}
