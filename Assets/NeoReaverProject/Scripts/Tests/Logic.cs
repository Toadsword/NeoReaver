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
using System.Runtime.CompilerServices;
using NeoReaverProject.Scripts;
using Packages.EZRollback.Runtime.Scripts;
using Photon.Realtime;
using UnityEditor;

public class Logic
{
    
    // Connection parameters
    public static string ServerAddress { get; set; }
    public static string AppId { get; set; }
    public static string GameVersion { get; set; }
    
    public int GetLocalPlayerId() {
        return localPlayer.LocalPlayer.ActorNumber - 1;
    }

    // Dictionaries for storing references to background games and remote players
    public GameLogic localPlayer { get; private set; }
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
        ServerAddress = serverAddress;
        AppId = appId;
        GameVersion = gameVersion;
        
        remotePlayers = new Dictionary<string, CustomPlayer>();
        playerObjects = new List<GameObject>();

        // Initialize local game
        localPlayer = new GameLogic(appId, gameVersion);
        localPlayer.NickName = nickName;
        localPlayer.UserId = nickName;

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
    private void OnJoinedPlayer(CustomPlayer customPlayer) {
        RollbackManager.rbInputManager.localPlayerId = localPlayer.LocalPlayer.ActorNumber - 1;
        if (!customPlayer.IsLocal)
        {
            // Adding the new player, that just joined the game
            lock (remotePlayers)
            {
                if (!remotePlayers.ContainsKey(customPlayer.NickName))
                {
                    GameObject playerPrefab = Resources.Load("NeoReaverProject/Prefabs/Player", typeof(GameObject)) as GameObject;
                    GameObject player = Object.Instantiate(playerPrefab, new Vector3(), new Quaternion());
                    RollbackManager.rbInputManager.AddPlayer();
                    player.name = customPlayer.NickName; 
                    
                    player.GetComponent<PlayerController>().SetupPlayer(customPlayer.ActorNumber - 1, customPlayer.NickName);
                    playerObjects.Add(player);
                    customPlayer.playerObject = player;
                    remotePlayers.Add(customPlayer.NickName, customPlayer);
                }
            }
        }
        else
        {
            // Adding the remote players that already appeared in the game
            lock (localPlayer)
            {
                foreach (Player player1 in localPlayer.LocalRoom.Players.Values)
                {
                    CustomPlayer p = (CustomPlayer) player1;
                    foreach (GameObject playerObject in playerObjects)
                    {
                        if (playerObject.name == p.NickName) return;
                    }

                    GameObject playerPrefab = Resources.Load("NeoReaverProject/Prefabs/Player", typeof(GameObject)) as GameObject;
                    GameObject player = Object.Instantiate(playerPrefab, new Vector3(), new Quaternion());
                    RollbackManager.rbInputManager.AddPlayer();
                    player.name = p.NickName;
                    
                    player.GetComponent<PlayerController>().SetupPlayer(p.ActorNumber - 1, p.NickName);
                    playerObjects.Add(player);
                    p.playerObject = player;
                    remotePlayers.Add(p.NickName, p);
                }
            }
        }

        if (!GameManager.Instance.gameStarted) {
            UpdateBasePositions();
            UpdateBaseColors();
            UpdateBaseNames();
        } else {
            DisablePlayer(customPlayer.ActorNumber - 1);
        }
    }

    public void StartGame() {
        GameUIManager.Instance.StartCountdown(0f);
        SoundManager.Instance.PlaySound(SoundManager.SoundList.TING);
        
        localPlayer.SendStartGameEvent();
    }

    /// <summary>
    /// Handler for "Player Leaved" Event
    /// </summary>
    /// <param name="CustomPlayer">Player that leaved the game</param>
    private void OnLeavedPlayer(CustomPlayer CustomPlayer)
    {
        string name = CustomPlayer.NickName;

        foreach (GameObject playerObject in playerObjects)
        {
            if (playerObject.name == name)
            {
                remotePlayers.Remove(name);
                playerObjects.Remove(playerObject);
                Object.Destroy(playerObject);
                return;
            }
        }

        if (!GameManager.Instance.gameStarted) {
            UpdateBasePositions();
            UpdateBaseColors();
            UpdateBaseNames();
        }
    }

    // Update is called once per frame
    public void UpdateLocal ()
    {
        if (localPlayer != null)
        {
            localPlayer.UpdateLoop();
            Move();
            if (localPlayer.State == ClientState.Joined) {
                UpdatePlayersPing();
            }
        }
    }

    // Update the position of the client
    private void Move()
    {
        if (LocalPlayerJoined())
        {
            localPlayer.UpdateLoop();
        }
    }

    /// <summary>
    /// Check if local player joined the game
    /// </summary>
    public bool LocalPlayerJoined()
    {
        if (localPlayer != null && localPlayer.State == ClientState.Joined && localPlayer.LocalRoom != null)
        {
            return true;
        }
        return false;
    }

    private void UpdateBasePositions() {
        float deltaAngle = 360.0f / localPlayer.LocalRoom.PlayerCount;
        
        float circleRadius = 10.0f;
        
        float currentAngle = 0.0f;
        //Update position of all players
        foreach (GameObject playerObject in playerObjects) {
            currentAngle = deltaAngle * playerObject.GetComponent<PlayerController>()._playerId;
            
            Vector3 pos = Vector3.zero;
            pos.x = circleRadius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            pos.y = circleRadius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            playerObject.transform.position = pos;
            
            playerObject.GetComponent<PlayerController>().GetRotationTransform().up = -pos;
        }
    }

    private void UpdateBaseColors() {
        foreach (GameObject playerObject in playerObjects) {
            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            playerController.SetupLocal(GetLocalPlayerId() == playerController._playerId);
        }
    }

    private void UpdateBaseNames() {
        for(int i = 0; i < playerObjects.Count; i++) {
            PlayerController playerController = playerObjects[i].GetComponent<PlayerController>();
            string newName = playerObjects[i].name;
            if (GetLocalPlayerId() == playerController._playerId) {
                newName = newName + " (You)";
            }
            playerController.GetPlayerUiController().UpdatePlayerText(newName);
        }
    }

    private void UpdatePlayersPing() {
        localPlayer.LocalPlayer.Ping = localPlayer.LocalRoom.LoadBalancingClient.LoadBalancingPeer.RoundTripTime;
        localPlayer.LocalPlayer.UpdatePingValue();
        
        foreach (var remotePlayer in remotePlayers) {
            remotePlayer.Value.UpdatePingValue();
        }
    }

    private void DisablePlayer(int playerId) {
        for(int i = 0; i < playerObjects.Count; i++) {
            PlayerController playerController = playerObjects[i].GetComponent<PlayerController>();
            if (playerId == playerController._playerId) {
                playerController.SetSpectator();
            } 
        }
    }
}