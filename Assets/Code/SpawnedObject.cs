using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedObject : MonoBehaviour
{
    public void OnObjectDied()
    {
        Instantiate(ArenaManager.Instance.crystal_original, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
    }
}
