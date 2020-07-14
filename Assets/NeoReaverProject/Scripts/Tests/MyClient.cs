﻿using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MyClient : MonoBehaviour {
    
    Logic _logic;

    string _serverAdress = "";
    string _appId = "6bf6b4e7-b37c-40e2-8548-11f41fbf3ae0";
    string _gameVersion = "0.1.0";

    [SerializeField] GameObject _connectionPanel;
    [SerializeField] GameObject _lobbyPanel;

    [SerializeField] Text _serverAddressInput;
    [SerializeField] Text _appIdInput;
    [SerializeField] Text _gameVersionInput;
    [SerializeField] Text _nickNameInput;

    private NetworkTimer inputRepeatTimer;
    
    // Start is called before the first frame update
    void Start() {
        _lobbyPanel.SetActive(false);
        inputRepeatTimer = new NetworkTimer(10);
    }

    public void ConnectToServer() {
        _logic = new Logic();
        _logic.ConnectToMaster(
            _serverAddressInput.text.ToString(),
            _appIdInput.text.ToString(), 
            _gameVersionInput.text.ToString(),
            _nickNameInput.text.ToString()
            );
        
        _logic.localPlayer.StateChanged += this.OnStateChanged;
        _logic.localPlayer.OpResponseReceived += this.OnOperationResponse;
        
        _connectionPanel.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_logic != null) {
            _logic.UpdateLocal();

            if (_logic.LocalPlayerJoined())
            {
                RenderPlayers();
                InputForControlledCube();
            }
        }
    }

    void OnGUI() {
        if (_logic != null){
            GUI.Label(new Rect(10, 10, 300, 50), this._logic.localPlayer.State.ToString());
        }
    }
    
    /// <summary>
    /// Convert integer value to Color
    /// </summary>
    public static Color IntToColor(int colorValue)
    {
        float r = (byte)(colorValue >> 16) / 255.0f;
        float g = (byte)(colorValue >> 8) / 255.0f;
        float b = (byte)colorValue / 255.0f;

        return new Color(r, g, b);
    }

    private void InputForControlledCube()
    {
        if (!inputRepeatTimer.ShouldExecute)
        {
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        if (math.abs(x) > 0.1f)
        {
            _logic.localPlayer.LocalPlayer.PosX += (int) x;
            inputRepeatTimer.Reset();
        }

        float y = Input.GetAxisRaw("Vertical");
        if (math.abs(y) > 0.1f) {
            _logic.localPlayer.LocalPlayer.PosY += (int) y;
            inputRepeatTimer.Reset();
        }
    }

    public void JoinRandomGame() {
        Debug.Log("JOINING RANDOM GAME");
        this._logic.localPlayer.OpJoinRandomRoom();
    }

    private void OnStateChanged(ClientState fromState, ClientState toState)
    {
        switch (toState) {
            case ClientState.ConnectedToMasterServer:
                //Show room panel
                _lobbyPanel.SetActive(true);
                break;
        }
    }
    
    /// <summary>
    /// Render cubes onto the scene
    /// </summary>
    void RenderPlayers()
    {
        lock (_logic.localPlayer)
        {
            foreach (CustomPlayer p in _logic.localPlayer.LocalRoom.Players.Values)
            {
                foreach (GameObject cube in _logic.playerObjects)
                {
                    if (cube.name == p.NickName)
                    {
                        cube.transform.position = new Vector3(p.PosX / 10f, p.PosY/ 10f, 0);
                        break;
                    }
                }
            }
        }
    }

    public void OnOperationResponse(OperationResponse operationResponse) {
        switch (operationResponse.OperationCode) {
            case OperationCode.JoinRandomGame:
            case OperationCode.JoinGame:
                _lobbyPanel.SetActive(false);
                break;  
        }
        Debug.Log("MyClient : " + operationResponse.ToString());
    }
}
