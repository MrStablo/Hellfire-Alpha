using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Grenade : NetworkBehaviour
{
    public LayerMask WhatIsPlayer;
    [Server]
    public void StartDecay(float T)
    {
        Invoke("EndDecay", T);
        Debug.Log("Start decay for " + T);
    }
    [Server]
    private void EndDecay()
    {
        Debug.Log("End decay");
        NetworkServer.Destroy(gameObject);
    }
    [Server]
    public void DealDamage(float ExplosionRange,int Dmg)
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, ExplosionRange, WhatIsPlayer);
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<PlayerStats>().TakeDamage(Dmg);
        }
    }
}
