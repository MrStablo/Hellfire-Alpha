using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class RoundManager : NetworkBehaviour
{
    private TextMeshProUGUI tmp;

    public List<Vector3> AttackerSpawnPoints = new List<Vector3>();
    public List<Vector3> DefenderSpawnPoints = new List<Vector3>();
    public GameObject[] BuyZones;
    public GameObject[] SpawnBarriers;

    public float RoundTimer;
    private float[] MaxTime = {25, 105, 5};
    [SyncVar] public int CurrentPhase;
    //0 - buy phase
    //1 - main phase
    //2 - post round

    public List<GameObject> players = new List<GameObject>();
    //teams-
    //0 - unassigned
    //1 - attackers
    //2 - defenders
    private void Awake()
    {
        BuyZones = GameObject.FindGameObjectsWithTag("BuyZone");
        SpawnBarriers = GameObject.FindGameObjectsWithTag("SpawnBarrier");
        tmp = GameObject.FindGameObjectWithTag("RoundTimer").GetComponent<TextMeshProUGUI>();
        CurrentPhase = 0;
    }
    private void Start()
    {
        Invoke("GameStart", 5f); 
        Invoke("RoundStart", 5.2f);
    }
    private void Update()
    {
        RoundTimer += Time.deltaTime;
        if (RoundTimer < MaxTime[CurrentPhase])
        {
            tmp.SetText(string.Format("{0:N0}", Mathf.Ceil(MaxTime[CurrentPhase] - RoundTimer)));
        }
        else
        {
            switch (CurrentPhase)
            {
                case 0:
                    EndBuyPhase();
                    break;
                case 1:
                    EndMainPhase();
                    break;
                case 2:
                    EndPostRound();
                    break;
            }
            if (isServer)
            {
                SyncTimer(RoundTimer);
            }
        }
    }
    private void GameStart()
    {
        for (int i = players.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, players.Count - 1);
            GameObject temp = players[i];
            players[i] = players[j];
            players[j] = temp;
        }
        for (int i = 0; i < players.Count; i++)
        {
            if (i <= Mathf.Ceil(players.Count / 2))
            {
                players[i].GetComponent<PlayerStats>().Team = 1;
                
            }
            else
            {
                players[i].GetComponent<PlayerStats>().Team = 2;
            }
        }
        if (isServer)
        {
            SyncTimer(RoundTimer);
        }
    }
    private void RoundStart() 
    {
        CurrentPhase = 0;
        RoundTimer = 0;

        foreach (GameObject i in BuyZones)
        {
            i.SetActive(true);
        }
        foreach (GameObject i in SpawnBarriers)
        {
            i.SetActive(true);
        }

        //shuffle spawn points
        for (int i = AttackerSpawnPoints.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, AttackerSpawnPoints.Count - 1);
            Vector3 temp = AttackerSpawnPoints[i];
            AttackerSpawnPoints[i] = AttackerSpawnPoints[j];
            AttackerSpawnPoints[j] = temp;
        }
        for (int i = DefenderSpawnPoints.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, DefenderSpawnPoints.Count - 1);
            Vector3 temp = DefenderSpawnPoints[i];
            DefenderSpawnPoints[i] = DefenderSpawnPoints[j];
            DefenderSpawnPoints[j] = temp;
        }


        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerStats>().Team == 1) 
            {
                players[i].transform.position = AttackerSpawnPoints[i];
            }
            else
            {
                players[i].transform.position = DefenderSpawnPoints[i];
            }
        }
    }
    private void EndBuyPhase()
    {
        CurrentPhase = 1;
        RoundTimer = 0;
        Debug.Log("Start main");

        foreach (GameObject i in BuyZones)
        {
            foreach (GameObject p in players)
            {
                PlayerStats playerstats = p.GetComponent<PlayerStats>();
                if (playerstats)
                {
                    playerstats.ExitBuyzone();
                }
            }

            i.SetActive(false);
        }
        foreach (GameObject i in SpawnBarriers)
        {
            i.SetActive(false);
        }
    }
    private void EndMainPhase()
    {
        CurrentPhase = 2;
        RoundTimer = 0;
        Debug.Log("Start end");
    }
    private void EndPostRound()
    {
        CurrentPhase = 0;
        RoundTimer = 0;
        Debug.Log("Start buy");
        RoundStart();
    }
    public void UpdatePlayerList()
    {
        PlayerStats[] playerStatsArray = FindObjectsOfType<PlayerStats>();
        foreach (PlayerStats i in playerStatsArray)
        {
            players.Add(i.gameObject);
        }
        if (isServer)
        {
            SyncTimer(RoundTimer);
        }
    }
    public void UpdateSpawnPoints(Vector3 position, bool IsAttacker)
    {
       if(IsAttacker == true)
       {
            AttackerSpawnPoints.Add(position);
       }
       else
       {
            DefenderSpawnPoints.Add(position);
       }
    }

    [ClientRpc]
    public void SyncTimer(float t)
    {
        RoundTimer = t;
    }
}