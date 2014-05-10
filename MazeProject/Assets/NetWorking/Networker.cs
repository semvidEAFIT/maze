﻿using UnityEngine;
using System.Collections.Generic;
using Boomlagoon.JSON;
using System;

[RequireComponent (typeof(NetworkView))]
public class Networker : MonoBehaviour {

    private const int PORT = 7456;
    public const int MAXPLAYERS = 5;

    private EPlayerType playerType;
    private int spawnIndex;

    private static Networker instance;

    public List<string> players;
    private string userName;
    public string UserName
    {
        get { return userName; }
        set { 
            userName = value;
            players.Add(userName);
        }
    }

    public static Networker Instance
    {
        get { return Networker.instance; }
    }

    public int SpawnIndex
    {
        get { return spawnIndex; }
    }

    public EPlayerType PlayerType
    {
        get { return playerType; }
        set { playerType = value; }
    }

    void Start() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Only one networker per client is allowed.");
            Destroy(gameObject);
        }
    }

    public void JoinMatch(string ip) {
        NetworkConnectionError error = Network.Connect(ip, PORT);
        if (error != NetworkConnectionError.NoError) throw new Exception(error.ToString());
    }

    public void CreateServer() {
        NetworkConnectionError error = Network.InitializeServer(MAXPLAYERS, PORT, false);
        if (error != NetworkConnectionError.NoError) throw new Exception(error.ToString());
    }

    public void LoadLevel(string level) {
        Network.SetSendingEnabled(0, false);
        Network.isMessageQueueRunning = false;
        switch (level) { 
            case "Level":
                DestroyImmediate(GetComponent<LobbyHandler>());
                gameObject.AddComponent<LevelHandler>();
                break;
            case "Lobby":
                DestroyImmediate(GetComponent<LevelHandler>());
                gameObject.AddComponent<LobbyHandler>();
                break;
            default:
                break;
        }
        Application.LoadLevel(level);
    }

    void OnLevelLoaded(int levelID) {
        Network.SetSendingEnabled(0, true);
        Network.isMessageQueueRunning = true;
    }
}
