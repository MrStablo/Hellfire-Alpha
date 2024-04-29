using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Gun", menuName = "Gun/Weapon")]
public class GunData : ScriptableObject
{
    public int Price;
    public int Slot;

    public Mesh mesh;
    public Material material;
    public enum FiringMode
    {
        Single,
        SemiAuto,
        BurstFire,
        FullAuto,
        Melee,
        Grenade
    }

    public float Mobility;

    [Header("Reload")]
    public int MagazineSize;
    public float DrawTime;
    public float ReloadTime;
    public float Reloadmultiplier;
    public AnimationCurve ReloadAnimCurve;
    public Vector3 ReloadTargetPosition;
    public Vector3 ReloadTargetRotation;

    [Header("Time")]
    public FiringMode shootMode;

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
}
