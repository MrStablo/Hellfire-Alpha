using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunObject
{
    public int Price;
    public int Slot;

    public Mesh mesh;
    public Material material;

    public float Mobility;

    [Header("Reload")]
    public int BulletsInClip;

    public int MagazineSize;
    public float DrawTime;
    public float ReloadTime;
    public float Reloadmultiplier;
    public AnimationCurve ReloadAnimCurve;
    public Vector3 ReloadTargetPosition;
    public Vector3 ReloadTargetRotation;

    [Header("Time")]
    public GunData.FiringMode shootMode;

    public float TimeBetweenShots;

    public float RecoilResetMultiplier;

    [Header("Bullets")]
    public bool HitScan;
    public int CloseBodyDamage;
    public int CloseHeadDamage;

    public int MidBodyDamage;
    public int MidHeadDamage;

    public int LongBodyDamage;
    public int LongHeadDamage;

    public int BulletsPerShot;

    [Header("Projectiles")]
    public bool Explosive;
    public bool IsProjectile;
    public bool ExplodeOnImpact;
    public float FuseTime;

    public Vector2 ShootForce;
    public GameObject ProjectileObject;
    public GameObject ExplosionObject;

    [Header("Range")]
    public float MidThreshold;
    public float LongThreshold;

    [Header("Recoil")]

    public int ShotsUntilRandom;

    public float[] verticalRecoil;
    public Vector2[] ShotOffset;

    public float spread;

    public List<ItemList> itemList = new List<ItemList>();

    public GunObject(GunData gd)
    {
        Price = gd.Price;
        Slot = gd.Slot;
        mesh = gd.mesh;
        material = gd.material;
        Mobility = gd.Mobility;
        MagazineSize = gd.MagazineSize;
        BulletsInClip = gd.MagazineSize;
        DrawTime = gd.DrawTime;
        ReloadTime = gd.ReloadTime;
        Reloadmultiplier = gd.Reloadmultiplier;
        ReloadAnimCurve = gd.ReloadAnimCurve;
        ReloadTargetPosition = gd.ReloadTargetPosition;
        ReloadTargetRotation = gd.ReloadTargetRotation;
        shootMode = gd.shootMode;
        TimeBetweenShots = gd.TimeBetweenShots;
        RecoilResetMultiplier = gd.RecoilResetMultiplier;
        HitScan = gd.HitScan;
        CloseBodyDamage = gd.CloseBodyDamage;
        CloseHeadDamage = gd.CloseHeadDamage;
        MidBodyDamage = gd.MidBodyDamage;
        MidHeadDamage = gd.MidHeadDamage;
        LongBodyDamage = gd.LongBodyDamage;
        LongHeadDamage = gd.LongHeadDamage;
        BulletsPerShot = gd.BulletsPerShot;
        Explosive = gd.Explosive;
        IsProjectile = gd.IsProjectile;
        ExplodeOnImpact = gd.ExplodeOnImpact;
        FuseTime = gd.FuseTime;
        ShootForce = gd.ShootForce;
        ProjectileObject = gd.ProjectileObject;
        ExplosionObject = gd.ExplosionObject;
        MidThreshold = gd.MidThreshold;
        LongThreshold = gd.LongThreshold;
        ShotsUntilRandom = gd.ShotsUntilRandom;
        verticalRecoil = gd.verticalRecoil;
        ShotOffset = gd.ShotOffset;
        spread = gd.spread;
    }
}
