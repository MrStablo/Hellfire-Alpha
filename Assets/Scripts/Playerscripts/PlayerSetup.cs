using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] GameObject CameraMountPoint;
    [SerializeField] GameObject PlayerBaseObject;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerStats playerStats;
    [SerializeField] GunManager gunManager;

    void Start()
    {
        PlayerList();
        if (isLocalPlayer)
        {
            BuyMenuScript buyMenu = Camera.main.gameObject.GetComponentInChildren<BuyMenuScript>();
            if (buyMenu)
            {
                buyMenu.playerStats = playerStats;
                buyMenu.gunManager = gunManager;
            }
            else
            {
                Debug.Log("Didnt find menu");
            }
            Transform cameraTransform = Camera.main.gameObject.transform;  //Find main camera which is part of the scene instead of the prefab
            cameraTransform.parent = CameraMountPoint.transform;  //Make the camera a child of the mount point
            cameraTransform.position = CameraMountPoint.transform.position;  //Set position/rotation same as the mount point
            cameraTransform.rotation = CameraMountPoint.transform.rotation;

            foreach (Transform childTransform in transform.GetComponentsInChildren<Transform>())
            {
                if (childTransform.name != "GunObject" && childTransform.name != "GunShootPoint") 
                {
                    childTransform.gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
                }
                else
                {
                    childTransform.gameObject.layer = LayerMask.NameToLayer("UI");
                }
            }
        }
    }
    [Server]
    private void PlayerList()
    {
        FindObjectOfType<RoundManager>().UpdatePlayerList();
    }
}
