using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameStart : MonoBehaviour
{

    [SerializeField] GameObject[] ObjectsToRegister;
    void Start()
    {
        foreach (GameObject i in ObjectsToRegister)
        {
            NetworkClient.RegisterPrefab(i);
        }
    }
}
