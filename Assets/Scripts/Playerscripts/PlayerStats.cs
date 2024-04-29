using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerStats : NetworkBehaviour
{
    [SyncVar(hook = "HealthChanged")]
    public int Health;

    [SyncVar]
    public int Money;

    [SyncVar]
    public int Team;

    public GunObject ActiveWeapon;

    public GunObject PrimaryGun;
    public GunObject SecondaryGun;
    public GunObject MeleeWeapon;
    public List<GunObject> Grenades = new List<GunObject>();
    public List<ItemList> items = new List<ItemList>();

    public bool InBuyZone;

    [SerializeField] GunData BaseMelee;
    [SerializeField] GunData BasePistol;

    [SerializeField] MouseLook mouseLook;

    //items.Add(new Itemlist(item, item.GiveName(), 1));
    private void Awake()
    {
        SecondaryGun = new GunObject(BasePistol);
        MeleeWeapon = new GunObject(BaseMelee);
        ActiveWeapon = SecondaryGun;
    }
    private void Start()
    {
        StartCoroutine(CallItemUpdate());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BuyZone"))
        {
            InBuyZone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("BuyZone"))
        {
            ExitBuyzone();
        }
    }
    public void ExitBuyzone()
    {
        InBuyZone = false;
        mouseLook.ExitBuyZone();
    }

    public void TakeDamage(int Damage)
    {
        foreach (ItemList i in items)
        {
            i.item.OnTakeDamage(this, i.stacks);
        }
        Health -= Damage;
    }
    public void HealthChanged(int OldHealth, int NewHealth)
    {
        Debug.Log(gameObject.name + " has " + Health + " left");
        if (NewHealth <= 0)
        {
            if (isLocalPlayer)
            {
                Debug.Log("Death");
            }
            else
            {
                Destroy(gameObject);
                Debug.Log("enemy death");
            }
        }
    }
    //----------------------------------------------------items--------------------------------------------------------
    IEnumerator CallItemUpdate()
    {
        foreach (ItemList i in items)
        {
            i.item.ItemUpdate(this, i.stacks);
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(CallItemUpdate());
    }
    private void CallOnHit()
    {
        foreach (ItemList i in items)
        {
            i.item.OnHit(this, i.stacks);
        }
        foreach (ItemList i in ActiveWeapon.itemList)
        {
            i.item.OnHit(this, i.stacks);
        }
    }
    private void CallOnAttack()
    {
        foreach (ItemList i in items)
        {
            i.item.OnAttack(this, i.stacks);
        }
        foreach (ItemList i in ActiveWeapon.itemList)
        {
            i.item.OnHit(this, i.stacks);
        }
    }
    private void CallOnThrowGrenade()
    {
        foreach (ItemList i in items)
        {
            i.item.OnThrowGrenade(this, i.stacks);
        }
        foreach (ItemList i in ActiveWeapon.itemList)
        {
            i.item.OnHit(this, i.stacks);
        }
    }
    private void CallOnGrenadeExplode()
    {
        foreach (ItemList i in items)
        {
            i.item.OnGrenadeExplode(this, i.stacks);
        }
        foreach (ItemList i in ActiveWeapon.itemList)
        {
            i.item.OnHit(this, i.stacks);
        }
    }
}
