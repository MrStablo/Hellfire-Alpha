using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Molatov : Grenade
{
    [SerializeField] float FireRange;
    [SerializeField] int FireDamage;

    [Server]
    private void Start()
    {
        StartCoroutine(FireTick());
    }
    private IEnumerator FireTick()
    {
        DealDamage(FireRange,FireDamage);

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(FireTick());
    }
}
