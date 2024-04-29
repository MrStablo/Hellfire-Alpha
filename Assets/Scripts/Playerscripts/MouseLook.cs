using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class MouseLook : NetworkBehaviour
{
    private PlayerInputHandler playerInputHandler;
    private BuyMenuScript buyMenu;
    private PlayerStats playerStats;

    public float mouseSensitivityX = 100f;
    public float mouseSensitivityY = 100f;

    public Transform playerBody;

    float xRotation = 0f;

    private bool LookLock = false;

    [Client]
    void Start()
    {
        playerStats = playerBody.GetComponent<PlayerStats>();
        if (isLocalPlayer)
        {
            
            buyMenu = Camera.main.gameObject.GetComponentInChildren<BuyMenuScript>();
            buyMenu.gameObject.SetActive(false);

            playerInputHandler = transform.parent.GetComponent<PlayerInputHandler>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    [Client]
    void Update()
    {
        if (isLocalPlayer)
        {
            if (!LookLock)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

                playerBody.Rotate((Vector3.up * mouseX));
            }
        }
    }
    [Client]
    public void ToggleLookLock(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (playerStats.InBuyZone)
            {
                if (LookLock)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    LookLock = false;
                    buyMenu.gameObject.SetActive(false);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    LookLock = true;
                    buyMenu.gameObject.SetActive(true);
                }
            }            
        }
    }
    public void ExitBuyZone()
    {
        if (LookLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            LookLock = false;
            buyMenu.gameObject.SetActive(false);
        }
    }

}
