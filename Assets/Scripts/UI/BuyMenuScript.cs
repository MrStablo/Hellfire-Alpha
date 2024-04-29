using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BuyMenuScript : NetworkBehaviour
{
    public GunData[] gundata;
    public Dictionary<string, GunData> BuyableGuns = new Dictionary<string, GunData>();

    public PlayerStats playerStats;
    public GunManager gunManager;

    private void Awake()
    {
        for (int i = 0; i < gundata.Length; i++)
        {
            BuyableGuns.Add(gundata[i].name, gundata[i]);
        }
    }

    [Command(requiresAuthority = false)]
    public void BuyGun(string GunToBuy)
    {
        if (playerStats)
        {
            if (BuyableGuns[GunToBuy].Price <= playerStats.Money)
            {
                playerStats.Money -= BuyableGuns[GunToBuy].Price;
                switch (BuyableGuns[GunToBuy].Slot) 
                {
                    case 1:
                        Debug.Log("primaryset");
                        if (playerStats.PrimaryGun != null)
                        {
                            playerStats.Money += playerStats.PrimaryGun.Price;
                        }
                        playerStats.PrimaryGun = new GunObject(BuyableGuns[GunToBuy]);
                        playerStats.ActiveWeapon = playerStats.PrimaryGun;
                        break;
                    case 2:
                        if (playerStats.SecondaryGun != null)
                        {
                            playerStats.Money += playerStats.SecondaryGun.Price;
                        }
                        Debug.Log("secondaryset");
                        playerStats.SecondaryGun = new GunObject(BuyableGuns[GunToBuy]);
                        playerStats.ActiveWeapon = playerStats.SecondaryGun;

                        break;
                    case 4:
                        playerStats.Grenades.Add(new GunObject(BuyableGuns[GunToBuy]));
                        break;
                    default:
                        Debug.Log("nothingset");
                        break;
                }
                gunManager.SwapGuns();
            }
            else
            {
                Debug.Log(playerStats.Money);
            }
        }
        else
        {
            Debug.Log("NotFound");
        }
    }
    public void WispPressed()
    {
        BuyGun("Wisp");
    }
    public void CrowPressed()
    {
        BuyGun("Crow");
    }
    public void TorrentPressed()
    {
        BuyGun("Torrent");
    }
    public void HellfirePressed()
    {
        BuyGun("Hellfire");
    }
    public void WardenPressed()
    {
        BuyGun("Warden");
    }
    public void HEPressed()
    {
        BuyGun("G_Molatov");
    }
    public void SmokePressed()
    {
        BuyGun("G_Smoke");
    }
    public void HWPressed()
    {
        BuyGun("G_Blast");
    }
}
