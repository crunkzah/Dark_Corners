using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ArenaState
{
    Interbellum,
    Combat
}

public class ArenaManager : MonoSingleton<ArenaManager>
{
    public ArenaState state;

    public static int Score;
    public GameObject golem_original;
    public GameObject lina_original;

    public GameObject crystal_original;

    public float spawning_timer;
    float spawn_freq = 3;

    public Transform[] spawns;

    void Awake()
    {
        state = ArenaState.Combat;
        spawning_timer = spawn_freq * 2;
    }


    Vector3 GetSpawnPosition()
    {
        Vector3 Result = new Vector3(0, 0, 0);

        NavMeshHit navMeshHit;

        Vector3 _p = spawns[Random.Range(0, spawns.Length-1)].position;

        for(int tries = 0; tries < 8; tries++)
        {
            float radius = Random.Range(0, 5f);
            Vector3 _dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            _dir.Normalize();
            Vector3 _pos = _p + radius * _dir;
            if(NavMesh.SamplePosition(_pos, out navMeshHit, radius, NavMesh.AllAreas))
            {
                Result = navMeshHit.position;
                tries = 99;
            }
        }

        return Result;
    }

    GameObject GetEnemy()
    {
        GameObject Result = null;

        Result = golem_original;

        return Result;
    }


    void Update()
    {
        switch(state)
        {
            case(ArenaState.Interbellum):
            {
                break;
            }
            case(ArenaState.Combat):
            {
                if(spawning_timer <= 0)
                {
                    //Debug.Log("Spawning");

                    Vector3 pos = GetSpawnPosition();
                    GameObject g = Instantiate(GetEnemy(), pos, Quaternion.identity);
                    Vector3 enemy_dir = PlayerController.Instance.transform.position - pos;
                    enemy_dir.y = 0;
                    enemy_dir.Normalize();
                    g.transform.forward = enemy_dir;
                    g.AddComponent<SpawnedObject>();
                    spawning_timer = spawn_freq;
                }
                else
                {
                    spawning_timer -= Time.deltaTime;
                }
                break;
            }
        }
    }

}
