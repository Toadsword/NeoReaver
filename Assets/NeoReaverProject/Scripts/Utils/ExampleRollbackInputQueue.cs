using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using InControl;
using NeoReaverProject.Scripts;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

public class ExampleRollbackInputQueue : InputQueue {
    const int numNetworkableActions = 5;

    [SerializeField] GameObject _playerPrefab;
    
    [SerializeField] List<ControllerToPlayer> controllerToPlayers = new List<ControllerToPlayer>();

    [Serializable]
    public struct ControllerToPlayer
    {
        public InputDevice device;
        public int playerId;
        public ControllerState controllerState;
        public GameObject player;
        public GameState gameState;
    }
    
    public enum ControllerState {
        ATTACHED,
        DETACHED
    }
    
    
    public enum GameState {
        NOT_PLAYING,
        PLAYING
    }
    
    void OnEnable() {
        if (controllerToPlayers == null) {
            controllerToPlayers = new List<ControllerToPlayer>();
        }

        InputManager.OnDeviceAttached += OnDeviceAttached;
        InputManager.OnDeviceDetached += OnDeviceDetached;
    }

    private void OnDeviceAttached(InputDevice newDevice) {
        ControllerToPlayer newPlayer;

        //Check if we've found again the controller
        bool reFoundController = false;
        for (int i = 0; i < controllerToPlayers.Count; i++)
        {
            if (controllerToPlayers[i].device.Equals(newDevice))
            {
                Debug.Log("Reconnected controller : " + controllerToPlayers[i].playerId);
                reFoundController = true;

                ControllerToPlayer ctPlayer = controllerToPlayers[i];
                ctPlayer.controllerState = ControllerState.ATTACHED;
                ctPlayer.gameState = GameState.PLAYING;
                ctPlayer.player.SetActive(true);

                controllerToPlayers[i] = ctPlayer;
                break;
            }
        }

        //Else, register the new controller
        if (!reFoundController)
        {
            Debug.Log("New Connected controller : " + controllerToPlayers.Count);
            ControllerToPlayer ctPlayer = new ControllerToPlayer();
            ctPlayer.playerId = AddController() - 1;
            ctPlayer.device = newDevice;
            ctPlayer.controllerState = ControllerState.ATTACHED;
            ctPlayer.gameState = GameState.PLAYING;

            if (_playerPrefab != null)
            {
                Vector3 spawnPosition = new Vector3(0,0,0); //Behind the camera

                GameObject newPlayerObj = Instantiate(_playerPrefab, spawnPosition, Quaternion.identity);
                newPlayerObj.name = "Player "+(ctPlayer.playerId+1).ToString();
                newPlayerObj.GetComponent<PlayerController>().SetControllerId(ctPlayer.playerId);
                ctPlayer.player = newPlayerObj;
                
                Camera.main.GetComponent<CameraScale>()
                    .RegisterNewPlayer(newPlayerObj.GetComponent<PlayerController>());
            }

            controllerToPlayers.Add(ctPlayer);
        }
    }

    private void OnDeviceDetached(InputDevice device)
    {
        for (int i = 0; i < controllerToPlayers.Count; i++)
        {
            if (controllerToPlayers[i].device.Equals(device))
            {
                Debug.Log("Detached Controller : ");
                ControllerToPlayer ctPlayer = controllerToPlayers[i];
                ctPlayer.controllerState = ControllerState.DETACHED;
                ctPlayer.gameState = GameState.NOT_PLAYING;
                ctPlayer.player.SetActive(false);
                break;
            }
        }
    }

    protected  override RollbackInputBaseActions GetCurrentActionsValue(int controllerId){
        if (controllerToPlayers.Count <= controllerId) {
            return new RollbackInputBaseActions();
        }
            
        InputDevice currentDevice = controllerToPlayers[controllerId].device;

        if (currentDevice == null) {
            return new RollbackInputBaseActions();
        }
        
        RollbackInputBaseActions actionsValue = new RollbackInputBaseActions(5);
        
        SetBitFromAction(InputActionManager.InputType.LEFT, ref actionsValue, currentDevice);
        SetBitFromAction(InputActionManager.InputType.RIGHT, ref actionsValue, currentDevice);
        SetBitFromAction(InputActionManager.InputType.UP, ref actionsValue, currentDevice);
        SetBitFromAction(InputActionManager.InputType.DOWN, ref actionsValue, currentDevice);
        SetBitFromAction(InputActionManager.InputType.SHOOT, ref actionsValue, currentDevice);

        actionsValue.horizontalValue = TransformAxisValueToSByte(InputActionManager.GetAxis(InputActionManager.AxisType.HORIZONTAL, currentDevice));
        actionsValue.verticalValue = TransformAxisValueToSByte(InputActionManager.GetAxis(InputActionManager.AxisType.VERTICAL, currentDevice));

        return actionsValue;
    }

    void SetBitFromAction(InputActionManager.InputType inputType, ref RollbackInputBaseActions actionsValue, InputDevice inputDevice) {
        actionsValue.SetOrClearBit((int)inputType, InputActionManager.GetInput(inputType, inputDevice));
    }

    public override string GetActionName(int actionIndex) {
        return ((InputActionManager.InputType)actionIndex).ToString();
    } 
}