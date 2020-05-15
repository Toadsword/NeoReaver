﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH">
//   Exit Games GmbH, 2012
// </copyright>
// <summary>
// This script must be added to game objects that describe clients
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using Photon.Realtime;

public class Logic
{
    
    // Connection parameters
    public static string ServerAddress { get; set; }
    public static string AppId { get; set; }
    public static string GameVersion { get; set; }

    // Dictionaries for storing references to background games and remote players
    public GameLogic localPlayer { get; private set; }
    public static Dictionary<string, GameLogic> clients;
    public static Dictionary<string, CustomPlayer> remotePlayers;

    // Cube GameObjects that represent players
    public List<GameObject> playerObjects;

    /// <summary>
    /// CallConnect the game using given connection parameters
    /// </summary>
    /// <param name="serverAddress">Server address</param>
    /// <param name="appId">Application ID</param>
    /// <param name="gameVersion">Game version</param>
    /// <param name="nickName">Nickname of the player</param>
    public void ConnectToMaster(string serverAddress, string appId, string gameVersion, string nickName)
    {
        Debug.Log("ConnectToMaster");
        ServerAddress = serverAddress;
        AppId = appId;
        GameVersion = gameVersion;

        clients = new Dictionary<string, GameLogic>();
        remotePlayers = new Dictionary<string, CustomPlayer>();
        playerObjects = new List<GameObject>();

        // Initialize local game
        localPlayer = new GameLogic(appId, gameVersion);
        localPlayer.NickName = nickName;

        localPlayer.OnEventJoin = this.OnJoinedPlayer;
        localPlayer.OnEventLeave = this.OnLeavedPlayer;

        if (!string.IsNullOrEmpty(serverAddress))
        {
            localPlayer.MasterServerAddress = serverAddress;
        }
        localPlayer.CallConnect();
    }

    /// <summary>
    /// Handler for "Player Joined" Event
    /// </summary>
    /// <param name="CustomPlayer">Player that joined the game</param>
    private void OnJoinedPlayer(CustomPlayer CustomPlayer)
    {
        Debug.Log("OnJoinedPlayer");
        if (!CustomPlayer.IsLocal)
        {
            lock (remotePlayers)
            {
                if (!remotePlayers.ContainsKey(CustomPlayer.NickName) && !clients.ContainsKey(CustomPlayer.NickName))
                {
                    GameObject playerPrefab = Resources.Load("NeoReaverProject/Prefabs/Player", typeof(GameObject)) as GameObject;
                    GameObject player = GameObject.Instantiate(playerPrefab, new Vector3(), new Quaternion());
                    player.name = CustomPlayer.NickName;
                    playerObjects.Add(player);
                    remotePlayers.Add(CustomPlayer.NickName, CustomPlayer);
                }
            }
        }
        else
        {
            lock (localPlayer)
            {
                foreach (CustomPlayer p in localPlayer.LocalRoom.Players.Values)
                {
                    foreach (GameObject cube in playerObjects)
                    {
                        if (cube.name == p.NickName) return;
                    }

                    GameObject playerPrefab = Resources.Load("NeoReaverProject/Prefabs/Player", typeof(GameObject)) as GameObject;
                    GameObject player = GameObject.Instantiate(playerPrefab, new Vector3(), new Quaternion());
                    player.name = CustomPlayer.NickName;
                    playerObjects.Add(player);
                    remotePlayers.Add(CustomPlayer.NickName, CustomPlayer);
                }
            }
        }
    }

    /// <summary>
    /// Handler for "Player Leaved" Event
    /// </summary>
    /// <param name="CustomPlayer">Player that leaved the game</param>
    private void OnLeavedPlayer(CustomPlayer CustomPlayer)
    {
        Debug.Log("OnLeavedPlayer");
        string name = CustomPlayer.NickName;

        foreach (GameObject cube in playerObjects)
        {
            if (cube.name == name)
            {
                remotePlayers.Remove(name);
                playerObjects.Remove(cube);
                GameObject.Destroy(cube);
                return;
            }
        }
    }

    // Update is called once per frame
    public void UpdateLocal ()
    {
        Debug.Log("UpdateLocal");
        if (localPlayer != null)
        {
            localPlayer.UpdateLoop();
            Move();
        }
    }

    // Update the position of the client
    private void Move()
    {
        Debug.Log("Move");
        if (LocalPlayerJoined())
        {
            foreach (GameLogic logic in clients.Values)
            {
                logic.UpdateLoop();
            }
        }
    }

    /// <summary>
    /// Check if local player joined the game
    /// </summary>
    public bool LocalPlayerJoined()
    {
        Debug.Log("LocalPlayerJoined");
        if (localPlayer != null && localPlayer.State == ClientState.Joined && localPlayer.LocalRoom != null)
        {
            return true;
        }
        return false;
    }
}