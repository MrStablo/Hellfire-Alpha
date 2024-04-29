using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] Camera MainCam;
    [SerializeField] GameObject CamPoint;
    [SerializeField] GameObject GunShootPoint;
    [SerializeField] PlayerStats PlayerStats;
    [SerializeField] GameObject BuyMenuObject;
    [SerializeField] PlayerStats playerStats;

    [SerializeField] GunManager gunManager;
    private Rigidbody rb;


    //grenades
    public LayerMask ShootLayers;

    private Vector3 lastShootposition;
    private Vector3 lastHitposition;
    private Vector3 targetPoint;
    private float Prediction_TimeSinceLastShot, TimeSinceLastShot;

    //input
    private bool ShootButtonDown;
    private float ShootBuffercounter;
    private float ShootBufferTime = 0.2f;

    private int Prediction_BulletsShot;
    private int BulletsShot;

    //recoil
    private float XRecoil, YRecoil;
    private float Prediction_XRecoil, Prediction_YRecoil;

    //visuals
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject BulletHolePrefab;


    //-----------------------------------------------------------------------------------------------Manager----------------------------------------------------------------------------

    private void Start()
    {
        MainCam = Camera.main;
        rb = playerStats.GetComponentInParent<Rigidbody>();
    }
    private void Update()
    {
        C_Update();
        S_Update();
    }
    [Client]
    private void C_Update()
    {
        if (isLocalPlayer)
        {
            ShootBuffercounter -= Time.deltaTime;
            Prediction_TimeSinceLastShot += Time.deltaTime;

            if (ShootBuffercounter > 0.01f && playerStats.ActiveWeapon.shootMode != GunData.FiringMode.FullAuto && !gunManager.reloading)
            {
                if (Prediction_TimeSinceLastShot >= playerStats.ActiveWeapon.TimeBetweenShots)
                {
                    C_AttackInput();
                }
            }
            else if (ShootButtonDown && playerStats.ActiveWeapon.shootMode == GunData.FiringMode.FullAuto && !gunManager.reloading)
            {
                if (Prediction_TimeSinceLastShot >= playerStats.ActiveWeapon.TimeBetweenShots)
                {
                    C_AttackInput();
                }
            }
            if (Prediction_TimeSinceLastShot >= 0.3f)
            {
                if (Prediction_YRecoil >= 0.001f)
                {
                    Prediction_YRecoil = Mathf.Max(Prediction_YRecoil - Time.deltaTime * playerStats.ActiveWeapon.RecoilResetMultiplier, 0);
                }
                else
                {
                    Prediction_BulletsShot = 0;
                }
            }

            MainCam.gameObject.transform.localRotation = Quaternion.Euler(-Prediction_YRecoil, 0f, 0f);
            if (gunManager.reloading)
            {
                gunManager.ReloadAnimation();
                
            }
        }
    }
    [Server]
    private void S_Update()
    {

        TimeSinceLastShot += Time.deltaTime;
        if (TimeSinceLastShot >= 0.3f)
        {
            if (YRecoil >= 0.001f)
            {
                YRecoil = Mathf.Max(YRecoil - Time.deltaTime * playerStats.ActiveWeapon.RecoilResetMultiplier, 0);
            }
            else
            {
                BulletsShot = 0;
            }
        }
        CamPoint.gameObject.transform.localRotation = Quaternion.Euler(YRecoil, 180, 0f);
    }
    [Client]
    private void C_AttackInput()
    {
        ShootBuffercounter = 0;
        switch (playerStats.ActiveWeapon.shootMode)
        {
            case (GunData.FiringMode.FullAuto):
                C_Shoot();
                Debug.Log("fullauto");
                break;
            case (GunData.FiringMode.BurstFire):
                C_Shoot();
                break;
            case (GunData.FiringMode.SemiAuto):
                C_Shoot();
                break;
            case (GunData.FiringMode.Single):
                C_Shoot();
                break;
            case (GunData.FiringMode.Melee):
                C_Melee();
                break;
            case (GunData.FiringMode.Grenade):
                C_Grenade();
                break;
        }
    }
    //-----------------------------------------------------------------------------------------------Shooting----------------------------------------------------------------------------
        [Client]
    private void C_Shoot()
    {
        if (playerStats.ActiveWeapon.BulletsInClip > 0)
        {
            Prediction_TimeSinceLastShot = 0;

            Vector3 direcionWOSpread = MainCam.transform.forward;

            //calculate spread
            float Rx = Random.Range(-playerStats.ActiveWeapon.spread, playerStats.ActiveWeapon.spread);
            float Ry = Random.Range(-playerStats.ActiveWeapon.spread, playerStats.ActiveWeapon.spread);

            Vector3 direcionWOffset = direcionWOSpread + transform.TransformDirection(new Vector3(playerStats.ActiveWeapon.ShotOffset[Prediction_BulletsShot].x / 60, playerStats.ActiveWeapon.ShotOffset[Prediction_BulletsShot].y / 60));
            float MovementError = Mathf.Abs(Mathf.Min(((rb.velocity.x + rb.velocity.y + rb.velocity.z) * 4f) - 1, 1f));
            Vector3 directionWSpread = direcionWOffset + new Vector3(Rx * MovementError, Ry * MovementError, 0);

            RaycastHit HitScanHit;
            Ray HitScanRay = new Ray(MainCam.transform.position, directionWSpread);

            //create shot
            if (Physics.Raycast(HitScanRay, out HitScanHit, 900, ShootLayers))
            {
                lastHitposition = HitScanHit.point;

                if (HitScanHit.collider.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
                {
                    Debug.Log("Hit a player on client");
                }
                else //Bullet holes
                {
                    GameObject bulletHoleLocal = Instantiate(BulletHolePrefab, lastHitposition, Quaternion.LookRotation(HitScanHit.normal));
                    bulletHoleLocal.transform.position += bulletHoleLocal.transform.forward / 100;

                    Destroy(bulletHoleLocal, 10f);
                }
            }
            else
            {
                lastHitposition = HitScanRay.GetPoint(900);
            }
            //create muzzle flash
            if (muzzleFlash != null)
            {
                GameObject bTracer = Instantiate(muzzleFlash, GunShootPoint.transform.position, Quaternion.identity);

                LineRenderer TracerLine = bTracer.GetComponent<LineRenderer>();

                lastShootposition = GunShootPoint.transform.position;
                TracerLine.SetPosition(0, lastShootposition);
                TracerLine.SetPosition(1, lastHitposition);

                Destroy(bTracer, 0.1f);
            }


            //recoil
            Prediction_YRecoil = playerStats.ActiveWeapon.verticalRecoil[Prediction_BulletsShot];
            if (Prediction_BulletsShot != playerStats.ActiveWeapon.ShotsUntilRandom - 1)
            {
                Prediction_BulletsShot += 1;
            }
            playerStats.ActiveWeapon.BulletsInClip --;

           

            CMD_Shoot(Rx, Ry);

            gunManager.UpdateText();
        }
        else
        {
            if (!gunManager.reloading)
            {
                gunManager.StartReload();
            }
        }
    }
    [Command]
    private void CMD_Shoot(float CMDRx, float CMDRy)
    {
        TimeSinceLastShot = 0;

        Vector3 direcionWOSpread = -CamPoint.transform.forward;
        Vector3 direcionWOffset = direcionWOSpread + transform.TransformDirection(new Vector3(PlayerStats.ActiveWeapon.ShotOffset[BulletsShot].x / 60, PlayerStats.ActiveWeapon.ShotOffset[BulletsShot].y / 60, 0));
        Vector3 directionWSpread = direcionWOffset + new Vector3(CMDRx, CMDRy, 0);

        RaycastHit HitScanHit;
        Ray HitScanRay = new Ray(CamPoint.transform.position, directionWSpread);
        bool HitsScanRayBool = Physics.Raycast(HitScanRay, out HitScanHit, 900, ShootLayers);

        //create shot
        bool HitWall = true;
        if (HitsScanRayBool)
        {
            lastHitposition = HitScanHit.point;

            if (HitScanHit.collider.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
            {
                Debug.Log("Hit a player on server");
                HitWall = false;
            }
        }
        else
        {
            lastHitposition = HitScanRay.GetPoint(900);
        }
        //recoil
        YRecoil = PlayerStats.ActiveWeapon.verticalRecoil[BulletsShot];
        if (BulletsShot != PlayerStats.ActiveWeapon.ShotsUntilRandom - 1)
        {
            BulletsShot += 1;
        }

        CRPC_Shoot(lastHitposition, HitScanHit.normal, HitWall);
    }
    [ClientRpc(includeOwner = false)]
    private void CRPC_Shoot(Vector3 HitPos, Vector3 ImpactNormal, bool HitWall)
    {
        Debug.Log("recievedShot");

        GameObject bTracer = Instantiate(muzzleFlash, CamPoint.transform.position, Quaternion.identity);

        LineRenderer TracerLine = bTracer.GetComponent<LineRenderer>();

        TracerLine.SetPosition(0, CamPoint.transform.position);
        TracerLine.SetPosition(1, HitPos);

        Destroy(bTracer, 0.5f);

        if (HitWall)
        {
            GameObject bulletHoleLocal = Instantiate(BulletHolePrefab, lastHitposition, Quaternion.LookRotation(ImpactNormal));
            bulletHoleLocal.transform.position += bulletHoleLocal.transform.forward / 100;

            Destroy(bulletHoleLocal, 10f);
        }
    }

    //-------------------------------------------------------------------------------------------------Melee------------------------------------------------------------------------------
   [Client]
    private void C_Melee()
    {
        Prediction_TimeSinceLastShot = 0;
        ShootBuffercounter = 0;

        Vector3 directionWOSpread = MainCam.transform.forward;

        RaycastHit HitScanHit;
        Ray HitScanRay = new Ray(MainCam.transform.position, directionWOSpread);
        bool HitsScanRayBool = Physics.Raycast(HitScanRay, out HitScanHit, 4, ShootLayers);



        //create shot
        if (HitsScanRayBool)
        {
            lastHitposition = HitScanHit.point;

            if (HitScanHit.collider.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
            {
                Debug.Log("Hit a player on client");
            }
            else //Bullet holes
            {
                GameObject bulletHoleLocal = Instantiate(BulletHolePrefab, lastHitposition, Quaternion.LookRotation(HitScanHit.normal));
                bulletHoleLocal.transform.position += bulletHoleLocal.transform.forward / 100;

                Destroy(bulletHoleLocal, 10f);
            }
        }
        else
        {
            lastHitposition = HitScanRay.GetPoint(4);
        }
        
        CMD_Melee();
    }
    [Command]
    private void CMD_Melee()
    {
        TimeSinceLastShot = 0;

        Vector3 directionWOSpread = -CamPoint.transform.forward;

        RaycastHit HitScanHit;
        Ray HitScanRay = new Ray(CamPoint.transform.position, directionWOSpread);
        bool HitsScanRayBool = Physics.Raycast(HitScanRay, out HitScanHit, 4, ShootLayers);

        //create shot
        bool HitWall = true;
        if (HitsScanRayBool)
        {
            lastHitposition = HitScanHit.point;

            if (HitScanHit.collider.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
            {
                Debug.Log("Hit a player on server");
                HitWall = false;
            }
        }
        else
        {
            lastHitposition = HitScanRay.GetPoint(4);
        }

        CRPC_Melee(lastHitposition, HitScanHit.normal, HitWall);
    }
    [ClientRpc(includeOwner = false)]
    private void CRPC_Melee(Vector3 HitPos, Vector3 ImpactNormal, bool HitWall)
    {
        Debug.Log("recievedMelee");
        if (HitWall)
        {
            GameObject bulletHoleLocal = Instantiate(BulletHolePrefab, lastHitposition, Quaternion.LookRotation(ImpactNormal));
            bulletHoleLocal.transform.position += bulletHoleLocal.transform.forward / 100;

            Destroy(bulletHoleLocal, 10f);
        }
    }
    //-----------------------------------------------------------------------------------------------Grenades----------------------------------------------------------------------------
    [Client]
    private void C_Grenade()
    {
        TimeSinceLastShot = 0;

        CMD_Grenade();

        playerStats.Grenades.RemoveAt(0);
        gunManager.QuickSwitch();

        if (!gunManager.reloading && playerStats.ActiveWeapon.BulletsInClip != playerStats.ActiveWeapon.MagazineSize)
        {
            gunManager.StartReload();
        }
    }
    [Command]
    private void CMD_Grenade()
    {
        GameObject LocalGrenade = Instantiate(PlayerStats.ActiveWeapon.ProjectileObject, CamPoint.transform.position - (CamPoint.transform.forward), CamPoint.transform.rotation);
        GrenadeObject GrenadeComponent = LocalGrenade.GetComponent<GrenadeObject>();
        GrenadeComponent.MaxFuseTime = playerStats.ActiveWeapon.FuseTime;
        GrenadeComponent.ExplodeOnImpact = playerStats.ActiveWeapon.ExplodeOnImpact;
        GrenadeComponent.playerShoot = this;
        GrenadeComponent.grenadeType = playerStats.Grenades[0];
        Rigidbody LocalRigidbody = LocalGrenade.GetComponent<Rigidbody>();
        LocalRigidbody.velocity = CamPoint.transform.forward * PlayerStats.ActiveWeapon.ShootForce.x;
        NetworkServer.Spawn(LocalGrenade);
    }
    [Server]
    public void S_ExplodeGrenade(GameObject g, GunObject grenadeType)
    {
        Debug.Log("Grenade exploded");
        GameObject GrenadeToDestroy = g;

        GameObject LocalExplosion = Instantiate(grenadeType.ExplosionObject, GrenadeToDestroy.transform.position, Quaternion.identity);
        NetworkServer.Spawn(LocalExplosion);
        LocalExplosion.GetComponent<Grenade>().StartDecay(12f);

        NetworkServer.Destroy(GrenadeToDestroy);
    }
    //-------------------------------------------------------------------------------------------------Inputs------------------------------------------------------------------------------
    public void Reload_Performed(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            gunManager.StartReload();
        }
    }
    public void Shoot_Performed(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            ShootBuffercounter = ShootBufferTime;
            ShootButtonDown = true;
        }
        if (ctx.canceled)
        {
            ShootButtonDown = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(MainCam.transform.position, lastHitposition);
        Gizmos.DrawSphere(targetPoint, 0.4f);
    }
}
