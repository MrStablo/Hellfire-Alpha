using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item
{
    public abstract string GiveName();
    public virtual void ItemUpdate(PlayerStats stats, int stacks)
    {

    }
    public virtual void OnHit(PlayerStats stats, int stacks)
    {

    }
    public virtual void OnAttack(PlayerStats stats, int stacks)
    {

    }
    public virtual void OnTakeDamage(PlayerStats stats, int stacks)
    {

    }
    public virtual void OnThrowGrenade(PlayerStats stats, int stacks)
    {

    }
    public virtual void OnGrenadeExplode(PlayerStats stats, int stacks)
    {

    }
}

public class TestItem : Item
{
    public override string GiveName()
    {
        return "Test item";
    }
}