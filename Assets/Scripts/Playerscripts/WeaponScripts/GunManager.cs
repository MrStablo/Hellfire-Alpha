using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Mirror;

public class GunManager : NetworkBehaviour
{

    [SerializeField] GameObject BuyMenuObject;
    [SerializeField] GameObject GunObject;
    [SerializeField] PlayerStats playerStats;

    public Vector2 MovementVector;
    public InputAction.CallbackContext JumpCtx;
    public bool LookLock = false;
    public int ActiveSlot;
    public int PreviousSlot;
    public bool reloading;
    public bool drawing;

    public float CurrentT, _target;
    public TextMeshProUGUI ammunitionDisplay;

    public int[] GrenadeTypesUsed;
    private void Awake()
    {
        ActiveSlot = 2;
        PreviousSlot = 3;
    }
    private void Start()
    {
        ammunitionDisplay = GameObject.Find("AmmoDisplay").GetComponent<TextMeshProUGUI>();
        SwapGuns();
        UpdateText();
    }
    public void ReloadAnimation()
    {
        _target = _target == 0 ? 1 : 0;
        CurrentT = Mathf.MoveTowards(CurrentT, 1, Time.deltaTime * playerStats.ActiveWeapon.Reloadmultiplier);
        GunObject.transform.localPosition = Vector3.Lerp(new Vector3(0.2f, -0.16f, 0.4f), playerStats.ActiveWeapon.ReloadTargetPosition, playerStats.ActiveWeapon.ReloadAnimCurve.Evaluate(Mathf.PingPong(CurrentT, 0.5f) * 2));
        GunObject.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, -90, 0), Quaternion.Euler(playerStats.ActiveWeapon.ReloadTargetRotation), playerStats.ActiveWeapon.ReloadAnimCurve.Evaluate(Mathf.PingPong(CurrentT, 0.5f) * 2));
    }
    public void UpdateText()
    {
        ammunitionDisplay.SetText(playerStats.ActiveWeapon.BulletsInClip + " / " + playerStats.ActiveWeapon.MagazineSize / playerStats.ActiveWeapon.BulletsPerShot);
    }
    public void StartReload()
    {
        CurrentT = 0;
        Invoke("EndReload", playerStats.ActiveWeapon.ReloadTime);
        reloading = true;
    }
    private void EndReload()
    {
        reloading = false;
        playerStats.ActiveWeapon.BulletsInClip = playerStats.ActiveWeapon.MagazineSize;
        CurrentT = 1;
        ReloadAnimation();
        UpdateText();
    }
    public void StartDraw()
    {
        CurrentT = 0.5f;
        Invoke("EndDraw", playerStats.ActiveWeapon.DrawTime);
        reloading = true;
    }
    private void EndDraw()
    {
        CurrentT = 0;
        ReloadAnimation();
        reloading = false;
    }
    public void Primary_Performed(InputAction.CallbackContext ctx)
    {
        if (ActiveSlot != 1 && playerStats.PrimaryGun != null && ctx.started)
        {
            PreviousSlot = ActiveSlot;
            ActiveSlot = 1;
            playerStats.ActiveWeapon = playerStats.PrimaryGun;
            SwapGuns();
        }
    }
    public void Secondary_Performed(InputAction.CallbackContext ctx)
    {
        if (ActiveSlot != 2 && playerStats.SecondaryGun != null && ctx.started)
        {
            PreviousSlot = ActiveSlot;
            ActiveSlot = 2;
            playerStats.ActiveWeapon = playerStats.SecondaryGun;
            SwapGuns();
        }
    }
    public void Melee_Performed(InputAction.CallbackContext ctx)
    {
        if (ActiveSlot != 3 && ctx.started)
        {
            PreviousSlot = ActiveSlot;
            ActiveSlot = 3;
            playerStats.ActiveWeapon = playerStats.MeleeWeapon;
            playerStats.ActiveWeapon.BulletsInClip = 1;
            SwapGuns();
        }
    }
    public void Grenade_Performed(InputAction.CallbackContext ctx)
    {
        if (ActiveSlot != 4 && playerStats.Grenades.Count > 0 && ctx.started )
        {
            PreviousSlot = ActiveSlot;
            ActiveSlot = 4;
            playerStats.ActiveWeapon = playerStats.Grenades[0];
            SwapGuns();
        }
    }
    public void QuickSwitch_Performed(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            QuickSwitch();
        }
    }
    public void QuickSwitch()
    {
        int temp = ActiveSlot;
        ActiveSlot = PreviousSlot;
        PreviousSlot = temp;

        Debug.Log("Activeslot = " + ActiveSlot + ", Previousslot = " + PreviousSlot);

        switch (ActiveSlot)
        {
            case 1:

                playerStats.ActiveWeapon = playerStats.PrimaryGun;
                SwapGuns();
                break;
            case 2:
                playerStats.ActiveWeapon = playerStats.SecondaryGun;
                SwapGuns();
                break;
            case 3:
                playerStats.ActiveWeapon = playerStats.MeleeWeapon;
                playerStats.ActiveWeapon.BulletsInClip = 1;
                SwapGuns();
                break;
            case 4:
                playerStats.ActiveWeapon = playerStats.Grenades[0];
                SwapGuns();
                break;
            default:
                break;
        }
    }
    public void SwapGuns()
    {
        GunObject.GetComponent<MeshFilter>().mesh = playerStats.ActiveWeapon.mesh;
        GunObject.GetComponent<MeshRenderer>().material = playerStats.ActiveWeapon.material;
        UpdateText();
        StartDraw();
    }
}
