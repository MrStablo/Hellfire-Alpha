using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeObject : MonoBehaviour
{
    public float MaxFuseTime;
    public float FuseTimer;

    public GunObject grenadeType;

    public bool ExplodeOnImpact;

    public PlayerShoot playerShoot;


    private void Update()
    {
        FuseTimer += Time.deltaTime;
        if (FuseTimer >= MaxFuseTime)
        {
            playerShoot.S_ExplodeGrenade(gameObject, grenadeType);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        gameObject.layer = LayerMask.NameToLayer("GrenadeLayer");
        if (ExplodeOnImpact)
        {
            playerShoot.S_ExplodeGrenade(gameObject, grenadeType);
        }
    }
}
