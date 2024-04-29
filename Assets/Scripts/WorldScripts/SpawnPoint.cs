using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnPoint : NetworkBehaviour
{
    [SerializeField] bool IsAttacker;

    
    void Awake()
    {
        FindObjectOfType<RoundManager>().UpdateSpawnPoints(transform.position, IsAttacker);
    }
}
