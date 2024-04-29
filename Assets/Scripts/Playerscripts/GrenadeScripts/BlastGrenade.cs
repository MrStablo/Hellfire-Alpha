using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BlastGrenade : Grenade
{
    [SerializeField] float BlastRange;
    [SerializeField] float BlastForce;
    [SerializeField] float BlastUpModifier;

    [Server]
    private void Start()
    {
        Debug.Log("Started");
        StartCoroutine("applyForce");
    }
    private IEnumerator applyForce()
    {
        yield return new WaitForFixedUpdate();
        Collider[] objs = Physics.OverlapSphere(transform.position, BlastRange, WhatIsPlayer);
        for (int i = 0; i < objs.Length; i++)
        {
            Rigidbody rb = objs[i].GetComponent<Rigidbody>();
            if (rb)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); ;
                rb.AddForce(Vector3.Normalize(objs[i].transform.position - transform.position) * BlastForce, ForceMode.VelocityChange);
            }
        }
    }
}
